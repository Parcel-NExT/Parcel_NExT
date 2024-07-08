# MicroParcel

<!-- Based on command-line format/obj format; Syntax similar to YAML. Much simpler than both MiniParcel and graph text-serialization format. -->

A minimal fully functional graph specification format (explicit style) with only two syntax elements:

1. Node definition following: `<Node Name>: <Node Type>`, where node name should not contain `:` and node type should be fully-addressed; Node name is NOT optional.
2. Node attribute following: `\t<Attribute Name>: <Attribute Value>`, where attribute value follows standard Parcel Document Specification; Attribute names are NOT optional. Multiline texts must escape line breaks using `\n`.
3. Lines starting with `#` (with leading spaces) are ignored; Empty lines are ignored.
4. Lines containing not `:` starts a new graph.

Useful for very quick graph drafts and used extensively in unit tests. All major Parcel front-ends (including MiniParcel CLI) should support this syntax for either/both import or export.

## Examples

### Hello World

```yaml
Node: WriteLine
    Value: Hello World!
```