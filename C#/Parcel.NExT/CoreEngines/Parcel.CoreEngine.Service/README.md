# Parcel.CoreEngine.Service

Main entry point for backends and frontends. Notice at this point the engine is largely context agnostic and makes no assumptions on available modules (be it Parcel standard or user custom) to load and only provide the necessary services to load modules when needed - this is delegated to the backend.

May provide managed wrappers for specific runtimes, in this case, C# and Python.