
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class BlitHybrid {

        private Queue<byte> sendStream = new Queue<byte>();

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

        private void CoreLoop () {

            if (hosting) {

                while (true) {

                    actioned = false;

                    mutex.WaitOne(); try { ListenToClients(); } finally { mutex.ReleaseMutex(); }
                    mutex.WaitOne(); try { TalkToClients(); } finally { mutex.ReleaseMutex(); }
                    mutex.WaitOne(); try { HandleDisconnections(); } finally { mutex.ReleaseMutex(); }

                    if (!actioned) Thread.Sleep(5);
                }

            } else {

                byte[] buffer = new byte[1];
                int recvByteCount = 0, packetLength = -1, packetId = -1;
                List<byte> recvBuffer = new List<byte>();

                while (true) {

                    actioned = false;

                    mutex.WaitOne(); try {

                        if (client.Available != 0) { actioned = true;

                            recvByteCount = stream.Read(buffer, 0, 1);
                            recvBuffer.Add(buffer[0]);

                            if (recvByteCount == 0) Disconnect();

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

                        } else if (sendStream.Count != 0) { actioned = true;

                            buffer[0] = sendStream.Dequeue();
                            stream.Write(buffer, 0, 1);
                        }

                    } finally { mutex.ReleaseMutex(); }

                    if (!actioned) Thread.Sleep(5);
                }
            }
        }

        byte[] buffer = new byte[1];
        List<TcpClient> clientsToDrop = new List<TcpClient>();
        bool actioned = false;

        int recvByteCount = 0, packetLength = -1, packetId = -1;
        List<byte> recvBuffer = new List<byte>();
        private void ListenToClients () {

            foreach (var client in clients) {

                if (client.Available != 0) { actioned = true;

                    recvByteCount = client.GetStream().Read(buffer, 0, 1);
                    recvBuffer.Add(buffer[0]);

                    if (recvByteCount == 0) clientsToDrop.Add(client);

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
                        // RelayPacket(packetId, recvBuffer.ToArray());
                        Send(packetId, recvBuffer.ToArray());

                        recvBuffer.Clear();
                        packetLength = -1;
                        packetId = -1;
                    }
                }
            }
        }

        int  _packetLength = -1, _packetId = -1;
        List<byte> _recvBuffer = new List<byte>();
        private void TalkToClients () {

            if (sendStream.Count != 0) { actioned = true;

                buffer[0] = sendStream.Dequeue();

                foreach (var client in clients)
                    client.GetStream().Write(buffer, 0, 1);

                _recvBuffer.Add(buffer[0]);

                if (_packetLength == -1) {

                    if (_recvBuffer.Count == 4) {

                        _packetLength = BitConverter.ToInt32(_recvBuffer.ToArray(), 0);
                        _recvBuffer.Clear();
                    }

                } else if (_packetId == -1) {

                    if (_recvBuffer.Count == 2) {

                        _packetId = BitConverter.ToUInt16(_recvBuffer.ToArray(), 0);
                        _recvBuffer.Clear();
                    }

                } else if (_recvBuffer.Count == _packetLength) {

                    RelayPacket(_packetId, _recvBuffer.ToArray());

                    _recvBuffer.Clear();
                    _packetLength = -1;
                    _packetId = -1;
                }
            }
        }
        private void HandleDisconnections () {

            while (clientsToDrop.Count != 0) { actioned = true;

                clients.Remove(clientsToDrop[0]);
                clientsToDrop[0].Close();
                clientsToDrop.RemoveAt(0);
            }
        }
    }
}
