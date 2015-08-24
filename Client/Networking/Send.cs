using Extensions;
using Extensions.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
