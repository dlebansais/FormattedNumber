namespace FormattedNumber
{
    /// <summary>
    /// Class that never parses anything.
    /// </summary>
    public class InvalidParsingInfo : ParsingInfo
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
