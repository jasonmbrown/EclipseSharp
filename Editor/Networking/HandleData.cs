using Editor.Database;
using Extensions.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        internal static void HandleMapList(DataBuffer buffer) {
            var max = buffer.ReadInt32();

            Program.FrmEditor.BeginInvoke((Action)(() => {
                Program.FrmEditor.ClearMaps();
                
            }));
            for (var i = 1; i <= max; i++) {
                    Program.FrmEditor.BeginInvoke((Action<Int32, String>)((num, name) => {
                        Program.FrmEditor.AddMap(String.Format("{0}: {1}", num, name));
                }), i, buffer.ReadString());
            }
        }
    }
}
