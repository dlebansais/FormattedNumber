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
        internal FormattedInvalid(string invalidText, CanonicalNumber canonical)
            : base(invalidText, canonical)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the significand part. Can be empty.
        /// This includes all characters up to and including the exponent character.
        /// </summary>
        public override string SignificandPart { get { return string.Empty; } }

        /// <summary>
        /// Gets the exponent part. Can be empty.
        /// This includes all characters after the exponent character and before the invalid text.
        /// </summary>
        public override string ExponentPart { get { return string.Empty; } }

        /// <summary>
        /// Gets a value indicating whether the number is valid.
        /// A valid number is finite in the sense of arithmetic (not NaN, not infinite), and has no trailing invalid text.
        /// </summary>
        public override bool IsValid { get { return false; } }

        /// <summary>
        /// Gets a diagnostic string for debug purpose.
        /// </summary>
        public override string Diagnostic { get { return $"{Canonical}/{InvalidText}"; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Returns the formatted number as a string.
        /// </summary>
        public override string ToString()
        {
            return InvalidText;
        }
        #endregion
    }
}
