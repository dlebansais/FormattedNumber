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
            : base(invalidText, CanonicalNumber.Zero)
        {
        }
        #endregion
    }
}
