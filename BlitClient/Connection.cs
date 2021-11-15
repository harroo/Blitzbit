
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace BlitzBit {

    public partial class BlitClient {

        private TcpClient client;
        private NetworkStream stream;

        public void Consign (TcpClient client, NetworkStream stream) {

            mutex.WaitOne(); try {

                if (connected) return;

                this.client = client;
                this.stream = stream;

                coreThread = new Thread(()=>CoreLoop());
                coreThread.Start();

                connected = true;

            } catch (Exception ex) {

                LogError(ex.Message);

            } finally { mutex.ReleaseMutex(); }
        }

        public void Connect (string address, int port) {

            mutex.WaitOne(); try {

                if (connected) return;

                client = new TcpClient(address, port);
                stream = client.GetStream();

                coreThread = new Thread(()=>CoreLoop());
                coreThread.Start();

                connected = true;

            } catch (Exception ex) {

                LogError(ex.Message);

                if (connected) coreThread.Abort();
                connected = false;
                try {

                    stream.Close();
                    client.Close();

                } catch {}

            } finally { mutex.ReleaseMutex(); }
        }

        public void Disconnect () {

            mutex.WaitOne(); try {

                if (!connected) return;

                coreThread.Abort();
                connected = false;
                try {

                    stream.Close();
                    client.Close();

                } catch {}

            } finally { mutex.ReleaseMutex(); }

            OnDisconnectEvent();
        }
    }
}
