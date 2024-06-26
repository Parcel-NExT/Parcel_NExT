# Parcel.Generators

Provides random generations of all kinds. This focuses on raw data generations is not for the purpose "randomization" of some inputs. The goals are threefold:
1. Quick demonstration/sample/algorithm testing purpose on-the-fly data generation (think of Lorem Ipsum for texts and images).
2. More pratically, generate useful stuff that have mathematical significance (e.g. random numbers and distributions) that can be used as seeds in downstream process.
3. Generate useful stuff that can directly serve as contents (e.g. random names).
4. (As integration service) General sample data for various parcel types to demonstrate the use of specific types e.g. DataGrid.
5. Simple randomization functionalities.

(Comment) Might be ok to depend on low-level basic types (as in Parcel.Types), e.g. Image and DataGrid.

## TODO

- [ ] Generate random images (ideally meaningful and very fast) (To avoid dependencies, we can write to raw PNG or PPM) (Ideally we avoid depend on Parcel.Types.Image type)
	* Consult: https://stackoverflow.com/questions/16636311/what-is-the-simplest-rgb-image-format and https://rosettacode.org/wiki/Bitmap/Write_a_PPM_file#C
	* On the other hand, since we already depend on some library, if we can find really lightweight libraries (e.g. https://github.com/yangcha/bmpmini)
- [ ] Provides Parcel.Generators.Integration to further provide sample generation for other parcel packages as strongly typed instances e.g. DataGrid.
- [ ] Incorporate https://github.com/dochoffiday/Lorem.NET
- [ ] Remove network dependency: better to use embedded image resource than rely on PicsumPhotos