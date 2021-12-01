
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class UBlitHybrid {

        public void Send (int packetId, byte[] packetData) {

            byte[] sendData = new byte[packetData.Length + 2];

            Buffer.BlockCopy(BitConverter.GetBytes((ushort)packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(packetData, 0, sendData, 2, packetData.Length);

            AddQueue(sendData);
        }

        public void SendT (int packetId, object obj) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, obj);

            byte[] packetData = memoryStream.ToArray();

            byte[] sendData = new byte[packetData.Length + 2];

            Buffer.BlockCopy(BitConverter.GetBytes((ushort)packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(packetData, 0, sendData, 2, packetData.Length);

            AddQueue(sendData);
        }

        private void CoreLoop () {

            bool actioned = false;

            if (hosting) {

                while (true) {

                    actioned = false;

                    while (serverClient.Available != 0) { actioned = true;

                        byte[] recvData = serverClient.Receive(ref listenPoint);

                        if (!clientPoints.Contains(listenPoint))
                            clientPoints.Add(listenPoint);

                        clientPoints.ForEach(delegate(IPEndPoint point) {

                            if (point != listenPoint)

                            serverClient.Send(recvData, recvData.Length, point.Address.ToString(), point.Port);
                        });

                        int packetId = BitConverter.ToUInt16(recvData, 0);
                        byte[] packetData = new byte[recvData.Length - 2];
                        Buffer.BlockCopy(recvData, 2, packetData, 0, packetData.Length);

                        RelayPacket(packetId, packetData);
                    }

                    while (QueueHasContents()) { actioned = true;

                        byte[] sendData = PopQueue();

                        clientPoints.ForEach(delegate(IPEndPoint point) {

                            if (point != listenPoint)

                            serverClient.Send(sendData, sendData.Length, point.Address.ToString(), point.Port);
                        });

                        int packetId = BitConverter.ToUInt16(sendData, 0);
                        byte[] packetData = new byte[sendData.Length - 2];
                        Buffer.BlockCopy(sendData, 2, packetData, 0, packetData.Length);

                        RelayPacket(packetId, packetData);
                    }

                    if (!actioned) Thread.Sleep(5);
                }

            } else {

                while (true) {

                    actioned = false;

                    while (client.Available != 0) { actioned = true;

                        byte[] recvData = client.Receive(ref point);

                        int packetId = BitConverter.ToUInt16(recvData, 0);
                        byte[] packetData = new byte[recvData.Length - 2];
                        Buffer.BlockCopy(recvData, 2, packetData, 0, packetData.Length);

                        RelayPacket(packetId, packetData);
                    }

                    while (QueueHasContents()) { actioned = true;

                        byte[] sendData = PopQueue();

                        client.Send(sendData, sendData.Length, targetAddress, targetPort);
                    }

                    if (!actioned) Thread.Sleep(5);
                }
            }
        }
    }
}
