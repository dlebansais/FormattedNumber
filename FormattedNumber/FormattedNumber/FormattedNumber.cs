namespace FormattedNumber
{
    using PeterO.Numbers;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Base interface for a number format that can parse any string.
    /// </summary>
    public interface IFormattedNumber
    {
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
    /// Base class for a number format that can parse any string.
    /// </summary>
    public abstract class FormattedNumber : IFormattedNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="canonical"/> is null.</exception>
        public static IFormattedNumber FromCanonical(ICanonicalNumber canonical)
        {
            EFloat NumberFloat = canonical.NumberFloat;

            EInteger Mantissa = NumberFloat.Mantissa;
            string MantissaText = Mantissa.ToRadixString(IntegerBase.DecimalRadix);
            OptionalSign Sign = MantissaText[0] == '-' ? OptionalSign.Negative : OptionalSign.None;
            if (MantissaText[0] == '-')
                MantissaText = MantissaText.Substring(1);

            EInteger Exponent = NumberFloat.Exponent;
            string ExponentText = Exponent.ToRadixString(IntegerBase.DecimalRadix);
            OptionalSign ExponentSign = ExponentText[0] == '-' ? OptionalSign.Negative : OptionalSign.None;
            if (ExponentText[0] == '-')
                ExponentText = ExponentText.Substring(1);

            return new FormattedReal(Sign, 0, MantissaText, '.', "0", 'e', ExponentSign, ExponentText, string.Empty, canonical);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedNumber"/> class.
        /// </summary>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        protected FormattedNumber(string invalidText, ICanonicalNumber canonical)
        {
            InvalidText = invalidText ?? throw new NullReferenceException(nameof(invalidText));
            Canonical = canonical ?? throw new NullReferenceException(nameof(canonical));
        }
        #endregion

        #region Properties
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
        protected string GetLeadingZeroesText(int leadingZeroesCount)
        {
            string Result = string.Empty;

            for (int i = 0; i < leadingZeroesCount; i++)
                Result += "0";

            return Result;
        }
        #endregion
    }
}
