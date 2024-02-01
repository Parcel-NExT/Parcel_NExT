# Patterns

This part covers some common usage patterns for solving practical problems.

## Data Table Based Processing - Attribute Create

<!--From functional scripting, from Excel usage, from Houdini point processing, from limitations of no-lambda LINQ. In Houdini it's the Attribute Wrangler node.-->
<!-- Like Pandas Dataframe -->

We can, through multiple succession of such processing, add attributes to existing table, which can then be used for latter actions.

## Graph as Data

Use graph to define "global" environment variables etc. and export them.

## Data Processing Patterns

In Parcel, there are six distinct patterns when dealing with large tabular data, which can be mixed and matched: (depending on lazy/immediate, procedural/functional/state-based, functional/OOP)

1. DataFrame/DataGrid based (procedural)
2. DataFrame/DataGrid based (functional)
3. Object based (procedural/functional): array and collection processing
4. SQL based (strictly functional)
5. SQL based (nodal and functional and delayed)
6. Espresso context (procedural)
7. Plan Based
8. Matrix based (purely numerical)

One should keep functional and use object based with delayed plan if performance is concern. In general, to keep the code clean, it's better to keep a single pattern inside a single graph.

(Observation) Why is delayed/lazy evaluation EVER useful in a programming context?
Isn't the script itself a "plan" already?
The ONLY value is if it has internal optimization of some kind AND in the end it triggers parallel processing - it's a bit harder to do parallel at each step otherwise (user wishes to do it explicitly).

Notice with any sort of delayed node, the Frontend should be able to get full preview results when previewing it.