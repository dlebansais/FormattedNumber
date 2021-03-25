namespace FormattedNumber
{
    /// <summary>
    /// Hold information during parsing of an integer in hexadecimal base.
    /// </summary>
    internal class HexadecimalIntegerParsingInfo : IntegerWithBaseParsingInfo, IHexadecimalIntegerParsingInfo
    {
        /// <summary>
        /// Gets the base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Hexadecimal; } }
    }
}
