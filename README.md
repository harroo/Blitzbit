# Blitzbit
The ultimate Networking-Interface for Game-Development and more.

## About
Made for use with Unity3D, but can be applied in any environment where it's uses are applicable.

## Pros and Cons
**Pros**
- Reliable, fast and easy to use.
- Packet-Size-Limit equivalent to the eighth Mersenne prime; 2,147,483,647.
- Extraordinarily simple and easy to use.
- Versatile.

**Cons**
- *Only works from Blit\* to Blit\**.

# How to Install / Setup..

There are three ways setup Blitzbit.

**Download Blitzbit.dll** [Recommended]
- To download and install Blitzbit this way, simply check the releases on this page, and download the latest `dll`.
- [Blitzbit Releases](https://github.com/harroo/Blitzbit/releases)

**Compile Blitzbit.dll**
- Clone this repository and once in the Root-Directory, run `make`.
- This will require `make` and `mcs`.

**Select specific components** [Recommended]
- Blitzbit is built in such a way that you can simply copy the folder of the desired component into your project. Since Blitzbit has no dependencies this should work fine.
- For example if you were to desire only the `BlitClient` for your project, then you could copy said component into your project.
- This is ideal for use in Unity3D when building for Android, as dll files do not work in Android builds, so importing the code itself will negate this issue.

In all cases the `namespace` for Blitzbit is `BlitzBit`.
```cs
using BlitzBit;
```
Yes, with a capital 'B' even though the actual project is spelled with a lower-case 'b'.. How odd!

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

/* Closing / Disconnecting */
if (hybrid.hosting) {

    hybrid.Stop();

} else {

    hybrid.Disconnect();
}
```

The `BlitHybrid` is used as both a Client and a Server, all in one.
It's most useful for Game-Clients that bare simplistic Multiplayer in mind.

# Usage: Stream example.

```cs
/* Declaration. */
BlitStream bstream = new BlitStream(client.GetStream());

/* Sending and Receiving. */

//  Receiving.
bstream.Recv(MyMethod);
bstream.RecvT(MyTypeMethod);

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

//  Sending.
bstream.Send(0, new byte[2]{6, 4});
bstream.SendT(1, "Text Message!");

/* Closing. */
bstream.Close();
```

**Note.**
The `BlitStream` will require a `TcpClient` to operate, it acts as a `NetworkStream` substitute and implements the BlitStream functionality at a raw level.
It is to be used in places where advanced and complex usage may be required.

# Usage: Packet example.

```cs
/* Constructing. */
BlitPacket packet = new BlitPacket();

packet.Append("string value");
packet.Append(137); // Integer value.
packet.Append(137.0f); // Floating-Point value.

packet.AppendT(Anything); // Any object.

byte[] myData = packet.ToArray();

/* Deconstructing. */
BlitPacket packet = new BlitPacket(myData);

string myText = packet.GetString();
int myNumber = packet.GetInt32();
float myFloat = packet.GetSingle();

MyClass anything = (MyClass)packet.GetObject();
```

**Note.**
`BlitPacket`s MUST be deconstructed in the exact same order that they were constructed. Else the information may become obfuscated.

The `BlitPacket` can be used to Construct and Deconstruct `byte[]`s for any purpose you see fit, not just for use with Blitzbit.

If you wish to "Re-Read" the information from a packet, you can simply call `BlitPacket.Reset()` and it will move the counter to the start of the buffer, therefore restarting the read out.

# Usage: Unity3D

For use with Unity Engine you will need to use Thread-Safe Call-Backs.
This is relatively easy and can done like so.

```cs
BlitClient client = new BlitClient();

void Start () {

    client.useCallBacks = true;
}

void Update () {

    client.RunCallBacks();
}
```

This is the same for all `Blit` components.

How it works is that specifying to "Use Call-Backs" means that when packets are received they are queued, instead of processed, and when you `RunCallBacks()` it will process the packets on the thread that calls `RunCallBacks()` method.
In the case of the Unity Engine this is the main thread.

This is the function that handles Packets when they are received.
```cs
private void RelayPacket (int packetId, byte[] data) {

    if (useCallBacks) packetCallQueue.Add(packetId, data);
    else RunPacketCall(packetId, data);
}
```
This can be found in `PacketManagement.cs`.

# More Examples.

For more Examples, see [Examples/](https://github.com/harroo/Blitzbit/tree/main/Examples)

# Epilogue.

Perhaps some sort of Udp variants will be useful also.
I shall consider this at some point in the near future, as it is something that I'd like to do.

---

Spelling and Orthography correction: [Kieralia](https://github.com/kieralia)
