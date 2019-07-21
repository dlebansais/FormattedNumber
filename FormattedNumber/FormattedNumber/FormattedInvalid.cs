namespace FormattedNumber
{
    /// <summary>
    /// The format for a number parsed as totally invalid.
    /// </summary>
    public class FormattedInvalid : FormattedNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedInvalid"/> class.
        /// </summary>
        /// <param name="invalidText">The invalid text.</param>
        public FormattedInvalid(string invalidText)
            : base(invalidText, CanonicalNumber.NaN)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedInvalid"/> class.
        /// </summary>
        /// <param name="invalidText">The invalid text.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        public FormattedInvalid(string invalidText, CanonicalNumber canonical)
            : base(invalidText, canonical)
        {
        }
        #endregion
    }
}
