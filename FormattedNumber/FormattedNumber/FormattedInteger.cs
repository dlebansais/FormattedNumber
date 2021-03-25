namespace FormattedNumber
{
    using System;

    /// <summary>
    /// A real number and its components.
    /// </summary>
    public class FormattedInteger : FormattedNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedInteger"/> class.
        /// </summary>
        /// <param name="integerBase">The base.</param>
        /// <param name="sign">The optional sign.</param>
        /// <param name="leadingZeroCount">The number of leading zeroes.</param>
        /// <param name="integerText">The integer text..</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        /// <exception cref="NullReferenceException"><paramref name="invalidText"/> or <paramref name="canonical"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="leadingZeroCount"/> is lesser than zero.</exception>
        internal FormattedInteger(IIntegerBase integerBase, OptionalSign sign, int leadingZeroCount, string integerText, string invalidText, CanonicalNumber canonical)
            : base(invalidText, canonical)
        {
            IntegerBase = integerBase;
            Sign = sign;
            LeadingZeroCount = leadingZeroCount;
            IntegerText = integerText ?? throw new ArgumentNullException(nameof(integerText), "Value cannot be null.");

            if (leadingZeroCount < 0)
                throw new ArgumentOutOfRangeException(nameof(leadingZeroCount));
        }
        #endregion

        #region Properties
        /// <summary>
        /// The base.
        /// </summary>
        public IIntegerBase IntegerBase { get; }

        /// <summary>
        /// Gets the optional sign.
        /// </summary>
        public OptionalSign Sign { get; }

        /// <summary>
        /// Gets the number of leading zeroes. Can be 0.
        /// </summary>
        public int LeadingZeroCount { get; }

        /// <summary>
        /// Gets the text before the decimal separator character. Can be empty.
        /// </summary>
        public string IntegerText { get; }

        /// <summary>
        /// The significand part. Can be empty.
        /// This includes all characters up to and including the exponent character.
        /// </summary>
        public override string SignificandPart
        {
            get
            {
                string SignText = GetSignText(Sign);
                string LeadingZeroesText = GetLeadingZeroesText(LeadingZeroCount);

                return $"{SignText}{LeadingZeroesText}{IntegerText}{IntegerBase.Suffix}";
            }
        }

        /// <summary>
        /// The exponent part. Can be empty.
        /// This includes all characters after the exponent character and before the invalid text.
        /// </summary>
        public override string ExponentPart { get { return string.Empty; } }

        /// <summary>
        /// True if the number is valid.
        /// A valid number is finite in the sense of arithmetic (not NaN, not infinite), and has no trailing invalid text.
        /// </summary>
        public override bool IsValid { get { return InvalidText.Length == 0; } }

        /// <summary>
        /// A diagnostic string for debug purpose.
        /// </summary>
        public override string Diagnostic
        {
            get
            {
                string SignText = GetSignText(Sign);
                string LeadingZeroesText = GetLeadingZeroesText(LeadingZeroCount);

                return $"{SignText}/{LeadingZeroesText}/{IntegerText}/{IntegerBase.Suffix}/{InvalidText}";
            }
        }
        #endregion

        #region Client Interface
            /// <summary>
            /// Returns the formatted number as a string.
            /// </summary>
        public override string ToString()
        {
            return $"{SignificandPart}{InvalidText}";
        }
        #endregion
    }
}
