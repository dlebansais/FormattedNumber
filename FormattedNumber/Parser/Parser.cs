namespace FormattedNumber
{
    using System;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Parser to extract a formatted number from any string.
    /// </summary>
    public class Parser
    {
        #region Constants
        /// <summary>
        /// The decimal separator in numeric values.
        /// </summary>
        public static readonly char DecimalSeparator;

        /// <summary>
        /// The dot (unicode U+002E) is always considered a valid separator.
        /// </summary>
        public static readonly char NeutralDecimalSeparator = '.';

        /// <summary>
        /// The character to use when a number doesn't contain the decimal separator.
        /// </summary>
        public static readonly char NoSeparator = '\0';
        #endregion

        #region Init
        static Parser()
        {
            string CurrentCultureSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            Debug.Assert(CurrentCultureSeparator.Length == 1);
            DecimalSeparator = CurrentCultureSeparator[0];
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Parse a string as a number.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <returns>
        /// An instance of <see cref="FormattedInvalid"/> if <paramref name="text"/> cannot be parsed as a number.
        /// An instance of <see cref="FormattedInteger"/> if the valid part of the parsed number is an integer.
        /// An instance of <see cref="FormattedReal"/> if <paramref name="text"/> can be parsed, but not as an integer.
        /// </returns>
        /// <exception cref="NullReferenceException"><paramref name="text"/> is null.</exception>
        public static FormattedNumber Parse(string text)
        {
            IBinaryIntegerParsingInfo BinaryIntegerParsing = new BinaryIntegerParsingInfo();
            IOctalIntegerParsingInfo OctalIntegerParsing = new OctalIntegerParsingInfo();
            IDecimalIntegerParsingInfo DecimalIntegerParsing = new DecimalIntegerParsingInfo();
            IHexadecimalIntegerParsingInfo HexadecimalIntegerParsing = new HexadecimalIntegerParsingInfo();
            IRealParsingInfo RealParsing = new RealParsingInfo();

            foreach (char c in text)
            {
                bool KeepParsing = false;

                if (BinaryIntegerParsing.StillParsing)
                {
                    BinaryIntegerParsing.Handler(c);
                    KeepParsing = true;
                }

                if (OctalIntegerParsing.StillParsing)
                {
                    OctalIntegerParsing.Handler(c);
                    KeepParsing = true;
                }

                if (DecimalIntegerParsing.StillParsing)
                {
                    DecimalIntegerParsing.Handler(c);
                    KeepParsing = true;
                }

                if (HexadecimalIntegerParsing.StillParsing)
                {
                    HexadecimalIntegerParsing.Handler(c);
                    KeepParsing = true;
                }

                if (RealParsing.StillParsing)
                {
                    RealParsing.Handler(c);
                    KeepParsing = true;
                }

                if (!KeepParsing)
                    break;
            }

            IParsingInfo Parsing = new InvalidParsingInfo();
            int LengthSuccessful = 0;

            BinaryIntegerParsing.UpdateBestParsing(ref Parsing, ref LengthSuccessful);
            OctalIntegerParsing.UpdateBestParsing(ref Parsing, ref LengthSuccessful);
            DecimalIntegerParsing.UpdateBestParsing(ref Parsing, ref LengthSuccessful);
            HexadecimalIntegerParsing.UpdateBestParsing(ref Parsing, ref LengthSuccessful);
            RealParsing.UpdateBestParsing(ref Parsing, ref LengthSuccessful);

            FormattedNumber Result = Parsing.GetResult(text);

            return Result;
        }
        #endregion
    }
}
