
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace BlitzBit {

    public partial class UBlitServer {

        private IPEndPoint listenPoint;
        private UdpClient serverClient;

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        private List<IPEndPoint> clientPoints = new List<IPEndPoint>();

        public UBlitServer (string address, int port) {

            Start(IPAddress.Parse(address), port);
        }

        public UBlitServer (int port) {

            Start(IPAddress.Any, port);
        }

        public UBlitServer () {}
    }
}
