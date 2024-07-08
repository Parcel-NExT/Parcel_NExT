# Genesis Package Manager & Package Manager Server

(This whole thing is a large piece on its own and requires dediated treatment, at the moment we keep it under Parcel NExT but may need to move to dedicated repo when it grows)

C# native AOT CLI utility for searching, managing, and publishing packages to the (Parcel NExT central package repo) PENDING NAME. Can also function as serving server for Genesis.

## Usage

Any user can public their own packages to Genesis following a few guidelines.

1. Remember to provide useful meta-data information in package XML file such as ndoe documentations and examples; Also provide package identification information as well as author information in YAML file. Provide variants for specific platforms if needed.
2. Zip everything as .zip.
3. Upload the entire zip to Genesis through the `gen` CLI program. (May requrire API key)

One can also go to https://genesis.methodox.io/create, register and login, and use online form to submit a package.

## TODO

At the moment we are mostly waiting for POS to complete sections on packages before we can finalize a Genesis Protocol.

- [ ] (Infrastructure) Provide some sort of authentication/authorization so as for quality control and avoiding users uploading without limits
- [ ] (Infrastructure) Shall we expose a plain FTP layer allowing general FTP clients to connect and upload?
- [ ] (Process) What's library authentication process like? After uploading when will the user receive notifications?
- [ ] (Infrastructure) Provide REST API server for package queries etc (or at least control commands through the Genesis Protocol).
- [ ] (Protocol, Infrastructure) Define fine-grained user/access mode priviledge, e.g. allow reading but not allow pushing, provide features like private packages with sharing capabilities

## Helper Needed

* Need experience in maintaining proper public-facing server infrastructure (e.g. in this case content management): Security, authentication, content management, infrastructure, coding

## Technical Notes

### Genesis Protocol

Text control transmission always in UTF8. Binary streams are little endian (we can avoid endianness by requires sending streams byte by byte as is).

PENDING MORE.

### Architecture

Two connection channel for control & data S/C.

AOT, zero dependency.

### References

* RFC 959: https://datatracker.ietf.org/doc/html/rfc959
* FTP: https://en.wikipedia.org/wiki/File_Transfer_Protocol
* NuGet protocol: https://learn.microsoft.com/en-us/nuget/api/nuget-protocols
* NuGet publishing: https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package

Additional reading/examples:

* https://learn.microsoft.com/en-us/dotnet/framework/wcf/samples/mtom-encoding?redirectedfrom=MSDN