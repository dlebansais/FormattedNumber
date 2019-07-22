namespace FormattedNumber
{
    /// <summary>
    /// Class to hold information during parsing of an integer in binary base.
    /// </summary>
    public class BinaryIntegerParsingInfo : IntegerWithBaseParsingInfo
    {
        /// <summary>
        /// The base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Binary; } }
    }
}
