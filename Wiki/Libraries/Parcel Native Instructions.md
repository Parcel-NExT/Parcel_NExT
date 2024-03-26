# (Experimental) Parcel Native Instructions (PNI)

<!-- (Remark, #20240326) Parcel Native Instructions should be a much smaller subset of PSL (Parcel Standard Libraries), which in theory when provided should be able to implement all upper layer functionalities directly as Parcel nodes/instructions -->
<!-- (Remark, #20240326) It looks like the original intention with PNI is to be used with MiniParcel, in which case it's still going to be a subset of PSL but it makes sense to be quite robust and comprehensive like PowerShell -->

<!-- We may or may not move this to Parcel Open Standards; At the moment, let's develop this as a standalone piece -->

One feature of Parcel original functions are:

1. Usually features standalone use (i.e. without need for constructions as in C++ style, and do not usually require additional wirings)
2. Supports simple and straightforward and common-place syntax like typical languages
3. Feature-packed with options and customizable behaviors with lots of defaults

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