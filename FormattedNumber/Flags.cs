namespace FormattedNumber
{
    using PeterO.Numbers;

    /// <summary>
    /// Flag containing information about the result of operations.
    /// </summary>
    public class Flags
    {
        #region Properties
        /// <summary>
        /// Signals a division of a nonzero number by zero.
        /// </summary>
        public bool DivideByZero { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Clear flags.
        /// </summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Update flags after an operation.
        /// </summary>
        /// <param name="context">The context used to perform the operation.</param>
        internal void Update(EContext context)
        {
            if (!context.HasFlags)
                return;

            if ((context.Flags | EContext.FlagDivideByZero) != 0)
                DivideByZero = true;

            context.Flags = 0;
        }
        #endregion
    }
}
