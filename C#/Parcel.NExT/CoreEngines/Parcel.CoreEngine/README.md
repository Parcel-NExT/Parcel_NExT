# Parcel.CoreEngine

CoreEngine provides the document model for Parcel NExT - i.e. basic types and protocol implementations conforming to POS and PDS, and provides graph resolving algorithms. It does not involve any runtime specific code; It does NOT provide anything related to module loading and function implementation.

Depiste being implemented in C#, this module should not be C#-tied and should be as general/abstract as possible - it mostly provides common sharable constructs that should be universal irrelevant of programming language. We try our best to eliminate external dependencies when we can!

There are a few dependencies that (may) make this module runtime dependant/platform dependant, or cannot be compiled as a single executable or AOT:

1. K4os.Compression.LZ4.Streams
2. (.Net reflections) Used by SimpleJSON, otherwise we avoid using reflection here but instead in a separate dll (Consider replace with System.Text.Json)
3. (Python.Net) (Used in a separate dll)
4. (Roslyn) (Used in another separate dll)

ADO: https://dev.azure.com/ParcelEngine/Parcel/_workitems/edit/3

## TODO

- [ ] Replace SimpleJSON with some simpler library/helper so this assembly is completely ".Net Standard" and have no C# specific dependencies/constructs, or can be compiled AOT.