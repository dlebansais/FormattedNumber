namespace FormattedNumber
{
    using System.Diagnostics;

    /// <summary>
    /// Hold information during parsing of an integer in base other than decimal.
    /// </summary>
    internal interface IIntegerWithBaseParsingInfo : IParsingInfo
    {
    }

    /// <summary>
    /// Hold information during parsing of an integer in base other than decimal.
    /// </summary>
    internal abstract class IntegerWithBaseParsingInfo : ParsingInfo, IIntegerWithBaseParsingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerWithBaseParsingInfo"/> class.
        /// </summary>
        public IntegerWithBaseParsingInfo()
        {
            Debug.Assert(Base.Radix != IntegerBase.DecimalRadix);
            Debug.Assert(!string.IsNullOrEmpty(Base.Suffix));

            Sign = OptionalSign.None;
            LeadingZeroCount = 0;
            StartOffset = 0;
            Length = 0;
            SuffixOffset = 0;
            Handler = ParseStart;
        }

        /// <summary>
        /// The base to use when parsing.
        /// </summary>
        protected abstract IIntegerBase Base { get; }

        /// <summary>
        /// Checks if the current parser went further than others.
        /// </summary>
        /// <param name="parsing">The previous best parser.</param>
        /// <param name="length">The length reached by <paramref name="parsing"/>.</param>
        public override void UpdateBestParsing(ref IParsingInfo parsing, ref int length)
        {
            if (StillParsing && LengthSuccessful == 0 && SuffixOffset == Base.Suffix.Length)
                LengthSuccessful = StartOffset + Length + SuffixOffset;

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

            Debug.Assert(StartOffset + Length + Base.Suffix.Length <= text.Length);

            string IntegerText = text.Substring(StartOffset + LeadingZeroCount, Length - LeadingZeroCount);
            string InvalidText = text.Substring(StartOffset + Length + Base.Suffix.Length);

            CanonicalNumber Canonical;

            if (IntegerText == IntegerBase.Zero)
                Canonical = CanonicalNumber.Zero;
            else
            {
                // Convert to decimal. The result can start or finish with a zero
                string DecimalSignificand = IntegerBase.Convert(IntegerText, Base, IntegerBase.Decimal);
                Canonical = new CanonicalNumber(Sign, DecimalSignificand);
            }

            return new FormattedInteger(Base, Sign, LeadingZeroCount, IntegerText, InvalidText, Canonical);
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
                Handler(c);
        }

        private void ParseLeadingZeroes(char c)
        {
            if (Base.IsValidDigit(c, out int DigitValue))
            {
                if (DigitValue == 0)
                {
                    Length++;
                    LeadingZeroCount++;
                }
                else
                {
                    Handler = ParseDigits;
                    Handler(c);
                }
            }
            else if (Length > 0)
            {
                Handler = ParseSuffix;
                Handler(c);
            }
            else
                StillParsing = false;
        }

        private void ParseDigits(char c)
        {
            if (Base.IsValidDigit(c, out int DigitValue))
                Length++;
            else
            {
                Debug.Assert(Length > 0);

                Handler = ParseSuffix;
                Handler(c);
            }
        }

        private void ParseSuffix(char c)
        {
            if (SuffixOffset < Base.Suffix.Length && c == Base.Suffix[SuffixOffset])
                SuffixOffset++;
            else
            {
                if (SuffixOffset == Base.Suffix.Length)
                    LengthSuccessful = StartOffset + Length + SuffixOffset;

                StillParsing = false;
            }
        }

        private OptionalSign Sign;
        private int LeadingZeroCount;
        private int StartOffset;
        private int Length;
        private int SuffixOffset;
    }
}
