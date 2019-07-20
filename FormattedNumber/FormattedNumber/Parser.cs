﻿namespace FormattedNumber
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Parser to extract a formatted number from any string.
    /// </summary>
    public class Parser
    {
        #region Constants
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
        #endregion

        #region Init
        static Parser()
        {
            string CurrentCultureSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            Debug.Assert(CurrentCultureSeparator.Length == 1);
            DecimalSeparator = CurrentCultureSeparator[0];
        }

        /// <summary>
        /// Parse a string as a number.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <returns>
        /// An instance of <see cref="FormattedInvalid"/> if <paramref name="text"/> cannot be parsed as a number.
        /// An instance of <see cref="FormattedInteger"/> if the valid part of the parsed number is an integer.
        /// An instance of <see cref="FormattedReal"/> if <paramref name="text"/> can be parsed, but not as an integer.
        /// </returns>
        /// <exception cref="NullReferenceException"><paramref name="text"/> is null.</exception>
        public static IFormattedNumber Parse(string text)
        {
            if (text == null)
                throw new NullReferenceException(nameof(text));

            Debug.Assert(text != null);

            // If empty, assume invalid.
            if (text.Length == 0)
                return new FormattedInvalid(text);

            // Parse the optional sign.
            OptionalSign NumberSign;
            int DigitStart;

            if (text[0] == '-')
            {
                NumberSign = OptionalSign.Negative;
                DigitStart = 1;
            }
            else if (text[0] == '+')
            {
                NumberSign = OptionalSign.Positive;
                DigitStart = 1;
            }
            else
            {
                NumberSign = OptionalSign.None;
                DigitStart = 0;
            }

            return ParseWithSign(text, NumberSign, DigitStart);
        }

        private static IFormattedNumber ParseWithSign(string text, OptionalSign numberSign, int digitStart)
        {
            // If only a sign, assume invalid.
            if (text.Length <= digitStart)
                return new FormattedInvalid(text);

            int LeadingZeroesCount;
            string ValidText;
            string InvalidText;
            char SeparatorCharacter;
            char NonDigitCharacter;
            string DecimalExponent;
            string SignificandText;
            ICanonicalNumber Canonical;
            int DigitValue;
            int n;
            OptionalSign ExponentSign;
            string IntegerText;

            // Check the first digit.
            char FirstDigit = text[digitStart];

            // We parse hexadecimal if it's detected at the first digit, and we tolerate real starting directly with the fractional part.
            if ((FirstDigit >= 'a' && FirstDigit <= 'f') || (FirstDigit >= 'A' && FirstDigit <= 'F'))
                return ParseHexadecimal(text, numberSign, digitStart);

            if (FirstDigit == NeutralDecimalSeparator || FirstDigit == DecimalSeparator)
            {
                LeadingZeroesCount = 0;
                NonDigitCharacter = FirstDigit;
                n = digitStart;
                IntegerText = string.Empty;
            }
            else
            {
                // Reject numbers that don't start with a digit.
                if (!IntegerBase.Decimal.IsValidDigit(text[digitStart], out DigitValue))
                    return new FormattedInvalid(text);

                int IntegerStart = digitStart;

                // If the first digit is 0, count leading zeroes.
                if (DigitValue == 0)
                {
                    while (text.Length > IntegerStart && text[IntegerStart] == '0')
                        IntegerStart++;
                }

                LeadingZeroesCount = IntegerStart - digitStart;

                // If the number only contains zeroes, parse it as the value 0.
                if (text.Length == IntegerStart)
                {
                    Debug.Assert(LeadingZeroesCount > 0);

                    // The last zero is not a leading zero if there is nothing else after.
                    LeadingZeroesCount--;

                    return new FormattedInteger(IntegerBase.Decimal, numberSign, LeadingZeroesCount, IntegerBase.Zero, text.Substring(digitStart + 1), CanonicalNumber.Zero);
                }

                // At this point, we have covered 0, 0000, +0, -0, +0000, -0000 etc.
                // We also know the first digit is not a zero.

                // Collect all digits before the decimal separator.
                n = IntegerStart;
                while (n < text.Length && IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                    n++;

                IntegerText = text.Substring(IntegerStart, n - IntegerStart);

                // If the string is only made of decimal digits, return a decimal integer.
                if (n == text.Length)
                {
                    Debug.Assert(IntegerText.Length > 0);
                    Debug.Assert(n > digitStart);
                    DecimalExponent = (n - digitStart + 1).ToString();

                    Canonical = new CanonicalNumber(numberSign, IntegerText, OptionalSign.None, DecimalExponent);
                    return new FormattedInteger(IntegerBase.Decimal, numberSign, LeadingZeroesCount, IntegerText, string.Empty, Canonical);
                }

                // Handle the case of 0 followed by a non-decimal digit.
                if (IntegerText.Length == 0 && LeadingZeroesCount > 0)
                {
                    IntegerText = "0";
                    LeadingZeroesCount--;
                }

                Debug.Assert(IntegerText.Length > 0);
                Debug.Assert(n < text.Length);

                NonDigitCharacter = text[n];

                // We parse hexadecimal if it's detected at any digit.
                if ((NonDigitCharacter >= 'a' && NonDigitCharacter <= 'f') || (NonDigitCharacter >= 'A' && NonDigitCharacter <= 'F'))
                    return ParseHexadecimal(text, numberSign, digitStart);

                string HexadecimalSuffix = IntegerBase.HexadecimalSuffix;
                string OctalSuffix = IntegerBase.OctalSuffix;
                string BinarySuffix = IntegerBase.BinarySuffix;

                // If the number is an hexadecimal integer, with its suffix but just decimal digits.
                if (n + HexadecimalSuffix.Length <= text.Length && text.Substring(n, HexadecimalSuffix.Length) == HexadecimalSuffix)
                {
                    // Convert to decimal. The result can start or finish with a zero
                    string DecimalSignificand = IntegerBase.Convert(IntegerText, IntegerBase.Hexadecimal, IntegerBase.Decimal);
                    DecimalExponent = (DecimalSignificand.Length - 1).ToString();

                    Canonical = new CanonicalNumber(numberSign, DecimalSignificand, OptionalSign.None, DecimalExponent);
                    return new FormattedInteger(IntegerBase.Hexadecimal, numberSign, LeadingZeroesCount, IntegerText, string.Empty, Canonical);
                }
                // If the number is an octal integer.
                else if (n + OctalSuffix.Length <= text.Length && text.Substring(n, OctalSuffix.Length) == OctalSuffix)
                {
                    // Convert to decimal. The result can start or finish with a zero
                    string DecimalSignificand = IntegerBase.Convert(IntegerText, IntegerBase.Octal, IntegerBase.Decimal);
                    DecimalExponent = (DecimalSignificand.Length - 1).ToString();

                    Canonical = new CanonicalNumber(numberSign, DecimalSignificand, OptionalSign.None, DecimalExponent);
                    return new FormattedInteger(IntegerBase.Octal, numberSign, LeadingZeroesCount, IntegerText, string.Empty, Canonical);
                }
                // If the number is a binary integer.
                else if (n + BinarySuffix.Length <= text.Length && text.Substring(n, BinarySuffix.Length) == BinarySuffix)
                {
                    // Convert to decimal. The result can start or finish with a zero
                    string DecimalSignificand = IntegerBase.Convert(IntegerText, IntegerBase.Binary, IntegerBase.Decimal);
                    DecimalExponent = (DecimalSignificand.Length - 1).ToString();

                    Canonical = new CanonicalNumber(numberSign, DecimalSignificand, OptionalSign.None, DecimalExponent);
                    return new FormattedInteger(IntegerBase.Binary, numberSign, LeadingZeroesCount, IntegerText, string.Empty, Canonical);
                }
            }

            string FractionalText;

            // If the significand is followed by a decimal separator.
            if (NonDigitCharacter == NeutralDecimalSeparator || NonDigitCharacter == DecimalSeparator)
            {
                SeparatorCharacter = NonDigitCharacter;

                n++;

                // Get the fractional part.
                int FractionalBegin = n;
                while (n < text.Length && IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                    n++;

                FractionalText = text.Substring(FractionalBegin, n - FractionalBegin);
            }
            else
            {
                SeparatorCharacter = NoSeparator;
                FractionalText = string.Empty;
            }

            // If the fractional part is not followed by an exponent, return the corresponding real number that has no exponent.
            if (n == text.Length || (text[n] != 'e' && text[n] != 'E'))
            {
                InvalidText = text.Substring(n);
                SignificandText = IntegerText + FractionalText;
                int Exponent = IntegerText.Length - 1;
                while (SignificandText.Length > 1 && SignificandText[0] == '0')
                {
                    Exponent--;
                    SignificandText = SignificandText.Substring(1);
                }

                if (Exponent < 0)
                {
                    ExponentSign = OptionalSign.Negative;
                    Exponent = -Exponent;
                }
                else
                    ExponentSign = OptionalSign.None;

                DecimalExponent = Exponent.ToString();

                Canonical = new CanonicalNumber(numberSign, SignificandText, ExponentSign, DecimalExponent);
                return new FormattedReal(numberSign, LeadingZeroesCount, IntegerText, SeparatorCharacter, FractionalText, NoSeparator, ExponentSign, DecimalExponent, InvalidText, Canonical);
            }

            Debug.Assert(n < text.Length);

            char ExponentCharacter = text[n];
            Debug.Assert(ExponentCharacter == 'e' || ExponentCharacter == 'E');

            SignificandText = IntegerText + FractionalText;

            int MantissaEnd = n;
            n++;

            ExponentSign = OptionalSign.None;

            // Allow exponent sign.
            if (n < text.Length)
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
            }

            // Get the exponent.
            int ExponentBegin = n;
            while (n < text.Length && IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                n++;
            int ExponentEnd = n;

            string ExponentText = text.Substring(ExponentBegin, ExponentEnd - ExponentBegin);

            // The exponent cannot be empty, and must not start with zero. In that case, the valid part ends at the exponent character.
            if (ExponentText.Length == 0 || ExponentText[0] == '0')
            {
                ValidText = text.Substring(digitStart, MantissaEnd);
                InvalidText = text.Substring(digitStart + ValidText.Length);
                DecimalExponent = (IntegerText.Length - 1).ToString();
                Canonical = new CanonicalNumber(numberSign, SignificandText, OptionalSign.None, DecimalExponent);
                return new FormattedReal(numberSign, LeadingZeroesCount, IntegerText, SeparatorCharacter, FractionalText, ExponentCharacter, OptionalSign.None, string.Empty, InvalidText, Canonical);
            }

            // A number with a valid mantissa and explicit exponent.
            ValidText = text.Substring(digitStart, ExponentEnd - digitStart);
            InvalidText = text.Substring(digitStart + ValidText.Length);
            Canonical = new CanonicalNumber(numberSign, SignificandText, ExponentSign, ExponentText);
            return new FormattedReal(numberSign, LeadingZeroesCount, IntegerText, SeparatorCharacter, FractionalText, ExponentCharacter, ExponentSign, ExponentText, InvalidText, Canonical);
        }

        private static IFormattedNumber ParseHexadecimal(string text, OptionalSign numberSign, int digitStart)
        {
            // We're here because we know the number has a chance to be an hexadecimal integer.
            Debug.Assert(text.Length > digitStart + 1);

            int LastDecimalDigitOffset = -1;
            int LeadingZeroesCount = -1;
            int DigitValue;

            int n = digitStart;
            while (n < text.Length && IntegerBase.Hexadecimal.IsValidDigit(text[n], out DigitValue))
            {
                if (!IntegerBase.Decimal.IsValidDigit(text[n], out DigitValue))
                {
                    if (LastDecimalDigitOffset < 0)
                        LastDecimalDigitOffset = n;
                }
                else if (DigitValue != 0)
                {
                    if (LeadingZeroesCount < 0)
                        LeadingZeroesCount = n - digitStart;
                }

                n++;
            }

            if (LastDecimalDigitOffset < 0)
                LastDecimalDigitOffset = n;

            if (LeadingZeroesCount < 0)
                LeadingZeroesCount = 0;

            Debug.Assert(n > digitStart);
            string IntegerText = text.Substring(digitStart, n - digitStart);

            // We know the number is not just 0.
            Debug.Assert(LeadingZeroesCount >= 0);

            string HexadecimalSuffix = IntegerBase.HexadecimalSuffix;

            // To be valid, the hexadecimal integer must be followed by the corresponding suffix.
            if (n + HexadecimalSuffix.Length <= text.Length && text.Substring(n, HexadecimalSuffix.Length) == HexadecimalSuffix)
            {
                // Convert to decimal. The result can start or finish with a zero
                string DecimalSignificand = IntegerBase.Convert(IntegerText, IntegerBase.Hexadecimal, IntegerBase.Decimal);
                string DecimalExponent = (DecimalSignificand.Length - 1).ToString();

                ICanonicalNumber Canonical = new CanonicalNumber(numberSign, DecimalSignificand, OptionalSign.None, DecimalExponent);
                return new FormattedInteger(IntegerBase.Hexadecimal, numberSign, LeadingZeroesCount, IntegerText, string.Empty, Canonical);
            }
            else
            {
                // If not valid, the number is the best decimal integer we can get.
                IntegerText = text.Substring(digitStart, LastDecimalDigitOffset - digitStart);
                string InvalidText = text.Substring(LastDecimalDigitOffset);
                string DecimalExponent = (IntegerText.Length - 1).ToString();

                ICanonicalNumber Canonical = new CanonicalNumber(numberSign, IntegerText, OptionalSign.None, DecimalExponent);
                return new FormattedInteger(IntegerBase.Hexadecimal, numberSign, LeadingZeroesCount, IntegerText, string.Empty, Canonical);
            }
        }
        #endregion
    }
}