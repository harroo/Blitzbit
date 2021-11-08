
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class BlitServer {

        private Dictionary<int, Queue<byte>> sendStreams
            = new Dictionary<int, Queue<byte>>();

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
    }
}
