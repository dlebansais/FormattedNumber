# FormattedNumber

Handle numbers obtained from any string, and arbitrary precision arithmetic.

[![Build Status](https://travis-ci.com/dlebansais/FormattedNumber.svg?branch=master)](https://travis-ci.com/dlebansais/FormattedNumber) [![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/formattednumber/badge)](https://www.codefactor.io/repository/github/dlebansais/formattednumber) [![codecov](https://codecov.io/gh/dlebansais/FormattedNumber/branch/master/graph/badge.svg)](https://codecov.io/gh/dlebansais/FormattedNumber)

This project can be used for the following purposes:

+ Parse any string that could be a number, and extract the relevant parts to obtain 3 strings
	* The significand, all characters before the exponent.
	* The exponent, starting after the letter `e`.
	* The invalid end of the string that could not be parsed after the exponent.

For example, the string `-3.1415e0X` is parsed as `-3.1415e`, `0` and `X`.

Since a string can always be parsed, and at worse will give two empty strings for the signficand and the exponent, with the invalid part being the string itself, it can be used in an editor or a text viewer that highlight these part.

For example, the Easly editor displays the significand with a normal font and color, the exponent as subscript font and normal color, and the invalid part with normal font but red color.

## Non-decimal integers

Programming languages have various ways to deal with number expressed in base different then the usual decimal base. For example, C and C++ use the `0x` prefix to represent hexadecimal number. The `FormattedNumber` class uses suffixes:

+ `:H` for hexadecimal,
+ `:O` for octal,
+ `:B` for binary. 

## Leading zeroes

Sometimes having zeroes at the begining of a number is acceptable. Typically, in fixed-length numbers such as hash keys, unicode key code, and so on. The FormattedNumber class handles them as a separate property.

For example, `0C4A9EF2:H` is parsed as `1` leading zero, significand `C4A9EF2`, empty exponent and empty invalid part.  