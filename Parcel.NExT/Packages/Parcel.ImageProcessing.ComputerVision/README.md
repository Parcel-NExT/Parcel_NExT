# Parcel.ImageProcessing.ComputerVision

Package for more advanced 2D image/signal processing; Highly practical. Dependency-light.

>Like many other experimental and really useful things, we need to survey existing solutions, and likely learn and implement a few on our own, then eventually find a more suitable low-level library or have written our own. Reducing dependencies and keep everything lean is the key.

## Dependency

* OpenCV for advanced computer vision (consider making a separate package). For .Net bindings for OpenCV, we have either OpenCVSharp or Emgu CV; The former doesn't have mac support but API is generally nicer; The latter's API looks bad. We will start with OpenCVSharp.
* Pending library for signal processing.

## Surveys

* https://github.com/keenua/OCR https://www.syncfusion.com/document-processing/pdf-framework/net/pdf-library/ocr-process