# Parcel Dashboard App

Tags: #Experimental

Quick dahbosard bulding based on existing service.

## Technical Notes

Architecture: Because Streamlit cannot be self-hosted within a python script (it requires triggering the entry script and an isolated python entrance anyway), we will just use CodeGen for the script; We will use Pythonnet for interoperation with existing .Net infrastructure but because streamlit is in general a completely separate service, there will be serialization needs and keep the calling process (Parcel) and the web app (Streamlit) separate.

We could serve data using web services from wihtin parcel but it's cleaner and ideal to have dashbaord running when parcel is closed so we will just save a copy of all needed data in the streaming folder.

To implement interactivity, we can either:
1. With full context support, generate code by reverse-parsing node graph
2. Rely on specific Lambda (action event) encapsulation - this supports only basic one-element input
3. Purely functional switches for basic inputs (e.g. non real axis)
4. Develop a sophisticated meta-dsl-languabe that permits more advanced direct Python code generation based on simple rules without relying on PythonNet or graph re-evaluation (and thus it's still a isolated Python script), optionally with some Python snippet plugin capabilities. (How?)

The conceptual significance here is NOT dependent on the use of Python Streamlit but in general the programming/runtime model:
1. Service based communication, versus
2. Embedding application 
3. Code generation and interoperation 

Conceptual it's similar to why one cannot start Streamlit directly from within a Python script - exactly because of the hosting model.

## Implementation 
Without invoking advanced meta-programming or PythonNet, below discussion investigates possibility of purely functional DSL based approach.

* Inputs are strongly typed nodes. Graph inputs definitely cannot be directly used for dashboard inputs UNLESS the entire framework is provided the other way around - we provide a dedicated runner for this purpose (in which the entire code generation process is different). It's essentially a new frontend EMBEDDING Parcel. There will not be a ConfigureService node in the graph this case. (An alternative to this would be to provide either dedicated frontend GUI interface, or provide dedicated API function that takes a subgraph as input and relies on meta-programming for nodes re-interpretation.
* Computation must be defined in terms of Python code or package-defined operations.
* The entire operation has nothing to do with Parcel once service started.
* It's clear that from high level functional architecture perspective, when some inputs changes, a function (action) must run and it returns an object which updates certain view. (Same even if we implement a dashboard using Parcel's own web service) The action is going to be a Lambda/action event parameter - representing a piectof code, not an already evaluated object.
* Drawing components can take a list of dependent inputs which defines their relationship, along with computing rules (kind of like how we implemented SQL in the past), but it's largely conventional and can look very ugly and again, feels nothing to do with Parcel and not native.