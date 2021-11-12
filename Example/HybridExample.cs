
using System;
using System.Collections.Generic;

using BlitzBit;

public static class Program {

    public static void Main (string[] args) {

        BlitHybrid hybrid = new BlitHybrid();

        hybrid.AddPacketT(0, OnPacket);

        Random rng = new Random();
        int me = rng.Next(-8664, 8664);

        Console.Write("1=host, 2=join: ");
        if (Console.ReadKey(false).KeyChar == '1')
            hybrid.Host(1234);
        else
            hybrid.Connect("localhost", 1234);

        int x = 8, y = 8;
        Render();

        while (true) {

            if (!hybrid.active) break;

            char c = Console.ReadKey(true).KeyChar;

            switch (c) {

                case 'w': x--; hybrid.SendT(0, new pos(me, x, y)); break;
                case 'a': y--; hybrid.SendT(0, new pos(me, x, y)); break;
                case 's': x++; hybrid.SendT(0, new pos(me, x, y)); break;
                case 'd': y++; hybrid.SendT(0, new pos(me, x, y)); break;

                case 'q': if (hybrid.hosting) hybrid.Stop(); else hybrid.Disconnect(); break;
            }
        }

        Console.WriteLine("bye!");
    }

    [Serializable]private struct pos {
        public int owner;
        public int x, y;
        public pos (int o, int a, int b) { owner=o; x=a; y=b; }
    }
    private static Dictionary<int, pos> posses = new Dictionary<int, pos>();

    private static void Render () {

        Console.Clear();

        for (int x = 0; x < 16; ++x) {

            for (int y = 0; y < 16; ++y) {

                bool printed = false;
                foreach (pos p in posses.Values) {

                    if (p.x == x && p.y == y) {

                        Console.Write('X');
                        printed = true;
                        break;
                    }
                }
                if (!printed)
                    Console.Write(' ');
            }

            Console.WriteLine();
        }
    }

    private static void OnPacket (object o) {

        pos p = (pos)o;

        if (posses.ContainsKey(p.owner))
            posses.Remove(p.owner);

        posses.Add(p.owner, p);

        Render();
    }
}
