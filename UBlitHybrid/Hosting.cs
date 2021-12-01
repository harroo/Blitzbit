
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace BlitzBit {

    public partial class UBlitHybrid {

        private UdpClient serverClient;
        private IPEndPoint listenPoint;

        private List<IPEndPoint> clientPoints = new List<IPEndPoint>();

        public void Host (int port) {

            Host(IPAddress.Any, port);
        }

        public void Host (IPAddress address, int port) {

            mutex.WaitOne(); try {

                if (hosting) return;

                listenPoint = new IPEndPoint(address, port);
                serverClient = new UdpClient(listenPoint);

                coreThread = new Thread(()=>CoreLoop());
                coreThread.Start();

                Log("Udp Server Running on: " + address.ToString() + ":" + port.ToString());

                hosting = true;

            } catch (Exception ex) {

                LogError(ex.Message);

                if (hosting) {

                    coreThread.Abort();
                }
                hosting = false;

            } finally { mutex.ReleaseMutex(); }
        }

        public void Stop () {

            mutex.WaitOne(); try {

                if (!hosting) return;

                if (hosting) {

                    coreThread.Abort();
                }
                hosting = false;

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
