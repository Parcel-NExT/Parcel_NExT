# Zora Expresso

Tags: Framework, Data Analytics, Ingest  
RFC: PENDING  
Status: Pending further consolidation with InMemoryDB; Might not be applicable to Parcel NExT.  

A SQL based functional paradigm dataflow processing framework. Consumes various tabular source formats and process using SQL.

We must make sure we write good post/blog, user manual/documentation, YouTube video/animation to introduce this library way otherwise it could easily get ignored and wasted because it's too powerful a paradigm on its own.

## TODO

Management:

- [ ] Consolidate README notes from [old repo](https://github.com/chaojian-zhang/Expresso/blob/master/README.md)

## Notes

* Based on https://github.com/chaojian-zhang/Expresso
* Similar functionalities might already available with InMemoryDB and Parcel - we might not need to provide a dedicated package for it. Instead, we could name "InMemoryDB" as "Expresso" when shown in toolbox.

## Technical Remark

Expresso was largely SQL driven - and those functionalities are already available in InMemoryDB (pending further implementation and consolidation). Other than that, maybe only the data readers/writers are of reference value, otherwise we don't need to pursue this library further.