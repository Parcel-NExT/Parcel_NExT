# Parcel.Plots

Author(s): Charles Zhang
Published by: Methodox Technologies, Inc.
License: MIT

Main official power-charged plotting and charting library for Parcel.

API style: data + dedicated configuration structure. Each plot type will have its own configuration structure that matches its plot type name. We have default values for all of configurations.

**API Modes**

The library will offer a few different options when it comes to authorizing plots. (In general, we will implement all of them first then think about consolidating things)

1. API mode - immediate: the ones we have now, where with very basic data inputs one can draw things with a single function call.
2. API mode - customization: we will provide a single `Custom` element which will offer rich "add-on" customization on existing plots, e.g. arbitrary placement of arbitrary additional things like labels and annotations.
3. API mode - deferred: Combined chart element on single chart (so far only support scottplot components). This is essential for things like moving average analysis. As moving average goes, we should also provide an entire DSL of investment analysis curves (ideally directly built upon this mode).
4. API Mode (DSL) Technical Analysis: Time series analysis per TradingView (https://www.tradingview.com/u/EliteTradingSignals/ and https://www.tradingview.com/scripts/), implement all kinds of signals. Also provide simple scripting (either as raw text or assemblying mini-language) capabilities for creating new signals.

## Plot Types

- [ ] Line Plot
- [ ] Pie Plot

## TODO

For all charts we shall provide a single cosmetic options input:

* Axes color
* Colorblind mode
* Title font/Size/color
* Label font/Size/color
* Axes font/size/color

In general, visual customization should (ideally) touch ALL perceptible aspects of a given plot.

## Technical Notes

Implementation:

* ~~Unfortunately, image rendering is platform dependent,and front-end dependent, so without creating all that bitmap, image, drawing infrastructure ourselves, this particular library at the moment handles everything as plain files.~~ Provided a standard `Image`, `Pixel` and `Color` type.
* For static charts, backend is ScottPlot, LiveChart2, GnePlot and a custom (SkiSharp) backend.
* For procedural/imperative charts, backend is GGNUPlot because its API lends more naturally to procedural build up.
