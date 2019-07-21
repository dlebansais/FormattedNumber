namespace FormattedNumber
{
    using System;
    using System.Threading;

    /// <summary>
    /// Specifies how operations on <see cref="CanonicalNumber"/> objects are performed.
    /// </summary>
    public static class Arithmetic
    {
        #region Init
        static Arithmetic()
        {
            Precision = 50;
            ThreadLocalFlags = new ThreadLocal<Flags>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The precision.
        /// </summary>
        public static long Precision
        {
            get { return ValidPrecision; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();

                ValidPrecision = value;
            }
        }
        private static long ValidPrecision;

        /// <summary>
        /// Flag containing information about the result of operations.
        /// </summary>
        public static Flags Flags
        {
            get
            {
                if (!ThreadLocalFlags.IsValueCreated)
                    ThreadLocalFlags.Value = new Flags();

                return ThreadLocalFlags.Value;
            }
        }
        private static ThreadLocal<Flags> ThreadLocalFlags;
        #endregion
    }
}
