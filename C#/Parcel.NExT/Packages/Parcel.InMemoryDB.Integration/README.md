# Parcel.InMemoryDB.Integration

Provides high level integration to Parcel.InMemoryDB, unifying all component libraries.  
The complication mostly comes from the variation of data source/targets including: DataGrid, (In-memory) SQLite, ODBC and MS Analysis Service.

## TODO

- [ ] Pending refine the paradigm for ProceduralInMemoryDB
	- [ ] Stored procedures are less practical/clear in practice, consider deprecating
	- [ ] Consider remove/migrate direct import from DSN or Microsoft analysis service elsewhere
- [ ] (CSV) Add functions: Export as CSV files.
	- [ ] Is there any way during serving (while InMemoryDB is executing) we constantly "live" update source CSV files (or a folder of CSV files)? May or may not have good utility, so not a priority.