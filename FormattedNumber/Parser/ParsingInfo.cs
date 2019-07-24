namespace FormattedNumber
{
    using System;

    /// <summary>
    /// Hold information during parsing of a number.
    /// </summary>
    public interface IParsingInfo
    {
        /// <summary>
        /// True if parsing is still going on.
        /// </summary>
        bool StillParsing { get; }

        /// <summary>
        /// Current handler for parsing.
        /// </summary>
        Action<char> Handler { get; }

        /// <summary>
        /// Checks if the current parser went further than others.
        /// </summary>
        /// <param name="parsing">The previous best parser.</param>
        /// <param name="length">The length reached by <paramref name="parsing"/>.</param>
        void UpdateBestParsing(ref IParsingInfo parsing, ref int length);

        /// <summary>
        /// Gets the formatted number that this parser is able to extract.
        /// </summary>
        /// <param name="text">The source string.</param>
        FormattedNumber GetResult(string text);
    }

    /// <summary>
    /// Hold information during parsing of a number.
    /// </summary>
    public abstract class ParsingInfo : IParsingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingInfo"/> class.
        /// </summary>
        protected ParsingInfo()
        {
            StillParsing = true;
            LengthSuccessful = 0;
        }

        /// <summary>
        /// True if parsing is still going on.
        /// </summary>
        public bool StillParsing { get; protected set; }

        /// <summary>
        /// Current handler for parsing.
        /// </summary>
        public Action<char> Handler { get; protected set; }

        /// <summary>
        /// Number of characters successfully parsed.
        /// </summary>
        protected int LengthSuccessful { get; set; }

        /// <summary>
        /// Checks if the current parser went further than others.
        /// </summary>
        /// <param name="parsing">The previous best parser.</param>
        /// <param name="length">The length reached by <paramref name="parsing"/>.</param>
        public virtual void UpdateBestParsing(ref IParsingInfo parsing, ref int length)
        {
            if (length < LengthSuccessful)
            {
                parsing = this;
                length = LengthSuccessful;
            }
        }

        /// <summary>
        /// Gets the formatted number that this parser is able to extract.
        /// </summary>
        /// <param name="text">The source string.</param>
        public abstract FormattedNumber GetResult(string text);
    }
}
