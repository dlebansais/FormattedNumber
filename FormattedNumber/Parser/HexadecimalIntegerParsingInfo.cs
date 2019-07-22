namespace FormattedNumber
{
    /// <summary>
    /// Class to hold information during parsing of an integer in hexadecimal base.
    /// </summary>
    public class HexadecimalIntegerParsingInfo : IntegerWithBaseParsingInfo
    {
        /// <summary>
        /// The base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Hexadecimal; } }
    }
}
