
using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class UBlitServer {

        public void Relay (int packetId, byte[] packetData) {

            byte[] sendData = new byte[packetData.Length + 2];

            Buffer.BlockCopy(BitConverter.GetBytes((ushort)packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(packetData, 0, sendData, 2, packetData.Length);

            AddQueue(sendData);
        }

        public void RelayT (int packetId, object obj) {

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

            while (true) { try {

                actioned = false;

                while (serverClient.Available != 0) { actioned = true;

                    byte[] recvData = serverClient.Receive(ref listenPoint);

                    if (!clientPoints.Contains(listenPoint))
                        clientPoints.Add(listenPoint);

                    clientPoints.ForEach(delegate(IPEndPoint point) {

                        if (point != listenPoint)

                        serverClient.Send(recvData, recvData.Length, point.Address.ToString(), point.Port);
                    });

                    RelayPacket(recvData);
                }

                while (QueueHasContents()) { actioned = true;

                    byte[] sendData = PopQueue();

                    clientPoints.ForEach(delegate(IPEndPoint point) {

                        if (point != listenPoint)

                        serverClient.Send(sendData, sendData.Length, point.Address.ToString(), point.Port);
                    });
                }

                if (!actioned) Thread.Sleep(5);

            } catch {} }
        }
    }
}
