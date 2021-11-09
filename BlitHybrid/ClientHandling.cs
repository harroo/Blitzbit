
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace BlitzBit {

    public partial class BlitHybrid {

        private List<TcpClient> clients = new List<TcpClient>();

        private void ListenLoop () {

            while (true) {

                TcpClient client = listener.AcceptTcpClient();

                mutex.WaitOne(); try {

                    Log("Client Accepted: " + client.Client.RemoteEndPoint.ToString());

                    clients.Add(client);

                } finally { mutex.ReleaseMutex(); }
            }
        }
    }
}
