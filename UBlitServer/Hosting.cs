
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class UBlitServer {

        private bool running;

        public void Start (int port) {

            Start(IPAddress.Any, port);
        }

        public void Start (IPAddress address, int port) {

            mutex.WaitOne(); try {

                if (running) return;

                listenPoint = new IPEndPoint(address, port);
                serverClient = new UdpClient(listenPoint);

                coreThread = new Thread(()=>CoreLoop());
                coreThread.Start();

                Log("Udp Server Running on: " + address.ToString() + ":" + port.ToString());

                running = true;

            } catch (Exception ex) {

                LogError(ex.Message);

                if (running) {

                    coreThread.Abort();
                }
                running = false;

            } finally { mutex.ReleaseMutex(); }
        }

        public void Stop () {

            mutex.WaitOne(); try {

                if (!running) return;

                if (running) {

                    coreThread.Abort();
                }
                running = false;

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
