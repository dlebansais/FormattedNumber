namespace FormattedNumber
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using PeterO.Numbers;

    /// <summary>
    /// Base interface for a number format that can parse any string.
    /// </summary>
    public interface IFormattedNumber
    {
        /// <summary>
        /// Gets the number of leading zeroes.
        /// </summary>
        int LeadingZeroesCount { get; }

        /// <summary>
        /// Gets the significand part of the formatted number. Can be empty.
        /// </summary>
        string SignificandString { get; }

        /// <summary>
        /// Gets the exponent part of the formatted number. Can be empty.
        /// </summary>
        string ExponentString { get; }

        /// <summary>
        /// The trailing invalid text, if any.
        /// </summary>
        string InvalidText { get; }

        /// <summary>
        /// The canonical form of the parsed number.
        /// </summary>
        ICanonicalNumber Canonical { get; }
    }

    /// <summary>
    /// Base interface for a number format that can parse any string.
    /// </summary>
    public abstract class FormattedNumber : IFormattedNumber
    {
        #region Constants
        /// <summary>
        /// The decimal separator in numeric values. The dot (unicode U+002E) is also always considered a valid separator.
        /// </summary>
        public static readonly string DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        #endregion

        #region Init
        /// <summary>
        /// Parses a string to a formatted number.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <param name="isLeadingZeroSupported">True if <paramref name="text"/> might have leading zeroes.</param>
        /// <exception cref="NullReferenceException"><paramref name="text"/> is null.</exception>
        /// <returns>The parsed number upon return.</returns>
        public static IFormattedNumber Parse(string text, bool isLeadingZeroSupported = true)
        {
            if (text == null)
                throw new NullReferenceException(nameof(text));

            bool Success = TryParseAsIntegerWithBase(text, isLeadingZeroSupported, IntegerBase.Decimal, out IFormattedNumber Result);

            if (!Success)
                Success = TryParseAsIntegerWithBase(text, isLeadingZeroSupported, IntegerBase.Hexadecimal, out Result);
            if (!Success)
                Success = TryParseAsIntegerWithBase(text, isLeadingZeroSupported, IntegerBase.Octal, out Result);
            if (!Success)
                Success = TryParseAsIntegerWithBase(text, isLeadingZeroSupported, IntegerBase.Binary, out Result);
            if (!Success)
                Result = ParseAsNumber(text);

            Debug.Assert(Result != null);
            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="canonical"/> is null.</exception>
        public static IFormattedNumber FromCanonical(ICanonicalNumber canonical)
        {
            return FromCanonical(0, string.Empty, canonical);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="leadingZeroesCount">The number of leading zeroes.</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        public static IFormattedNumber FromCanonical(int leadingZeroesCount, string invalidText, ICanonicalNumber canonical)
        {
            EFloat NumberFloat = canonical.NumberFloat;

            EInteger Mantissa = NumberFloat.Mantissa;
            string MantissaText = Mantissa.ToRadixString(IntegerBase.DecimalRadix);

            EInteger Exponent = NumberFloat.Exponent;
            string ExponentText = Exponent.ToRadixString(IntegerBase.DecimalRadix);
            OptionalSign ExponentSign = ExponentText[0] == '-' ? OptionalSign.Negative : OptionalSign.None;
            if (ExponentText[0] == '-')
                ExponentText = ExponentText.Substring(1);

            return new RealNumber(MantissaText, '.', "0", 'e', ExponentSign, ExponentText, string.Empty, canonical);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="leadingZeroesCount">The number of leading zeroes.</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        protected FormattedNumber(int leadingZeroesCount, string invalidText, ICanonicalNumber canonical)
        {
            LeadingZeroesCount = leadingZeroesCount;
            InvalidText = invalidText ?? throw new NullReferenceException(nameof(invalidText));
            Canonical = canonical ?? throw new NullReferenceException(nameof(canonical));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of leading zeroes.
        /// </summary>
        public int LeadingZeroesCount { get; }

        /// <summary>
        /// Gets the significand part of the formatted number. Can be empty.
        /// </summary>
        public abstract string SignificandString { get; }

        /// <summary>
        /// Gets the exponent part of the formatted number. Can be empty.
        /// </summary>
        public abstract string ExponentString { get; }

        /// <summary>
        /// The trailing invalid text, if any.
        /// </summary>
        public string InvalidText { get; }

        /// <summary>
        /// The canonical form of the parsed number.
        /// </summary>
        public ICanonicalNumber Canonical { get; }
        #endregion

        #region Implementation
        /// <summary>
        /// Tries to parse a string as an integer of the given base.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <param name="isLeadingZeroSupported">True if <paramref name="text"/> might have leading zeroes.</param>
        /// <param name="integerBase">The integer base.</param>
        /// <param name="number">The parsed number upon return, null on failure.</param>
        /// <returns>True if the string was parsed successfully; Otherwise, false.</returns>
        /// <exception cref="NullReferenceException"><paramref name="text"/> or <paramref name="integerBase"/> is null.</exception>
        private protected static bool TryParseAsIntegerWithBase(string text, bool isLeadingZeroSupported, IIntegerBase integerBase, out IFormattedNumber number)
        {
            if (text == null)
                throw new NullReferenceException(nameof(text));
            if (integerBase == null)
                throw new NullReferenceException(nameof(integerBase));

            number = null;
            bool IsParsed = false;

            if (integerBase.Suffix != null && text.EndsWith(integerBase.Suffix))
            {
                ParseAsIntegerWithBase(text, isLeadingZeroSupported, integerBase, out number);
                IsParsed = true;
            }

            return IsParsed;
        }

        /// <summary>
        /// Parse a string as an integer of the given base.
        /// </summary>
        /// <param name="text">The string to parse. It must end with the suffix of <paramref name="integerBase"/>.</param>
        /// <param name="isLeadingZeroSupported">True if <paramref name="text"/> might have leading zeroes.</param>
        /// <param name="integerBase">The integer base.</param>
        /// <param name="number">The parsed number upon return, null on failure.</param>
        private protected static void ParseAsIntegerWithBase(string text, bool isLeadingZeroSupported, IIntegerBase integerBase, out IFormattedNumber number)
        {
            Debug.Assert(text != null);
            Debug.Assert(integerBase != null);

            int LeadingZeroesCount = 0;

            if (isLeadingZeroSupported)
                text = LeadingZeroesRemoved(text, out LeadingZeroesCount);

            string Suffix = integerBase.Suffix;

            Debug.Assert(Suffix == null || (Suffix.Length <= text.Length && text.EndsWith(integerBase.Suffix)));
            string DigitText = Suffix == null ? text : text.Substring(0, text.Length - integerBase.Suffix.Length);

            int i;
            for (i = 0; i < DigitText.Length; i++)
            {
                int DigitValue;
                if (!integerBase.IsValidDigit(DigitText[i], out DigitValue))
                    break;
            }

            Debug.Assert(i <= text.Length);

            string ValidText = DigitText.Substring(0, i);
            string InvalidText = DigitText.Substring(i);

            // Convert to decimal. The result can start or finish with a zero
            string DecimalSignificand = IntegerBase.Convert(ValidText, integerBase, IntegerBase.Decimal);

            // Eliminate starting zeroes.
            while (DecimalSignificand.Length > 1 && DecimalSignificand[0] == '0')
                DecimalSignificand = DecimalSignificand.Substring(1);

            // Eliminate ending zeroes. Don't forget they are significant.
            int BaseExponent = 0;
            while (DecimalSignificand.Length > 1 && DecimalSignificand[DecimalSignificand.Length - 1] == '0')
            {
                DecimalSignificand = DecimalSignificand.Substring(0, DecimalSignificand.Length - 1);
                BaseExponent++;
            }
            IntegerBase.Decimal.IsValidSignificand(DecimalSignificand);

            string DecimalExponent = (BaseExponent + DecimalSignificand.Length - 1).ToString();

            ICanonicalNumber Canonical = new CanonicalNumber(OptionalSign.None, DecimalSignificand, OptionalSign.None, DecimalExponent);

            number = new IntegerNumberWithBase(LeadingZeroesCount, ValidText, InvalidText, Canonical, integerBase);
        }

        /// <summary>
        /// Parse a string as a number.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <returns>
        /// An instance of <see cref="IntegerNumber"/> if the valid part of the parsed number is an integer.
        /// An instance of <see cref="RealNumber"/> if <paramref name="text"/> can be parsed, but not as an integer.
        /// An instance of <see cref="InvalidNumber"/> if <paramref name="text"/> cannot be parsed as a number.
        /// </returns>
        private protected static IFormattedNumber ParseAsNumber(string text)
        {
            Debug.Assert(text != null);

            // If empty, assume invalid.
            if (text.Length == 0)
                return new InvalidNumber(text);

            OptionalSign SignificandSign;
            int DigitStart;

            if (text[0] == '-')
            {
                SignificandSign = OptionalSign.Negative;
                DigitStart = 1;
            }
            else if (text[0] == '+')
            {
                SignificandSign = OptionalSign.Positive;
                DigitStart = 1;
            }
            else
            {
                SignificandSign = OptionalSign.None;
                DigitStart = 0;
            }

            // Reject numbers that don't start with a digit.
            int DigitValue;
            if (text.Length <= DigitStart || !IntegerBase.Decimal.IsValidDigit(text[DigitStart], out DigitValue))
                return new InvalidNumber(text);

            // If the first digit is 0, only accept the number zero.
            if (DigitValue == 0)
                if (SignificandSign != OptionalSign.None)
                    return new InvalidNumber(text);
                else
                {
                    Debug.Assert(DigitStart == 0);
                    return new IntegerNumber(0, IntegerBase.Zero, text.Substring(DigitStart + 1), CanonicalNumber.Zero);
                }

            string ValidText;
            string InvalidText;
            string DecimalExponent;
            string SignificandText;
            ICanonicalNumber Canonical;

            // Collect all digits before the decimal separator.
            int n = DigitStart + 1;
            while (n < text.Length && IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                n++;

            string IntegerText = text.Substring(DigitStart, n - DigitStart);
            Debug.Assert(IntegerText.Length > 0 && IntegerText[0] != '0');

            int TrailingZeroesCount;
            char SeparatorCharacter = DecimalSeparator[0];

            // If the number is a simple decimal number or continues with an invalid character, return now.
            // '.' is ALWAYS a valid separator.
            if (n >= text.Length || (text[n] != '.' && text[n] != SeparatorCharacter))
            {
                InvalidText = text.Substring(n);
                DecimalExponent = (n - 1).ToString();
                Canonical = new CanonicalNumber(SignificandSign, TrailingZeroesRemoved(IntegerText, out TrailingZeroesCount), OptionalSign.None, DecimalExponent);
                return new IntegerNumber(0, IntegerText, InvalidText, Canonical);
            }

            SeparatorCharacter = text[n];

            n++;

            // Get the fractional part.
            int FractionalBegin = n;
            while (n < text.Length && IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                n++;

            string FractionalText = text.Substring(DigitStart + IntegerText.Length + 1, n - IntegerText.Length - 1 - DigitStart);

            // If the fractional part is empty, or ends with a zero but is not zero, just return the integer.
            if (FractionalText.Length == 0 || FractionalText[FractionalText.Length - 1] == '0')
            {
                ValidText = IntegerText;
                InvalidText = text.Substring(DigitStart + ValidText.Length);
                DecimalExponent = (IntegerText.Length - 1).ToString();
                Canonical = new CanonicalNumber(SignificandSign, TrailingZeroesRemoved(ValidText, out TrailingZeroesCount), OptionalSign.None, DecimalExponent);
                return new IntegerNumber(0, ValidText, InvalidText, Canonical);
            }

            // If the fractional part is not followed by an exponent, return the corresponding real number that has no exponent.
            if (n == text.Length || (text[n] != 'e' && text[n] != 'E'))
            {
                InvalidText = text.Substring(n);
                SignificandText = IntegerText + FractionalText;
                DecimalExponent = (IntegerText.Length - 1).ToString();
                return new RealNumber(IntegerText, SeparatorCharacter, FractionalText, 'e', OptionalSign.None, string.Empty, InvalidText, new CanonicalNumber(SignificandSign, SignificandText, OptionalSign.None, DecimalExponent));
            }

            Debug.Assert(FractionalText.Length > 0);
            Debug.Assert(FractionalText == IntegerBase.Zero || FractionalText[FractionalText.Length - 1] != '0');
            Debug.Assert(n < text.Length);

            char ExponentCharacter = text[n];
            Debug.Assert(text[n] == 'e' || text[n] == 'E');

            // When an expontent is present, the mantissa part must have exactly one digit on the left side. We already know it's not zero.
            if (IntegerText.Length != 1)
            {
                ValidText = text.Substring(DigitStart, 1);
                InvalidText = text.Substring(DigitStart + ValidText.Length);
                return new RealNumber(ValidText, SeparatorCharacter, string.Empty, ExponentCharacter, OptionalSign.None, string.Empty, InvalidText, new CanonicalNumber(SignificandSign, ValidText, OptionalSign.None, IntegerBase.Zero));
            }

            SignificandText = TrailingZeroesRemoved(IntegerText + FractionalText, out TrailingZeroesCount);

            int MantissaEnd = n;
            n++;

            OptionalSign ExponentSign;

            // Allow exponent sign.
            if (n + 1 < text.Length)
            {
                if (text[n] == '-')
                {
                    ExponentSign = OptionalSign.Negative;
                    n++;
                }

                else if (text[n] == '+')
                {
                    ExponentSign = OptionalSign.Positive;
                    n++;
                }

                else
                    ExponentSign = OptionalSign.None;
            }
            else
                ExponentSign = OptionalSign.None;

            // Get the exponent.
            int ExponentBegin = n;
            while (n < text.Length && IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                n++;
            int ExponentEnd = n;

            string ExponentText = text.Substring(ExponentBegin, ExponentEnd - ExponentBegin);

            // The exponent cannot be empty, and must not start with zero. In that case, the valid part ends at the exponent character.
            if (ExponentText.Length == 0 || ExponentText[0] == '0')
            {
                ValidText = text.Substring(DigitStart, MantissaEnd);
                InvalidText = text.Substring(DigitStart + ValidText.Length);
                DecimalExponent = (IntegerText.Length - 1).ToString();
                return new RealNumber(IntegerText, SeparatorCharacter, FractionalText, ExponentCharacter, OptionalSign.None, string.Empty, InvalidText, new CanonicalNumber(SignificandSign, SignificandText, OptionalSign.None, DecimalExponent));
            }

            // A number with a valid mantissa and explicit exponent.
            ValidText = text.Substring(DigitStart, ExponentEnd - DigitStart);
            InvalidText = text.Substring(DigitStart + ValidText.Length);
            return new RealNumber(IntegerText, SeparatorCharacter, FractionalText, ExponentCharacter, ExponentSign, ExponentText, InvalidText, new CanonicalNumber(SignificandSign, SignificandText, ExponentSign, ExponentText));
        }

        /// <summary>
        /// Remove all zeroes at the begining of a string. If the string is just made of zeroes, leave the last one untouched.
        /// </summary>
        /// <param name="text">The string to clean up.</param>
        /// <param name="leadingZeroesCount">The number of zeroes removed upon return.</param>
        /// <returns>The string with zeroes removed.</returns>
        private protected static string LeadingZeroesRemoved(string text, out int leadingZeroesCount)
        {
            Debug.Assert(text != null);

            leadingZeroesCount = 0;

            while (text.Length > 1 && text[0] == '0')
            {
                text = text.Substring(1);
                leadingZeroesCount++;
            }

            return text;
        }

        /// <summary>
        /// Remove all zeroes at the end of a string. If the string is just made of zeroes, leave the last one untouched.
        /// </summary>
        /// <param name="text">The string to clean up.</param>
        /// <param name="trailingZeroesCount">The number of zeroes removed upon return.</param>
        /// <returns>The string with zeroes removed.</returns>
        private protected static string TrailingZeroesRemoved(string text, out int trailingZeroesCount)
        {
            Debug.Assert(text != null);

            trailingZeroesCount = 0;

            while (text.Length > 1 && text[text.Length - 1] == '0')
            {
                text = text.Substring(0, text.Length - 1);
                trailingZeroesCount++;
            }

            return text;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns the formatted number as a string.
        /// </summary>
        public override string ToString()
        {
            return $"~{InvalidText} ({Canonical})";
        }
        #endregion
    }
}
