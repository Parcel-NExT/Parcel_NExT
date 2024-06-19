# Parcel.CoreEngine.Service

Main entry point for backends and frontends, provides low-level routines for managing runtime; Does not provide routines for execution (those are handled in specific interpreters). Notice the engine is largely context agnostic and makes no assumptions on available modules (be it Parcel standard or user custom) to load and only provide the necessary services to load modules when needed - the actual loading is delegated to the backend code (or if the front-end embeds the runtime, it's handled directly on the front-end).

May provide managed wrappers for specific runtimes, in this case, C# and Python.

## TODO

- [ ] Why doesn't the service handles loading and managing all the modules directly?