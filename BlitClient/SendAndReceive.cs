
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class BlitClient {

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

            byte[] buffer = new byte[1];
            int recvByteCount = 0, packetLength = -1, packetId = -1;
            List<byte> recvBuffer = new List<byte>();

            bool actioned = false;

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
}
