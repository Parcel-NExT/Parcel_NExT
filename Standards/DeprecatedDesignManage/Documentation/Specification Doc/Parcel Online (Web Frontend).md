# Overview

Experimental web interface for Parcel; Do not provide full interactivity when it comes to data preview capabilities.

# Description

Currently backend expect a single api: it receives a fully serialized node network and performs the tasks and returns a single (array of) outputs to the front-end. 

The implementation of the front end shall at first focus entirely on node drawing and node graph generation - after the serialization of the node graph is done, we can further decide appropriate API design and format specification.