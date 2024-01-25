---
Type: Feature
Title: Parcel Online (Experimental)
Request Date: 2022-04-28
Update Date: 2022-04-28
# ID: 000001
Status: New
Priority: High

Tags: "#HashTag"
---

# Overview

Vincent mentioned some enterprise will prefer a pure-web based environment; This will require not just server-client interfacing but full cloud support.

# Description

For a prototype setup, it will suffice to make front-end serverless and responsible only for drawing and generating the node graph network.

# Notes

The following things need to be done:

1. The server needs a way to provide the front-end with available node definitions
2. The front-end should have a way to communicate/serialize the output graph to server for execution; The server should be able to respond with some outputs

Notice this S/C approach is at the moment **different from** Parcel's workflow-oriented design: aka. *ideally the server itself should be defined by a Parcel graph, and somehow the client is merely interfacing with the exposed interfaces within that server graph in order to affect the state of the server*.

# Destination

* For formal design/interface spec, see this [design document](https://github.com/Charles-Zhang-Parcel/ParcelDesignManage/blob/main/Designs/Specification%20Doc/Parcel%20Online%20(Web%20Frontend).md).

# Progress

* 20220428: Initialize Parcel Web, [Vincent](https://github.com/hsinyuu) will be fully responsible for it.