# Pure

Default interactive Pure interpreter (REPL). Pure 2 is the frontend to Parcel NExT, a C#-based scripting platform. See RFC for more details.

## TODO

Runtime:

- [ ] Per 2024 July, Pure tends to say "Method not found" when the method is indeed defined - running the same script twice works around the issue. What's causing this? E.g. `Import(ProjectNine.FictionalWorld); var data = DataSet.GetPopulationData()`