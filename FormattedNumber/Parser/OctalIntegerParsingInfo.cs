namespace FormattedNumber
{
    /// <summary>
    /// Hold information during parsing of an integer in octal base.
    /// </summary>
    internal class OctalIntegerParsingInfo : IntegerWithBaseParsingInfo, IOctalIntegerParsingInfo
    {
        /// <summary>
        /// Gets the base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Octal; } }
    }
}
