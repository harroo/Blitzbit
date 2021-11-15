
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class BlitClient {

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        public BlitClient (TcpClient client, NetworkStream stream) {

            Consign(client, stream);
        }

        public BlitClient (TcpClient client) {

            Consign(client, client.GetStream());
        }

        public BlitClient (string address, int port) {

            Connect(address, port);
        }
        public BlitClient () {}

        public void Close () {

            Disconnect();
        }
    }
}
