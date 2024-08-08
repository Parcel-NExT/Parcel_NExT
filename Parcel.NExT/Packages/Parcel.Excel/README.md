# Parcel.Excel

Provides Parcel NExT abstraction for Excel. Notice it's essential to implement our own layer for the purpose of interacting with Excel so that: 1) We have clear understanding of what API features we want, 2) We can easily switch underlying implementation.

Provides Parcel-native DataGrid and CSV integration for Excel files.

## Technical Note

* Notice historically Express used ExcelDataReader for excel reading and ExcelLibrary for excel writing. The latter is not cross-platform and not on .Net core.
* Among the three best open-source reader/writer (EPPlus, ClosedXML, and NPOI): 1) EEPPlus licence is weird and configuration is ugly; 2) Worst overall though some claim it's fast; Depending on SixLabors.Fonts. License looks fine untill you actually link to it from NuGet; Very massive. 3) NPOI is most comprehensive but it depends on ImageSharp - but both license and actual final output is managable.

We are using ExcelDataReader for reading because it's the most lightweight.

## References

* https://dev.to/xeshan6981/5-excel-libraries-every-net-developer-mostly-use-2hh7
* https://github.com/chaojian-zhang/Expresso