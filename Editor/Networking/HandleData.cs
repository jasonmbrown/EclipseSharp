using Editor.Database;
using Extensions.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.Networking {
    public static class HandleData {

        public static void HandleLoginOk(DataBuffer buffer) {
            Program.FrmLogin.BeginInvoke((Action)(()=> {
                Program.FrmLogin.Visible = false;
                Program.FrmEditor = new MainEditor();
                Program.FrmEditor.Show();
            }));
        }

        internal static void HandlePlayerId(DataBuffer buffer) {
            Data.MyId = buffer.ReadInt32();
        }
    }
}
