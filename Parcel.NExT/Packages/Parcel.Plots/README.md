# Parcel.Plots

Unfortunately, image rendering is platform dependent,and front-end dependent, so without creating all that bitmap, image, drawing infrastructure ourselves, this particular library at the moment handles everything as plain files.

API style: data + dedicated configuration structure. Each plot type will have its own configuration structure that matches its plot type name. We have default values for all of configurations.

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