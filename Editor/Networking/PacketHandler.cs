using System;
using Extensions.Networking;
using System.Collections.Generic;
using Extensions;
using System.Windows.Forms;

namespace Editor.Networking {

    public static class PacketHandler {

        private static Dictionary<Packets.Server, Action<DataBuffer>> Handlers = new Dictionary<Packets.Server, Action<DataBuffer>>() {
            { Packets.Server.Ping,                  (b)=> { /* No actual data to process here, sorry! */ } },
            { Packets.Server.LoginOk,               HandleData.HandleLoginOk },
            { Packets.Server.PlayerId,              HandleData.HandlePlayerId },
            { Packets.Server.MapList,               HandleData.HandleMapList }

        };

        internal static void HandleConnected(Boolean success) {
            if (success) {
                Program.FrmLogin.BeginInvoke((Action)(()=>{
                    Program.FrmLogin.UpdateStatus(success);
                }));
            } else {
                MessageBox.Show("Could not connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static void HandlePacket(DataBuffer buffer) {
            var packet = (Packets.Server)buffer.ReadInt32();
            Handlers.TryGet(packet, (b) => { /* Do Nothing */ })(buffer);
        }

        internal static void HandleDisconnected() {
            
        }

        internal static void PingServer(object state) {
            Send.Ping();
        }
    }
}
