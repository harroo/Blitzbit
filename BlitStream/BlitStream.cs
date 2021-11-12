
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class BlitStream {

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        private TcpClient client;
        private NetworkStream stream;

        public BlitStream (TcpClient client, NetworkStream stream) {

            this.client = client;
            this.stream = stream;

            coreThread = new Thread(()=>CoreLoop());
            coreThread.Start();

            connected = true;
        }

        public BlitStream (TcpClient client) {

            this.client = client;
            stream = client.GetStream();

            connected = true;
        }

        public void Close () {

            coreThread.Abort();

            stream.Close();
            client.Close();

            connected = false;
        }
    }
}
