using Extensions.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Networking {
    public class Client : IDisposable {

        #region Declarations
        private Socket MainSocket;
        private Int32 BufferSize;

        public Action<Boolean> ConnectedHandler;
        public Action<DataBuffer> PacketHandler;
        #endregion

        #region Constructors
        public Client() {
            var perm = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
            perm.Demand();
            this.BufferSize = 1024;
        }
        public void Dispose() {
            this.MainSocket = null;
            this.ConnectedHandler = null;
        }
        #endregion

        #region Utility
        public void Open(String address, Int32 port) {
            IPAddress ip = null;
            IPAddress.TryParse(address, out ip);
            var endpoint = new IPEndPoint(ip, port);

            this.MainSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.MainSocket.LingerState = new LingerOption(false, 0);
            this.MainSocket.NoDelay = false;
            this.MainSocket.BeginConnect(endpoint, new AsyncCallback(Connected), null);
        }
        public void SendData(Byte[] data) {
            this.MainSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(DataSent), null);
        }
        #endregion

        #region Events
        private void Connected(IAsyncResult ar) {
            try {
                this.MainSocket.EndConnect(ar);
            } catch {
                this.ConnectedHandler(false);
                return;
            }
            var buffer = new Byte[this.BufferSize];
            this.MainSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveData), buffer);
            this.ConnectedHandler(true);
        }
        private void ReceiveData(IAsyncResult ar) {
            var buf = (Byte[])ar.AsyncState;
            try {
                var receive = this.MainSocket.EndReceive(ar);
                using (var buffer = new DataBuffer()) {
                    if (receive > 0) {
                        buffer.FromArray(buf);
                        this.PacketHandler(buffer);
                        buf = new Byte[this.BufferSize];
                        this.MainSocket.BeginReceive(buf, 0, this.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), buf);
                    }
                }
            } catch (SocketException) { }
        }
        private void DataSent(IAsyncResult ar) {
            this.MainSocket.EndSend(ar);
        }
        #endregion
    }
}
