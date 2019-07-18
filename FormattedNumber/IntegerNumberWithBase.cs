namespace FormattedNumber
{
    /// <summary>
    /// The format for a number parsed as an integer, with a base.
    /// </summary>
    public class IntegerNumberWithBase : IntegerNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerNumberWithBase"/> class.
        /// </summary>
        /// <param name="leadingZeroesCount">The number of leading zeroes.</param>
        /// <param name="integerText">The integer text.</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <param name="integerBase">The base.</param>
        public IntegerNumberWithBase(int leadingZeroesCount, string integerText, string invalidText, ICanonicalNumber canonical, IIntegerBase integerBase)
            : base(leadingZeroesCount, integerText, invalidText, canonical)
        {
            IntegerBase = integerBase;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The base.
        /// </summary>
        public IIntegerBase IntegerBase { get; private set; }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns the invalid number as a string.
        /// </summary>
        public override string ToString()
        {
            return $"{IntegerText}[{IntegerBase.Radix}]{base.ToString()}";
        }
        #endregion
    }
}
