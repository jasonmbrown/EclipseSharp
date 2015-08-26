using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Networking;
using Client.Rendering;
using Extensions.Database;
using Client.Database;

namespace Client.Networking {
    public static class HandleData {

        internal static void HandleAlertMessage(DataBuffer buffer) {
            var message = buffer.ReadString();
            Interface.ShowMessagebox("Error", message);
        }

        internal static void HandleLoginOk(DataBuffer buffer) {
            if (Interface.CurrentUI == Interface.Windows.Loading) {
                Interface.GUI.Get<TGUI.Panel>("loadpanel").Get<TGUI.Label>("loadtext").Text = "Receiving Data...";
            }
        }

        internal static void HandleErrorMessage(DataBuffer buffer) {
            var message = buffer.ReadString();
            Interface.ShowAlertbox("Error", message);
            Program.NetworkClient.Close();
        }

        internal static void HandlePlayerId(DataBuffer buffer) {
            Data.MyId = buffer.ReadInt32();
            var p = new Player();
            Data.Players.Add(Data.MyId, p);
        }

        internal static void HandleSelectCharacterData(DataBuffer buffer) {
            // Retrieve our data.
            for (var i = 0; i < Data.Players[Data.MyId].Characters.Length; i++) {
                Data.Players[Data.MyId].Characters[i].Name  = buffer.ReadString();
                Data.Players[Data.MyId].Characters[i].Level = buffer.ReadInt32();
            }

            // Move to the select screen.
            Interface.ChangeUI(Interface.Windows.CharacterSelect);
        }

        internal static void HandleCreateCharacterData(DataBuffer buffer) {
            // get amount of classes.
            var classes = buffer.ReadInt32();

            for (var i = 1; i < classes + 1; i++) {
                var c = new Class();
                c.Name = buffer.ReadString();
                c.MaleSprite = buffer.ReadInt32();
                c.FemaleSprite = buffer.ReadInt32();
                Data.Classes.Add(i, c);
            }

            Interface.ChangeUI(Interface.Windows.CharacterCreate);
        }
    }
}
