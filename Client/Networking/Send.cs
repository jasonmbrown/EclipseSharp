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
                buffer.WriteInt32((Int32)Packets.Client.UseCharacter);
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
        public static void MapOK() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.MapOK);
                SendData(buffer);
            }
        }
        public static void RequestMap() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.RequestMap);
                SendData(buffer);
            }
        }
        public static void ChatMessage(String msg) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.ChatMessage);
                buffer.WriteString(msg);
                SendData(buffer);
            }
        }
        public static void PlayerMoving() {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Client.PlayerMoving);
                for (var i = 0; i < (Int32)Enumerations.Direction.Direction_Count; i++) {
                    buffer.WriteBoolean(Logic.Input.DirectionPressed[i]);
                }
                SendData(buffer);
            }
        }
    }
}
