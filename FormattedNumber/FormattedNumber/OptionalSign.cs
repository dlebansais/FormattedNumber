namespace FormattedNumber
{
    /// <summary>
    /// Optional signs in front of a signficand or exponent.
    /// </summary>
    public enum OptionalSign
    {
        /// <summary>
        /// No sign (same semantic as <see cref="Positive"/>).
        /// </summary>
        None,

        /// <summary>
        /// + sign.
        /// </summary>
        Positive,

        /// <summary>
        /// - sign.
        /// </summary>
        Negative,
    }
}
