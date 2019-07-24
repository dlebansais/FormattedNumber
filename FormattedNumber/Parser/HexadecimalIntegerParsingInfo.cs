namespace FormattedNumber
{
    /// <summary>
    /// Hold information during parsing of an integer in hexadecimal base.
    /// </summary>
    public interface IHexadecimalIntegerParsingInfo : IIntegerWithBaseParsingInfo
    {
    }

    /// <summary>
    /// Hold information during parsing of an integer in hexadecimal base.
    /// </summary>
    public class HexadecimalIntegerParsingInfo : IntegerWithBaseParsingInfo, IHexadecimalIntegerParsingInfo
    {
        /// <summary>
        /// The base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Hexadecimal; } }
    }
}
