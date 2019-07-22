namespace FormattedNumber
{
    using System.Diagnostics;

    /// <summary>
    /// Class to hold information during parsing of an integer in decimal base.
    /// </summary>
    public class DecimalIntegerParsingInfo : ParsingInfo
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
        /// Gets the formatted number that this parser is able to extract.
        /// </summary>
        /// <param name="text">The source string.</param>
        public override FormattedNumber GetResult(string text)
        {
            Debug.Assert(LengthSuccessful > 0);

            if (LeadingZeroCount == Length)
            {
                Debug.Assert(LeadingZeroCount > 0);
                LeadingZeroCount--;
            }

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
                    LengthSuccessful = StartOffset + Length;
                    LeadingZeroCount++;
                }
                else
                {
                    Handler = ParseDigits;
                    ParseDigits(c);
                }
            }
            else
                StillParsing = false;
        }

        private void ParseDigits(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
            {
                Length++;
                LengthSuccessful = StartOffset + Length;
            }
            else
                StillParsing = false;
        }

        private OptionalSign Sign;
        private int LeadingZeroCount;
        private int StartOffset;
        private int Length;
    }
}
