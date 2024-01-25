# Parcel Open Standard

> (Motto) Make advanced data analysis easy and fun. Use visual and graphical programming to solve common problems.

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

## Parcel Solution Architecture

A typical parcel implementation includes an editor, an execution engine, and a graph description.

```mermaid
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

### Parcel Nodes

Everything is a node - nodes are responsible for declarations, definitions, constructions, and procedural actions.

Nodes are just subroutines as in a typical procedural language; Nodes can be pure, in the sense that given certain inputs it always generates the same outputs. Nodes (even pure nodes) can have IO and all bunch of other procedural actions in it.

## File Structure

Parcel features a maintenance-free file format:

```
MAGIC
Parcel Document (Magic: DOCU)
    Document Properties (Magic: PROP)
    Graphs (Including subgraphs) (Magic: GRAP)
    Nodes (Magic: NODE)
    Revisions (Graphs) (Magic: REVG)
    Revisions (Nodes) (Magic: REVN)
    (Internalized) Payloads (Magic: PAYL)
(Optional) Parcel Graph External Payload Pack
    (Internalized) Payloads (Magic: PAYL)
```

Parcel supports those serialization: Binary (C# style/.Net core/.Net standard, little endian), (JSON), YAML, (RTS). Additional types might be provided through converters (if necessary). Parcel MAGIC number is `PARCEL WORKFLOW ENGINE - BINARY` for binary file, and `PARCEL WORKFLOW ENGINE - TEXT\n` for text file.

We use those somewhat confusing extensions for official parcel file format:

* `.parcel` for generic file extension (either `.document` or `.graph`)
* `.document` for text-based storage
* `.graph` for binary files storage

### Parcel File Format Storage Requirements Specification

* Take Blender style file blocks that allows easier and more selective use of data and arbitrary data payload

## Parcel Objects -> File Sections

If we consider Parcel as a loosely typed scripting engine, then nodes define basic binding points for specific APIs. Nodes themselves do not have inherent types.

### Graphs

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
    "environments": {   // Environment attributes

    },
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

A node is essentially a container of reference and data. EVERYTHING IS A NODE.

```json
{
    "displayName": "<Display Name>", // Empty value signify use default, which usually just shows the node type
    "guid": "<Node GUID>",
    "revision": "<Revision Number>",
    "metadata": {
        "creationData": "<Creation Date>",
        "lastModifyDate": "<Last Modify Date>"
    },

    "type": "<Node Content Type>",  // Path to type
    "construct": { // Used especially for declarative usages, provides explicitly typed and named construction parameters

    },
    "content": "<Node Content>",    // Text-based contents and context-free definitions, notice this is NOT payloads
    "payload": "<Node Payload Reference>",
    "attributes": {

    // Front-end use; Works just like as in CSS for HTML
    "style": "",
    "class": "",
        
    },
    "tags": "", // Provides symbols and affects additional GUI behaviors (as binary toggles); Derived from function/class attributes or from presets or from user specification; Symbol set: pure, procedural, blocking, server, plotting, locked, log, document, content-only (no inputs and output pins are allowed, useful for annotations)
    "inputs": { // User authored input definitions and connections

    },
    "outputs": { // User authored output definitions and connections

    },

    "proceduralSteps": [ // For procedural context, we just need (un)conditional jump, and variable read/write
        // The first valid option is taken as next step
        { 
            "condition" : "",
            "nextNode": 
        },
    ]
}
```

Nodes do not need to explicitly be aware of their payloads/caches - those are stored in a separate section.

### Revisions

This section is the same as the Nodes section except they are older version of the nodes; Only the latest version of the node is used; All older version of the node should generally be put inside Revisions section.

Revisions are plain text copies of entire definition of nodes from an earlier revision. They are identified through node GUID.

### Payloads

Payloads are execution results and are arbitrary binary data or cache and are stored in dedicated file sections. They are "attachments" to node definitions.  
They can be either internal (in binary graph), or external binary file (in plain-text graph).  

ALL GRAPH AND SUBGRAPH NODES will have cached payloads from previous invocation.

## Parcel Frontends

Any functional frontend should NOT ONLY target node-graph drafting completeness but take the efficiency of such construction - both in terms of keyboard shortcuts, non-mouse usage, and version control capabilities - to the highest standard. Houdini is a decent but not good enough example, Unreal Engine Blueprint is an exceptionally bad despite pretty looking example.

## Canonical Implementation: Parcel NExT (2024)

This section documents a reference implementation, known as Parcel NExT, based on **latest version of .Net** (at the moment it's .Net 8). It includes complete suite of core engine, back end, front end, and domain specific libraries.

### Core Engine

The core engine will be an interpretative engine with support for (final) code generation. This is chosen so debugging is possible and easier - if we were to actually compile the code, then it's harder to set breakpoints and have meaningful intermediate REPL-like prompts. The interpretation engine is based on Roslyn and implementation is similar to Pure.

### Caching

All computed node result within a functional graph for all pure functions are automatically cached to avoid unnecessary re-computation.  
Anything that takes more than 5 seconds to execute are automatically cached permanent within the document during saving.

### Node Attributes

Because nodes are just subroutines, there is no inherent inputs and outputs - there are however archetypical inputs and outputs in the sense of a typical programming subroutine. The provision of node inputs and output pins and node properties panel is entirely for the sake of interface and simplicity of construction of nodes.

### Tagging

Tagging is used to both modify interface display and interactive behaviors.

* Pure: Pure functions have reusable caches. A function can be marked pure or not pure.

### Front End

Cached results can be viewed either as a pop-up window (like original Parcel) or as an in-graph Preview Node (which can also be used to view any other properties of a node).

The graph itself is an object which can be exposed as reference through a node (which constructs and overrides default), which can then be pulled properties out and attach properties to (key:value pairs).

## References

Resources and Parcel development related references.

Reference implementations:

* SWIProlog
* Elixir
* Haskell
