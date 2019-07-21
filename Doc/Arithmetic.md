# The Arithmetic class

This class controls how operations on `Number` objects are performed, and most notably the precision used. It contains only static members, all of which applying to the entire process, with the exception of the `Flags` member that is stored per thread (see below).

## Precision

The `Precision` member of this class indicates the maximum number of digits in the significand for non-polynomial.

The code currently doesn't support value `0` that indicates infinite precision.

## Flags

Each thread keeps a copy of flags that provide information about performed operations. These flags accumulate: as more operations are performed, each flag is set depending on the result, but they are not cleared.

For example, if a non-zero number is divided by zero, the `DivideByZero` flag is set. If the result of this operation (either `PositiveInfinity` or `NegativeInfinity`) is added to a constant, the result is unchanged and the `DivideByZero` flag remains set.

To clear flags after they have been read and handled in the application code, call the `Clear` method. This clears flags for the current thread, and flags for other threads are unaffected.

### DivideByZero

Signals a division of a non-zero number by zero.

