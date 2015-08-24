using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Networking {
    class Server : IDisposable {

        #region Declarations
        private Socket MainSocket;
        private Dictionary<Int32, Socket> Clients = new Dictionary<Int32, Socket>();
        private Int32 MaxClients;
        private Timer ConnectionCheck;

        public Int32 BufferSize { get; set; }

        public Action<Int32, DataBuffer> PacketHandler { get; set; }
        public Action<Int32> ConnectHandler { get; set; }
        public Action<Int32> DisconnectHandler { get; set; }
        #endregion

        #region Constructors
        public Server(Int32 maxclients, Int32 port) {
            var perm = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
            IPAddress ip; IPAddress.TryParse("0.0.0.0", out ip);
            var endpoint = new IPEndPoint(ip, port);
            perm.Demand();

            this.BufferSize = 1024;

            this.MaxClients = maxclients;
            this.MainSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.MainSocket.Bind(endpoint);
        }
        public void Dispose() {
            this.Close();
            this.MainSocket.Close();
            this.MainSocket.Dispose();
            this.Clients = null;
            this.PacketHandler = null;
            this.ConnectHandler = null;
            this.DisconnectHandler = null;
            this.ConnectionCheck.Dispose();
        }
        #endregion

        #region Utility
        public void Open() {
            this.MainSocket.Listen(10);
            this.MainSocket.BeginAccept(new AsyncCallback(AcceptConnection), MainSocket);
            this.ConnectionCheck = new Timer(new TimerCallback(CheckConnections), null, 0, 2000);
        }
        public void Close() {
            if (this.MainSocket.Connected) this.MainSocket.Disconnect(false);
            this.MainSocket.Close();
            FlushClients();
        }
        private Int32 FindNewId() {
            var t = (from i in Enumerable.Range(1, MaxClients)
                     let id = i
                     let usable = Clients.ContainsKey(i) == false ? true : false
                     where usable == true
                     select new { id, usable }).ToArray().FirstOrDefault();
            return t != null ? t.id : 0;
        }
        private Int32 FindId(Socket inp) {
            try {
                return (from c in Clients where c.Value == inp select c.Key).Single();
            } catch {
                return 0;
            }
        }
        private void FlushClient(Int32 id) {
            if (Clients.ContainsKey(id)) {
                if (Clients[id].Connected) Clients[id].Disconnect(false);
                Clients[id].Close();
                Clients.Remove(id);
            }
        }
        private void FlushClients() {
            for (var i = 0; i < MaxClients; i++) {
                FlushClient(i);
            }
        }
        public void SendDataTo(Int32 id, Byte[] data) {
            try {
                Clients[id].BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(DataSent), Clients[id]);
            } catch (SocketException) { }
        }
        public void SendDataToAll(Byte[] data) {
            foreach (var client in Clients) {
                this.SendDataTo(client.Key, data);
            }
        }
        #endregion

        #region Events
        private void AcceptConnection(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            var buffer = new Byte[this.BufferSize];
            var id = FindNewId();

            if (id == 0) throw new NotImplementedException();
            var client = socket.EndAccept(ar);
            client.NoDelay = false;

            this.Clients.Add(id, client);

            this.ConnectHandler(id);

            var obj = new Object[3]; obj[0] = buffer; obj[1] = client; obj[2] = id;
            client.LingerState = new LingerOption(true, 0);
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveData), obj);
            socket.BeginAccept(new AsyncCallback(AcceptConnection), MainSocket);
        }
        private void ReceiveData(IAsyncResult ar) {
            object[] obj = new object[3]; obj = (object[])ar.AsyncState;
            var buf = (Byte[])obj[0];
            var sock = (Socket)obj[1];
            var id = (Int32)obj[2];

            if (!Clients.ContainsKey(id)) return;

            Int32 receive = 0;
            try {
                receive = sock.EndReceive(ar);
            } catch { }
            using (var buffer = new DataBuffer()) {
                if (receive > 0) {
                    buffer.FromArray(buf);
                    this.PacketHandler(id, buffer);
                    buf = new Byte[this.BufferSize];
                    obj[0] = buf; obj[1] = sock; obj[2] = id;
                    sock.BeginReceive(buf, 0, this.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), obj);
                }
            }
        }
        private void CheckConnections(Object e) {
            var list = new List<Int32>();

            SendDataToAll(new Byte[0]);
            foreach (var client in Clients) {
                if (((!client.Value.Poll(1, SelectMode.SelectWrite) && client.Value.Available == 0) || !client.Value.Connected)) {
                    list.Add(client.Key);
                }
            }
            foreach (var l in list) {
                this.FlushClient(l);
                this.DisconnectHandler(l);
            }
        }
        private void DataSent(IAsyncResult ar) {
            var client = (Socket)ar.AsyncState;
            client.EndSend(ar);
        }
        #endregion
    }
}