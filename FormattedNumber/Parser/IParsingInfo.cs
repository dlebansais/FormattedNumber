namespace FormattedNumber
{
    using System;

    /// <summary>
    /// Hold information during parsing of a number.
    /// </summary>
    internal interface IParsingInfo
    {
        /// <summary>
        /// Gets a value indicating whether parsing is still going on.
        /// </summary>
        bool StillParsing { get; }

        /// <summary>
        /// Gets the current handler for parsing.
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
}
