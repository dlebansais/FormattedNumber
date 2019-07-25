namespace FormattedNumber
{
    using System.Diagnostics;

    /// <summary>
    /// Interface describing an hexadecimal (base 16) integer.
    /// </summary>
    public interface IHexadecimalIntegerBase : IIntegerBase
    {
    }

    /// <summary>
    /// Class describing an hexadecimal (base 16) integer.
    /// </summary>
    public class HexadecimalIntegerBase : IntegerBase, IHexadecimalIntegerBase
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="HexadecimalIntegerBase"/> class.
        /// </summary>
        internal HexadecimalIntegerBase()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// The suffix for hexadecimal integers.
        /// </summary>
        public override string Suffix { get { return HexadecimalSuffix; } }

        /// <summary>
        /// The number of digits for hexadecimal integers.
        /// </summary>
        public override int Radix { get { return HexadecimalRadix; } }
        #endregion

        #region Client Interface
        /// <summary>
        /// Checks if a character is an hex digit, and return the corresponding value.
        /// </summary>
        /// <param name="digit">The character to check.</param>
        /// <param name="value">The digit's value.</param>
        /// <returns>True if <paramref name="digit"/> is a valid digit; Otherwise, false.</returns>
        public override bool IsValidDigit(char digit, out int value)
        {
            value = 0;
            bool IsParsed = false;

            if (digit >= '0' && digit <= '9')
            {
                value = digit - '0';
                IsParsed = true;
            }

            else if (digit >= 'a' && digit <= 'f')
            {
                value = digit - 'a' + 10;
                IsParsed = true;
            }

            else if (digit >= 'A' && digit <= 'F')
            {
                value = digit - 'A' + 10;
                IsParsed = true;
            }

            return IsParsed;
        }

        /// <summary>
        /// Returns the digit corresponding to a value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override char ToDigit(int value)
        {
            Debug.Assert(value >= 0 && value < Radix);

            return value < 10 ? (char)('0' + value) : (char)('A' + value - 10);
        }

        /// <summary>
        /// Returns the value corresponding to a digit.
        /// </summary>
        /// <param name="digit">The digit.</param>
        public override int ToValue(char digit)
        {
            bool IsDecimalDigit = digit >= '0' && digit <= '9';
            bool IsHexadecimalDigitLower = digit >= 'a' && digit <= 'f';
            bool IsHexadecimalDigitUpper = digit >= 'A' && digit <= 'F';

            Debug.Assert(IsDecimalDigit || IsHexadecimalDigitLower || IsHexadecimalDigitUpper);

            if (digit >= '0' && digit <= '9')
                return digit - '0';

            else if (digit >= 'a' && digit <= 'f')
                return digit - 'a' + 10;

            else
                return digit - 'A' + 10;
        }
        #endregion
    }
}
