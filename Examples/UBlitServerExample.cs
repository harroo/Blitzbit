
using System;

using BlitzBit;

public class Test {

    static UBlitServer server;

    public static void Main (string[] args) {

        server = new UBlitServer("127.0.0.1", 12345);

        server.AddPacket(0, OnPacketIdZero);

        server.AddPacketT(1, OnString);

        Console.ReadKey();

        server.RelayT(1, "closing the server now, bye");
        server.AwaitEmptySendQueue();

        server.Stop();
    }

    private static void OnPacketIdZero (byte[] data) {

        Console.WriteLine("PacketId: 0, Length: " + data.Length + ", ");
        Console.Write("Values: ");
        foreach (byte b in data) {

            Console.Write(b.ToString() + ", ");
        }
        Console.WriteLine();

        server.Relay(0, data);
    }

    private static void OnString (object s) {

        Console.WriteLine("String: " + (string)s);

        server.RelayT(1, s + "Hello");
        server.Relay(0, new byte[5]{8, 6, 0, 6, 4});
    }
}
