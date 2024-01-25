# Parcel Open Standard

Parcel Open Standard is the core specification of expected behaviors for interoperability and observability of the entire Parcel platform and ecosystem.

The core aspect of this standard is **graph behavior compatibility**, which refers to and contains directly **user-authored contents**, while all frontend and backends are generally tolerant and free-for-all-kinds-of-implementations.  
In general, any successful Parcel implementation should have a super lightweight, fast and efficient core engine, versatile backend services, and very heavy and powerful and intelligent frontend.

## Parcel Solution Architecture

A typical parcel implementation includes an editor, an execution engine, and a graph description.

```mermaid
```

Execution of the graph may be interpreted, or compiled, or sent to backend service for execution.  
For functional graphs, no debugging facility is needed.
For procedural graphs, the front end need to be able to send only the needed nodes for execution, while the backend need to be able to maintain current execution state.

Parcel has three distinct models of execution (called Interpretation Modes) that's applied on the graph level and mostly defined by the GUI that can be used independently and mixed together through GUI support - fundamentally everything is just a node. The declarative/functional model can be considered one, while the procedural model is distinct.
All parcel native frameworks will be declarative and functional. Procedural contexts (when supported by the GUI) can usually be used directly as anonymous sub-graphs for drop-in logics (whenever a variable is declared, or any sort of flow control is used, they are automatically encapsulated inside a procedural context with drawn boundaries to the rest of the context, with explicit input and return nodes).
Pure and Python script - and for that matter, all functions/nodes/graphs - can be used either as pure functions or as process nodes.

In terms of procedural context, certain flow control constructs and variable related constructs (including the execution order itself) must be implemented as higher level of entities (e.g. variables of graph) for they cannot be represented with functional nodes.

* Parcel Backends: Parcel Backends provide services for frontends to be built upon. Certain frontends may also directly use Core Engine.

## File Structure

Parcel features a maintenance-free file format:

```
Parcel Graph
    Document Properties
    Graphs (Including subgraphs)
    Nodes
    Revisions (Graphs)
    Revisions (Nodes)
    (Internalized) Payloads

(Optional) Parcel Graph External Payload Pack
    (Internalized) Payloads
```
Parcel supports those serialization: Binary, (JSON), YAML, (RTS).

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