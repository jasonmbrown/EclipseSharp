using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Networking;
using Client.Rendering;
using Extensions.Database;

namespace Client.Networking {
    public static class HandleData {

        internal static void HandleAlertMessage(DataBuffer buffer) {
            var message = buffer.ReadString();
            Interface.ShowMessagebox("Error", message);
        }

        internal static void HandleLoginOk(DataBuffer buffer) {
            Interface.GUI.Get<TGUI.Panel>("loadpanel").Get<TGUI.Label>("loadtext").Text = "Receiving Data...";
        }

        internal static void HandleCreateCharacterData(DataBuffer buffer) {
            // get amount of classes.
            var classes = buffer.ReadInt32();

            for (var i = 1; i < classes; i++) {
                var c = new Class();
                c.Name = buffer.ReadString();
                c.MaleSprite = buffer.ReadInt32();
                c.FemaleSprite = buffer.ReadInt32();
            }

            Interface.ChangeUI(Interface.Windows.CharacterCreate);
        }
    }
}
