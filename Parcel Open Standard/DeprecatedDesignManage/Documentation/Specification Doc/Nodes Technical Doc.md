# Overview

We use a **unified node architecture** acroos all three components of Parcel (Parcel Datatables, Parcel Workflow, Parcel Presentation). In each environment, the nodes may be called different names: 

1. In Parcel Datatables: Nodes are usually represented/called **Tables**; They are saved inside *.pdb* (Parcel Data-Block) files.
2. In Parcel Workflow: Nodes are usually called **Processor Nodes**; They are stored inside *.workflow* files.
3. In Parcel Presentation: Nodes are usually called **Documents/Views**; They are stored inside *.layout* files.

No matter what they are called or in which storage file they are stored, they all serve the same functions as described below:

1. They either directly contain or uses a pointer to point to some data source
2. They can have additional layer of operations stored as parameters that perform actions on those data sources
3. They can take multiple inputs to modify the behavior of their processing
4. They can generate some kind of output.

Here is an important concept: the entire Parcel solution is Node-based. It's node file-based, or whatever-component/workspace-based, or software/presentation/offline/web-based. Meaning that the program will tries its best to **find and index all Nodes (rather than files) (through user usage history and manual addition) that exist**. This encourages re-use of nodes and avoids repetitive definitions as much as possible: *A workflow can have a clone Node from a Parcel Datablock output Node, which itself takes inputs from a few Nodes within that Datablock file some of which references some CSV files stored on disk, and others of which references Nodes defined in other Parcel Datablock files*. 

We can see that how movement and renaming of files can easily break such dependancy - that is intentional! For web-based presentation, depends on final package type (whether or not it has a server and whether or not it is run locally), things can automatically collect and package themselves for a self-contained presentation. For file-sharing, one can also easily pack a sharable **Parcel package** from any of the *Parcel context*, with custom options as to whether a particular data node needs to download/cache any sort of data. There will be a **Dependancy Manager** for clearly viewing and toggling such dependancies.

<!--Modify Workflow Syntax to incorporate this new unified Node structure.-->

# Node Structure

* Metadata (Basic attributes)
* Payload (Stored binary data, can be a pointer to cache file; Parameters)
* Outputs (Procedural results)

# Node and Scripting

* Generally, use an Export node to export user defined functions (like Macros but more functional and don't require additional UI)

# Graph Interpretataion

* Consider thinking of all nodes functional - might be helpful in thinking.