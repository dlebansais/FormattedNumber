namespace FormattedNumber
{
    /// <summary>
    /// Never parses anything.
    /// </summary>
    internal class InvalidParsingInfo : ParsingInfo, IInvalidParsingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidParsingInfo"/> class.
        /// </summary>
        public InvalidParsingInfo()
        {
            StillParsing = false;
            Handler = null;
        }

        /// <summary>
        /// Gets the formatted number that this parser is able to extract.
        /// </summary>
        /// <param name="text">The source string.</param>
        public override FormattedNumber GetResult(string text)
        {
            return new FormattedInvalid(text);
        }
    }
}
