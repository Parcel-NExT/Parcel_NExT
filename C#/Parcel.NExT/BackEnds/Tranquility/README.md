# Tranquility (WebSocket Server)

A WebSocket based stated full-feature C# backend. By design, this module should be able to handle isolated contexts and serve multple standalone sessions - even though most of the time we will create a dedicated server process for each running instance of Parcel environment.

## Endpoints

All messages must point to a specific functional endpoint. At the moment they are either `LibraryProviderServices` or `InterpolationServiceProvider`.

## Message Format

Everything is sent as string. 

All simple values (including arrays and Parcel primitives) are sent as plain string, all complex types by default use JSON; The differentiating factor is complex types start with `{` symbol.

Certain types might use custom string format. We are debating whether we should just customize it or use TOML for simplicity.

Ultimately the protocol is function specific.