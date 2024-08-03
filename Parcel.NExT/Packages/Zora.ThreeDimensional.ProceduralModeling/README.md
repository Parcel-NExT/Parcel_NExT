# Zora.ThreeDimensional.ProceduralModeling

Provides preliminary modeling and rendering.

Requires: Python (3.10+), Blender (bpy)

(Procedural Preview result: Box, Bevel)

Implementation wise, we could make it like LINQ - save instructions then build everything on demand.

## Technical Remark

- [ ] (As of 20240803) At the moment when previewing multiple operations in the PV1 Neo OutputWindow the windows show the same content. This is not an issue with this package (except that we do need to implement Serializable) but an issue with the frontend - the frontend should automatically cache all "Preview Points" (for all output windows) before letting the evaluation state progressing.