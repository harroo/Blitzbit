
using System;

using BlitzBit;

public class Test {

    public static void Main (string[] args) {

        UBlitClient client = new UBlitClient("localhost", 12345);

        client.AddPacket(0, OnPacketIdZero);

        client.AddPacketT(1, OnString);

        client.AddPacketT(2, OnStruct);
        client.AddPacketT(3, NameTheseAnything);

        client.Send(0, new byte[4]{4, 3, 2, 1});

        client.SendT(1, "string message! :D");

        client.SendT(2, new SomeStruct("cool value that is  struct it is!"));
        client.SendT(3, new SomeClass("cool value that is cooler cos class awesome!"));

        Console.ReadKey();

        client.Close();
    }

    private static void OnPacketIdZero (byte[] data) {

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

    private static void OnStruct (object s) {

        SomeStruct some = (SomeStruct)s;

        Console.WriteLine("OnStruct: " + some.myValue);
    }

    private static void NameTheseAnything (object s) {

        SomeClass some = (SomeClass)s;

        Console.WriteLine("NameTheseAnything: " + some.myValue);
    }


    [Serializable] public struct SomeStruct {

        public SomeStruct (string s) { myValue = s; }

        public string myValue;
    }

    [Serializable] public class SomeClass {

        public SomeClass (string s) { myValue = s; }

        public string myValue;
    }
}
