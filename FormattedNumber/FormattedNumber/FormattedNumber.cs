namespace FormattedNumber
{
    using System;
    using System.Diagnostics;
    using EaslyNumber;

    /// <summary>
    /// Base class for a number format that can parse any string.
    /// </summary>
    public abstract class FormattedNumber
    {
        #region Constants
        /// <summary>
        /// The formatted number for NaN.
        /// </summary>
        public static readonly FormattedNumber NaN = new FormattedInvalid(double.NaN.ToString(), CanonicalNumber.NaN);

        /// <summary>
        /// The canonical number for positive infinity.
        /// </summary>
        public static readonly FormattedNumber PositiveInfinity = new FormattedInvalid(double.PositiveInfinity.ToString(), CanonicalNumber.PositiveInfinity);

        /// <summary>
        /// The canonical number for negative infinity.
        /// </summary>
        public static readonly FormattedNumber NegativeInfinity = new FormattedInvalid(double.NegativeInfinity.ToString(), CanonicalNumber.NegativeInfinity);
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="canonical"/> is null.</exception>
        internal static FormattedNumber FromCanonical(CanonicalNumber canonical)
        {
            if (canonical == null)
                throw new ArgumentNullException(nameof(canonical), "Value cannot be null.");

            if (canonical == CanonicalNumber.NaN)
                return NaN;

            if (canonical == CanonicalNumber.PositiveInfinity)
                return PositiveInfinity;

            if (canonical == CanonicalNumber.NegativeInfinity)
                return NegativeInfinity;

            string SignificandText = canonical.SignificandText;

            int SeparatorOffset = SignificandText.IndexOf(Parser.NeutralDecimalSeparator);
            string IntegerText;
            char SeparatorCharacter;
            string FractionalText;

            if (SeparatorOffset > 0)
            {
                IntegerText = SignificandText.Substring(0, SeparatorOffset);
                SeparatorCharacter = Parser.DecimalSeparator;
                FractionalText = SignificandText.Substring(SeparatorOffset + 1);
            }
            else
            {
                IntegerText = SignificandText;
                SeparatorCharacter = Parser.NoSeparator;
                FractionalText = string.Empty;
            }

            return new FormattedReal(canonical.SignificandSign, 0, IntegerText, SeparatorCharacter, FractionalText, 'e', canonical.ExponentSign, canonical.ExponentText, string.Empty, canonical);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        internal FormattedNumber(string invalidText, CanonicalNumber canonical)
        {
            InvalidText = invalidText ?? throw new ArgumentNullException(nameof(invalidText), "Value cannot be null.");
            Canonical = canonical ?? throw new ArgumentNullException(nameof(canonical), "Value cannot be null.");
        }
        #endregion

        #region Properties
        /// <summary>
        /// The significand part. Can be empty.
        /// This includes all characters up to and including the exponent character.
        /// </summary>
        public abstract string SignificandPart { get; }

        /// <summary>
        /// The exponent part. Can be empty.
        /// This includes all characters after the exponent character and before the invalid text.
        /// </summary>
        public abstract string ExponentPart { get; }

        /// <summary>
        /// The trailing invalid text, if any.
        /// </summary>
        public string InvalidText { get; }

        /// <summary>
        /// True if the number is valid.
        /// A valid number is finite in the sense of arithmetic (not NaN, not infinite), and has no trailing invalid text.
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// The canonical form of the parsed number.
        /// </summary>
        internal CanonicalNumber Canonical { get; }

        /// <summary>
        /// The the parsed number.
        /// </summary>
        public Number Value { get { return Canonical.NumberFloat; } }

        /// <summary>
        /// A diagnostic string for debug purpose.
        /// </summary>
        public abstract string Diagnostic { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Parse a string as a number.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <returns>
        /// An instance of <see cref="FormattedInvalid"/> if <paramref name="text"/> cannot be parsed as a number.
        /// An instance of <see cref="FormattedInteger"/> if the valid part of the parsed number is an integer.
        /// An instance of <see cref="FormattedReal"/> if <paramref name="text"/> can be parsed, but not as an integer.
        /// </returns>
        public static FormattedNumber Parse(string text)
        {
            return Parser.Parse(text);
        }
        #endregion

        #region Implementation
        /// <summary></summary>
        protected string GetSignText(OptionalSign sign)
        {
            string Result = null;

            switch (sign)
            {
                case OptionalSign.None:
                    Result = string.Empty;
                    break;

                case OptionalSign.Positive:
                    Result = "+";
                    break;

                case OptionalSign.Negative:
                    Result = "-";
                    break;
            }

            Debug.Assert(Result != null);

            return Result;
        }

        /// <summary></summary>
        protected string GetLeadingZeroesText(int leadingZeroCount)
        {
            string Result = string.Empty;

            for (int i = 0; i < leadingZeroCount; i++)
                Result += IntegerBase.Zero;

            return Result;
        }
        #endregion

        #region Arithmetic
        /// <summary>
        /// Returns the sum of two numbers: x + y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static FormattedNumber operator +(FormattedNumber x, FormattedNumber y)
        {
            CanonicalNumber OperationResult = x.Canonical + y.Canonical;

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the difference of two numbers: x - y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static FormattedNumber operator -(FormattedNumber x, FormattedNumber y)
        {
            CanonicalNumber OperationResult = x.Canonical - y.Canonical;

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the product of two numbers: x * y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static FormattedNumber operator *(FormattedNumber x, FormattedNumber y)
        {
            CanonicalNumber OperationResult = x.Canonical * y.Canonical;

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the ratio of two numbers: x / y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static FormattedNumber operator /(FormattedNumber x, FormattedNumber y)
        {
            CanonicalNumber OperationResult = x.Canonical / y.Canonical;

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the negation of a number: -x.
        /// </summary>
        /// <param name="x">The number.</param>
        public static FormattedNumber operator -(FormattedNumber x)
        {
            CanonicalNumber OperationResult = -x.Canonical;

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the absolute value.
        /// </summary>
        public FormattedNumber Abs()
        {
            CanonicalNumber OperationResult = Canonical.Abs();

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns e (the base of natural logarithms) raised to the power of this object's value.
        /// </summary>
        public FormattedNumber Exp()
        {
            CanonicalNumber OperationResult = Canonical.Exp();

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the natural logarithms of this object's value.
        /// </summary>
        public FormattedNumber Log()
        {
            CanonicalNumber OperationResult = Canonical.Log();

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the base-10 logarithms of this object's value.
        /// </summary>
        public FormattedNumber Log10()
        {
            CanonicalNumber OperationResult = Canonical.Log10();

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns this object's value raised to the power x.
        /// </summary>
        /// <param name="x">The number.</param>
        public FormattedNumber Pow(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.Pow(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the square root of this object's value.
        /// </summary>
        public FormattedNumber Sqrt()
        {
            CanonicalNumber OperationResult = Canonical.Sqrt();

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns this object's value multiplied by a specified power of two.
        /// </summary>
        /// <param name="x">The number.</param>
        public FormattedNumber ShiftLeft(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.ShiftLeft(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns this object's value divided by a specified power of two.
        /// </summary>
        /// <param name="x">The number.</param>
        public FormattedNumber ShiftRight(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.ShiftRight(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the remainder when this object's value is divided by x.
        /// </summary>
        /// <param name="x">The divisor.</param>
        public FormattedNumber Remainder(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.Remainder(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the bitwise AND of this object's value and x.
        /// </summary>
        /// <param name="x">The number.</param>
        public FormattedNumber BitwiseAnd(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.BitwiseAnd(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the bitwise OR of this object's value and x.
        /// </summary>
        /// <param name="x">The number.</param>
        public FormattedNumber BitwiseOr(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.BitwiseOr(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }

        /// <summary>
        /// Returns the bitwise XOR of this object's value and x.
        /// </summary>
        /// <param name="x">The number.</param>
        public FormattedNumber BitwiseXor(FormattedNumber x)
        {
            CanonicalNumber OperationResult = Canonical.BitwiseXor(x.Canonical);

            FormattedNumber Result = FromCanonical(OperationResult);
            return Result;
        }
        #endregion
    }
}
