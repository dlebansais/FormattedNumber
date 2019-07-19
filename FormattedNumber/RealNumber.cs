namespace FormattedNumber
{
    using System.Diagnostics;

    /// <summary>
    /// The format for a number parsed as real.
    /// </summary>
    public class RealNumber : FormattedNumber
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="RealNumber"/> class.
        /// </summary>
        /// <param name="integerText">The integer part of the the mantissa (before the dot).</param>
        /// <param name="separatorCharacter">The separator character.</param>
        /// <param name="fractionalText">The fractional part of the the mantissa (after the dot).</param>
        /// <param name="exponentCharacter">The exponent character.</param>
        /// <param name="exponentSign">The exponent sign.</param>
        /// <param name="exponentText">The exponent text.</param>
        /// <param name="invalidText">The trailing invalid text, if any.</param>
        /// <param name="canonical">The canonical form of the number.</param>
        public RealNumber(string integerText, char separatorCharacter, string fractionalText, char exponentCharacter, OptionalSign exponentSign, string exponentText, string invalidText, ICanonicalNumber canonical)
            : base(0, invalidText, canonical)
        {
            IntegerText = integerText;
            SeparatorCharacter = separatorCharacter;
            FractionalText = fractionalText;
            ExponentCharacter = exponentCharacter;
            ExponentSign = exponentSign;
            ExponentText = exponentText;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The integer part of the the mantissa (before the dot).
        /// </summary>
        public string IntegerText { get; private set; }

        /// <summary>
        /// The separator character.
        /// </summary>
        public char SeparatorCharacter { get; private set; }

        /// <summary>
        /// The factional part of the the mantissa (after the dot).
        /// </summary>
        public string FractionalText { get; private set; }

        /// <summary>
        /// The exponent character.
        /// </summary>
        public char ExponentCharacter { get; private set; }

        /// <summary>
        /// The exponent, if any.
        /// </summary>
        public OptionalSign ExponentSign { get; private set; }

        /// <summary>
        /// The exponent text, if <see cref="ExponentSign"/> is not <see cref="OptionalSign.None"/>.
        /// </summary>
        public string ExponentText { get; private set; }

        /// <summary>
        /// Gets the significand part of the formatted number.
        /// </summary>
        public override string SignificandString { get { return $"{IntegerText}.{FractionalText}"; } }

        /// <summary>
        /// Gets the exponent part of the formatted number.
        /// </summary>
        public override string ExponentString
        {
            get
            {
                string Result = string.Empty;

                if (ExponentText.Length > 0)
                {
                    Result += ExponentCharacter;

                    bool IsHandled = false;
                    switch (ExponentSign)
                    {
                        case OptionalSign.None:
                            IsHandled = true;
                            break;
                        case OptionalSign.Negative:
                            Result += "-";
                            IsHandled = true;
                            break;
                        case OptionalSign.Positive:
                            Result += "+";
                            IsHandled = true;
                            break;
                    }
                    Debug.Assert(IsHandled);

                    Result += ExponentText;
                }

                return Result;
            }
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Returns the invalid number as a string.
        /// </summary>
        public override string ToString()
        {
            string Exp = ExponentSign == OptionalSign.None ? string.Empty : (ExponentSign == OptionalSign.Negative ? "-" : "+");
            return $"{IntegerText}{SeparatorCharacter}{FractionalText}{ExponentCharacter}{Exp}{ExponentText}{base.ToString()}";
        }
        #endregion
    }
}
