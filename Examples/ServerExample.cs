
using System;

using BlitzBit;

public static class Program {

    private static BlitServer server;

    public static void Main (string[] args) {

        // server = new BlitServer("192.168.0.202", 1234);
        //OR:
        // server = new BlitServer(1234);
        server = new BlitServer();

        server.onLog = OnServerLog;
        server.onError = OnServerError;
        server.onUnknownPacket = RelayUnknown;
        server.onClientConnect = OnConnect;
        server.onClientDisconnect = OnDisconnect;

        server.Start(1234);

        server.AddPacket(0, OnPacketIdZero);

        server.AddPacketT(1, OnString);


        Console.ReadKey();

        server.Stop();
    }

    private static void OnServerLog (string msg) {

        Console.WriteLine("Server: " + msg);
    }

    private static void OnServerError (string msg) {

        Console.WriteLine("Server::ERROR: " + msg);
    }

    private static void RelayUnknown (int senderId, int packetId, byte[] data) {

        server.RelayAll(packetId, data);
    }

    private static void OnConnect (int clientId) {

        server.RelayExcludeT(1, "new client to be welcomeD :D :: " + clientId.ToString(), clientId);
        server.RelayToT(1, clientId, "welcome mr client");
    }

    private static void OnDisconnect (int clientId) {

        server.RelayAllT(1, "a client just left :: " + clientId.ToString());
    }

    private static void OnPacketIdZero (int senderId, byte[] data) {

        Console.WriteLine("PacketId: 0, Length: " + data.Length + ", ");
        Console.Write("Values: ");
        foreach (byte b in data) {

            Console.Write(b.ToString() + ", ");
        }
        Console.WriteLine();

        server.RelayExclude(0, data, senderId);
    }

    private static void OnString (int senderId, object s) {

        Console.WriteLine("String: " + (string)s);

        server.RelayAllT(1, s + "Hello");
        server.RelayAll(0, new byte[5]{8, 6, 0, 6, 4});
    }
}
