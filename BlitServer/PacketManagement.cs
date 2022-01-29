
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class BlitServer {

        private Dictionary<int, Action<int, byte[]>> packetEvents
            = new Dictionary<int, Action<int, byte[]>>();

        private Dictionary<int, Action<int, object>> packetEventsT
            = new Dictionary<int, Action<int, object>>();

        public Action<int, int, byte[]> onUnknownPacket;

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

        private void RelayPacket (int senderId, int packetId, byte[] data) {

            if (useCallBacks) {

                packetCallQueue_Sender.Add(senderId);
                packetCallQueue_Id.Add(packetId);
                packetCallQueue_Data.Add(data);

            } else RunPacketCall(senderId, packetId, data);
        }
        private void RunPacketCall (int senderId, int packetId, byte[] data) {

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

        public bool useCallBacks = false;

        public List<int> packetCallQueue_Sender = new List<int>();
        public List<int> packetCallQueue_Id = new List<int>();
        public List<byte[]> packetCallQueue_Data = new List<byte[]>();

        public void RunCallBacks () {

            mutex.WaitOne(); try {

                for (int i = 0; i < packetCallQueue_Id.Count; ++i)
                    RunPacketCall(packetCallQueue_Sender[i],
                        packetCallQueue_Id[i], packetCallQueue_Data[i]);

                packetCallQueue_Sender.Clear();
                packetCallQueue_Id.Clear();
                packetCallQueue_Data.Clear();

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
