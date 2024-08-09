# Parcel.Vector

Provides Parcel NExT abstraction for numerical Vector.

Very comprehensive vector library for handling with 1D data. The goal is short-hand utility of common functions.

Also provides domain-specific extensions, utilities and analytics. Those are implemented as partial classes.

## Technical Note

A note regarding engine level support and adoption:

* In general, for anything that's "Parcel" related we should use Vector instead of double[]
* For packages that wishes not having heavy dependency, they can use double[] because runtime should support automatic double[]/Vector conversion (both way)

## Usage

```C#
Import(Vector)
using Vector = Vector1D;
Vector v = [1, 2, 3, 4, 5];
```

```C#
Import(Vector)
public class Vector: Vector1D {}
Vector v = [1, 2, 3, 4, 5];
```

```C#
Import(Vector)
Vector([1.0, 2.0, 3.0])
```

## TODO

- [x] Implement `Vector a = [1, 2, 3]` syntax (C#12)
    * This syntax is available out-of-box in C#12.
- [x] Implement `Load(filepath)` for plain text rows (optionally skip header row) and CSV (default 0th column)