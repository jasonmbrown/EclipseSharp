using Extensions.Networking;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Editor.Networking {
    public class Client : IDisposable {

        #region Declarations
        private Socket MainSocket;

        public Action<Boolean>      ConnectedHandler;
        public Action<DataBuffer>   PacketHandler;
        public Action               DisconnectedHandler;
        private DateTime            LastData;
        private Timer               ConnectionCheck;
        #endregion

        #region Constructors
        public Client() {
            var perm = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
            perm.Demand();
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
        public void Close() {
            this.MainSocket.Disconnect(false);
            this.MainSocket.Close();
        }
        public void SendData(Byte[] data) {
            var b = new DataBuffer();
            b.WriteInt32(data.Length);
            b.Append(data);
            try {
                this.MainSocket.BeginSend(b.ToArray(), 0, (Int32)b.Length(), SocketFlags.None, new AsyncCallback(DataSent), null);
            } catch { }
        }
        private void FlushClient() {
                if (this.MainSocket.Connected) this.MainSocket.Disconnect(false);
                this.MainSocket.Close();
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
            var state = new StateObject();
            this.MainSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), state);
            this.LastData = DateTime.UtcNow;
            this.ConnectedHandler(true);
            this.ConnectionCheck = new Timer(new TimerCallback(CheckConnection), null, 0, 2000);
            
        }
        private void ReceiveData(IAsyncResult ar) {
            var state = (StateObject)ar.AsyncState;
            var receive = 0;

            this.LastData = DateTime.UtcNow;

            try {
                receive = this.MainSocket.EndReceive(ar);
            } catch { }

            if (receive > 0) {
                state.Data.Append(state.Buffer);
                state.Received += receive;
                var temp = new DataBuffer();
                temp.FromArray(state.Data.ToArray());
                var alength = temp.ReadInt32();
                if ((state.Received - 4) >= alength) {

                    var work = true;
                    while (work) {
                        var calc = new DataBuffer();
                        calc.FromArray(state.Data.ToArray());
                        var length = calc.ReadInt32();
                        if (state.Data.Length() - 4 >= length) {
                            var buffer = new DataBuffer();
                            buffer.FromArray(state.Data.ToArray().Skip(4).Take(length).ToArray());
                            var data = state.Data.ToArray().Skip(4 + length).Take(state.Received - (4 + length)).ToArray();
                            state.Data.FromArray(data);
                            state.Buffer = data;
                            this.PacketHandler(buffer);
                        } else {
                            var newstate = new StateObject();
                            this.MainSocket.BeginReceive(newstate.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), newstate);
                            work = false;
                        }
                    }
                } else {
                    this.MainSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), state);
                }

            }
        }
        private void CheckConnection(Object e) {
                // Check if the server hasn't been sending data to us.
                if ((DateTime.UtcNow - this.LastData).TotalSeconds > 10) {
                    this.FlushClient();
                    this.DisconnectedHandler();
                }
            }

        private void DataSent(IAsyncResult ar) {
            this.MainSocket.EndSend(ar);
        }
        #endregion
    }
}
