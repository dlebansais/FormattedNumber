namespace FormattedNumber
{
    using System.Diagnostics;
    using PeterO.Numbers;

    /// <summary>
    /// Interface to manipulate integer or real numbers of any size.
    /// </summary>
    public interface ICanonicalNumber
    {
        /// <summary>
        /// The sign of the significand.
        /// </summary>
        OptionalSign SignificandSign { get; }

        /// <summary>
        /// The significand.
        /// </summary>
        string SignificandText { get; }

        /// <summary>
        /// The sign of the exponent.
        /// </summary>
        OptionalSign ExponentSign { get; }

        /// <summary>
        /// The exponent.
        /// </summary>
        string ExponentText { get; }

        /// <summary>
        /// The canonic representation.
        /// </summary>
        string CanonicRepresentation { get; }

        /// <summary>
        /// Checks if two numbers are equal.
        /// </summary>
        /// <param name="other">The other instance.</param>
        bool IsEqual(ICanonicalNumber other);

        /// <summary>
        /// Returns the opposite number.
        /// </summary>
        ICanonicalNumber OppositeOf();

        /// <summary>
        /// Checks if this instance is greater than another constant.
        /// </summary>
        bool IsGreater(ICanonicalNumber other);

        /// <summary>
        /// Gets the value if it can be represented with a <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value upon return.</param>
        bool TryParseInt(out int value);
    }

    /// <summary>
    /// Interface to manipulate integer or real numbers of any size.
    /// </summary>
    public class CanonicalNumber : ICanonicalNumber
    {
        #region Constants
        /// <summary>
        /// The canonical number for zero.
        /// </summary>
        public static readonly CanonicalNumber Zero = new CanonicalNumber(OptionalSign.None, IntegerBase.Zero, OptionalSign.None, IntegerBase.Zero);
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalNumber"/> class.
        /// </summary>
        /// <param name="significandSign">The sign of the significand.</param>
        /// <param name="significandText">The significand.</param>
        /// <param name="exponentSign">The sign of the exponent.</param>
        /// <param name="exponentText">The exponent.</param>
        public CanonicalNumber(OptionalSign significandSign, string significandText, OptionalSign exponentSign, string exponentText)
        {
            Debug.Assert(IntegerBase.Decimal.IsValidSignificand(significandText));
            Debug.Assert(IntegerBase.Decimal.IsValidNumber(exponentText, supportLeadingZeroes: false));
            Debug.Assert(significandText != IntegerBase.Zero || (significandSign != OptionalSign.Negative && exponentSign != OptionalSign.Negative && exponentText == IntegerBase.Zero));

            SignificandSign = significandSign;
            SignificandText = significandText;
            ExponentSign = exponentSign;
            ExponentText = exponentText;

            FormatCanonicString();

            eFloat = CreateEFloat();
        }

        private EFloat CreateEFloat()
        {
            EInteger Mantissa = CreateEInteger(SignificandSign, SignificandText);
            EInteger Exponent = CreateEInteger(ExponentSign, ExponentText);
            return EFloat.Create(Exponent, Mantissa);
        }

        private EInteger CreateEInteger(OptionalSign sign, string text)
        {
            string SignedText = sign == OptionalSign.Negative ? "-" + text : text;
            return EInteger.FromRadixString(SignedText, IntegerBase.DecimalRadix);
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

            string s = n.ToString();

            ExponentSign = OptionalSign.None;
            ExponentText = (s.Length - 1).ToString();

            while (s.Length > 1 && s[s.Length - 1] == '0')
                s = s.Substring(0, s.Length - 1);

            SignificandText = s;

            FormatCanonicString();
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
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if two numbers are equal.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public virtual bool IsEqual(ICanonicalNumber other)
        {
            return SignificandSign == other.SignificandSign && SignificandText == other.SignificandText && ExponentSign == other.ExponentSign && ExponentText == other.ExponentText;
        }

        /// <summary>
        /// Returns the opposite number.
        /// </summary>
        public virtual ICanonicalNumber OppositeOf()
        {
            return new CanonicalNumber(SignificandSign == OptionalSign.Negative ? OptionalSign.None : OptionalSign.Negative, SignificandText, ExponentSign, ExponentText);
        }

        /// <summary>
        /// Checks if this instance is greater than another constant.
        /// </summary>
        public virtual bool IsGreater(ICanonicalNumber other)
        {
            return this > (CanonicalNumber)other;
        }

        /// <summary>
        /// Gets the value if it can be represented with a <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value upon return.</param>
        public bool TryParseInt(out int value)
        {
            value = 0;

            if (ExponentSign == OptionalSign.Negative)
                return false;

            if (SignificandText.Length > 10 || ExponentText.Length > 1)
                return false;

            int Significand;
            int Exponent;
            if (!int.TryParse(SignificandText, out Significand) || !int.TryParse(ExponentText, out Exponent))
                return false;

            if (Exponent + 1 < SignificandText.Length)
                return false;

            value = Significand;
            int RemainingDigits = Exponent + 1 - SignificandText.Length;

            while (RemainingDigits-- > 0)
                value *= 10;

            if (SignificandSign == OptionalSign.Negative)
                value = -value;

            return true;
        }

        /// <summary>
        /// Checks if <paramref name="number1"/> is lesser than <paramref name="number2"/>.
        /// </summary>
        /// <param name="number1">The first number.</param>
        /// <param name="number2">The second number.</param>
        public static bool operator <(CanonicalNumber number1, CanonicalNumber number2)
        {
            bool IsNegative1 = number1.SignificandSign == OptionalSign.Negative;
            bool IsNegative2 = number2.SignificandSign == OptionalSign.Negative;

            // Compare positive and negative numbers.
            if (IsNegative1 != IsNegative2)
                return IsNegative1;

            bool IsExponentNegative1 = number1.ExponentSign == OptionalSign.Negative;
            bool IsExponentNegative2 = number2.ExponentSign == OptionalSign.Negative;

            // If both positive or negative, compare positive and negative exponents.
            if (IsExponentNegative1 && !IsExponentNegative2)
                return !IsNegative1;

            else if (!IsExponentNegative1 && IsExponentNegative2)
                return IsNegative1;

            // If signs of significands and signs of exponents are identical.
            else
            {
                int ComparedExponent = string.Compare(number1.ExponentText, number2.ExponentText);

                if (ComparedExponent < 0)
                    return IsNegative1 == IsExponentNegative1;
                else if (ComparedExponent > 0)
                    return IsNegative1 != IsExponentNegative1;

                // If exponents are identical, compare significands.
                else
                {
                    int ComparedSignificand = string.Compare(number1.SignificandText, number2.SignificandText);

                    // If they are equal, return not lesser than.
                    if (ComparedSignificand == 0)
                        return false;
                    else
                        return (ComparedSignificand < 0) == IsNegative1;
                }
            }
        }

        /// <summary>
        /// Checks if <paramref name="number1"/> is greater than <paramref name="number2"/>.
        /// </summary>
        /// <param name="number1">The first number.</param>
        /// <param name="number2">The second number.</param>
        public static bool operator >(CanonicalNumber number1, CanonicalNumber number2)
        {
            return number2 < number1;
        }
        #endregion

        #region Implementation
        private void FormatCanonicString()
        {
            if (SignificandText == IntegerBase.Zero)
                CanonicRepresentation = IntegerBase.Zero;

            else
            {
                if (SignificandText.Length == 1)
                    CanonicRepresentation = SignificandText[0] + ".0e" + (ExponentSign == OptionalSign.Negative ? "-" : "+") + ExponentText;

                else
                    CanonicRepresentation = SignificandText[0] + "." + SignificandText.Substring(1) + "e" + (ExponentSign == OptionalSign.Negative ? "-" : "+") + ExponentText;

                if (SignificandSign == OptionalSign.Negative)
                    CanonicRepresentation = '-' + CanonicRepresentation;
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

        private EFloat eFloat;
    }
}
