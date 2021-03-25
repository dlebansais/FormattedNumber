namespace FormattedNumber
{
    using System.Diagnostics;
    using EaslyNumber;

    /// <summary>
    /// Interface to manipulate integer or real numbers of any size.
    /// </summary>
    internal class CanonicalNumber
    {
        #region Constants
        /// <summary>
        /// The canonical number for zero.
        /// </summary>
        public static readonly CanonicalNumber Zero = new CanonicalNumber(OptionalSign.None, IntegerBase.Zero, OptionalSign.None, IntegerBase.Zero);

        /// <summary>
        /// The canonical number for NaN.
        /// </summary>
        public static readonly CanonicalNumber NaN = new CanonicalNumber(Number.NaN);

        /// <summary>
        /// The canonical number for positive infinity.
        /// </summary>
        public static readonly CanonicalNumber PositiveInfinity = new CanonicalNumber(Number.PositiveInfinity);

        /// <summary>
        /// The canonical number for negative infinity.
        /// </summary>
        public static readonly CanonicalNumber NegativeInfinity = new CanonicalNumber(Number.NegativeInfinity);
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="significandSign">The sign of the significand.</param>
        /// <param name="integerText">The integer significand.</param>
        public CanonicalNumber(OptionalSign significandSign, string integerText)
        {
            // For the value 0, use the CanonicalNumber.Zero constant.
            Debug.Assert(integerText != IntegerBase.Zero);

            SignificandSign = significandSign;
            SignificandText = integerText + Parser.NeutralDecimalSeparator + IntegerBase.Zero;
            ExponentSign = OptionalSign.None;
            ExponentText = IntegerBase.Zero;

            FormatCanonicString();

            NumberFloat = CreateFloat();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="significandSign">The sign of the significand.</param>
        /// <param name="significandText">The significand.</param>
        /// <param name="exponentSign">The sign of the exponent.</param>
        /// <param name="exponentText">The exponent.</param>
        public CanonicalNumber(OptionalSign significandSign, string significandText, OptionalSign exponentSign, string exponentText)
        {
            Debug.Assert(IntegerBase.Decimal.IsValidNumber(exponentText, supportLeadingZeroes: false));
            Debug.Assert(significandText != IntegerBase.Zero || (significandSign != OptionalSign.Negative && exponentSign != OptionalSign.Negative && exponentText == IntegerBase.Zero));

            SignificandSign = significandSign;
            SignificandText = significandText;
            ExponentSign = exponentSign;
            ExponentText = exponentText;

            FormatCanonicString();

            NumberFloat = CreateFloat();
        }

        private Number CreateFloat()
        {
            return new Number(CanonicRepresentation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="n">An integer.</param>
        public CanonicalNumber(int n)
        {
            if (n < 0)
            {
                n = -n;
                SignificandSign = OptionalSign.Negative;
            }
            else
                SignificandSign = OptionalSign.None;

            SignificandText = n.ToString() + Parser.NeutralDecimalSeparator + IntegerBase.Zero;
            ExponentSign = OptionalSign.None;
            ExponentText = IntegerBase.Zero;

            FormatCanonicString();

            NumberFloat = CreateFloat();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="f">An EFloat.</param>
        public static CanonicalNumber FromEFloat(Number f)
        {
            if (f.IsNaN)
                return NaN;

            if (f.IsPositiveInfinity)
                return PositiveInfinity;

            if (f.IsNegativeInfinity)
                return NegativeInfinity;

            Debug.Assert(!f.IsInfinite);

            OptionalSign SignificandSign;
            string SignificandText;
            OptionalSign ExponentSign;
            string ExponentText;

            string MantissaText = f.ToString();

            Debug.Assert(MantissaText.Length > 0 && (MantissaText[0] != '-' || MantissaText.Length > 1));
            if (MantissaText[0] == '-')
            {
                SignificandSign = OptionalSign.Negative;
                SignificandText = MantissaText.Substring(1);
            }
            else
            {
                SignificandSign = OptionalSign.None;
                SignificandText = MantissaText;
            }

            ExponentSign = OptionalSign.None;
            ExponentText = IntegerBase.Zero;

            CanonicalNumber Result = new CanonicalNumber(SignificandSign, SignificandText, ExponentSign, ExponentText);

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="f">An EFloat.</param>
        private CanonicalNumber(Number f)
        {
            Debug.Assert(f.IsNaN || f.IsInfinite);

            SignificandSign = OptionalSign.None;
            SignificandText = string.Empty;
            ExponentSign = OptionalSign.None;
            ExponentText = string.Empty;

            NumberFloat = f;

            if (f.IsPositiveInfinity)
                CanonicRepresentation = double.PositiveInfinity.ToString();
            else if (f.IsNegativeInfinity)
                CanonicRepresentation = double.NegativeInfinity.ToString();
            else
                CanonicRepresentation = double.NaN.ToString();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the sign of the significand.
        /// </summary>
        public OptionalSign SignificandSign { get; private set; }

        /// <summary>
        /// Gets the significand.
        /// </summary>
        public string SignificandText { get; private set; }

        /// <summary>
        /// Gets the sign of the exponent.
        /// </summary>
        public OptionalSign ExponentSign { get; private set; }

        /// <summary>
        /// Gets the exponent.
        /// </summary>
        public string ExponentText { get; private set; }

        /// <summary>
        /// Gets the canonic representation.
        /// </summary>
        public string CanonicRepresentation { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the float.
        /// </summary>
        public Number NumberFloat { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if two numbers are equal.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public virtual bool IsEqual(CanonicalNumber other)
        {
            return SignificandSign == other.SignificandSign && SignificandText == other.SignificandText && ExponentSign == other.ExponentSign && ExponentText == other.ExponentText;
        }

        /// <summary>
        /// Gets the value if it can be represented with a <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value upon return.</param>
        public bool TryParseInt(out int value)
        {
            return NumberFloat.TryParseInt(out value);
        }

        #endregion

        #region Arithmetic
        /// <summary>
        /// Returns the sum of two numbers: x + y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static CanonicalNumber operator +(CanonicalNumber x, CanonicalNumber y)
        {
            return x.Add(y);
        }

        /// <summary>
        /// Returns the sum of this number and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber Add(CanonicalNumber other)
        {
            CanonicalNumber Result = FromEFloat(NumberFloat + other.NumberFloat);
            return Result;
        }

        /// <summary>
        /// Returns the difference between two numbers: x - y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static CanonicalNumber operator -(CanonicalNumber x, CanonicalNumber y)
        {
            return x.Subtract(y);
        }

        /// <summary>
        /// Returns the difference between this number and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber Subtract(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat - other.NumberFloat);
        }

        /// <summary>
        /// Returns the product of two numbers: x * y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static CanonicalNumber operator *(CanonicalNumber x, CanonicalNumber y)
        {
            return x.Multiply(y);
        }

        /// <summary>
        /// Returns the product of this number and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber Multiply(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat * other.NumberFloat);
        }

        /// <summary>
        /// Returns the ratio of two numbers: x / y.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static CanonicalNumber operator /(CanonicalNumber x, CanonicalNumber y)
        {
            return x.Divide(y);
        }

        /// <summary>
        /// Returns the ratio of this number and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber Divide(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat / other.NumberFloat);
        }

        /// <summary>
        /// Returns the negation of a number: -x.
        /// </summary>
        /// <param name="x">The number.</param>
        public static CanonicalNumber operator -(CanonicalNumber x)
        {
            return x.Negate();
        }

        /// <summary>
        /// Returns the negation of the number.
        /// </summary>
        public CanonicalNumber Negate()
        {
            return FromEFloat(-NumberFloat);
        }

        /// <summary>
        /// Returns the absolute value.
        /// </summary>
        public CanonicalNumber Abs()
        {
            return FromEFloat(NumberFloat.Abs());
        }

        /// <summary>
        /// Returns e (the base of natural logarithms) raised to the power of this object's value.
        /// </summary>
        public CanonicalNumber Exp()
        {
            return FromEFloat(NumberFloat.Exp());
        }

        /// <summary>
        /// Returns the natural logarithms of this object's value.
        /// </summary>
        public CanonicalNumber Log()
        {
            return FromEFloat(NumberFloat.Log());
        }

        /// <summary>
        /// Returns the base-10 logarithms of this object's value.
        /// </summary>
        public CanonicalNumber Log10()
        {
            return FromEFloat(NumberFloat.Log10());
        }

        /// <summary>
        /// Returns this object's value raised to the power x.
        /// </summary>
        /// <param name="x">The number.</param>
        public CanonicalNumber Pow(CanonicalNumber x)
        {
            return FromEFloat(NumberFloat.Pow(x.NumberFloat));
        }

        /// <summary>
        /// Returns the square root of this object's value.
        /// </summary>
        public CanonicalNumber Sqrt()
        {
            return FromEFloat(NumberFloat.Sqrt());
        }

        /// <summary>
        /// Returns this object's value multiplied by a specified power of two.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber ShiftLeft(CanonicalNumber other)
        {
            if (other.TryParseInt(out int Shift))
                return FromEFloat(NumberFloat << Shift);
            else
                return NaN;
        }

        /// <summary>
        /// Returns this object's value divided by a specified power of two.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber ShiftRight(CanonicalNumber other)
        {
            if (other.TryParseInt(out int Shift))
                return FromEFloat(NumberFloat >> Shift);
            else
                return NaN;
        }

        /// <summary>
        /// Returns the remainder when this object's value is divided by another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber Remainder(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat.Remainder(other.NumberFloat));
        }

        /// <summary>
        /// Returns the bitwise AND of this object's value and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber BitwiseAnd(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat & other.NumberFloat);
        }

        /// <summary>
        /// Returns the bitwise OR of this object's value and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber BitwiseOr(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat | other.NumberFloat);
        }

        /// <summary>
        /// Returns the bitwise OR of this object's value and another.
        /// </summary>
        /// <param name="other">The other number.</param>
        public CanonicalNumber BitwiseXor(CanonicalNumber other)
        {
            return FromEFloat(NumberFloat ^ other.NumberFloat);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Checks if <paramref name="x"/> is lesser than <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static bool operator <(CanonicalNumber x, CanonicalNumber y)
        {
            return x.NumberFloat < y.NumberFloat;
        }

        /// <summary>
        /// Checks if <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static bool operator >(CanonicalNumber x, CanonicalNumber y)
        {
            return x.NumberFloat > y.NumberFloat;
        }
        #endregion

        #region Implementation
        private void FormatCanonicString()
        {
            if (SignificandText == IntegerBase.Zero)
                CanonicRepresentation = IntegerBase.Zero;
            else
            {
                string Mantissa;
                string Exponent;

                if (SignificandSign == OptionalSign.Negative)
                    Mantissa = "-" + SignificandText;
                else
                    Mantissa = SignificandText;

                if (ExponentSign == OptionalSign.Negative)
                    Exponent = "-" + ExponentText;
                else
                    Exponent = ExponentText;

                CanonicRepresentation = Mantissa + "e" + Exponent;
            }
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return CanonicRepresentation;
        }
        #endregion
    }
}
