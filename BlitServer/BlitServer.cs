
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public class BlitServer {

        private TcpListener listener;

        private bool running;

        private Thread coreThread;
        private List<Thread> threadCache = new List<Thread>();
        private Mutex mutex = new Mutex();

        public BlitServer (string address, int port) {

            Start(IPAddress.Parse(address), port);
        }
        public BlitServer (int port) {

            Start(IPAddress.Any, port);
        }
        public BlitServer () {}

        public void Start (int port) {

            Start(IPAddress.Any, port);
        }
        public void Start (IPAddress address, int port) {

            mutex.WaitOne(); try {

                if (running) return;

                listener = new TcpListener(address, port);
                listener.Start();

                coreThread = new Thread(()=>Loop());
                coreThread.Start();

                Log("Server Listening on: " + address.ToString() + ":" + port.ToString());

                running = true;

            } catch (Exception ex) {

                LogError(ex.Message);
                Crash();
                throw;

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

        private Dictionary<int, Action<int, byte[]>> packetEvents
            = new Dictionary<int, Action<int, byte[]>>();

        private Dictionary<int, Action<int, object>> packetEventsT
            = new Dictionary<int, Action<int, object>>();

        public void AddPacket (int packetId, Action<int, byte[]> method) {

            mutex.WaitOne(); try {

                if (packetEvents.ContainsKey(packetId))
                    packetEvents[packetId] = method;
                else
                    packetEvents.Add(packetId, method);

            } finally { mutex.ReleaseMutex(); }
        }
        public void AddPacketT (int packetId, Action<int, object> method) {

            mutex.WaitOne(); try {

                if (packetEventsT.ContainsKey(packetId))
                    packetEventsT[packetId] = method;
                else
                    packetEventsT.Add(packetId, method);

            } finally { mutex.ReleaseMutex(); }
        }

        public Action<int, int, byte[]> onUnknownPacket;
        private void RelayPacket (int senderId, int packetId, byte[] data) {

            if (packetEvents.ContainsKey(packetId)) {

                packetEvents[packetId](senderId, data);

            } else if (packetEventsT.ContainsKey(packetId)) {

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();

                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                packetEventsT[packetId](senderId, binaryFormatter.Deserialize(memoryStream));

            } else {

                Log("Unknown Packet Id: " + packetId.ToString());

                if (onUnknownPacket != null) onUnknownPacket(senderId, packetId, data);
            }
        }

        public void RelayAll (int packetId, byte[] data) {

            mutex.WaitOne(); try {

                foreach (var sendStream in sendStreams.Values) {

                    foreach (byte b in BitConverter.GetBytes(data.Length))
                        sendStream.Enqueue(b);

                    foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                        sendStream.Enqueue(b);

                    foreach (byte b in data)
                        sendStream.Enqueue(b);
                }

            } finally { mutex.ReleaseMutex(); }
        }
        public void RelayExclude (int packetId, byte[] data, int excluderId) {

            mutex.WaitOne(); try {

                foreach (var stream in sendStreams) {

                    if (stream.Key == excluderId) continue;

                    foreach (byte b in BitConverter.GetBytes(data.Length))
                        stream.Value.Enqueue(b);

                    foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                        stream.Value.Enqueue(b);

                    foreach (byte b in data)
                        stream.Value.Enqueue(b);
                }

            } finally { mutex.ReleaseMutex(); }
        }
        public void RelayTo (int packetId, int targetId, byte[] data) {

            mutex.WaitOne(); try {

                foreach (var stream in sendStreams) {

                    if (stream.Key != targetId) continue;

                    foreach (byte b in BitConverter.GetBytes(data.Length))
                        stream.Value.Enqueue(b);

                    foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                        stream.Value.Enqueue(b);

                    foreach (byte b in data)
                        stream.Value.Enqueue(b);
                }

            } finally { mutex.ReleaseMutex(); }
        }

        public void RelayAllT (int packetId, object obj) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, obj);

            byte[] data = memoryStream.ToArray();

            mutex.WaitOne(); try {

                foreach (var sendStream in sendStreams.Values) {

                    foreach (byte b in BitConverter.GetBytes(data.Length))
                        sendStream.Enqueue(b);

                    foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                        sendStream.Enqueue(b);

                    foreach (byte b in data)
                        sendStream.Enqueue(b);
                }

            } finally { mutex.ReleaseMutex(); }
        }
        public void RelayExcludeT (int packetId, object obj, int excluderId) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, obj);

            byte[] data = memoryStream.ToArray();

            mutex.WaitOne(); try {

                foreach (var stream in sendStreams) {

                    if (stream.Key == excluderId) continue;

                    foreach (byte b in BitConverter.GetBytes(data.Length))
                        stream.Value.Enqueue(b);

                    foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                        stream.Value.Enqueue(b);

                    foreach (byte b in data)
                        stream.Value.Enqueue(b);
                }

            } finally { mutex.ReleaseMutex(); }
        }
        public void RelayToT (int packetId, int targetId, object obj) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, obj);

            byte[] data = memoryStream.ToArray();

            mutex.WaitOne(); try {

                foreach (var stream in sendStreams) {

                    if (stream.Key != targetId) continue;

                    foreach (byte b in BitConverter.GetBytes(data.Length))
                        stream.Value.Enqueue(b);

                    foreach (byte b in BitConverter.GetBytes((ushort)packetId))
                        stream.Value.Enqueue(b);

                    foreach (byte b in data)
                        stream.Value.Enqueue(b);
                }

            } finally { mutex.ReleaseMutex(); }
        }

        private void Loop () {

            while (true) {

                TcpClient client = listener.AcceptTcpClient();

                mutex.WaitOne(); try {

                    Log("Client Accepted: " + client.Client.RemoteEndPoint.ToString());

                    Thread thread = new Thread(()=>HandleClient(client));
                    threadCache.Add(thread);
                    thread.Start();

                } finally { mutex.ReleaseMutex(); }
            }
        }

        private Dictionary<int, Queue<byte>> sendStreams
            = new Dictionary<int, Queue<byte>>();

        private void HandleClient (TcpClient client) {

            mutex.WaitOne(); try {
                sendStreams.Add(client.GetHashCode(), new Queue<byte>());
            } finally { mutex.ReleaseMutex(); }

            Thread thread = new Thread(()=>ClientSendLoop(client));
            threadCache.Add(thread);
            thread.Start();

            ClientRecvLoop(client);
        }
        private void ClientSendLoop (TcpClient client) {

            NetworkStream stream = client.GetStream();
            int hashCode = client.GetHashCode();

            byte[] buffer = new byte[1];

            bool actioned = false;

            while (sendStreams.ContainsKey(hashCode)) {

                mutex.WaitOne(); try {

                    if (sendStreams[hashCode].Count != 0) { actioned = true;

                        buffer[0] = sendStreams[hashCode].Dequeue();
                        stream.Write(buffer, 0, 1);
                    }

                } catch { break; } finally { mutex.ReleaseMutex(); }

                if (actioned) actioned = false;
                else Thread.Sleep(10);
            }

            Log("Client Send-Loop Disconnected");
        }
        private void ClientRecvLoop (TcpClient client) {

            NetworkStream stream = client.GetStream();
            int hashCode = client.GetHashCode();

            byte[] buffer = new byte[1];
            int recvByteCount = 0, packetLength = -1, packetId = -1;
            List<byte> recvBuffer = new List<byte>();

            while (true) { try {

                recvByteCount = stream.Read(buffer, 0, 1);
                recvBuffer.Add(buffer[0]);

                if (recvByteCount == 0) break;

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

                    mutex.WaitOne(); try {

                        RelayPacket(hashCode, packetId, recvBuffer.ToArray());
                        recvBuffer.Clear();
                        packetLength = -1;
                        packetId = -1;

                    } finally { mutex.ReleaseMutex(); }
                }

            } catch { break; } }

            Log("Client Disconnected: " + client.Client.RemoteEndPoint.ToString());

            mutex.WaitOne(); try {
                sendStreams.Remove(hashCode);
            } finally { mutex.ReleaseMutex(); }

            try {

                stream.Close();
                client.Close();

            } catch {}
        }

        private void Crash () {

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
