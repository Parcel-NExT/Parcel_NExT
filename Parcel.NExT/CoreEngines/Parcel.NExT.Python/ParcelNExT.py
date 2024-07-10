"""
Calling C# from Python.
Recommended import as `parcel`: import ParcelNExT as parcel

Automatically finds and adds dependent Parcel Next runtime packages to os path.
Naming convention: use PascalCase for overall consistency
"""

# Routines
def _setupEnvironmentPaths():
    import sys
    import os
    myLibsPath = os.getenv("PYTHONPATH") # TODO: Do you want to add PYTHONPATH or PATH?
    if myLibsPath is not None and myLibsPath not in sys.path:
        sys.path.append(myLibsPath)

def _setupPythonNet():
    import pythonnet
    pythonnet.load("coreclr")

def _setupCLR():
    import clr
    clr.AddReference("Core")

def _setupParcelNExT():
    from Core import Interpreter

def _init(): # Remark-cz: Important to call this in order to make sure parcel next packages can be found properly and python import statement works
    # Add environment paths
    _setupEnvironmentPaths()

    # Interoperation: Setup pythonnet and Parcel related references
    # Notice pythonnet finds modules from sys.path, which only appends paths from PYTHONPATH but not PATH, so we will need to explicitly append the path to Parcel dlls in, or add that to PYTHONPATH (sounds not right)
    _setupPythonNet()
    _setupCLR()
    _setupParcelNExT()
_init()

# Exposed routine
def LoadPackage(packageName):
    import clr
    clr.AddReference(packageName)
