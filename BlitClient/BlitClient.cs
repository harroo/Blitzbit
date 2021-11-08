
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public class BlitClient {

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

                coreThread = new Thread(()=>Loop());
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

        private Dictionary<int, Action<byte[]>> packetEvents
            = new Dictionary<int, Action<byte[]>>();

        private Dictionary<int, Action<object>> packetEventsT
            = new Dictionary<int, Action<object>>();

        public void AddPacket (int packetId, Action<byte[]> method) {

            mutex.WaitOne(); try {

                if (packetEvents.ContainsKey(packetId))
                    packetEvents[packetId] = method;
                else
                    packetEvents.Add(packetId, method);

            } finally { mutex.ReleaseMutex(); }
        }
        public void AddPacketT (int packetId, Action<object> method) {

            mutex.WaitOne(); try {

                if (packetEventsT.ContainsKey(packetId))
                    packetEventsT[packetId] = method;
                else
                    packetEventsT.Add(packetId, method);

            } finally { mutex.ReleaseMutex(); }
        }
        private void RelayPacket (int packetId, byte[] data) {

            if (packetEvents.ContainsKey(packetId)) {

                packetEvents[packetId](data);

            } else if (packetEventsT.ContainsKey(packetId)) {

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();

                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                packetEventsT[packetId](binaryFormatter.Deserialize(memoryStream));

            } else
                Log("Unknown Packet Id: " + packetId.ToString());
        }

        public void Send (int packetId, byte[] data) {

            mutex.WaitOne(); try {

                foreach (byte b in BitConverter.GetBytes(data.Length))
                    sendStream.Enqueue(b);

                foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                    sendStream.Enqueue(b);

                foreach (byte b in data)
                    sendStream.Enqueue(b);

            } finally { mutex.ReleaseMutex(); }
        }
        public void SendT (int packetId, object obj) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, obj);

            byte[] data = memoryStream.ToArray();

            mutex.WaitOne(); try {

                foreach (byte b in BitConverter.GetBytes(data.Length))
                    sendStream.Enqueue(b);

                foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                    sendStream.Enqueue(b);

                foreach (byte b in data)
                    sendStream.Enqueue(b);

            } finally { mutex.ReleaseMutex(); }
        }

        private Queue<byte> sendStream = new Queue<byte>();
        // private Queue<byte[]> recvStream = new Queue<byte[]>();

        private void Loop () {

            byte[] buffer = new byte[1];
            int recvByteCount = 0, packetLength = -1, packetId = -1;
            List<byte> recvBuffer = new List<byte>();

            while (true) {

                mutex.WaitOne(); try {

                    if (client.Available != 0) {

                        recvByteCount = stream.Read(buffer, 0, 1);
                        recvBuffer.Add(buffer[0]);

                        if (recvByteCount == 0) Crash();

                        if (packetLength == -1) {

                            if (recvBuffer.Count == 4) {

                                packetLength = BitConverter.ToInt32(recvBuffer.ToArray(), 0);
                                recvBuffer.Clear();
                            }

                        } else if (packetId == -1) {

                            if (recvBuffer.Count == 2) {

                                packetId = BitConverter.ToUInt16(recvBuffer.ToArray(), 0);
                                recvBuffer.Clear();
                            }

                        } else if (recvBuffer.Count == packetLength) {

                            // recvStream.Enqueue(recvBuffer.ToArray());
                            RelayPacket(packetId, recvBuffer.ToArray());
                            recvBuffer.Clear();
                            packetLength = -1;
                            packetId = -1;
                        }

                    } else if (sendStream.Count != 0) {

                        buffer[0] = sendStream.Dequeue();
                        stream.Write(buffer, 0, 1);
                    }

                } finally { mutex.ReleaseMutex(); }
            }
        }

        private void Crash () {

            if (connected) coreThread.Abort();
            connected = false;
            try {

                stream.Close();
                client.Close();

            } catch {}
        }

        public Action<string> onLog;
        private void Log (string message) {

            if (onLog != null) onLog(message);
        }

        public Action<string> onError;
        private void LogError (string message) {

            if (onError != null) onError(message);
        }
    }
}
