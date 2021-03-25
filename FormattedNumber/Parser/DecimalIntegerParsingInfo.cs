namespace FormattedNumber
{
    using System.Diagnostics;

    /// <summary>
    /// Hold information during parsing of an integer in decimal base.
    /// </summary>
    internal interface IDecimalIntegerParsingInfo : IParsingInfo
    {
    }

    /// <summary>
    /// Hold information during parsing of an integer in decimal base.
    /// </summary>
    internal class DecimalIntegerParsingInfo : ParsingInfo, IDecimalIntegerParsingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalIntegerParsingInfo"/> class.
        /// </summary>
        public DecimalIntegerParsingInfo()
        {
            Sign = OptionalSign.None;
            LeadingZeroCount = 0;
            StartOffset = 0;
            Length = 0;
            Handler = ParseStart;
        }

        /// <summary>
        /// Checks if the current parser went further than others.
        /// </summary>
        /// <param name="parsing">The previous best parser.</param>
        /// <param name="length">The length reached by <paramref name="parsing"/>.</param>
        public override void UpdateBestParsing(ref IParsingInfo parsing, ref int length)
        {
            if (StillParsing && LengthSuccessful == 0 && (LeadingZeroCount > 0 || Length > 0))
                LengthSuccessful = StartOffset + Length;

            base.UpdateBestParsing(ref parsing, ref length);
        }

        /// <summary>
        /// Gets the formatted number that this parser is able to extract.
        /// </summary>
        /// <param name="text">The source string.</param>
        public override FormattedNumber GetResult(string text)
        {
            Debug.Assert(LengthSuccessful > 0);

            if (LeadingZeroCount == Length && LeadingZeroCount > 0)
                LeadingZeroCount--;

            Debug.Assert(StartOffset + Length <= text.Length);

            string IntegerText = text.Substring(StartOffset + LeadingZeroCount, Length - LeadingZeroCount);
            string InvalidText = text.Substring(StartOffset + Length);

            CanonicalNumber Canonical;

            if (IntegerText == IntegerBase.Zero)
                Canonical = CanonicalNumber.Zero;
            else
                Canonical = new CanonicalNumber(Sign, IntegerText);

            return new FormattedInteger(IntegerBase.Decimal, Sign, LeadingZeroCount, IntegerText, InvalidText, Canonical);
        }

        private void ParseStart(char c)
        {
            Handler = ParseLeadingZeroes;

            if (c == '-')
            {
                Sign = OptionalSign.Negative;
                StartOffset = 1;
            }
            else if (c == '+')
            {
                Sign = OptionalSign.Positive;
                StartOffset = 1;
            }
            else
                ParseLeadingZeroes(c);
        }

        private void ParseLeadingZeroes(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
            {
                if (DigitValue == 0)
                {
                    Length++;
                    LeadingZeroCount++;
                }
                else
                {
                    Handler = ParseDigits;
                    ParseDigits(c);
                }
            }
            else
            {
                if (LengthSuccessful == 0)
                    LengthSuccessful = StartOffset + Length;

                StillParsing = false;
            }
        }

        private void ParseDigits(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
                Length++;
            else
            {
                if (LengthSuccessful == 0)
                    LengthSuccessful = StartOffset + Length;

                StillParsing = false;
            }
        }

        private OptionalSign Sign;
        private int LeadingZeroCount;
        private int StartOffset;
        private int Length;
    }
}
