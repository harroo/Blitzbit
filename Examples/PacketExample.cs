
using System;

using BlitzBit;

public static class Program {

    public static void Main (string[] args) {

        SendExample();
        RecvExample();
    }

    public static void SendExample () {

        BlitPacket packet = new BlitPacket();

        packet.Append("this string value");
        packet.AppendT(new int[]{1, 2, 3, 4});
        packet.Append(32768);
        packet.Append(new byte[4]{0, 128, 255, 65});

        packet.AppendT(new MyClass(4, 5));

        byte[] sendData = packet.ToArray();

        //see how big it is
        Console.WriteLine("packet size is: " + packet.Size);

        //send send data byte[]
        //this could be like feeding it into a blitclient or server maybe
        //or even any stream, its good for anything u want
        //but here we just protending
        protendSendingThing = sendData;
    }

    public static byte[] protendSendingThing;

    public static void RecvExample () {

        //recv byte[]
        //so like maybe an on receive or read from a buffer maybe
        //but here we just simple for example
        byte[] recvData = protendSendingThing;

        BlitPacket packet = new BlitPacket(recvData);

        //the values MUST be deconstructed in the same order
        //they were added to the packet

        string message = packet.GetString();
        int[] scores = (int[])packet.GetObject();
        int secretNumber = packet.GetInt32();
        byte[] myBytes = packet.GetByteArray();

        MyClass stuff = (MyClass)packet.GetObject();

        //do some logging to test
        Console.WriteLine("read: " + message);
        Console.Write("read: ");
        foreach (var i in scores) Console.Write(i.ToString() + ", ");
        Console.WriteLine();
        Console.WriteLine("read: " + secretNumber.ToString());
        Console.Write("read: ");
        foreach (var i in myBytes) Console.Write(i.ToString() + ", ");
        Console.WriteLine();
        Console.WriteLine("read:.Add(): " + stuff.Add().ToString());
    }

    [Serializable]
    public class MyClass {

        public int x, y;

        public MyClass (int a, int b) {

            x = a; y = b;
        }

        public int Add () {

            return x + y;
        }
    }
}
