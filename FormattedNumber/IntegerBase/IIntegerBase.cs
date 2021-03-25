namespace FormattedNumber
{
    using System.Diagnostics;

    /// <summary>
    /// Interface describing an integer with a specified digits base.
    /// </summary>
    public interface IIntegerBase
    {
        /// <summary>
        /// Gets the suffix used to specify the base, null if none.
        /// </summary>
        string Suffix { get; }

        /// <summary>
        /// Gets the number of digits in the base.
        /// </summary>
        int Radix { get; }

        /// <summary>
        /// Checks if a character is a digit in this base, and return the corresponding value.
        /// </summary>
        /// <param name="digit">The character to check.</param>
        /// <param name="value">The digit's value.</param>
        /// <returns>True if <paramref name="digit"/> is a valid digit; Otherwise, false.</returns>
        bool IsValidDigit(char digit, out int value);

        /// <summary>
        /// Checks if a number is made of digits in this base.
        /// A valid number must not start with 0 (unless it is zero or <paramref name="supportLeadingZeroes"/> is set), and must not be empty.
        /// </summary>
        /// <param name="text">The number to check.</param>
        /// <param name="supportLeadingZeroes">True if <paramref name="text"/> might have leading zeroes.</param>
        /// <returns>True if <paramref name="text"/> is a valid number; Otherwise, false.</returns>
        bool IsValidNumber(string text, bool supportLeadingZeroes = true);

        /// <summary>
        /// Checks if a number is made of digits in this base.
        /// A valid significand must be a valid number and not end with a zero (unless it is zero).
        /// </summary>
        /// <param name="text">The number to check.</param>
        /// <returns>True if <paramref name="text"/> is a valid significand; Otherwise, false.</returns>
        bool IsValidSignificand(string text);

        /// <summary>
        /// Returns the digit corresponding to a value.
        /// </summary>
        /// <param name="value">The value.</param>
        char ToDigit(int value);

        /// <summary>
        /// Returns the value corresponding to a digit.
        /// </summary>
        /// <param name="digit">The digit.</param>
        int ToValue(char digit);

        /// <summary>
        /// Returns the input number divided by two.
        /// </summary>
        /// <param name="text">The number to divide.</param>
        /// <param name="hasCarry">True upon return if <paramref name="text"/> is odd.</param>
        string DividedByTwo(string text, out bool hasCarry);

        /// <summary>
        /// Returns the input number multiplied by two, with an optional carry to add.
        /// </summary>
        /// <param name="text">The number to multiply.</param>
        /// <param name="addCarry">True if a carry should be added.</param>
        string MultipliedByTwo(string text, bool addCarry);
    }
}
