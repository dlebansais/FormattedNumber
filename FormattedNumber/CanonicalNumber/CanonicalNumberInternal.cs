namespace FormattedNumber
{
    using EaslyNumber;

    /// <summary>
    /// Interface to manipulate integer or real numbers of any size.
    /// </summary>
    internal class CanonicalNumberInternal
    {
#if NONE
#region Context
        /// <summary>
        /// The last context used for an operation.
        /// </summary>
        private protected static string LastContext
        {
            get
            {
                if (Context == null)
                {
                    Context = string.Empty;
                }

                return Context;
            }
        }
        private static string Context;

        private protected static void UpdateFlags()
        {
            Flags Flags = Arithmetic.Flags;
            Flags.Update(LastContext);
        }

        private protected static void UpdateInexact(bool isInexact)
        {
            if (isInexact)
            {
                Flags Flags = Arithmetic.Flags;
                Flags.SetInexact();
            }
        }
#endregion
#endif
    }
}
