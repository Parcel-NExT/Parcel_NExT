# Parcel Standard Libraries/Parcel Native Instructions (PNI)

<!-- We may or may not move this to Parcel Open Standards -->

One feature of Parcel original functions are:

1. Usually features standalone use
2. Supports simple and straightforward and common-place syntax like typical languages
3. Feature packed with options and customizable behaviors with lots of defaults

## Standard Constructions

|Name|Signature|Function|
|-|-|-|
|Parameters||Groups incoming parameters onto a single node and creates the same output parameters. Can also define new outputs through GUI (provide default values), or define new attributes.|
|Initialize||Provide the same functionality as Parameters, plus it allow using simple code to initialize and define many variables (instead of doing it through GUI).|
|Preview||Feedforward and preview an incoming node's value.|

## File System IO

**File Path**

**File System Entry**

**File Manipulation**

|Name|Alias|Signature|Description|Options/Attributes|Additional Returns|NExT Mapping|
|-|-|-|-|-|-|-|
|**GetFiles**|*ls*|`GetFiles Folder`<br/>`GetFiles Folders`|Get an array of file paths within folder.|`No Additional Returns = false`|File paths<br/>File names<br/>File names without extentions<br/>File extensions<br/>File sizes|`System.IO.Directory.EnumerateFiles`|
|**Rename**|*RenameFile*|`Rename OldPath NewPath`<br/>`Rename PathPairTuples`<br/>`Rename DataTable`<br/>`Rename ObjectsArray`|Rename files from a variety of permissible sources.|`Use Positional = true`<br/>`Default Source Attribute Names = [:Source, :From]`<br/>`Default Destination Attribute Names = [:Destination, :To]`||`System.IO.Directory.EnumerateFiles`|
|**Move**|*mv*|`Move OldPath NewPath`<br/>`Move PathPairTuples`<br/>`Move DataTable`<br/>`Move ObjectsArray`|Moves files as defined in a variety of permissible sources. Notice this is different from `Rename`.|`Use Positional = true`<br/>`Default Source Attribute Names = [:Source, :From]`<br/>`Default Destination Attribute Names = [:Destination, :To]`||`System.IO.Directory.EnumerateFiles`|

**Directory Manipulation**