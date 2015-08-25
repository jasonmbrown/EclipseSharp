using Extensions;
using Extensions.Networking;
using System;

namespace Client.Networking {
    public static class Send {

        public static void SendData(Byte[] data) {
            Program.NetworkClient.SendData(data);
        }

        public static void NewAccount(String username, String Password) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.NewAccount);
                buffer.WriteString(username);
                buffer.WriteString(Password);
                SendData(buffer.ToArray());
            }
        }

        public static void Logout() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.Logout);
                SendData(buffer.ToArray());
            }
        }

        public static void Login(String username, String Password) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.Login);
                buffer.WriteString(username);
                buffer.WriteString(Password);
                SendData(buffer.ToArray());
            }
        }
    }
}
