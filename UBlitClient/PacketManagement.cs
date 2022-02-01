
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class UBlitClient {

        private Dictionary<int, Action<byte[]>> packetEvents
            = new Dictionary<int, Action<byte[]>>();

        private Dictionary<int, Action<object>> packetEventsT
            = new Dictionary<int, Action<object>>();

        public Action<int, byte[]> onUnknownPacket;

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

            if (useCallBacks) {

                packetCallQueue_Id.Add(packetId);
                packetCallQueue_Data.Add(data);

            } else RunPacketCall(packetId, data);
        }
        private void RunPacketCall (int packetId, byte[] data) { try {

            if (packetEvents.ContainsKey(packetId)) {

                packetEvents[packetId](data);

            } else if (packetEventsT.ContainsKey(packetId)) {

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();

                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                packetEventsT[packetId](binaryFormatter.Deserialize(memoryStream));

            } else {

                Log("Unknown Packet Id: " + packetId.ToString());

                if (onUnknownPacket != null) onUnknownPacket(packetId, data);
            }

        } catch {} }

        public bool useCallBacks = false;

        public List<int> packetCallQueue_Id = new List<int>();
        public List<byte[]> packetCallQueue_Data = new List<byte[]>();

        public void RunCallBacks () {

            mutex.WaitOne(); try {

                while (packetCallQueue_Id.Count != 0) {

                    RunPacketCall(packetCallQueue_Id[0], packetCallQueue_Data[0]);

                    packetCallQueue_Id.RemoveAt(0);
                    packetCallQueue_Data.RemoveAt(0);
                }

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
