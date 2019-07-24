namespace FormattedNumber
{
    /// <summary>
    /// Hold information during parsing of an integer in binary base.
    /// </summary>
    public interface IBinaryIntegerParsingInfo : IIntegerWithBaseParsingInfo
    {
    }

    /// <summary>
    /// Hold information during parsing of an integer in binary base.
    /// </summary>
    public class BinaryIntegerParsingInfo : IntegerWithBaseParsingInfo, IBinaryIntegerParsingInfo
    {
        /// <summary>
        /// The base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Binary; } }
    }
}
