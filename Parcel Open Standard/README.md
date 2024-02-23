# Parcel Open Standard (POS)

> (Motto) Make advanced data analysis easy and fun. Use visual and graphical programming to solve common problems.
> A scripting platform is just one-third of the solution: the other two is domain specific methods, and actually getting the work done. But a good platform can make one start far.

Parcel Open Standard is the core specification of expected behaviors for interoperability and observability of the entire Parcel platform and ecosystem.

The core aspect of this standard is **graph behavior compatibility**, which refers to and contains directly **user-authored contents**, while all frontend and backends are generally tolerant and free-for-all-kinds-of-implementations.  
In general, any successful Parcel implementation should have a super lightweight, fast and efficient core engine, versatile backend services, and very heavy and powerful and intelligent frontend.

## Premise: High-Level Design Goal

The goal of Parcel is to drastically improve data analysis efficiency by introducing a lower or zero entry barrier while at the same time fostering high solution maintainability. It's our goal to introduce a new role named "Technical Analyst" into the industry, bridging between traditional analysts and tool/engine/software developers. It's parallel to artist-technical artist-developer relation in VFX and game industry. Conceptually, (in terms of abstraction level) Parcel sits between Python/MATLAB and Excel.

Parcel is intended for non-programmers while with a focus to "technical-design" perspective. The main problem it tries to solve is for teams with very liquid personnel and a need for maintainable automated software solutions without the involvement of strong IT support. 

~~The major inspiration for Parcel is Houdini, along with many other node-based content editing tools. Due to the nature of 2D graphical node-based representation, Parcel's data is inherently unfriendly to version control and maybe subject to potential limits in scalability - however this problem is alleviated internally through unique asset ID per document; Even so, it's recognized that for those well versed in the art of programming, it might be more efficient to write well-organized codes instead of using Parcel for sophisticated software solutions.~~

## Topics

* Typical Implementation Architecture
* File Format
* File Sections

## The Parcel Virtual Machine

The Parcel virtual machine (PVM) defines an intermediate layer between higher level instructions and lower level implementations. This document describes the structure, elements, description and behavior of such a virtual machine. Essentially, PVM is a very simple graph-like node-based structure describing instructions encapsulated as nodes - some of those nodes have processing impacts, others are merely descriptive (e.g. annotations).

A proper parcel program is thus just the description in the form of this node graph - called "Parcel Node Graph (PNG)", and nothing more, and nothing less. The material behavior of such a parcel program when executed by an execution engine depends on implementation. This specifically specifies how any particular implementation should interpret the contents of a parcel program.

## Parcel Solution Architecture

A typical parcel implementation includes an editor, an execution engine, and a graph description.

```mermaid
mindmap
  root((Parcel Impl.))
    Graph Editor
        Frontend
        Backend
    Execution Engine
        Runtimes
    Graph Document
        Binary
        Text
        MiniParcel
```

Execution of the graph may be interpreted, or compiled, or sent to backend service for execution.  
For functional graphs, no debugging facility is needed.
For procedural graphs, the front end need to be able to send only the needed nodes for execution, while the backend need to be able to maintain current execution state.
The engine should generally make ZERO assumption of whether a node/function is pure - because it's responsible for only executing things.

Parcel has three distinct models of execution (called Interpretation Modes) that's applied on the graph level and mostly defined by the GUI that can be used independently and mixed together through GUI support - fundamentally everything is just a node. The declarative/functional model can be considered one, while the procedural model is distinct.
All parcel native frameworks will be declarative and functional. Procedural contexts (when supported by the GUI) can usually be used directly as anonymous sub-graphs for drop-in logics (whenever a variable is declared, or any sort of flow control is used, they are automatically encapsulated inside a procedural context with drawn boundaries to the rest of the context, with explicit input and return nodes).
Pure and Python script - and for that matter, all functions/nodes/graphs - can be used either as pure functions or as process nodes.

In terms of procedural context, certain flow control constructs and variable related constructs (including the execution order itself) must be implemented as higher level of entities (e.g. variables of graph) for they cannot be represented with functional nodes.

* Parcel Backends: Parcel Backends provide services for frontends to be built upon. Certain frontends may also directly use Core Engine.

### Parcel Nodes as Subroutines

Everything is a node - nodes are responsible for declarations, definitions, constructions, and procedural actions.

Nodes are just subroutines as in a typical procedural language; Nodes can be pure, in the sense that given certain inputs it always generates the same outputs. Nodes (even pure nodes) can have IO and all bunch of other procedural actions in it.

A node is tagged as either functional or procedural. This is mostly to simplify architecture, and put the control to node designers.

### Node Content Types

Node content types (or "type path") is a protocol and should satisfy the following purposes: (Notice that nodes are not always implemented as typical objects or functions, e.g. they can be specially handled by frontend/backend/engine)

1. They should be able to reference global assemblies (including .Net standard/system assemblies), and name specific classes and functions that they reference
2. They should be able to reference external graphs, C#/Pure/Parcel scripts, and Python scripts
3. They should be able to reference external files
4. They should be able to serve "special purpose" frontend/backend construct usages; One example of this is the famous Preview node, which is a construct.
5. They should also be able to serve arbitrary user-payload use purpose and thus custom data; One example of this are the annotations, which are Parcel-document specific constructs and do not serve as coding/logic purpose, and do not correspond to any implemented-based classes or functions.
6. The referencing file/assembly paths may either be absolute, global, relative, or even URL based.
7. The referencing behaviors may be either specified as tagging or as attributes; The referenced data may be cached or explicitly stored inside a Payload

Notice even though not all nodes are "logic" or "execution" related, the engine need to be able to handle all of them (e.g. loading images and store them in payloads) and recognize and disregard irrelevant ones.

Here we provide a specific protocol that most implementations should respect: `[Engine Symbol]Optional File Protocol://Assembly Path:Object or Function Name.In-Content Navigation Paths`.  
A default scheme is always for referencing C# classes/functions: `Assembly.dll:namespace.class.function`. Full path for this would be: `[Parcel/C#]Assembly.dll:namespace.class.function`.
A proper protocol format provides failure-remedies in the sense that in case of missing references, it should provide enough information to an experienced human technical analyst what's the originally intended reference source.

Some other examples include: 

* `[Python]Module Path:Function` for executable python functions (which may be implemented as a plain C# routine eventually).
* `[Parcel]Document Location:Graph Name` for parcel graphs.
* `[Special]Preview` for Preview nodes.
* `[Annotation]Text` for annotation nodes.
* `{System}`: For system stuff

The `[Engine Symbol]` part may be optional when contexts are clear. Notice certain node attributes may be used for additional identification and behavior customization purpose per specific usages.

### Node Inputs, Outputs and Inter-Node References

Parcel features a edge-definition-free scheme for node connections.

Inputs have name and source. Values are feed automatically to attribute with the same name. Source can be used to refer to graph variables, or other nodes. Sources can fetch data DIRECTLY from attributes - it's just if an output node with same name was defined, then it has a different visual appearance. Any inputs automatically becomes an attribute (if it's not already defined).
Outputs have name ~~and source~~ (might be enough just having a name). Values are fetched automatically from sources with the same name. Outputs just provide named definitions, but not actual connections. In fact, outputs are not even necessary for other nodes to connect and fetch data from a given node - it's mostly for visual purpose.
The assumption here is that the same output may be used multiple times, yet the same input can have only one source connection.

Attribute values are string representations ALWAYS and should follow the following syntax guides:

* Plain, literal values start with `:`, e.g. `:15`, `:This is a string`
* Graph variables reading uses `$`, e.g. `$graphVariable`
* Node reference uses `@`: `@node.attribute`
* Reserved: `!` - for lambdas, e.g. `!Apply`
* Reserved: `!#` - for lambdas referencing graphs, e.g. `!#MyGraph`
* (Notice essentially attributes must start with a special symbol)

Attribute accessor is a descriptor that can reference either (in terms of underlying construct) parameters or return values, or (in terms of PVM) node attributes or payloads. It's entirely string based. If an accessor contains an object (rather than string-serialized primitive), it will be passing the reference address of that object, then get dereferenced at the time of invocation.

## File Structure -> Parcel Document Specification (PDS)

Parcel features a maintenance-free file format: (Note that nodes must be serialized BEFORE graphs section)

```
MAGIC
(Pure Declarative Sections)
Parcel Document (Magic: DOCU)
    Document Properties (Magic: PROP)
    Nodes (Magic: NODE)
    Graphs (Including subgraphs) (Magic: GRAP)
Instructions
(Automatic Change Sections)
    Revisions (Graphs) (Magic: REVG)
    Revisions (Nodes) (Magic: REVN)
(Optionally Externalized and Binary)
    (Internalized) Current Graph Runtime (RUNT)
    (Internalized) Payloads (Magic: PAYL)
(Optional) Parcel Graph External Payload Pack
    (Internalized) Current Graph Runtime (RUNT)
    (Internalized) Payloads (Magic: PAYL)
```

Parcel supports those serialization: Binary (C# style/.Net core/.Net standard, little endian), (JSON), YAML, (RTS). Additional types might be provided through converters (if necessary). Parcel MAGIC number is ~~`PARCEL WORKFLOW ENGINE - BINARY`~~ `PSF-B` for binary file, and ~~`PARCEL WORKFLOW ENGINE - TEXT\n`~~ `PSF-T` for text file.

We use those somewhat confusing extensions for official parcel file format:

* ~~`.parcel`~~ `.psf` for generic file extension (either ~~`.document`~~ `.psft` or ~~`.graph`~~ `.psfb`)
* ~~`.document`~~ `.psft` for text-based storage
* ~~`.graph`~~ `.psfb` for binary files storage

### Parcel File Format Storage Requirements Specification

* Take Blender style file blocks that allows easier and more selective use of data and arbitrary data payload

## Parcel Serialization Formats

### Text-Based Command-Line Format (MiniParcel)

(Pending consolidating with Mini Parcel)

MiniParcel is not suited for full serialization notably because: 1) It's instruction only and intended to be simply and high level, 2) It doesn't not name specific end targets, 3) It doesn't have concept of binary payload, 4) It's node invocation is positional based and not Parcel document graph-based and key:value attribute based.

### General Serialization Formats

* Header takes two lines
* May contain (file-format-level) comments: just ignore when parsing
* Empty spaces are used for readability for text format. Ignore empty spaces when parsing

### Parcel Serialization Formats - Text (PSF-T)

Requirements:

1. As capable as the binary format, with a binary payload as separate file
2. Must be declarative
3. Must be as clean and short as possible, must be human readable and human-modifiable (generated document can be edited by a text editor), but do NOT necessarily need to be human authorizable (do not need to be efficient if a human were to be author from scratch) - that's the purpose of Mini Parcel.

## Parcel Objects -> File Sections

If we consider Parcel as a loosely typed scripting engine, then nodes define basic binding points for specific APIs. Nodes themselves do not have inherent types.

Notice the revision feature is not necessary for all implementations.

### Document Meta-Data

Document should be lean and contains no logic-critical data - all those are stored in graphs and nodes.

```json
{
    "metaData": {
        "creationDate": "",
        "lastModifyDate": "",
        "editHistory": [
            {
                "date": "",
                "graphUpdates": [
                    {
                        "graph": "",
                        "revision": "",
                    }
                ],
                "nodeUpdates": [
                    {
                        "node": "",
                        "revision": "",
                    }
                ]
            }
        ]
    },
    "comment": "", // Usually not used and not accessed
    "userVersion": ""
}
```

### Graphs

> (Idea) Everything is a graph. (Like blocks in Ruby.)

Graphs are containers of nodes and provide layout and position for nodes - predominantly in a Canvas layout. They serve both as document graphs, special purpose graphs (e.g. HUD), and as functions/macros.

```json
{
    "id": "<Graph ID>",
    "name": "<Graph Name>",
    "type": "<Graph Archetype>",    // Execution mode
    "metadata": {
        "creationData": "<Creation Date>",
        "lastModifyDate": "<Last Modify Date>"
    },
    "revision": "<Revision Number>",

    "contents" : [ // Layout
        { 
            "type": "canvas",
            "contents": [
                {
                    "type": "node",
                    "guid": "",
                    "location": "",
                }
            ]
        }
    ],
    "packages": [],  // Reference package list
    "inputs": { // User authored input definitions and connections

    },
    "outputs": { // User authored output definitions and connections

    }
}
```

Graphs have their own scope of definitions and variables, and is either functional/declaration or procedural - and cannot be both.  
Notice variables are runtime results and thus not part of serialization.  
In terms of GUI, it's possible to "embed"/"inline" subgraphs on current graph, to make navigation easier.

### Nodes

A node is essentially a container of reference and anchor of data. EVERYTHING IS A NODE.

Nodes themselves should provide complete description to where to find (the protocol) and which class/function the node is referring to. From the perspective of Core Engine, it's not necessary to tell which modules/mega-packages to import - because such information is available completely from node definitions themselves alone.

Notice that a node target path CAN NEVER refer to anything but (exposed) classes and class methods. Aka. it is invalid to try to use a node to access (instance or static) class (C#) properties or attributes - or even if it does, it works like a function.

Referencing/invoking a subgraph is just like calling a function, and the referee can provide parameters.

```json
{
    "displayName/name": "<Display Name>", // Empty value signify use default, which usually just shows the node type; Notice (display)name has significannce beyond just used for display - when unique, it COULD serve as unit ID for the node, and is the preferred instance name used in MiniParcel and during code generation.
    "guid": "<Node GUID>",  // Only need to guarantee unique within document, since all nodes are stored in a single section
    "revision": "<Revision Number>",
    "metadata": {
        "creationData": "<Creation Date>",
        "lastModifyDate": "<Last Modify Date>",
        "comment": "<Node Comment>"
    },

    "type": "<Node Content Type>",  // Path to type    
    // Provides misc. key:value specifications for all kinds of purposes; Notice this is NOT payloads
    // - For function calls, provides arguments mapping
    // - For class instance construction, provides construction parameters
    // - For declarative usages, provides explicitly typed and named construction parameters, or contents
    // - For potential payload reference, defines output binding points
    // - For whatever other arbitrary user-created custom purpose, this can hold context-free definitions
    // - For annotations, those host text-based contents and other specifications
    // May optionally be type-hinted, we will simplify things by just putting it as part of name decoration
    "attributes": {
        // Front-end use; Works just like as in CSS for HTML
        "style": "",
        "class": "",
    },
    "tags": "", // Provides symbols and affects additional GUI behaviors (as binary toggles); Derived from function/class attributes or from presets or from user specification; Symbol set: pure, procedural, blocking, server, plotting, locked, log, document, content-only (no inputs and output pins are allowed, useful for annotations), lazy (caches are preview only and won't save to document payload), deligent (caches are always saved to document payload), non-executable (data-only as attributes)

    // [Deprecating] Deprecate Input/Output, use "<Input/Output>" syntax for attributes.
    "inputs": { // User authored input definitions and connections

    },
    "outputs": { // User authored output definitions and connections

    },

    // This section can be replaced with special symbols for attributes
    "proceduralSteps": [ // For procedural context, we just need jump; The (un)conditions and variable read/write are handled inside flow control implementation using attribute values
        // The specific path to take is determined by implementation
        { 
            "pathName" : "",
            "nextNode": "<Node Name>"
        },
    ]
}
```

Node attribute names are camelCased! (Because that matches more directly to JSON representation, C# function parameter name, and feels more scripting like) Notice all attribute values are strictly strings! This applies even if the attributes are serialized as JSON.

Certain attribute names have special meanings:

* `$1, $2, $3...` such numbered attribute names are reserved for positional attribute matching for functions
* `#<Name>`: Such hash names are reserved for execution pins and denotes executional paths
* `<Name`: Indicates an attribute should be exposed as input (in the front end).
* `Name>`: Indicates an attribute should be exposed as output (in the front end).
* `<Name>`: Indicates an attribute should be exposed as both input and output (in the front end).
* `%Name`: Indicates a front-end only attribute (core engine will just ignore), useful for things like front-end native behaviors and styling attributes.
* `~`: Attribute level comments. All contents after this symbol is treated as comment.
* `.` in attribute names denote nested attributes, e.g. attributes within a tab/subpanel.
* `:`: Optional restriction on attribute type

NODE ATTRIBUTES HAS NO CONCEPT OF TYPE AND VALUES ARE REPRESENTED EXPLICITLY AS STRINGS! This sacrifices a bit storage efficiency but greatly simplifies serialization and parsing. They may have "types" but it's for annotation purpose only - real types are only evaluated during execution/interpretation/compilation time! The string-based nature is expected and reasonable for anything that's user-authored. For larger contents, we can consider using payloads for that purpose. Nodes do not need to explicitly be aware of their payloads/caches - those are stored in a separate section.

Attributes can have type hints as part of their names. This is mostly used by front-ends and backend/runtime depending on actual runtime may choose to ignore them. Attribute name MUST be unique irrelevant whether they are used as inputs or outputs (this should be obvious since input/output is indicated through name syntax, not as separate new attributes).
All values or instances are either primitives or objects. And those are the assumed basic types (names should reflect underlying implementation only): 1) Primitives: Number, String, Bool, 2) Objects: Object class is the base of all objects.

Attribute behaviors:

|Format|Function|Runtime Behavior|
|-|-|-|
|`Attribute:Type Hint`|Provide type hint on attribute.|Runtime should completely ignore type hint and treat attribute name as key cue.|
|`Attribute ~Comments`|Provide comments on attribute.|Runtime should strip the comments.|
|`Section.Attribute`|Provide sectioning of attributes.|Runtime should ignore the sections and treat attribute name as key cue - except if the target node expects an `<MethodName>Options` struct, in which case such notation means addressing inside the structure (in a hierarchical fashion).|
|`%`|Front-end attribute|Runtime should ignore such attributes.|
|`$1, $2, $3...`|Positional attributes|Runtime pass those attributes directly in the order to target node.|
|`#`||PENDING|
|`<AttributeName>`|Front-end denoted input/output pins.|Runtime should strip the `<>` symbol and treat attribute name as key cue.|

All nodes will have some sort of `value`/`returnResult` attribute, which will map automatically to `payload:value` if not explicitly defined.

Nodes boundaries are entirely string based! It is NOT possible (out of box) to pass objects directly through input/output/attribute connections! Everything must be referenced by primitive values or as strings (names). In the case of calling instance functions, assuming an attribute accessor contains a valid object, it will be passing the reference address of that object, then get dereferenced at the time of invocation.

### Revisions

This section is the same as the Nodes section except they are older version of the nodes; Only the latest version of the node is used; All older version of the node should generally be put inside Revisions section.

Revisions are plain text copies of entire definition of nodes from an earlier revision. They are identified through node GUID.

### Payloads

> Payloads are NOT merely cached results for display or optimization purpose - they can serve structural purposes!

Payloads are execution results and may represent either arbitrary binary data or runtime cache and are stored in dedicated file sections. Payloads are NOT just for cache! PAYLOADS MAY BE USED TO STORE APPLICATION-CRITICAL DATA SO EVEN THOUGH IT'S NOT IN TEXT-BASED VERSION CONTROL (LIKE GIT AND PARCEL NATIVE REVISIONS SYSTEM), IT DOESN'T MEAN THEY ARE COMPLETELY UNNECESSARY. Notably, payloads will be used to store spreadsheet data and image data.
They are "attachments" to node definitions. They can be either internal (in binary graph), or external binary file (in plain-text graph).

```json
{
    "guid": "<Payload GUID>",
    "node": "<Target Node>",
    "data": {   // "<Payload Data Sections>"
        "name": "<Value>", 
    }
}
```

Payload data is either plain string or full binary data (we have to support binary format for storage efficiency because Parcel WILL deal with large tabular data):

1. Either format must indicate whether its TEXT or BINA in ASCII at the beginning.
2. Either format must hint on potential data type for deserialization (C# string type name)
3. Text format can be plain data or in YAML format
4. Binary format must also indicate content length in bytes

ALL GRAPH AND SUBGRAPH NODES will have cached payloads from previous invocation.

ALL PAYLOADS WILL HAVE A `value` section (tentatively name those as: `result`, `preview`).

Payloads serve critical functions like:

* For front-end purpose: the `preview` section contains pointers for data for display purpose (could point to `value`)
* For runtime purpose: the `value` section contains cached data for functional nodes for optimization purpose
* For meta-programming purpose: the `instruct` section can contain instructs as post-processing behaviors for modifying graph specification

Like attributes, payload names are repurposed for specialized usages, and corresponding handlers can just ignore if do not recognize:

* `!instruct`: This section contains instruction to frontends for graph-level meta-programming purpose; A semi-standard format will be provided.
* `%message`: This provides a front-end use-only (distinct from console prints) messages on the display medium. Use `%error` for errors (e.g. exceptions.)
* `%error`

### Instructions

A plain text based stated sequential sequence of instructions that implements the same functions/execution model as the graphs. This is for runtime implementations that do not handle graphs as defined in earlier sections. On the other hand, all runtimes/backend engines should implement this section to ensure proper understanding of the execution logic and as a fallback way of executing the graph. The text instructions each occupy a single line and thus have line significance, should always end a line with `\n` and line order is VERY important. The instructions are written in Parcel Instruction Set (PIS) - see Parcel Virtual Machine (PVM) documentation for more details. The instructions second should NOT have trailing empty lines.

Below defines all instructions: <!--Consider moving this section to PVM, or just talk about PVM in this document instead of as a dedicated document-->

|Instruction|Name|Meaning|
|-|-|-|
|` `|Empty line|Should ignore; Do not count to line number.|
|`#`|Comment line|Parser should ignore; Do not count to line number.|
|`.<Section Name>`|Section|Defines a code section, can be used for function definitions etc. Non-essential: if an implementation handles this, then it should allow `CALL` to provide section name as the first parameter; Otherwise an implementation can just ignore this. Do not count to line number.|
|`SET <Variable Name> <Variable Value>`|Set variable|Sets a named variable; Different implementations shall provide different number of variables and different available variable names, but those four variables must be provided (the `$` prefix has no significance): `$1`, `$2`, `$3`, `$4`. The `<Variable Value>` is plain text value that may represent either string or number.|
|`COPY <Source Variable> <Target Variable>`|Copy variable|Copies value from source variable to target variable.|
|`CALL <Function Name> [<Function Parameters>...]`|Call function|Calls a function, optionally with a sequence of arguments - the argument values MUST be provided in variables, thus the parameters are all variable names.|
|`JUMP <Line Number>`|Jump|Jump next execution to specified line number (inclusive).|
|`BRANCH <Variable> <Line Number 1> <Line Number 2>`|Branch|Point IP to Line number 1 if variable is true, otherwise point IP to line number 2.|
|`LINE <Line Value>`|Line|Denotes a single value|
|`TEXT <Variable Name> <Line Range>`|Set text|Assigns multi-line text to variables; Since each instruction is a single line thus variable values in instructions do not have capacity to contain new line, the TEXT command is provided to assign multi-line values to a variable. Line range has the format like this: 15-75; It can also be a single line: 15.|

There is no ABI (application binary interface) since it's text-based, and there is no primitive operators like `ADD` or `SUB` because those must be implemented as functions. When passing variables to functions, the address are always passed - depending on implementation and the functions, such addresses might immediately be interpreted as values, or used as is (i.e. inside functions, they can modify variable values).
Besides above, there is a special `@` symbol that can be used to denote valid program lines and is optional. On the other hand, all parameters are space delimited and spaces can be escaped using `"` quotes. That it's, it's just like plain CLI arguments.

Those are ALL the PIS instructions! As you can see, two notably features of PIS are:

1. It's text-based.
2. Its variables can hold arbitrary things.
3. The instruction set itself provides no way to represent binary values in text format - however variables can hold binary variables per runtime implementation. Parcel Standard Libraries will define functions that provides conversion of plain-text to binary.
4. PIS on its own DO NOT define functional programs - all such functionalities require functions, which are provided by the runtime.

All implementations must provide those variable names (the `$` has no significance):

|Variable|Name|Purpose|Size|
|-|-|-|-|
|`$i`|Instruction Pointer|The next execution line. Notice with `SET` and `COPY` one can assign to this variable directly.|8 bytes|
|`$1`|Variable 1|Holds variable.|Varying size.|
|`$2`|Variable 2|Holds variable.|Varying size.|
|`$3`|Variable 3|Holds variable.|Varying size.|
|`$4`|Variable 4|Holds variable.|Varying size.|
|`$r`|Function Result|Holds result of last function call.|Varying size.|

To illustrate the point, here is a basic "Hello World!" example written in PIS:

```PIS
# Call print
SET $1 "Hello World"
CALL Print $1
```

This example illustrates calculating `sin(PI)` and print in formatted string:

```PIS
# Do calculation
@1 SET $1 3.1415926
@2 SET $2 "Calculation result: %i"
@3 CALL Sin $1
@4 CALL Print $2 $r
```

A C++ implementation of a PVM will be provided for reference purpose that can understand all functions that are defined in Parcel Standard Libraries (PSL).

## Engine (Execution Behavior)

In this section we document and specifies some expected engine behavior:

* ALWAYS-EXECUTE (TAG) node in a graph are always executed before any other nodes in the order they are defined in the graph.

(PENDING) During execution, a node can optionally have access to those global states besides the explicitly passed in values through their attributes: current graph/document reference handle, current graph/document variables. Those must be enabled with special toggles (tags), otherwise each node should generally have as isolated as possible runtime context. In general, nodes should not create any side effect except for the manipulating object instance itself represents.

## Parcel Front-Ends

Any functional frontend should NOT ONLY target node-graph drafting completeness but take the efficiency of such construction - both in terms of keyboard shortcuts, non-mouse usage, and version control capabilities - to the highest standard. Houdini is a decent but not good enough example, Unreal Engine Blueprint is an exceptionally bad despite pretty looking example.

## Parcel Back-Ends

Unless explicitly running node by node or as interpretative mode, type check for all nodes should happen at compile time.

A backend can be either stated or not stated (as typical REST APIs). A WebSocket based backbend could potentially have states for each connection as a session - and should indeed utilize such capacity. In this case, all client (for any specific session) requests should be queued and reply messages should be handled in the order of original requests. This could greatly simplify both front-end and back end design, by avoiding concurrent node execution possibilities.

A naive implementation might treat all nodes as strongly typed objects, then runtime behavior of those nodes would simply be predefined methods on those objects, just like original Parcel prototype. For instance, all incoming node definitions will be serialized to proper runtime objects (parameters or attributes are just object properties), and execution of nodes cause results to be cached. Data are passed passed between objects as initialization parameters. This way there is no involvement of reflection or RTTI (runtime type information). However, this either requires explicitly hard code definition of each node type, which is not scalable nor extensible, or have some kind of runtime type generation or runtime object generation or runtime code execution mechanism, which essentially falls into the next implementation.

A more advanced implementation is to generate codes for each node and execution of node graph is just execution of those generated codes. There are lots of ways to implement this scheme: we can generate object definitions for each node, or we can directly generate the final execution code for the entire graph without objectify each node.

## Parcel Native Style, Standard Libraries and Original Frameworks

A node is parcel native if it's implemented as Parcel graph using nodes without external codes.  
POF get things done at unprecedented high level intent and efficiency, with as few nodes as possible.

It makes lots of assumptions that's how it takes minimal setup to get most complicated things done.
It greatly streamlines getting common things done faster while providing room for customization.
It usually provides lots of customization options - so without modifying modular pieces, one can already further customize the behaviors.
It usually provides "macros/template/preset" setups that allows direct implementation-level customizations.

Parcel will dedicate development effort into standalone libraries in the following domains:....

Even though it's legit to reference runtime C# and even Python functions directly in a node, it's never recommended - except those part of the implementation hosting environment RAW standard libraries, e.g. all dotnet runtime libraries and all standard Python distribution libraries - those can expect long-term support. Whenever a Parcel Original library is available, users should preferably use those higher level wrappers.

Only specific subset of runtime and derived functionalities will be exposed through Parcel native libraries/wrappers. Those include those areas:
* File IO
* Socket IO
* Web Services

## Canonical Implementation: Parcel NExT (2024) - Reference Implementation

> Parcel NExT is a C# based hybrid-functional-procedural-logical-OOP graphical scripting platform designed for high-level automation and data analytics.
> Motto: Parcel is the best of all worlds, easy to get in, easier to get out, addictive to use, impossible to abuse, and requires knowledge of all domains to go deep. All your olf knowledge and scripts from C#, Python, and MATLAB will still applu and work, better, and more harmoniously.

This section documents a reference implementation, known as Parcel NExT, based on **latest version of .Net** (at the moment it's .Net 8). It includes complete suite of core engine, back end, front end, and domain specific libraries.

1. Ama: C# first multi-runtime hybrid execution mode runtime and execution engine.
2. Tranquility: WebSocket service provider.
3. Gospel: Godot graph editor.

```mermaid
---
title: The Parcel Platform Overview (Parcel.NExT Reference Implementation)
---
flowchart TD
    id0[The Parcel Platform]

    id1[Parcel Open Standards]
    id10[Reference Implementation]
    id3[Other Potential Parcel Implementations]

    id4[Main Specification]
    id5[PVM]
    id6[PIS]

    id11[Parcel.V1]

    id2[Parcel NExT Ecosystem]
    id12[Core Components]
    id13[Standalone Tools]
    id14[Community Services]

    id7[Ama]
    id8[Tranquility]
    id9[Gospel]

    id15[Escort]

    id16[Arcadia]

    id0 --> id1
    id0 --> id10
    id0 --> id3

    id1 --> id4
    id1 --> id5
    id1 --> id6

    id10 --> id2
    id3 --> id11
    
    id2 --> id12
    id2 --> id13
    id2 --> id14

    id12 --> id7
    id12 --> id8
    id12 --> id9

    id13 --> id15
```

### Parcel.NExT

<!-- Naming -->

This suite is collectively known as **Parcel.NExT**, including following notable components:

* Ama: Core Engine
* Merlin: REST API Backend
* Gospel: Desktop-first cross-platform graph editor
* (Lightening Browser)
* Paper Space: Web-first Frontend (NodeJS)
* Airi: Arcadia Agent/General front-end deskmate
* Escort (Parcel, Parcel-cli): Standalone executioner and service runner, logger.
* Tranquility (C#): WebSocket based stated full-feature C# backend.
* Flux: Standalone package manager program. (Might also provide Forge: standalone packager or uploader to Arcanum)
* Arcanum: Package index/sharing platform. (Don't call it "marketplace"!)
* Catalyst: Gospel based cloud processing platform.
<!-- Use Medalian and myth names -->

Additional (experimental) Front-ends:

* Relic

Experimental services:

* Arcadia (Parcel Hive/Sanctuary): Simple universal websocket text-chat server with multiple channels providing live-online community support.

Other names:

* (POF) Destiny: Desktop automation.
* (POF) Avalanche: Parcel Distribution Service
* Hermit: ????
* (POF) Messiah: JavaScript-native frontend runtime web-building framework that utilizes backend through websockets
* Kalos: Node-native file browser and media viewer <!--From kaleidoscope-->
* Lightening: Node-native interface to Lightening browswer
* Phanto: Time-domain simulation
* Polyglot: CodeGen

### Core Engine

The core engine will be an interpretative engine with support for (final) code generation. This is chosen so debugging is possible and easier - if we were to actually compile the code, then it's harder to set breakpoints and have meaningful intermediate REPL-like prompts. The interpretation engine is based on Roslyn and implementation is similar to Pure.

#### Numerical Handling

All in-transit numbers are represented as string, and all default number representations are double. A single `Number` class is defined to encapsulate this construct and **provide all primitive mathematical operations as functions**. A meta-layer could be provided, which on the frontend when performing number operations special nodes are created which by default maps to the `Number` class functions as mentioned above, but otherwise during code generation gets converted directly to C# basic operators.

### Caching

All computed node result within a functional graph for all pure functions are automatically cached to avoid unnecessary re-computation.  
Anything that takes more than 5 seconds to execute are automatically cached permanent within the document during saving.

### Special Nodes

#### Preview Node

Preview node is an engine construct and handled in such a way that it takes reference of the incoming nodes' payload.

### Node Attributes

Because nodes are just subroutines, there is no inherent inputs and outputs - there are however archetypical inputs and outputs in the sense of a typical programming subroutine. The provision of node inputs and output pins and node properties panel is entirely for the sake of interface and simplicity of construction of nodes.

### Node Execution

All node execution results are immediately cached in-memory, but deleted if computation was fast, or nodes are marked `lazy`.

### Payloads

(If needed, payloads can contain Pure scripts, caching executions instead of saving results - although this is not necessary if we have lazy/diligent tag)

### Tagging

Tagging is used to both modify interface display and interactive behaviors.

* Pure: Pure functions have reusable caches. A function can be marked pure or not pure.

### Front End

> The GUI is all about producitivty, comfort, and efficiency.

Parcel.NExT is a professional and developer-style frontend suitable for distraction-free and productive work.

Key design guidelines for Parcel.NExT Front-end Node Editor:

1. Avoid clustering, show only key information
2. Quick, easy, multidimensional preview of many nodes data.
3. Everything is a node.
4. Utilize visual location memory.
5. Offering node editing, node organization, node execution, node debugging, annotations, real-time collaboration, presentation, and learning, documentation, IDE, refactoring functionalities.

Node attributes can be selectively exposed as inputs/outputs and edited either in property panel/window or directly on the node.

The frontend also features real-time progress indicator and node highlight for currently executing graph.

The Front End also features a top-level left-side current active preview pane, and a terminal pane.

Cached results can be viewed either as a pop-up window (like original Parcel) or as an in-graph Preview Node (which can also be used to view any other properties of a node).

The graph itself is an object which can be exposed as reference through a node (which constructs and overrides default), which can then be pulled properties out and attach properties to (key:value pairs).

A typical usage scenario goes like this:

1. User open a document or creates and saves a new empty document (through GUI or CLI)
2. User selects from and drops a few module nodes to define which modules to discover ("import") - this is only significant to the front end
3. User optionally imports existing user libraries from discovery paths, or drag in directly (will reference as relative path by default)
4. User keeps adding nodes and complete the graph functionalities.

Annoymous/In-line graphs are supported to keep lambda-like references simple and clean.

When promoting user to select node, Parcel.NExT presents the nodes through three different dimensions: by application domain, by (operating) object types, and by high-level libraries/categories. One can also search for nodes by name and tags, have favorites, and have history sorted by most frequent use, and show nodes already used in current document.

### MiniParcel

Either the text format proper, or a REPL DSL frontend.

MiniParcel is human-oriented declarative scripting language that provides a quick interface into graph authorization. It is less verbose to proper text-based format.

MiniParcel makes assumptions about target names and function calling signatures and greatly simplifes syntax.

## Key Design Decisions

### Parcel Node ID Uniqueness

Theoretically when referencing external documents we can be specific to specific graph and node id to reference (in terms of protocol string). Practically when we reference external documents we reference the graph, not the ID.
On the other hand, we wish to have a flat, non-hierarchical storage scheme for easier parsing of document storage contents, so all nodes reside in a single section, instead of having nodes for each section for each graph.

Because of the way storage is done, nodes must have unique IDs cross the document scope, not graph scope.

### Necessity of Graph Runtime and Node Payload

Like Excel, Pure Notebook, and Jupyter Notebook, we wish to and must keep non-code and non-logic part of runtime-only content for self-contained documentation of the entire Parcel Document. However, those things changes and those things are not USER-SPECIFIED CONTENT DATA, but runtime results.
Those can be binary, and thus should be able to be stored in separate file sections or as externalized files.

The need for graph runtime is a complexity due to requirements of procedural contexts's state bookkeeping, and it apparently can involve dynamic type of intermediate state objects as such.

### Per Parcel document file structure, Where do we put payload reference? (Similarly, where do we keep graph runtime)

If payload reference is put directly as node attribute, then it could often change when nodes are executed - though if we are careful in implementation, then we could avoid reference id change by reuse existing payload objects. On the other hand, things like Revision ID changes anyway - but things like revision and last modification only changes when there are actual user content change.

Since payloads are attachments, and from a lean structure perspective nodes DO NOT NEED to be aware of underlying implementation and content styles and payload type, it makes sense to keep payloads completeme separate to nodes and make nodes NOT aware of payloads.
Similarly, graph runtime should store in separate file data section.

### Procedural Context: Implementation

(Also see Observation 20240126, pending rephrasing and documenting it here)

We realized that For procedural context, we just need (un)conditional jump and variable read/write - like any typical minimal turing complete logic. A DSL based approach is to define conditions and potential paths in a list and the first valid option is taken as next step.

The latest specification/implementation in Parcel.NExT however, handles conditions inside flow control implementation using attribute values, and the paths defined are just for reference purpose only. This way there is no need for DSL, and everything is declarative. 
The maintainence of current flow control state can be managed inside payload as a localized state, and the flow control element itself decides which next step to take based on its current state, and flow control logic.

## Further Proposals

### Parcel Native Instruction Set

(See ADO Item)

## FAQ

### What is a node? Is node an object, a function, or a construct?

> Everything is a node; Node is everything.

A Parcel Node is just a specification; A front-end visual node can be either a primitive, a macro, a graph reference, a display feature, or a proper Parcel node. Broadly speaking, a node might also define a preset, define a type, or contain graph/document settings.

## References

Resources and Parcel development related references.

Reference implementations:

* SWIProlog
* Elixir
* Haskell
