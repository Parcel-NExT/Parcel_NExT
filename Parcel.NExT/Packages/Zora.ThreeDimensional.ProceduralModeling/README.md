# Zora.ThreeDimensional.ProceduralModeling

Provides preliminary modeling and rendering.

A entirely "Image-based" library without any demand on front-end. (Scope) This library focuses entirely on procedural modeling for form study and (pre)visualization purposes. Requires: Python (3.10+), Blender (bpy). 

Implementation wise, we could make it like LINQ - save instructions then build everything on demand.

## Technical Remark

- [ ] (As of 20240803) At the moment when previewing multiple operations in the PV1 Neo OutputWindow the windows show the same content. This is not an issue with this package (except that we do need to implement Serializable) but an issue with the frontend - the frontend should automatically cache all "Preview Points" (for all output windows) before letting the evaluation state progressing. Similar thing happens when we split the execution flow - states will not reflect properly at logic split point, which is also due to runtime implementation issue, not this library per se.