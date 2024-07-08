# Workflow

Workflow is described using a YAML in this revision; Additional program binary data (if any) can be packed inside a single (temporary use) **.zip** archive or a cache folder.

# Main Structure

A **Workflow** is essentially a collection of interconnected nodes. The main structure of a workflow specification file contains a list of nodes.

Node attributes will be dynamic - depending on specific node and whether the input is a reference, the specification structure can change (so it's not a simple map to a single data structure and might require dynamic parsing).

A Node can be referencd, in which case all of the referencing node's parameters are the same as the referenced node. <!--There will not be instancing since the node data block is not as complicated as that of the Clarisse-->

# Common Node Attributes

Those attributes are common to all nodes:

1. **Name**: This serve both as display name and unique ID of the node
2. **Type**: This represents the underlying node object type; The type of a node affects its behaviors and parameter sets
3. **Parameters**: Map of named inputs to the node

Most nodes if having an output, will have at least one single output pin with the name **Main Output**.

* Output pins can have multiple names/alias
* Nodes can have keywords and alias

Notice that the viewport location of nodes are managed in a separate data structure - **they are not part of the inherent data of Nodes**.

# Node Type and Overloading Conventions

I am thinking we should avoid overloading node inputs too much and specialize nodes depending on *the type of data source* they use and the *running efficiency of a particular algorithm*, for instance, instead of having a single **Read Table** node that can ~~read either CSV, XLSL, database and online URL, we can have Read CSV from file, Read XLSL from file, Read Table from SQL Database, and Read Table from URL~~ <!--That's too many; I think when there are so many similar varieties, we'd better just encompass it in a single node.-->