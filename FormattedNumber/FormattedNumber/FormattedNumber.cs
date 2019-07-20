namespace FormattedNumber
{
    using System;
    using System.Diagnostics;
    using PeterO.Numbers;

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
            string SignificandText = canonical.SignificandText;

            int SeparatorOffset = SignificandText.IndexOf(Parser.NeutralDecimalSeparator);
            string IntegerText;
            char SeparatorCharacter;
            string FractionalText;

            if (SeparatorOffset > 0)
            {
                IntegerText = SignificandText.Substring(0, SeparatorOffset);
                SeparatorCharacter = Parser.NeutralDecimalSeparator;
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
                Result += IntegerBase.Zero;

            return Result;
        }
        #endregion
    }
}
