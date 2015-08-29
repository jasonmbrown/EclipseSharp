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
            this.Stream.Dispose();
            this.Stream = null;
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
            this.Stream = new MemoryStream();
            this.Stream.Write(value, 0, value.Length);
            this.ResetPosition();
        }
        public void Append(Byte[] value) {
            this.Stream.Write(value, 0, value.Length);
        }
        public Int64 Length() {
            return this.Stream.ToArray().Length;
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
        public void WriteInt64(Int64 value) {
            var data = BitConverter.GetBytes(value);
            this.Stream.Write(data, 0, 8);
        }
        public void WriteChar(Char value) {
            var data = BitConverter.GetBytes(value);
            this.Stream.Write(data, 0, 2);
        }
        public void WriteString(String value) {
            var l = value.Length;
            this.WriteInt32(l);
            foreach (var c in value) {
                this.WriteChar(c);
            }
        }
        public void WriteBytes(Byte[] value) {
            var l = value.Length;
            this.WriteInt32(l);
            this.Stream.Write(value, 0, l);
        }
        public void WriteBoolean(Boolean value) {
            var data = BitConverter.GetBytes(value);
            this.Stream.Write(data, 0, 1);
        }
        public void WriteDateTime(DateTime value) {
            this.WriteInt64((Int64)value.ToBinary());
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
        public Int64 ReadInt64() {
            var data = new Byte[8];
            this.Stream.Read(data, 0, 8);
            return BitConverter.ToInt64(data, 0);
        }
        public Char ReadChar() {
            var data = new Byte[2];
            this.Stream.Read(data, 0, 2);
            return BitConverter.ToChar(data, 0);
        }
        public String ReadString() {
            var d = new List<Char>();
            var l = this.ReadInt32();
            for (var i = 0; i < l; i++) {
                d.Add(this.ReadChar());
            }
            return string.Join("", d.ToArray());
        }
        public Boolean ReadBoolean() {
            var data = new Byte[1];
            this.Stream.Read(data, 0, 1);
            return BitConverter.ToBoolean(data, 0);
        }
        public DateTime ReadDateTime() {
            return DateTime.FromBinary(this.ReadInt64());
        }
        public Byte[] ReadBytes() {
            var l = this.ReadInt32();
            Byte[] data = new Byte[l];
            this.Stream.Read(data, 0, l);
            return data;
        }
        #endregion
    }
}
