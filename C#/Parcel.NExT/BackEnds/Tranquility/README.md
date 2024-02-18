# Tranquility (WebSocket Server)

A WebSocket based stated full-feature C# backend. By design, this module should be able to handle isolated contexts and serve multple standalone sessions - even though most of the time we will create a dedicated server process for each running instance of Parcel environment.

Message format: All simple values (like arrays or primitives) are sent as plain string, all complex types by default use JSON; Certain types might use custom string format. Everything is sent as string. Ultimately the protocol is function specific.