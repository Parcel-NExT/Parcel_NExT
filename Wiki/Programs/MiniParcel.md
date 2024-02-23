# Mini Parcel

MiniParcel is both a declarative DSL language and a parser program. For language specification and standard behaviors, please refer to Parcel Open Standards. MiniParcel is smart - lots of intuitively expected behaviors behave just as they are expected, optionally made explicit with specific instructions.

Below provides quick references of how to use it:

* Fully addressed or simplified class/function target path (requires `using`).
* Lots of contextual names are default imported (unpacked)/used upon first encountering, including all functions from Parcel Standard Libraries, and development-time added lists of parcel original frameworks.
* `unpack(package/module/framework/path)`: Preloads function lists.
* `using namespace/type`: Allows simplifying addressing in subsequent codes.
* Key syntax: `NodeName = TargetPath [ParameterName:]ParameterValue....`, where `Parameter value` can be a reference of a previously defined node.

(PENDING MIGRATION OF SOME OF THE EXAMPLES FROM MINIPARCEL UNIT TESTS HERE FOR REFERENCE PURPOSE)