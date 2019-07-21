# FormattedNumber

Handle numbers obtained from any string, and combine them with arbitrary precision arithmetic.

[![Build Status](https://travis-ci.com/dlebansais/FormattedNumber.svg?branch=master)](https://travis-ci.com/dlebansais/FormattedNumber) [![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/formattednumber/badge)](https://www.codefactor.io/repository/github/dlebansais/formattednumber) [![codecov](https://codecov.io/gh/dlebansais/FormattedNumber/branch/master/graph/badge.svg)](https://codecov.io/gh/dlebansais/FormattedNumber)

This project can be used for the following purposes:

+ Parse any string that could be a number, and extract relevant parts. For example, the character that was used for the exponent (`e` or `E`), or invalid characters that could not be parsed.
+ Perform operations on these parsed numbers with arbitrary precision.

For instance, the string `3.141592653589793462643383279502884197169399375105e0foo` is parsed as `3`, `.`, `141592653589793462643383279502884197169399375105`, `e`, `0` and `foo`. The parsed result can be used to calculate π/2 with a precision of more than 40 digits.

Since a string can always be parsed, it can be used in editors or text viewers that highlight relevant parts.

For example, the [Easly controller](https://github.com/dlebansais/Easly-Controller) displays the significand with a normal font and color, the exponent with subscript font and normal color, and the invalid part with normal font but red color.

## Non-decimal integers

Programming languages have various ways to deal with numbers expressed in a base different than the usual decimal base. For example, C and C++ use the `0x` prefix to represent hexadecimal numbers. The `FormattedNumber` class uses a suffix:

+ `:H` for hexadecimal,
+ `:O` for octal,
+ `:B` for binary. 

## Leading zeroes

Sometimes having zeroes at the begining of a number is acceptable. Typically, in fixed-length numbers such as hash keys, checksums, unicode key code, and so on. The `FormattedNumber` class handles them with a separate property.

For example, `0C4A9EF2:H` is parsed as one leading zero, significand `C4A9EF2` in hexadecimal base, empty exponent and empty invalid part.

## Arbitrary precision

The precision at which operations are performed is not infinite, but is arbitrary, and can be tuned using the `Arithmetic` class (see the [doc](https://github.com/dlebansais/FormattedNumber/blob/master/Doc/Arithmetic.md)).

This is done using a fork of [Peter Occil](https://github.com/peteroupc)'s [Numbers](https://github.com/peteroupc/Numbers) project. If you like this software, considered donating at the link provided in the main page of that project.

## Detailed interface

A formatted number can be created in two ways:

+ Parsing a string and extracting the sign, the number of leading zeroes and so on, as well as a `Number` object, and the invalid part at the end, if any.
+ Initializing the formatted number directly with all relevant values.

The second method is not terribly interesting, and in what follows we will just focus on parsing.

### Valid numbers

We first read the optional sign.

Then we extract leading zeroes. They are all removed from the significand unless the last zero is followed by a separator, an integer suffix, the exponent character or plain invalid text.

Once leading zeroes are handled, the number is parsed as follow:

1. The string `0` gives an instance of `FormattedInteger` with the value 0.
+ A valid significand is either an integer or a real number.
	* The string is parsed as a decimal integer.
	* If not a valid decimal integer, and the string ends with a base suffix, it is parsed as an integer in that base.
	* If parsing as an integer failed, the string is parsed as a real number.
+ A real number begins with (at least one) decimal digits, and is followed by the decimal separator as specified by the current culture (a dot is also always accepted as decimal separator), or an exponent.
+ If there is no decimal separator and exponent, digits are parsed to obtain an instance of an `FormattedInteger`, followed by an invalid part.
+ If there is a decimal separator, and digits are not followed by the `e` or `E` character, they are parsed to obtain an instance of a `FormattedReal`, followed by an invalid part.
+ If the exponent character is found, it can optionally be followed by either `+` or `-`, and decimal digits (the first digit is not allowed to be 0). Unless this first digit exists, the exponent is not valid and treated as the case above.
+ If found, the exponent is parsed to the last digit. The number is then an instance of `FormattedReal` with this exponent, and whatever follows is the invalid string part.

### Reconstructing the string

From a `FormattedNumber` object, one can reconstruct the original string. Each type of formatted number concatenates its own relevant information:

+ `FormattedInvalid`
    * The `NaN`, `∞` or `-∞` string, depending on the type of invalid number.
    * The invalid part, if any.
+ `FormattedInteger`
    * The sign, if any.
    * Leading zeroes, if any.
    * Digits.
    * The suffix, if any.
    * The invalid part, if any.
+ `FormattedReal`
    * The sign, if any.
    * Leading zeroes, if any.
    * Digits before the separator.
    * If there is a separator:
        * The separator character.
        * Digits after the separator.
    * The exponent character, if any.
    * The exponent sign, if any.
    * Exponent digits, if any.
    * The invalid part, if any.
