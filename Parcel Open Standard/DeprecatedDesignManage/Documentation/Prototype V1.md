# Todo

* (#20220409) Now factor analysis is working, next we should do: Basic Plotting (web or in window) (Produce lots of plotting types and options - like Python, unlike Excel, but make it easy to use), Basic Hosting (Web); Complete the analysis (report generation?)?
	Scientific Plotting (mostly provide templates rather than customization)

# Notes

<!--A-->
<!--B-->
<!--C-->
* (Color) Subgraphs will have a *a slightly transparent-looking gray-green background*. The procedural context will have *a slightly transparent-looking gray-red background*.
<!--D-->
* (Design, #Usability, UI) In the future (e.g. for the **Procedural context**) we can have a full suite of low-level strongly typed nodes, in which case a simple Pop-up will be less convinient to use; Instead, devise a tree-view structure like in Unreal Engine, or equivalently, as in Clarisse shader nodes. Very likely we can/should re-design a specialized node framework for this context - so everything there truly gets generated directly to C# source code (because it's so low level and strongly typed). For instance, for numbers we have Number Add, Number Subtract nodes; For strings we have String Replace, String Concatenate nodes, etc. It's completely different and doesn't use the old graph queue for execution - it generates everything directly to C# and runs from there (much like Clarisse compiles shader nodes to OSL and UE4 compiles blueprints to C++) - this way we don't need to worry about defining specific C# classes for those pseudo-codes, because there are merely descriptions of underlying codes (for implementation with Nodify, most likely we just provide a single node like AutomaticProcessorNode, but for procedural context, and all signatures of function calls are defined in that node). The procedural context will have *a slightly transparent-looking gray-red background*.
* (Design, #Web Host) **Preview** can be made into an interactive page,with multiple visual options. 
* (Design, #Web Host) Graphing outputs configured graph presets as Server Config (we might encapsulate directly as a Present object), used for Dashboard building.
* (Demonstration) For demo purpose, provide Bar Chart, Line Chart and Raw Table visualization for Present. For the dashboard itself, the more elaborate we can make it the better - because that's the apparent selling point for those who can't yet appreciate the power of the node graph itself.
* (Development, Project, Example) Facto Analysis and its matrix multiplication stuff
* (Development, Project, Example) Country aggregation CSV, SQL, Visualization
* (Development, #20220515) Before we go back to OTPP we might want a complete extension framework - so we can develop proprietary Toolboxes in-house without pulling the entire Parcel repo. Be  mindful that it might need support in both: Nodes development, customized GUI (front-end) support, and new node data type support.
<!--E-->
<!--F-->
* (Feature, Component, #Important) As mentioned by Ellot, Plotting is VERY important for this thing to be useful and as a self-contained package and depend less on Excel/Python/Matlab as interoperation. "Graphing and plotting is most important."
<!--G-->
<!--H-->
<!--I-->
* (Implementation) (Parcel, Prototype, Proposal, Idea) Just use MS binary serialization framework for now. For full YAML support, we will not be Nodify dependant so it's a completely different implementation in the future. Maybe do that only for import/export.
* (Issue) As "convincing (Excel) users" goes, we need advice on (in terms of presentation): Presentation, software ease of use, practical concern. (To be honest, for actual appeal, we just need to make this software feature-packed and people will be attracted to the FEATURES themselves - **it's otherwise very HARD to find someone who will possess the same level of *vision* as us, in terms of how powerful this node-based non-destructive workflow-graph paradigm is**)
* (Issue, #20220409, #Build, #Solution) Currently we cannot build runnable builds - Rider simply can't build, while Visual Studio can build as self-contained but it won't run. (The problem is solved when we host WPF inside ASP.Net Core and publish using WebHost instead of WPF project, also, Self-Contained simply won't work: have to be framework-dependant; We might include the framework installation in our installer in this case)
* (Issue, #20220410) SideBar not showing in .netapp 3.1 and PreferExactMatch tag removed and Virtualizing reference not used. (Looks like this is because of API change and we are using .net5.0 template for .net 3.1)
* (Issue, #Bug, #20220410) Currently when click "Preview" for WebPreview nodes lots of preview tabs are opened for various nodes all conflicting with the single available pointer - we might want to automatically avoid showing previewing for such nodes by not keeping their IsPreview on. Another issue is that we are doing "OpenPreview" during WebPreviewNodes' Execute function, which when having lots of chained config files it will open a whole bunch of tabs.
* (Issue, #Bug, #20220518) Currently the file *NFC Wall Perimeter Calculation.parcel* when re-opened and click "Preview" on the last node causes an invalid cast from Int32 to Double problem - likely there is some node/connector data type issue during serialization (because the graph didn't raise any issue when we first create it - or it might also because at that time we weren't running the debugger and was using the publisehd build). Also as error message goes, currently the **Calculator** will not emit an error message when incorrectly entered `pi` for `Pi` - also I really just want this to be case insensitive (can we do a TitleCase trick?).
* (Issue, #20220516, Node, Feature, #Specification) Currently "Preview" node is not (completely) implemented - you can't just connect arbitary connectors to it; Also the Preview Node should have an output - so it's kind of like a named Knot, useful for redirection. <!--(Remark, Specification) Pending extract this requirement to node specification.-->
* (Issue, #20220409, Solution, Implementation) In case we can't get hybrid model to work - just develop it as a self-contained program, and communicate as sub process using args and maybe REST API: mostly it just does one or two things - Preview, offline node logic execution (when we get saving working).
* (Parcel, Issue, Question, Methodology, #Design) For implementing any given functionality, how do we decide whether to do it as a single node or multiple functionally related nodes, and providing parameters as inputs or on the node as controls or as editable properties in Property panel, or from upstream configuration node ouputs? For instance, for Pivot Table function, we can do with a Reorder + Pivot with no additional inputs, or we can let Pivot node has its own override of field order. Maybe an Instruction Designer can shed light on this? Some questions we may ask to help us decide: Is it easy? Is it straightforward? Is it learnable?
<!--J-->
<!--K-->
<!--L-->
<!--M-->
<!--N-->
* (Node, Suggestion, #Change, Design) UX Feature: Make **Extract node** name ambiguous - allow rename selection case insensitive.
<!--O-->
<!--P-->
<!--Q-->
<!--R-->
<!--S-->
* (Specification, #Layout) For layout, we can start with a gtid system before moving on to more flexible ones. And we can start with a 3 Grid default.
* (Specification) Graph/Layout server config can have children, those automatically segments sections, Grids, and Pages (side bar). Also provide Label in Graph, which automatically formats it as header or plain text span depending on which grid location it shows. For our grid system, we have: Page, Section, Header Line/Title Line, 3 Grids, 4 small items per grid line.
* (Specification) During Present we can first create endpoints for each data table at /data/TableID.csv so instead of creating explicitly as HTML tag elements, those data table data can be streamed through a GET request. This way backend can reply on some sort of list/dictionary of tables when needed (e.g. let Javascript do the work) instead of solely on raw table objets (when C# is usable).
* (Suggestion, UI, #Design) UI Feature - to avoid clustering: Highlight selected node's path
<!--T-->
* (Toolbox, Design, Suggestion, Summary) Toolbox: people might want Code Geenration for "transferrrability" - however, in essence, or most importantly, all they actually want is to provide Python module binding to execute the graph file directly and be callable in other programs (aka. not the generated code string itself).
<!--U-->
<!--V-->
<!--W-->
* (WebHost) Three main visualizations for webhost: SQL interface, Data Grid and plotting, Layout Present (Dashboard)
<!--X-->
<!--Y-->
<!--Z-->

# Design Specification

* To avoid potential abusing and over-complication, the main workflow graph context shall NOT support conditional, loop, storage/variable/state of any kind - it's strictly functional; The array/batch job behavior is achieved through a combination of array/batch targeted input nodes, and graph-level scheduling based on detection of mismatching output/input types that automatically dispatch a sequential/parallel job if the input is an array type of the expected singular type instance - this is absolutely transparent on individual processor nodes (aka. the processor nodes won't need to be aware of this); Aka, it's strictly array based and functional. However, for certain specific application scenarios (e.g. business operation), we can have a complete programmable **State Graph** (it's a node), that is just a complete implementation of UE4 style Blueprint with explicit **Execute** node that defines execution order.
* As execution model goes, at the moment we are using an execution graph - however, as in Nodify.Calculator example goes - we can actually make everything real-time, this will make connector communication smarter and generally easier to implement. The tradeoffs are that users will have to wait a bit longer when creating/initializing nodes, and when making connections. Maybe we can provide this as a "**Real-Time**" mode.