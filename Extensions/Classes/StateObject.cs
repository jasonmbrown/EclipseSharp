using System;
using System.Net.Sockets;

namespace Extensions.Networking {
    public class StateObject {
        public Int32 Id { get; set; }
        public Socket Connection = null;
        public const Int32 BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public DataBuffer Data = new DataBuffer();
        public Int32 Received { get; set; }
    }
}
