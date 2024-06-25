# Parcel.InMemoryDB

Provides Parcel NExT abstraction for in-memory DB and related services. This package is for standalone use and only based on SQLite and doesn't involve any other advanced features offered in the entire InMemoryDB suite (e.g. ODBC integration, DataGrid integration, services etc.).

## TODO

- [ ] Remove dependency on ConsoleTable and Parcel.CoreEngine and Parcel.DataGrid
- [ ] Deal with dependency on Parcel.DataGrid and ODBC: we want to focus entirely on SQLite
- [ ] Implement Parcel ODBC like strongly typed generic interface (with Dapper)
	- [ ] Investigate further and see whether we can do more with Dapper
- [ ] Think about the utility of this library compared to using raw Dapper.
- [ ] Handle the case with network connection (current implementation only deals with local connections)