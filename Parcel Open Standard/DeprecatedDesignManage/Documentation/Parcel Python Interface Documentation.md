# Overview

As a custom implementation by IronPython, Parcel's Python environment can directly make use of exposed C# types, thus we don't need to import external third-party libraries as long as those interfaces are exposed by C#.

Python is used in two scenarios:

1. As Python addons: provide native-level node definitions, must be functional (will run in isolated scope);
2. As Python script open/import nodes: allow direct execution of snippets in isolated environments, must be functional (will run in isolated scope);
3. As Python program (in-place/reference) nodes: Execute inside a State Graph, shares a scope.

# Parcel Environment Header

Below is automatically appended for any Python snippet executed in Parcel environment:

```python
import clr
clr.AddReference("Parcel.Shared")

import Parcel.Shared.DataTypes as PDT
from Parcel.Shared.DataTypes import DataGrid 
```

Below is the typical Parcel-Python program entry point:

```python
def main():
    return 'test' # Return any object
```

# Basic Types

## Data Grid

Example:

```python
d = DataGrid()
```

# Snippets

```python
import clr
clr.AddReference("System.Xml")
"System.Xml" in [assembly.GetName().Name for assembly in clr.References] # True
clr.AddReference("System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")

# https://ironpython.net/documentation/dotnet/
import System
System # <module 'System' (CLS module, ... assemblies loaded)>
System.Collections # <module 'Collections' (CLS module, ... assemblies loaded)>
```

```python
import clr
clr.AddReference("System.Windows.Forms")
from System.Windows.Forms import MessageBox, MessageBoxButtons

MessageBox.Show("Hello World!", "Greetings", MessageBoxButtons.OKCancel)
```

```c#
var eng = IronPython.Hosting.Python.CreateEngine();
var scope = eng.CreateScope();
eng.Execute(@"
def greetings(name):
    return 'Hello ' + name.title() + '!'
", scope);
dynamic greetings = scope.GetVariable("greetings");
System.Console.WriteLine(greetings("world"));
```