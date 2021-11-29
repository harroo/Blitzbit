
using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlitzBit {

    public partial class BlitPacket {

        public void Append (string val) {

            byte[] data = Encoding.Unicode.GetBytes(val);

            this.AddArray(BitConverter.GetBytes(data.Length));
            this.AddArray(data);
        }

        // public void Append (System.SByte val) { this.AddArray(BitConverter.GetBytes(val)); }
        // public void Append (System.Byte val) { this.AddArray(BitConverter.GetBytes(val)); }

        public void Append (System.Int16 val) { this.AddArray(BitConverter.GetBytes(val)); }
        public void Append (System.UInt16 val) { this.AddArray(BitConverter.GetBytes(val)); }

        public void Append (System.Int32 val) { this.AddArray(BitConverter.GetBytes(val)); }
        public void Append (System.UInt32 val) { this.AddArray(BitConverter.GetBytes(val)); }

        public void Append (System.Int64 val) { this.AddArray(BitConverter.GetBytes(val)); }
        public void Append (System.UInt64 val) { this.AddArray(BitConverter.GetBytes(val)); }

        public void Append (System.Single val) { this.AddArray(BitConverter.GetBytes(val)); }
        public void Append (System.Double val) { this.AddArray(BitConverter.GetBytes(val)); }
        // public void Append (System.Decimal val) { this.AddArray(BitConverter.GetBytes(val)); }

        public void AppendT (object val) {

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, val);

            byte[] data = memoryStream.ToArray();

            this.AddArray(BitConverter.GetBytes(data.Length));
            this.AddArray(data);
        }
    }
}
