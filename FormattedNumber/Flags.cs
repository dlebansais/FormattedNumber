namespace FormattedNumber
{
    using System.Diagnostics;
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

        /// <summary>
        /// Signals that the result was rounded to a different mathematical value, but as close as possible to the original.
        /// </summary>
        public bool Inexact { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Clear flags.
        /// </summary>
        public void Clear()
        {
            DivideByZero = false;
            Inexact = false;
        }

        /// <summary>
        /// Update flags after an operation.
        /// </summary>
        /// <param name="context">The context used to perform the operation.</param>
        internal void Update(EContext context)
        {
            Debug.Assert(context.HasFlags);

            if ((context.Flags & EContext.FlagDivideByZero) != 0)
                DivideByZero = true;

            if ((context.Flags & EContext.FlagInexact) != 0)
                Inexact = true;

            context.Flags = 0;
        }

        /// <summary>
        /// Forces the inexact flag to true.
        /// </summary>
        internal void SetInexact()
        {
            Inexact = true;
        }
        #endregion
    }
}
