
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class BlitClient {

        private TcpClient client;
        private NetworkStream stream;

        private bool connected;

        private Thread coreThread;
        private Mutex mutex = new Mutex();

        public BlitClient (string address, int port) {

            Connect(address, port);
        }
        public BlitClient () {}

        public void Connect (string address, int port) {

            mutex.WaitOne(); try {

                if (connected) return;

                client = new TcpClient(address, port);
                stream = client.GetStream();

                coreThread = new Thread(()=>RecvLoop());
                coreThread.Start();

                connected = true;

            } catch (Exception ex) {

                LogError(ex.Message);
                Crash();
                throw;

            } finally { mutex.ReleaseMutex(); }
        }

        public void Close () {

            mutex.WaitOne(); try {

                if (!connected) return;

                coreThread.Abort();
                connected = false;
                try {

                    stream.Close();
                    client.Close();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }

        private void Crash () {

            if (connected) coreThread.Abort();
            connected = false;
            try {

                stream.Close();
                client.Close();

            } catch {}
        }
    }
}
