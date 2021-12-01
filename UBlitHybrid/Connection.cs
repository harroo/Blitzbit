
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class UBlitHybrid {

        private UdpClient client;
        private IPEndPoint point;

        private string targetAddress;
        private int targetPort;

        public void Connect (string address, int port) {

            targetAddress = address;
            targetPort = port;

            Random random = new Random();
            point = new IPEndPoint(IPAddress.Any, random.Next(50000, 60000));
            client = new UdpClient(point);

            coreThread = new Thread(()=>CoreLoop());
            coreThread.Start();

            connected = true;
        }

        public void Disconnect () {

            coreThread.Abort();

            connected = false;
        }
    }
}
