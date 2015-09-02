using Extensions;
using Extensions.Networking;
using System;

namespace Editor.Networking {
    public static class Send {

        #region Do not touch
        private static void SendData(DataBuffer buffer) {
            Program.NetworkClient.SendData(buffer.ToArray());
        }
        #endregion

        public static void Ping() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.Ping);
                SendData(buffer);
            }
        }

        public static void RequestMapList() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.RequestMapList);
                SendData(buffer);
            }
        }

        public static void Login(String username, String Password) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.Login);
                buffer.WriteString(username);
                buffer.WriteString(Password);
                buffer.WriteBoolean(true);     //Signifies we're an editor, not a client.
                SendData(buffer);
            }
        }

    }
}
