
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class BlitHybrid {

        private TcpListener listener;

        private bool running;

        public void Start (int port) {

            Start(IPAddress.Any, port);
        }

        public void Start (IPAddress address, int port) {

            mutex.WaitOne(); try {

                if (running) return;

                listener = new TcpListener(address, port);
                listener.Start();

                coreThread = new Thread(()=>CoreLoop());
                coreThread.Start();

                listenThread = new Thread(()=>ListenLoop());
                listenThread.Start();

                Log("Server Listening on: " + address.ToString() + ":" + port.ToString());

                running = true;

            } catch (Exception ex) {

                LogError(ex.Message);

                if (running) {

                    coreThread.Abort();
                    listenThread.Abort();
                }
                running = false;
                try {

                    listener.Stop();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }

        public void Stop () {

            mutex.WaitOne(); try {

                if (!running) return;

                if (running) {

                    coreThread.Abort();
                    listenThread.Abort();
                }
                running = false;
                try {

                    listener.Stop();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
