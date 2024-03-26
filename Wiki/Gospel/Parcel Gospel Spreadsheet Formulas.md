# Parcel Gospel Spreadsheet Formulas

Notice Parcel Gospel spreadsheet formulas do NOT behave exactly as excel counterparts. It's advisable that user study them carefully to avoid unexpected results. In the following documentation, we will just call "Parcel Gospel spreadsheet (formulas)" as "spreadsheet (formulas)".

In particular, expect the following differences:

* Spreadsheet formulas are cleaner and curated.
* Spreadsheet formulas have cleaner and "make more sense" signatures.
* Spreadsheet formulas expect to operate on ranges of literal values; Variable length arguments are not supported.
* Spreadsheet formulas never spills: they return cell value.
* Spreadsheet formulas offer basic numerical and data table operations, and they do not provide programming capabilities.
* Spreadsheet formulas are implemented entirely in the frontend (Gospel).
* Spreadsheet formulas are not to be abused (otherwise our dear Parcel developers will get sad😟).
* Spreadsheet formulas are not intended to be fast - use Parcel nodes instead for efficient processing.

## Equations

<!-- Internally, equations are just Godot native math functions. -->

Equations only take primitive or single cell values.

|Equation|Category|Description|Signature|
|-|-|-|-|
|POW|Math|Get power.|`pow(base:float, exponent:float) -> float`|
|RANDF|Math|Get random number between [0,1].|`randf() -> float`|

## Formulas

|Formula|Category|Description|Exception|Signature|Note|
|-|-|-|-|-|-|
|AVERAGE|Numerical|Computes average of elements in a range.|Ignores invalid values.|`average(range:String) -> float`|-|
|RAND|Table|Get random element in range.||`rand(range:String) -> String`|Different from Excel. Use `RANDF` for random floating number.|
|SUM|Numerical|Computes sum of elements in a range.|Ignores invalid values.|`sum(range:String) -> float`|-|
|VLOOKUP|Table|Matches value on selected column exactly and returns found value on match column.|When value not found, return empty.|`vlookup(lookup_value:String, range:String, source_column:int, match_column:int) -> Variant`|Different from Excel.|

## Reference

* [Excel formulas](https://support.microsoft.com/en-us/office/excel-functions-alphabetical-b3944572-255d-4efb-bb96-c6d90033e188#bm22)