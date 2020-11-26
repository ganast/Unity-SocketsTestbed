Unity-SocketsTestbed
====================

Experimentation with .NET sockets and the necessary multithreading stuff in Unity without messing (too badly) with the engine's runtime.

The code starts a TCP listening socket on loopback:55000, accepts connections (e.g., using a terminal such as PuTTY), reads lines of text and dumps them on the console. Each connection is served by its own handler thread, as is the listener. All data exchange is via a instance member of the hosting GameObject passed to and used by the listener thread and all connection handler threads.

DISCLAIMER: This is just an experimental sample. It does not mean to demonstrate preffered techniques, Unity-, networking- or other-wise.
