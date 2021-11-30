
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class UBlitServer {

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

        private void RelayPacket (byte[] raw) {

            int packetId = BitConverter.ToUInt16(raw, 0);
            byte[] data = new byte[raw.Length - 2];
            Buffer.BlockCopy(raw, 2, data, 0, data.Length);

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
        }
    }
}
