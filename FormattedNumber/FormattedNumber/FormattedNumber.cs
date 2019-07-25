namespace FormattedNumber
{
    using System;
    using System.Diagnostics;
    using PeterO.Numbers;

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
        public static FormattedNumber FromCanonical(CanonicalNumber canonical)
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
        protected FormattedNumber(string invalidText, CanonicalNumber canonical)
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
        public CanonicalNumber Canonical { get; }

        /// <summary>
        /// A diagnostic string for debug purpose.
        /// </summary>
        public abstract string Diagnostic { get; }
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
        #endregion
    }
}
