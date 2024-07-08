# Parcel Packages (Official)

Naming conventions:

* Assembly name IS the package name.
* Using C#-suitable namespace and type names are recommended - though do recognize that we will directly display those names in the front end.
* In general, namespaces should be more "functional" or "descriptive" or "catogorical" than type specific.

Design guidances:

* Try to provide default "typical usage scenario"/"demo use" values so as to minimal graph-use setup
* Try to be dependency free
* Try to restrict the number of public types that gets exposed

## TODO

Packages:

- [ ] Add (SQL) Database package: Utilize ODBC or otherwise
- [ ] Add (NoSQL) Database package
- [ ] Add (MS MDX) Database package (Microsoft Analysis Service)