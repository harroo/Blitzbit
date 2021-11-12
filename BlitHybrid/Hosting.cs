
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class BlitHybrid {

        private TcpListener listener;

        public void Host (int port) {

            Host(IPAddress.Any, port);
        }

        public void Host (IPAddress address, int port) {

            if (connected) {

                LogError("Cannot Host when Connected!!"); return;
            }

            mutex.WaitOne(); try {

                if (hosting) return;

                listener = new TcpListener(address, port);
                listener.Start();

                coreThread = new Thread(()=>CoreLoop());
                coreThread.Start();

                listenThread = new Thread(()=>ListenLoop());
                listenThread.Start();

                Log("Server Listening on: " + address.ToString() + ":" + port.ToString());

                hosting = true;

            } catch (Exception ex) {

                LogError(ex.Message);

                if (hosting) {

                    coreThread.Abort();
                    listenThread.Abort();
                }
                hosting = false;
                try {

                    listener.Stop();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }

        public void Stop () {

            mutex.WaitOne(); try {

                if (!hosting) return;

                if (hosting) {

                    coreThread.Abort();
                    listenThread.Abort();
                }
                hosting = false;
                try {

                    listener.Stop();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
