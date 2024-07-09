"""
Calling C# from Python.
Recommended import as `parcel`: import parcel_next as parcel

Automatically finds and adds dependent Parcel_Next runtime packages to os path.
Naming convention: use CamelCase for overall consistency?
"""

# Routines
def _setup_environment_paths():
    import sys
    import os
    my_libs_path = os.getenv("PYTHONPATH") # TODO: Do you want to add PYTHONPATH or PATH?
    if my_libs_path is not None and my_libs_path not in sys.path:
        sys.path.append(my_libs_path)

def _setup_pythonnet():
    import pythonnet
    pythonnet.load("coreclr")

def _setup_clr():
    import clr
    clr.AddReference("Core")

def _setup_parcel_next():
    from Core import Interpreter

def _init(): # Remark-cz: Important to call this in order to make sure parcel next packages can be found properly and python import statement works
    # Add environment paths
    _setup_environment_paths()

    # Interoperation: Setup pythonnet and Parcel related references
    # Notice pythonnet finds modules from sys.path, which only appends paths from PYTHONPATH but not PATH, so we will need to explicitly append the path to Parcel dlls in, or add that to PYTHONPATH (sounds not right)
    _setup_pythonnet()
    _setup_clr()
    _setup_parcel_next()
_init()

# Exposed routine
def LoadPackage(package_name):
    import clr
    clr.AddReference(package_name)
