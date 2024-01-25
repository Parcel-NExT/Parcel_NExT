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

## File Structure

Parcel features a maintenance-free file format:

```
Parcel Graph
    Document Properties
    Graphs (Including subgraphs)
    Nodes
    Revisions
    (Internalized) Payloads

(Optional) Parcel Graph External Payload Pack
    (Internalized) Payloads
```
Parcel supports those serialization: Binary, (JSON), YAML, (RTS).

## Parcel Objects -> File Sections

If we consider Parcel as a loosely typed scripting engine, then nodes define basic binding points for specific APIs. Nodes themselves do not have inherent types.

### Graphs

Graphs are containers of nodes and provide layout and position for nodes - predominantly in a Canvas layout. They serve both as document graphs, special purpose graphs (e.g. HUD), and as functions/macros.

```json
{
    "name": "<Graph Name>",
    "type": "<Graph Archetype>",
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
    "content": "<Node Content>",
    "payload": "<Node Payload Reference>",
    "attributes": {
        
    },
    "inputs": { // User authored input definitions and connections

    },
    "outputs": { // User authored output definitions and connections

    }
}
```

### Revisions

This section is the same as the Nodes section except they are older version of the nodes; Only the latest version of the node is used; All older version of the node should generally be put inside Revisions section.

Revisions are plain text copies of entire definition of nodes from an earlier revision. They are identified through node GUID.

### Payloads

Payloads are arbitrary binary data or cache and are stored in dedicated file sections. They can be either internal (in binary graph), or external binary file (in plain-text graph).

## Parcel File Format Storage Requirements Specification

* Take Blender style file blocks that allows easier and more selective use of data and arbitrary data payload