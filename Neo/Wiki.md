# PV1 Neo Wiki

## Architecture

### Extension Points and Utilization

PV1 Neo can be extended both using the `plugin-model` (external dlls) or `framework-mode` (directly modify source code):

1. Plugin model: Using pre-built binaries, load arbitrary dlls from conventional place. (At the moment no CLI or GUI interface is provided for loading such packages)
2. Source code modification: Modify source code directly to implement intended functions.

From a framework perspective, PV1 Neo offers `IToolboxDefinition` which allows explicit definition of nodes, using either "automatic nodes" or "explicit nodes".  
From plugin perspective, external dlls can be loaded directly from `PARCEL_PACKAGES` environment variable, and all public types will be loaded.

At the moment we are still consolidating on Neo's five ways of extension: 1) Source code using IToolbox, which provides two ways; 2) Dynamic assembly import of either general C# assembly or IToolboxDefinition assembly; 3) Source code level using `RegisterType`. In general, when IToolboxDefinition is present in an assembly, we will use that to import nodes; Otherwise all general public static and nonstatic types are imported.

### Architecture Notes

Current handling of nodes simply references strongly typed MethodInfo from loaded assemblies. In the past serialization was a challenge because we relied on BinaryFormatter, nowadays with POS PDS we have straightforward way of representing nodes in saved files.

## Development

### Loading Parcel Packages

PENDING.

### Loading C# Assemblies

PENDING.

### To Add New Functionalities

Either inherit a class, or use shortcut methods. In the future, if we shall implement a way to load from assemblies automatically, it would make things way easier and allow interoperation from Parcel NExT.