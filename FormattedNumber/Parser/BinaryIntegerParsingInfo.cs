namespace FormattedNumber
{
    /// <summary>
    /// Hold information during parsing of an integer in binary base.
    /// </summary>
    internal class BinaryIntegerParsingInfo : IntegerWithBaseParsingInfo, IBinaryIntegerParsingInfo
    {
        /// <summary>
        /// Gets the base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Binary; } }
    }
}
