namespace FormattedNumber
{
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Class to hold information during parsing of an integer in base other than decimal.
    /// </summary>
    public class RealParsingInfo : ParsingInfo
    {
        /// <summary>
        /// The decimal separator in numeric values.
        /// </summary>
        public static readonly char DecimalSeparator;

        /// <summary>
        /// The dot (unicode U+002E) is always considered a valid separator.
        /// </summary>
        public static readonly char NeutralDecimalSeparator = '.';

        /// <summary>
        /// The character to use when a number doesn't contain the decimal separator.
        /// </summary>
        public static readonly char NoSeparator = '\0';

        static RealParsingInfo()
        {
            string CurrentCultureSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            Debug.Assert(CurrentCultureSeparator.Length == 1);
            DecimalSeparator = CurrentCultureSeparator[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RealParsingInfo"/> class.
        /// </summary>
        public RealParsingInfo()
        {
            Sign = OptionalSign.None;
            LeadingZeroCount = 0;
            StartOffset = 0;
            IntegerLength = 0;
            SeparatorCharacter = NoSeparator;
            FractionalLength = 0;
            ExponentCharacter = NoSeparator;
            ExponentSign = OptionalSign.None;
            ExponentLength = 0;
            Handler = ParseStart;
        }

        /// <summary>
        /// Checks if the current parser went further than others.
        /// </summary>
        /// <param name="parsing">The previous best parser.</param>
        /// <param name="length">The length reached by <paramref name="parsing"/>.</param>
        public override void UpdateBestParsing(ref ParsingInfo parsing, ref int length)
        {
            if (StillParsing && LengthSuccessful == 0 && (IntegerLength > 0 || FractionalLength > 0) && (SeparatorCharacter != NoSeparator || ExponentLength > 0))
                if (ExponentLength > 0)
                    if (SeparatorCharacter != NoSeparator)
                        LengthSuccessful = StartOffset + IntegerLength + 1 + FractionalLength + 1 + ExponentStartOffset + ExponentLength;
                    else
                        LengthSuccessful = StartOffset + IntegerLength + 1 + ExponentStartOffset + ExponentLength;
                else
                    LengthSuccessful = StartOffset + IntegerLength + 1 + FractionalLength;

            base.UpdateBestParsing(ref parsing, ref length);
        }

        /// <summary>
        /// Gets the formatted number that this parser is able to extract.
        /// </summary>
        /// <param name="text">The source string.</param>
        public override FormattedNumber GetResult(string text)
        {
            Debug.Assert(LengthSuccessful > 0);

            if (LeadingZeroCount == IntegerLength && LeadingZeroCount > 0)
                LeadingZeroCount--;

            Debug.Assert(StartOffset + IntegerLength + 1 + FractionalLength <= text.Length);

            string IntegerText = text.Substring(StartOffset + LeadingZeroCount, IntegerLength - LeadingZeroCount);
            string FractionalText = text.Substring(StartOffset + IntegerLength + 1, FractionalLength);

            if (ExponentLength > 0)
            {
                string ExponentText;
                string InvalidText;
                string SignificandText;

                if (SeparatorCharacter != NoSeparator)
                {
                    Debug.Assert(IntegerText.Length > 0 || FractionalText.Length > 0);

                    ExponentText = text.Substring(StartOffset + IntegerLength + 1 + FractionalLength + 1 + ExponentStartOffset, ExponentLength);
                    InvalidText = text.Substring(StartOffset + IntegerLength + 1 + FractionalLength + 1 + ExponentStartOffset + ExponentLength);

                    SignificandText = GetSignificandText(IntegerText, FractionalText);
                }
                else
                {
                    ExponentText = text.Substring(StartOffset + IntegerLength + 1 + ExponentStartOffset, ExponentLength);
                    InvalidText = text.Substring(StartOffset + IntegerLength + 1 + ExponentStartOffset + ExponentLength);

                    SignificandText = IntegerText;
                }

                CanonicalNumber Canonical = GetCanonical(Sign, SignificandText, ExponentSign, ExponentText);
                return new FormattedReal(Sign, LeadingZeroCount, IntegerText, SeparatorCharacter, FractionalText, ExponentCharacter, ExponentSign, ExponentText, InvalidText, Canonical);
            }
            else
            {
                string SignificandText = GetSignificandText(IntegerText, FractionalText);
                string InvalidText = text.Substring(StartOffset + IntegerLength + 1 + FractionalLength);

                CanonicalNumber Canonical = GetCanonical(Sign, SignificandText, OptionalSign.None, IntegerBase.Zero);
                return new FormattedReal(Sign, LeadingZeroCount, IntegerText, SeparatorCharacter, FractionalText, NoSeparator, OptionalSign.None, string.Empty, InvalidText, Canonical);
            }
        }

        private string GetSignificandText(string integerText, string fractionalText)
        {
            Debug.Assert(integerText.Length > 0 || fractionalText.Length > 0);

            string SignificandText;

            if (integerText.Length == 0)
                if (fractionalText == IntegerBase.Zero)
                    SignificandText = IntegerBase.Zero;
                else
                    SignificandText = IntegerBase.Zero + NeutralDecimalSeparator + fractionalText;
            else if (fractionalText.Length == 0)
                if (integerText == IntegerBase.Zero)
                    SignificandText = IntegerBase.Zero;
                else
                    SignificandText = integerText + NeutralDecimalSeparator + IntegerBase.Zero;
            else if (integerText == IntegerBase.Zero && fractionalText == IntegerBase.Zero)
                SignificandText = IntegerBase.Zero;
            else
                SignificandText = integerText + NeutralDecimalSeparator + fractionalText;

            return SignificandText;
        }

        private CanonicalNumber GetCanonical(OptionalSign significandSign, string significandText, OptionalSign exponentSign, string exponentText)
        {
            CanonicalNumber Canonical;

            if (significandText == IntegerBase.Zero)
                Canonical = CanonicalNumber.Zero;
            else
                Canonical = new CanonicalNumber(significandSign, significandText, exponentSign, exponentText);

            return Canonical;
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
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
            {
                if (DigitValue == 0)
                {
                    IntegerLength++;
                    LeadingZeroCount++;
                }
                else
                {
                    Handler = ParseIntegerDigits;
                    Handler(c);
                }
            }
            else if (c == NeutralDecimalSeparator || c == DecimalSeparator)
            {
                SeparatorCharacter = c;
                Handler = ParseFractionalDigits;
            }
            else if (c == 'e' || c == 'E')
            {
                ExponentCharacter = c;

                if (IntegerLength > 0)
                    Handler = ParseExponent;
                else
                    StillParsing = false;
            }
            else
                StillParsing = false;
        }

        private void ParseIntegerDigits(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
                IntegerLength++;
            else if (c == NeutralDecimalSeparator || c == DecimalSeparator)
            {
                SeparatorCharacter = c;
                Handler = ParseFractionalDigits;
            }
            else if (c == 'e' || c == 'E')
            {
                ExponentCharacter = c;

                if (IntegerLength > 0)
                    Handler = ParseExponent;
                else
                    StillParsing = false;
            }
            else
                StillParsing = false;
        }

        private void ParseFractionalDigits(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
                FractionalLength++;
            else if (c == 'e' || c == 'E')
            {
                ExponentCharacter = c;

                if (IntegerLength > 0 || FractionalLength > 0)
                    Handler = ParseExponent;
                else
                    StillParsing = false;
            }
            else
            {
                if (IntegerLength > 0 || FractionalLength > 0)
                    LengthSuccessful = StartOffset + IntegerLength + 1 + FractionalLength;

                StillParsing = false;
            }
        }

        private void ParseExponent(char c)
        {
            Handler = ParseExponentFirstDigit;

            if (c == '-')
            {
                ExponentSign = OptionalSign.Negative;
                ExponentStartOffset = 1;
            }
            else if (c == '+')
            {
                ExponentSign = OptionalSign.Positive;
                ExponentStartOffset = 1;
            }
            else
                Handler(c);
        }

        private void ParseExponentFirstDigit(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue) && DigitValue > 0)
            {
                ExponentLength++;
                Handler = ParseExponentDigits;
            }
            else
            {
                if (SeparatorCharacter != NoSeparator && (IntegerLength > 0 || FractionalLength > 0))
                    LengthSuccessful = StartOffset + IntegerLength + 1 + FractionalLength;

                StillParsing = false;
            }
        }

        private void ParseExponentDigits(char c)
        {
            if (IntegerBase.Decimal.IsValidDigit(c, out int DigitValue))
                ExponentLength++;
            else
            {
                Debug.Assert(ExponentLength > 0);

                if (SeparatorCharacter != NoSeparator && (IntegerLength > 0 || FractionalLength > 0))
                    LengthSuccessful = StartOffset + IntegerLength + 1 + FractionalLength + 1 + ExponentStartOffset + ExponentLength;
                else
                    LengthSuccessful = StartOffset + IntegerLength + 1 + ExponentStartOffset + ExponentLength;

                StillParsing = false;
            }
        }

        private OptionalSign Sign;
        private int LeadingZeroCount;
        private int StartOffset;
        private int IntegerLength;
        private char SeparatorCharacter;
        private int FractionalLength;
        private char ExponentCharacter;
        private OptionalSign ExponentSign;
        private int ExponentStartOffset;
        private int ExponentLength;
    }
}
