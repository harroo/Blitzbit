
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace BlitzBit {

    public partial class BlitServer {

        private void ListenLoop () {

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

        private void HandleClient (TcpClient client) {

            int hashCode = client.GetHashCode();

            mutex.WaitOne(); try {
                sendStreams.Add(hashCode, new Queue<byte>());
            } finally { mutex.ReleaseMutex(); }

            OnClientConnectEvent(hashCode);

            Thread thread = new Thread(()=>ClientSendLoop(client));
            threadCache.Add(thread);
            thread.Start();

            ClientRecvLoop(client);

            OnClientDisconnectEvent(hashCode);
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
    }
}
