# Parcel.NExT.CodeGen (PolyGlot)

Handles C#, Python code generation and complete .net solution build and compilation.

There are N types of code gen:

1. Referential: With or without intermediary, calling of library nodes are handled through DLL linking.
2. Raw: Only subsets of standard libraries are supported - produces code with zero dependency.
3. Self-contained: Produces single executable, does not support Pure/snippet codes.