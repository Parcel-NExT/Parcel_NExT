# MiniParcel Shell (Shell Program/Language)

Known as: MiniParcel, MiniParcel CLI, MiniParcel Shell

MiniParcel REPL and evaluation engine for Parcel NExT. This program is more than a wrapper for MiniParcel format services - it's a complete when used shell program and shell scripting language!. MiniParcel can digest nodes/functions from those sources: system commands (available from PATH), C# functions and classes, Python functions and classes.

<!-- Note: In terms of implementation, might be able to just make use of existing Ama runtime engine/service provider. We should also separate the shell scripting language part from shell (TUI) program itself (e.g. colorful displays); The shell scripting language part should be implemented in Core because we wish to use that in Gospel as well. -->
