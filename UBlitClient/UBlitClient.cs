
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class UBlitClient {

        private UdpClient client;
        private IPEndPoint point;

        private string targetAddress;
        private int targetPort;

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        public UBlitClient () {}

        public UBlitClient (string address, int port) {

            Setup(address, port);
        }

        public void Setup (string address, int port) {

            targetAddress = address;
            targetPort = port;

            Random random = new Random();
            point = new IPEndPoint(IPAddress.Any, random.Next(50000, 60000));
            client = new UdpClient(point);

            coreThread = new Thread(()=>CoreLoop());
            coreThread.Start();
        }

        public void Close () {

            coreThread.Abort();
        }
    }
}
