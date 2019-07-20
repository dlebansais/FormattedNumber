namespace FormattedNumber
{
    using System;

    /// <summary>
    /// A real number and its components.
    /// </summary>
    public interface IFormattedReal : IFormattedNumber
    {
        /// <summary>
        /// Gets the optional sign.
        /// </summary>
        OptionalSign Sign { get; }

        /// <summary>
        /// Gets the number of leading zeroes.
        /// </summary>
        int LeadingZeroesCount { get; }

        /// <summary>
        /// Gets the text before the decimal separator character. Can be empty.
        /// </summary>
        string IntegerText { get; }

        /// <summary>
        /// Gets the decimal separator character. Can be <see cref="Parser.NoSeparator"/>.
        /// </summary>
        char SeparatorCharacter { get; }

        /// <summary>
        /// Gets the text after the decimal separator character and before the exponent. Can be empty.
        /// </summary>
        string FractionalText { get; }

        /// <summary>
        /// Gets the exponent character (e or E). Can be <see cref="Parser.NoSeparator"/>.
        /// </summary>
        char ExponentCharacter { get; }

        /// <summary>
        /// Gets the optional exponent sign.
        /// </summary>
        OptionalSign ExponentSign { get; }

        /// <summary>
        /// Gets the text after the exponent character.
        /// </summary>
        string ExponentText { get; }
    }

    /// <summary>
    /// A real number and its components.
    /// </summary>
    public class FormattedReal : FormattedNumber, IFormattedReal
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedReal"/> class.
        /// </summary>
        /// <param name="sign">The optional sign.</param>
        /// <param name="leadingZeroesCount">The number of leading zeroes.</param>
        /// <param name="integerText">The text before the decimal separator character. Can be empty.</param>
        /// <param name="separatorCharacter">The decimal separator character. Can be <see cref="Parser.NoSeparator"/>.</param>
        /// <param name="fractionalText">The text after the decimal separator character and before the exponent. Can be empty.</param>
        /// <param name="exponentCharacter">The exponent character (e or E). Can be <see cref="Parser.NoSeparator"/>.</param>
        /// <param name="exponentSign">The optional exponent sign.</param>
        /// <param name="exponentText">The text after the exponent character. Can be empty.</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        public FormattedReal(OptionalSign sign, int leadingZeroesCount, string integerText, char separatorCharacter, string fractionalText, char exponentCharacter, OptionalSign exponentSign, string exponentText, string invalidText, ICanonicalNumber canonical)
            : base(invalidText, canonical)
        {
            Sign = sign;
            LeadingZeroesCount = leadingZeroesCount;
            IntegerText = integerText ?? throw new NullReferenceException(nameof(integerText));
            SeparatorCharacter = separatorCharacter;
            FractionalText = fractionalText ?? throw new NullReferenceException(nameof(fractionalText));
            ExponentCharacter = exponentCharacter;
            ExponentSign = exponentSign;
            ExponentText = exponentText ?? throw new NullReferenceException(nameof(exponentText));

            if (leadingZeroesCount < 0)
                throw new ArgumentOutOfRangeException(nameof(leadingZeroesCount));

            if (SeparatorCharacter != Parser.NoSeparator)
            {
                if (integerText.Length + fractionalText.Length == 0)
                    throw new ArgumentException($"Either {nameof(integerText)} or {nameof(fractionalText)} must not be empty.");
            }
            else
            {
                if (integerText.Length == 0)
                    throw new ArgumentException($"{nameof(integerText)} must not be empty.");
            }

            if (exponentCharacter != Parser.NoSeparator)
            {
                if (exponentText.Length == 0)
                    throw new ArgumentException($"{nameof(exponentText)} must not be empty.");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the optional sign.
        /// </summary>
        public OptionalSign Sign { get; }

        /// <summary>
        /// Gets the number of leading zeroes. Can be 0.
        /// </summary>
        public int LeadingZeroesCount { get; }

        /// <summary>
        /// Gets the text before the decimal separator character. Can be empty.
        /// </summary>
        public string IntegerText { get; }

        /// <summary>
        /// Gets the decimal separator character. Can be <see cref="Parser.NoSeparator"/>.
        /// </summary>
        public char SeparatorCharacter { get; }

        /// <summary>
        /// Gets the text after the decimal separator character and before the exponent. Can be empty.
        /// </summary>
        public string FractionalText { get; }

        /// <summary>
        /// Gets the exponent character (e or E). Can be <see cref="Parser.NoSeparator"/>.
        /// </summary>
        public char ExponentCharacter { get; }

        /// <summary>
        /// Gets the optional exponent sign.
        /// </summary>
        public OptionalSign ExponentSign { get; }

        /// <summary>
        /// Gets the text after the exponent character. Can be empty.
        /// </summary>
        public string ExponentText { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Returns the formatted number as a string.
        /// </summary>
        public override string ToString()
        {
            string SignText = GetSignText(Sign);
            string LeadingZeroesText = GetLeadingZeroesText(LeadingZeroesCount);
            string ExponentSignText = GetSignText(ExponentSign);

            if (SeparatorCharacter != Parser.NoSeparator)
                return $"{SignText}{LeadingZeroesText}{IntegerText}{SeparatorCharacter}{FractionalText}{ExponentCharacter}{ExponentSignText}{ExponentText}{InvalidText}";
            else
                return $"{SignText}{LeadingZeroesText}{IntegerText}{ExponentCharacter}{ExponentSignText}{ExponentText}{InvalidText}";
        }
        #endregion
    }
}
