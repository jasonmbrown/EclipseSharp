﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Extensions.Networking;

namespace Server.Networking { 
    class Server : IDisposable {

        #region Declarations
        private Socket MainSocket;
        private Dictionary<Int32, Socket>   Clients     = new Dictionary<Int32, Socket>();
        private Dictionary<Int32, DateTime> LastData    = new Dictionary<Int32, DateTime>();
        private Int32 MaxClients;
        private Timer ConnectionCheck;

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
                LastData.Remove(id);
            }
        }
        private void FlushClients() {
            for (var i = 0; i < MaxClients; i++) {
                FlushClient(i);
            }
        }
        public void SendDataTo(Int32 id, Byte[] data) {
            try {
                var b = new DataBuffer();
                b.WriteInt32(data.Length);
                b.Append(data);
                Clients[id].BeginSend(b.ToArray(), 0, (Int32)b.Length(), SocketFlags.None, new AsyncCallback(DataSent), Clients[id]);
            } catch { }
        }
        public void SendDataToAll(Byte[] data) {
            for (var i = 0; i < Clients.Count; i++) {
                var key = Clients.ElementAt(i).Key;
                this.SendDataTo(key, data);
            }
        }
        public void DisconnectClient(Int32 id) {
            if (!this.Clients.ContainsKey(id)) return;
            this.FlushClient(id);
            this.DisconnectHandler(id);
        }
        #endregion

        #region Events
        private void AcceptConnection(IAsyncResult ar) {
            var socket = (Socket)ar.AsyncState;
            var id = FindNewId();

            // TODO: Notify Client that the server is full.
            if (id == 0) throw new NotImplementedException();
            IPAddress ip; IPAddress.TryParse("0.0.0.0", out ip); 
            var client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try {
                client = socket.EndAccept(ar);
            } catch { }
            client.NoDelay = false;

            this.Clients.Add(id, client);
            this.LastData.Add(id, DateTime.UtcNow);

            this.ConnectHandler(id);

            var state = new StateObject();
            state.Id = id;
            state.Connection = client;
            state.Data = new DataBuffer();
            client.LingerState = new LingerOption(true, 0);
            try {
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), state);
                socket.BeginAccept(new AsyncCallback(AcceptConnection), MainSocket);
            } catch { }
        }
        private void ReceiveData(IAsyncResult ar) {
            var state = (StateObject)ar.AsyncState;

            if (!Clients.ContainsKey(state.Id)) return;

            LastData[state.Id] = DateTime.UtcNow;

            Int32 receive = 0;
            try {
                receive = state.Connection.EndReceive(ar);
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
                            this.PacketHandler(state.Id, buffer);
                        } else {
                            var newstate = new StateObject();
                            newstate.Id = state.Id;
                            newstate.Connection = state.Connection;
                            newstate.Data = new DataBuffer();
                            try {
                                state.Connection.BeginReceive(newstate.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), newstate);
                            } catch { }
                            work = false;
                        }
                    }
                } else {
                    try {
                        state.Connection.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveData), state);
                    } catch { }
                }
            }
        }
        private void CheckConnections(Object e) {
            for (var i = 0; i < this.Clients.Count; i++) {
                var id = Clients.ElementAt(i).Key;

                // Check if the client hasn't been sending data to us.
                if ((DateTime.UtcNow - this.LastData[id]).TotalSeconds > 10) {
                    this.FlushClient(id);
                    this.DisconnectHandler(id);
                }
            }
        }
        private void DataSent(IAsyncResult ar) {
            try {
                var client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            } catch { }
        }
        #endregion
    }
}