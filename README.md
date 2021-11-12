# Blitzbit
The ultimate Networking-Interface for Game-Development and more.

# About
Made for use with Unity3D, but can be applied in any environment where it's uses are applicable.

# Pros and Cons
**Pros**
- Reliable, fast and easy to use.
- Packet-Size-Limit equivalent to the eighth Mersenne prime; 2,147,483,647.
- Extraordinarily simple and easy to use.
- Versatile.

**Cons**
- *Only works from Blit\* to Blit\**.

# Usage: Client example.

```cs
/* Connecting. */
BlitClient client = new BlitClient(); // Recommended.
client.Connect("localhost", 1234);
// Or.
BlitClient client = new BlitClient("localhost", 1234);


/* Packets. */
int packetId = 0;
client.AddPacket(packetId, MyMethod);
// Or.
// AddPacketT specifies listening for an object, not a buffer.
client.AddPacketT(packetId, MyTypeMethod);

// ... //
public void MyMethod (byte[] data) {

    // ... Do something with the Received-Buffer.
}

// Can receive classes, structs or anything.
// Even string and int or any other types.
public void MyTypeMethod (object obj) {

    MyClass recvClass = (MyClass)obj;
    // ... Do something with the received Class-Object.
}


/* Sending. */
int packetId = 0;
byte[] data = Something.GetByteArray();
client.Send(packetId, data);
// Or.
// SendT specifies that we want to send an object.
client.SendT(packetId, myClass);
// Or even.
client.SendT(packetId, "Declare or use a string maybe?");
```

**Note.**
It is wise to declare the `BlitClient` without connection arguments.
Instead one should do the following:

```cs
BlitClient client = new BlitClient();

// AddPacket calls ...

client.Connect("localhost", 1234);
```

This shall prevent missed packets, should they be received before the Client-Program is able to make the `AddPacket` calls.

# Usage: Server example.

```cs
/* Hosting / Initializing Server. */
BlitServer server = new BlitServer();
// AddPacket calls ...
server.Start(1234);


/* Packets. */
int packetId = 0;
client.AddPacket(packetId, MyMethod);
// Or.
// AddPacketT specifies listening for an object, not a buffer.
client.AddPacketT(packetId, MyTypeMethod);

// ... //
public void MyMethod (int senderId, byte[] data) {

    // ... Do something with the Received-Buffer.
}

// Can receive classes, structs or anything.
// Even string and int or any other types.
public void MyTypeMethod (int senderId, object obj) {

    MyClass recvClass = (MyClass)obj;
    // ... Do something with the received Class-Object.
}


/* Sending. */
// Sends message to all connected clients.
server.RelayAll(packetId, new byte[4]{1, 2, 3, 4});
// Sends message to all clients excluding the one specified.
server.RelayExclude(packetId, yourByteArray);
// Sends message to a specified client.
server.RelayTo(packetId, yourByteArray);

// * All "Relay" Functions have object variants. * //
server.RelayAllT(packetId, myClass);
server.RelayExcludeT(packetId, myClass);
server.RelayToT(packetId, myClass);
```

**Note.**
The `BlitServer` is best used in a Client & Server model; Where you have a Game-Client and a Server Software-Package.
For simplistic use where a Game-Client can either host, or join, all in one and without requiring a Server, see the `BlitHybrid` class.

# Usage: Hybrid example.

This is not yet completed, and is currently still "Under Development".
However, I can let you in on some sort of idea as to how it'll operate.

```cs
/* Declaration. */
BlitHybrid hybrid = new BlitHybrid();

/* Packets. */
// Exactly the same as the BlitClient and the BlitServer.
hybrid.AddPacket(packetId, OnPacket);
hybrid.AddPacketT(packetId, OnPacketT);

/* Connections / Hosting */
hybrid.Connect("localhost", 1234);
// Or.
hybrid.Host(1234);

/* Sending. */
// Exactly the same as the BlitClient.
client.Send(packetId, new byte[4]{1, 2, 3, 4});
client.SendT(packetId, myClass);
```

# Planned Features.

- BlitStream, Simple `NetworkStream` implementation using `Blitzbit` functionality.
- BlitPacket, Basically `StringBuilder` but for `byte[]`s.

# Epilogue.

Perhaps some sort of Udp variants will be useful also.
I shall consider this at some point in the near future, as it is something that I'd like to do.

---

Spelling and Orthography correction: [Kieralia](https://github.com/kieralia)
