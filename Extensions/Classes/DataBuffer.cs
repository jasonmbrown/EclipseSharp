using System;
using System.Collections.Generic;
using System.IO;

namespace Extensions.Networking {
    public class DataBuffer : IDisposable {

        #region Declarations
        MemoryStream Stream = null;
        #endregion

        #region Constructors
        public DataBuffer() {
            this.Stream = new MemoryStream();
        }
        public void Dispose() {
            try {
                this.Stream.Dispose();
                this.Stream = null;
            } catch { }
        }
        #endregion

        #region Utility
        public void ResetPosition() {
            this.Stream.Position = 0;
        }
        public Byte[] ToArray() {
            return this.Stream.ToArray();
        }
        public void FromArray(Byte[] value) {
            this.Stream.Write(value, 0, value.Length);
            this.ResetPosition();
        }
        public Int64 Length() {
            return this.Stream.Position;
        }
        #endregion

        #region Data Writing
        public void WriteByte(Byte value) {
            var data = BitConverter.GetBytes(value);
            this.Stream.Write(data, 0, 1);
        }
        public void WriteInt16(Int16 value) {
            var data = BitConverter.GetBytes(value);
            this.Stream.Write(data, 0, 2);
        }
        public void WriteInt32(Int32 value) {
            var data = BitConverter.GetBytes(value);
            this.Stream.Write(data, 0, 4);
        }
        public void WriteString(String value) {
            var l = value.Length;
            this.WriteInt32(l);
            foreach (var c in value) {
                this.WriteByte((Byte)c);
            }
        }
        #endregion

        #region Data Reading
        public Byte ReadByte() {
            var data = new Byte[1];
            this.Stream.Read(data, 0, 1);
            return data[0];
        }
        public Int32 ReadInt16() {
            var data = new Byte[2];
            this.Stream.Read(data, 0, 2);
            return BitConverter.ToInt16(data, 0);
        }
        public Int32 ReadInt32() {
            var data = new Byte[4];
            this.Stream.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }
        public String ReadString() {
            var d = new List<Char>();
            var l = this.ReadInt32();
            for (var i = 0; i < l; i++) {
                d.Add((Char)this.ReadByte());
            }
            return string.Join("", d.ToArray());
        }
        #endregion
    }
}
