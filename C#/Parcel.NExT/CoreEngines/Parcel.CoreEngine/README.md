# Parcel.CoreEngine

We try our best to eliminate external dependencies when we can!

There are a few dependencies that make this module runtime dependant/platform dependant, or cannot be compiled as a single executable or AOT:

1. .Net reflections
2. K4os.Compression.LZ4.Streams
3. (Python.Net) (Used in a separate dll)
4. (Roslyn) (Used in another separate dll)

ADO: https://dev.azure.com/ParcelEngine/Parcel/_workitems/edit/3