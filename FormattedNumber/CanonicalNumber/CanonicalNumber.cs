namespace FormattedNumber
{
    using System.Diagnostics;
    using PeterO.Numbers;

    /// <summary>
    /// Interface to manipulate integer or real numbers of any size.
    /// </summary>
    public class CanonicalNumber : CanonicalNumberInternal
    {
        #region Constants
        /// <summary>
        /// The canonical number for zero.
        /// </summary>
        public static readonly CanonicalNumber Zero = new CanonicalNumber(OptionalSign.None, IntegerBase.Zero, OptionalSign.None, IntegerBase.Zero);

        /// <summary>
        /// The canonical number for NaN.
        /// </summary>
        public static readonly CanonicalNumber NaN = new CanonicalNumber(EFloat.NaN);

        /// <summary>
        /// The canonical number for positive infinity.
        /// </summary>
        public static readonly CanonicalNumber PositiveInfinity = new CanonicalNumber(EFloat.PositiveInfinity);

        /// <summary>
        /// The canonical number for negative infinity.
        /// </summary>
        public static readonly CanonicalNumber NegativeInfinity = new CanonicalNumber(EFloat.NegativeInfinity);
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

            NumberFloat = CreateEFloat();
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

            NumberFloat = CreateEFloat();
        }

        private EFloat CreateEFloat()
        {
            return EFloat.FromString(CanonicRepresentation);
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

            NumberFloat = CreateEFloat();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="f">An EFloat.</param>
        public static CanonicalNumber FromEFloat(EFloat f)
        {
            if (f.IsNaN())
                return NaN;

            if (f.IsPositiveInfinity())
                return PositiveInfinity;

            if (f.IsNegativeInfinity())
                return NegativeInfinity;

            Debug.Assert(f.IsFinite);

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
        private CanonicalNumber(EFloat f)
        {
            Debug.Assert(f.IsNaN() || f.IsInfinity());

            SignificandSign = OptionalSign.None;
            SignificandText = null;
            ExponentSign = OptionalSign.None;
            ExponentText = null;

            NumberFloat = f;

            if (f.IsPositiveInfinity())
                CanonicRepresentation = double.PositiveInfinity.ToString();
            else if (f.IsNegativeInfinity())
                CanonicRepresentation = double.NegativeInfinity.ToString();
            else
                CanonicRepresentation = double.NaN.ToString();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The sign of the significand.
        /// </summary>
        public OptionalSign SignificandSign { get; private set; }

        /// <summary>
        /// The significand.
        /// </summary>
        public string SignificandText { get; private set; }

        /// <summary>
        /// The sign of the exponent.
        /// </summary>
        public OptionalSign ExponentSign { get; private set; }

        /// <summary>
        /// The exponent.
        /// </summary>
        public string ExponentText { get; private set; }

        /// <summary>
        /// The canonic representation.
        /// </summary>
        public string CanonicRepresentation { get; private set; }

        /// <summary>
        /// The float.
        /// </summary>
        public EFloat NumberFloat { get; private set; }
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
            value = 0;

            EInteger eInteger = NumberFloat.ToEInteger();
            EFloat eFloat = EFloat.FromEInteger(eInteger);

            if (!eFloat.Equals(NumberFloat))
                return false;

            if (!eInteger.CanFitInInt32())
                return false;

            value = eInteger.ToInt32Unchecked();
            return true;
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
            EFloat OperationResult = NumberFloat.Add(other.NumberFloat, LastContext);
            UpdateFlags();

            CanonicalNumber Result = FromEFloat(OperationResult);
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
            EFloat OperationResult = NumberFloat.Subtract(other.NumberFloat, LastContext);
            UpdateFlags();

            CanonicalNumber Result = FromEFloat(OperationResult);
            return Result;
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
            EFloat OperationResult = NumberFloat.Multiply(other.NumberFloat, LastContext);
            UpdateFlags();

            CanonicalNumber Result = FromEFloat(OperationResult);
            return Result;
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
            EFloat OperationResult = NumberFloat.Divide(other.NumberFloat, LastContext);
            UpdateFlags();

            CanonicalNumber Result = FromEFloat(OperationResult);
            return Result;
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
            EFloat OperationResult = NumberFloat.Negate(LastContext);
            UpdateFlags();

            CanonicalNumber Result = FromEFloat(OperationResult);
            return Result;
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
            bool Result = x.NumberFloat.CompareToTotal(y.NumberFloat, LastContext) < 0;
            return Result;
        }

        /// <summary>
        /// Checks if <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The first number.</param>
        /// <param name="y">The second number.</param>
        public static bool operator >(CanonicalNumber x, CanonicalNumber y)
        {
            return y < x;
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
