namespace FormattedNumber
{
    using PeterO.Numbers;

    /// <summary>
    /// Interface to manipulate integer or real numbers of any size.
    /// </summary>
    public class CanonicalNumberInternal
    {
        #region Context
        /// <summary>
        /// The last context used for an operation.
        /// </summary>
        private protected static EContext LastContext
        {
            get
            {
                if (Context == null)
                {
                    EInteger BigPrecision = EInteger.FromInt64(Arithmetic.Precision);
                    EInteger BigExponentMin = EInteger.FromInt64(-128);
                    EInteger BigExponentMax = EInteger.FromInt64(+128);
                    Context = new EContext(BigPrecision, ERounding.HalfEven, BigExponentMin, BigExponentMax, true);
                    Context = Context.WithBlankFlags();
                }

                return Context;
            }
        }
        private static EContext Context;

        private protected static void UpdateFlags()
        {
            Flags Flags = Arithmetic.Flags;
            Flags.Update(LastContext);
        }
        #endregion
    }
}
