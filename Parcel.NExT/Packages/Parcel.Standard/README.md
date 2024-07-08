# Parcel Standard Library (PSL)

This module provides standard libraries for Parcel. This is a placeholder assembly while I am still working on the standard. I think by and large the scope of the standard library depends on how much effort I can put into it, and what are the reliable ways to implement things. Another key factor for is standardization effort and interoperability: we want to expose only those functionalities that would be available in ANY platform, or basic ANSI C. A key design feature is dependency free: this library is .Net standard compliant and has ZERO external dependencies AND avoid uncommon .net standard dependencies (e.g. System.Data) (maybe except Parcel Core, for the purpose of parcel-native features; Ideally not). As such there will not be anything related to data-grid, or anything specific to Microsoft or .Net environment. Notice this package must be AOT.

PSL is a suite of functionalities but I don't see any point in splitting into multiple assemblies and thus cause management burden - so at the moment it's implemented in a single assembly. This is adapted from what's originally a Demo module: A demo-use module that also informs design of Parcel core engine. This C#-only module contains methods and implementations that both uses only native C# types (plain standard C# code with zero dependancy), and ones that also implements parcel specific constructs (well-conformed Parcel nodes for types and methods), and also ones that interfaces with meta-programming capabilities of parcel (payload interfacing).

## TODO

- [ ] Remove dependency on Parcel.NExT.Interpreter

## Contents

<!-- Put contents here instead of on the wiki for key reference -->

* Process management

## Notes

Comments:

* (20240215) The Demo module demonstrates how very crucial it is to design interfaces suitable for specific use cases, and function reusability is of major concern and lean dependancies is crucial. It also shows how the design of interfaces can have drastically varying possibilities, guided by the capabilities of runtime engine (for parsing such signatures).
* (20240704) Math focuses only on `double`; Will provided dedicated `Decimal` package.