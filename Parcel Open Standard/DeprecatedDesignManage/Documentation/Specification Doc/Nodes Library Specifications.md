<!--This is the official text doc for final specification of all nodes.-->

# Temp Notes

(Parcel Workflow) Function Library: Python Node - takes input through input(Name) function and output values through outputType(Value) function.
(Parcel Editor) Add a Compute Node if we want to allow users to perform arbitrary math operations on arbitrary cells from arbitrary tables - like Excel. However, I am strongly against having such over generalized functionality at all - it's too technical and can be easily abused, so we might want to do our "strongly-typed" functionalities first (by implementing high level workflow-oriented functions).
(Parcel Editor) Add a "Evaluate" node that outputs a single cell. This can be later used for Compounds (in Parcel Present).

# (Presentation Only) Grid Entry node

Allows arbitrary layout and input, *no processing is allowed on such object* - even with the **Compute** node (Compute node only allows cell-based operations on **Table** objects). 

A grid will have a **Cell Actions** pop up; Grid cells can expand and change span.

# Special Nodes

<!--Special nodes usually have specialized front-end supports, but they are defined in their own Toolbox assembly, so when CLI program tries to execute them it can just ignore those nodes and can still deserialize the graph file - and there is no dependancy on WPF module.-->

**Decoration**

* Heading (Header plus text)
* Text
* URL (with header)
* Image
* Markdown (with text and header)
* Audio (require SFML)
* Web Page
* Help Page (select the subject from Properties panel)
  
**System**

* Contact (Email Suggestion/Bug/General Inquiry; "Double Click to Open Contact Form"; Editing details in Properties Panel, trigger event when close panel).
* Graph Attributes (provide access to graph attribute settings)
* About