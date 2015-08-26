using Extensions;
using Extensions.Networking;
using System;

namespace Client.Networking {
    public static class Send {

        public static void SendData(DataBuffer data) {
            Program.NetworkClient.SendData(data.ToArray());
        }

        public static void NewAccount(String username, String Password) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.NewAccount);
                buffer.WriteString(username);
                buffer.WriteString(Password);
                SendData(buffer);
            }
        }

        public static void Logout() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.Logout);
                SendData(buffer);
            }
        }

        public static void Login(String username, String Password) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.Login);
                buffer.WriteString(username);
                buffer.WriteString(Password);
                SendData(buffer);
            }
        }

        public static void AddCharacter(String name, Int32 pclass, Enumerations.Gender gender) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.AddCharacter);
                buffer.WriteString(name);
                buffer.WriteInt32(pclass);
                buffer.WriteByte((Byte)gender);
                SendData(buffer);
            }
        }
        public static void UseCharacter(Int32 slot) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.AddCharacter);
                buffer.WriteInt32(slot);
                SendData(buffer);
            }
        }
        public static void RequestNewCharacter() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.RequestNewCharacter);
                SendData(buffer);
            }
        }
    }
}
