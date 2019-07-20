namespace FormattedNumber
{
    using System;

    /// <summary>
    /// An octal integer and its components.
    /// </summary>
    public interface IFormattedOctalInteger : IFormattedInteger
    {
    }

    /// <summary>
    /// An octal integer and its components.
    /// </summary>
    public class FormattedOctalInteger : FormattedInteger, IFormattedOctalInteger
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedOctalInteger"/> class.
        /// </summary>
        /// <param name="sign">The optional sign.</param>
        /// <param name="leadingZeroesCount">The number of leading zeroes.</param>
        /// <param name="integerText">The integer text.</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        public FormattedOctalInteger(OptionalSign sign, int leadingZeroesCount, string integerText, string invalidText, ICanonicalNumber canonical)
            : base(sign, leadingZeroesCount, integerText, invalidText, canonical)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the suffix to add to the integer text.
        /// </summary>
        protected override string SuffixText { get { return IntegerBase.OctalSuffix; } }
        #endregion
    }
}
