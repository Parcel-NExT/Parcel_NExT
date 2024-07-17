# PV1 Neo Specification (Front-end)

# PV1 Feature Proposal

This is specific to PV1 Neo and does not form part of POS for front-ends in general; When things are mature and makes sense and is easy to implement on specific front-ends, we can formalize it and make it official in revisions of POS on front-end.

## Functions Tray

### Condense functions of the same series as "variants"

Functions with Overloads (same name different parameters) and Variants (Sequential name) should for ease of display and verbal referencing be considered same "Series/Strand". We will encourage such idea by providing two features:

1. In Node Palette create submenu for nodes of same strands.
2. RMB on node itself should bring up context menu that allows selecting variation.

E.g. `VectorHelper.Range(count, start=0, increment=1)` and `VectorHelper.Range(start, end)` and `VectorHelper.Range2(Start, End, Increment=1)` are all under "Range" series.

## Node Palette

Each node can optionally provide a SVG instead of using default node rendering; Node rendering can be done in custom style instead of using Nodify to simply and standardize implementation (facilitate MiniParcel rendering progress).