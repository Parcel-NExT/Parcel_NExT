# Overview

Use a single binary Parcel file as main application data containing all workspace and cache data.

Application file formats:

* *.parcel*: Singleton package.

# The .Parcel Archive

Currently implementation uses a Zip archive, but it can be custom-binary. Either way, a *.Parcel* archive contains the following *data-blocks*, which in Zip structure are represented as folders:

* Raw Data (Tables): Raw serialized hyper-table data (tables and objects)
* Nodes
* Workflows (Pages)
* Layouts

**Caches** if any are stored outside the archive at the same folder with the *.Parcel* archive.

Those sub-file-types are used for actual application storage:

* .node: Node properties
* .present: Present layouts.
* .page: Workflow pages.
* .settings: General YAML formatted document settings.

Those file types are usable as application-specific data transferring formats - those can be imported and exported (those files are self-contained) but is not the actual application storage:

* .layout file
* .workflow file

# The Parcel Distributable

We will pack a bunch of things for the final Parcel distributable (while keeping the size of it as small as possible):

1. Sparse contents at the root folder: Root folder may contain all necessary .Net dependancies (ideally we pack a single executable target specifically to Win64)
2. "Modules" at the root folder: Dll dependancies and modules for the application; They represent optional library functions packed with an executable; This is not a real folder since they must be on the same folder as the main executable assembly; Notice such modules are linked at compile time - we are not supporting live modules.
3. **Components** folder: Core library functions implemented using the Python scripting interface are put here.
4. **Plugins** folder: Empty folder for user and third-party use; Functionally the same as "Components" folder.
5. **Documentation** folder: Generated HTML documentation of nodes and wiki and everything.
6. **Samples** folder: Sample CSV, sample datasets, sample parcel packages.
7. **Tutorials** folder: We will implement a layer of tutorial scripting support <!--Something we learnt at BBI with game campaigns and tutorials--> and write actual tutorials in Python.

# Node Graph Specification

1. All nodes are uniquely identified by its type, node data, and layout information; Nodes don't generally need an ID;
2. All connections are connections between pins (connectors) of nodes - the pins must be able to be identified using an ID/index; For dynamic pins, the owning node itself must have a way to identify the corresponding PIN and its data with an index.