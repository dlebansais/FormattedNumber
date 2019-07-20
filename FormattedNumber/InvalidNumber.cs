namespace FormattedNumber
{
    /// <summary>
    /// The format for a number parsed as totally invalid.
    /// </summary>
    public class InvalidNumber : FormattedNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNumber"/> class.
        /// </summary>
        /// <param name="invalidText">The invalid text.</param>
        public InvalidNumber(string invalidText)
            : base(invalidText, CanonicalNumber.Zero)
        {
        }
        #endregion
    }
}
