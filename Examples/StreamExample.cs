
using System;
using System.Net;
using System.Net.Sockets;

using BlitzBit;

public static class Program {

    public static void Main (string[] args) {

        TcpClient client = new TcpClient("localhost", 1234);

        NetworkStream stream = client.GetStream();

        //send and recv or setup connection or watever u need todo
        //then pass control over to blitstream

        BlitStream bstream = new BlitStream(client, stream);
        //or
        //BlitStream bstream = new BlitStream(client);

        bstream.AddPacket(0, OnPacketZero);
        bstream.AddPacketT(1, OnString);

        bstream.Send(0, new byte[2]{6, 4});
        bstream.SendT(1, "string");

        Console.ReadKey();

        //blitstream will take care of the client for us
        bstream.Close();
    }

    private static void OnPacketZero (byte[] data) {

        Console.WriteLine("PacketId: 0, Length: " + data.Length + ", ");
        Console.Write("Values: ");
        foreach (byte b in data) {

            Console.Write(b.ToString() + ", ");
        }
        Console.WriteLine();
    }

    private static void OnString (object s) {

        Console.WriteLine("String: " + (string)s);
    }
}
