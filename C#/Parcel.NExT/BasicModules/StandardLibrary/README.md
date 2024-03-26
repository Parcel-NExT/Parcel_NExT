# Parcel Standard Libraries

It's a single assembly.

Provides standard libraries for Parcel. This is a placeholder assembly while I am still working on the standard. I think by and large the scope of the standard library depends on how much effort I can put into it, and what are the reliable ways to implement things.

This is adapted from what's originally a Demo module: A demo-use module that also informs design of Parcel core engine. This C#-only module contains methods and implementations that both uses only native C# types (plain standard C# code with zero dependancy), and ones that also implements parcel specific constructs (well-conformed Parcel nodes for types and methods), and also ones that interfaces with meta-programming capabilities of parcel (payload interfacing).

Comments:

* (20240215) The Demo module demonstrates how very crucial it is to design interfaces suitable for specific use cases, and function reusability is of major concern and lean dependancies is crucial. It also shows how the design of interfaces can have drastically varying possibilities, guided by the capabilities of runtime engine (for parsing such signatures).

## Contents

