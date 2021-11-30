
using System;

using BlitzBit;

public class Test {

    public static void Main (string[] args) {

        UBlitServer server = new UBlitServer("127.0.0.1", 12345);

        Console.ReadKey();

        server.Stop();
    }
}
