namespace FormattedNumber
{
    /// <summary>
    /// Hold information during parsing of an integer in octal base.
    /// </summary>
    public interface IOctalIntegerParsingInfo : IIntegerWithBaseParsingInfo
    {
    }

    /// <summary>
    /// Hold information during parsing of an integer in octal base.
    /// </summary>
    public class OctalIntegerParsingInfo : IntegerWithBaseParsingInfo, IOctalIntegerParsingInfo
    {
        /// <summary>
        /// The base to use when parsing.
        /// </summary>
        protected override IIntegerBase Base { get { return IntegerBase.Octal; } }
    }
}
