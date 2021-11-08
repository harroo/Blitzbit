
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BlitzBit {

    public partial class BlitServer {

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

                coreThread = new Thread(()=>ListenLoop());
                coreThread.Start();

                Log("Server Listening on: " + address.ToString() + ":" + port.ToString());

                running = true;

            } catch (Exception ex) {

                LogError(ex.Message);

                if (running) {

                    coreThread.Abort();

                    threadCache.ForEach(delegate(Thread thread){

                        thread.Abort();
                    });
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

                    threadCache.ForEach(delegate(Thread thread){

                        thread.Abort();
                    });
                }
                running = false;
                try {

                    listener.Stop();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
