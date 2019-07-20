namespace FormattedNumber
{
    using System;

    /// <summary>
    /// A real number and its components.
    /// </summary>
    public interface IFormattedInteger : IFormattedNumber
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
        /// Gets the integer text.
        /// </summary>
        string IntegerText { get; }

        /// <summary>
        /// The base.
        /// </summary>
        IIntegerBase IntegerBase { get; }
    }

    /// <summary>
    /// A real number and its components.
    /// </summary>
    public class FormattedInteger : FormattedNumber, IFormattedInteger
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedInteger"/> class.
        /// </summary>
        /// <param name="integerBase">The base.</param>
        /// <param name="sign">The optional sign.</param>
        /// <param name="leadingZeroesCount">The number of leading zeroes.</param>
        /// <param name="integerText">The integer text..</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        public FormattedInteger(IIntegerBase integerBase, OptionalSign sign, int leadingZeroesCount, string integerText, string invalidText, ICanonicalNumber canonical)
            : base(invalidText, canonical)
        {
            Sign = sign;
            LeadingZeroesCount = leadingZeroesCount;
            IntegerText = integerText ?? throw new NullReferenceException(nameof(integerText));

            if (leadingZeroesCount < 0)
                throw new ArgumentOutOfRangeException(nameof(leadingZeroesCount));
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
        /// The base.
        /// </summary>
        public IIntegerBase IntegerBase { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Returns the formatted number as a string.
        /// </summary>
        public override string ToString()
        {
            string SignText = GetSignText(Sign);
            string LeadingZeroesText = GetLeadingZeroesText(LeadingZeroesCount);

            return $"{SignText}{LeadingZeroesText}{IntegerText}{IntegerBase.Suffix}{InvalidText}";
        }
        #endregion
    }
}
