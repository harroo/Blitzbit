
using System;
using System.Net;
using System.Net.Sockets;

using BlitzBit;

//this is how its planned to work, and will be implemented at some point

public static class Program {

    public static void Main (string[] args) {

        TcpClient client = new TcpClient("localhost", 1234);

        NetworkStream stream = client.GetStream();

        //send and recv or setup connection or watever u need todo
        //then pass control over to blitstream

        BlitStream bstream = new BlitStream(stream);
        //or
        //BlitStream bstream = new BlitStream(client.GetStream());

        bstream.Recv(OnPacketZero);
        bstream.RecvT(OnString);

        bstream.Send(0, new byte[2]{6, 4});
        bstream.SendT(1, "string");

        Console.ReadKey();

        client.Close();
        bstream.Close();
    }

    private static void OnPacketZero (int packetId, byte[] data) {

        Console.WriteLine("PacketId: " + packetId.ToString() + ", Length: " + data.Length + ", ");
        Console.Write("Values: ");
        foreach (byte b in data) {

            Console.Write(b.ToString() + ", ");
        }
        Console.WriteLine();
    }

    private static void OnString (int packetId, object s) {

        Console.WriteLine("String: " + (string)s);
    }
}
