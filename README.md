# FormattedNumber

Handle numbers obtained from any string, and arbitrary precision arithmetic.

[![Build Status](https://travis-ci.com/dlebansais/FormattedNumber.svg?branch=master)](https://travis-ci.com/dlebansais/FormattedNumber) [![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/formattednumber/badge)](https://www.codefactor.io/repository/github/dlebansais/formattednumber) [![codecov](https://codecov.io/gh/dlebansais/FormattedNumber/branch/master/graph/badge.svg)](https://codecov.io/gh/dlebansais/FormattedNumber)

This project can be used for the following purposes:

+ Parse any string that could be a number, and extract the relevant parts to obtain 3 substrings:
	* The significand, all characters before the exponent.
	* The exponent, starting after the letter `e`.
	* The invalid end of the string that could not be parsed after the exponent.
+ Perform operations on these parsed numbers with arbitrary precision.

For example, the string `3.141592653589793462643383279502884197169399375105e0X` is parsed as `3.141592653589793462643383279502884197169399375105e`, `0` and `X`. The parsed result can be used to calculate π/2 with a precision of more than 40 digits.

Since a string can always be parsed, and at worse will give two empty strings for the significand and the exponent, with the invalid part being the string itself, it can be used in editors or text viewers that highlight these parts.

For example, the [Easly controller](https://github.com/dlebansais/Easly-Controller) displays the significand with a normal font and color, the exponent with subscript font and normal color, and the invalid part with normal font but red color.

## Non-decimal integers

Programming languages have various ways to deal with numbers expressed in a base different then the usual decimal base. For example, C and C++ use the `0x` prefix to represent hexadecimal numbers. The `FormattedNumber` class uses a suffix:

+ `:H` for hexadecimal,
+ `:O` for octal,
+ `:B` for binary. 

## Leading zeroes

Sometimes having zeroes at the begining of a number is acceptable. Typically, in fixed-length numbers such as hash keys, checksums, unicode key code, and so on. The `FormattedNumber` class handles them with a separate property.

For example, `0C4A9EF2:H` is parsed as one leading zero, significand `C4A9EF2`, empty exponent and empty invalid part.

## Arbitrary precision

The precision at which operations are performed is not infinite, but is arbitrary, and can be tuned using the `Arithmetic` class.

This is done using a fork of [Peter Occil](https://github.com/peteroupc)'s [Numbers](https://github.com/peteroupc/Numbers) project. If you like this software, considered donating at the link provided in the main page of that project.
