
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

                packetCallQueue.Add(packetId, data);
                packetCallSenders.Add(packetId, senderId);

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

        public Dictionary<int, byte[]> packetCallQueue
            = new Dictionary<int, byte[]>();

        public Dictionary<int, int> packetCallSenders
            = new Dictionary<int, int>();

        public void RunCallBacks () {

            mutex.WaitOne(); try {

                foreach (var pair in packetCallQueue) {

                    RunPacketCall(packetCallSenders[pair.Key], pair.Key, pair.Value);
                }

                packetCallQueue.Clear();
                packetCallSenders.Clear();

            } finally { mutex.ReleaseMutex(); }
        }
    }
}
