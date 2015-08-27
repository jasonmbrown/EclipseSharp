using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Networking;
using Client.Rendering;
using Extensions.Database;
using Client.Database;
using System.IO;
using Extensions;

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
            var p = new Character();
            Data.Players.Add(Data.MyId, p);
        }

        internal static void HandleMapData(DataBuffer buffer) {
            var id = buffer.ReadInt32();
            var temp = buffer.ReadBytes();
            using (var fs = new MemoryStream()) {
                fs.Write(temp, 0, temp.Length);
                fs.Position = 0;
                using (var re = new BinaryReader(fs)) {
                    Data.Map.Name = re.ReadString();
                    Data.Map.Music = re.ReadString();
                    Data.Map.Revision = re.ReadInt32();
                    Data.Map.SizeX = re.ReadInt32();
                    Data.Map.SizeY = re.ReadInt32();

                    var layers = re.ReadInt32();
                    Data.Map.Layers.Clear();

                    for (var l = 0; l < layers; l++) {
                        Data.Map.Layers.Add(new LayerData(Data.Map.SizeX, Data.Map.SizeY));
                        Data.Map.Layers[l].Name = re.ReadString();
                        Data.Map.Layers[l].BelowPlayer = re.ReadBoolean();
                        for (var x = 0; x < Data.Map.SizeX; x++) {
                            for (var y = 0; y < Data.Map.SizeY; y++) {
                                Data.Map.Layers[l].Tiles[x, y].Tileset = re.ReadInt32();
                                Data.Map.Layers[l].Tiles[x, y].TileX = re.ReadInt32();
                                Data.Map.Layers[l].Tiles[x, y].TileY = re.ReadInt32();
                            }
                        }
                    }
                }
            }
            Data.SaveMap(id);
            Send.MapOK();
        }

        internal static void HandleRemovePlayer(DataBuffer buffer) {
            var id = buffer.ReadInt32();
            if (!Data.Players.ContainsKey(id)) return;
            Data.Players.Remove(id);
        }

        internal static void HandlePlayerData(DataBuffer buffer) {
            var id = buffer.ReadInt32();
            if (!Data.Players.ContainsKey(id)) Data.Players.Add(id, new Character());

            Data.Players[id].Name = buffer.ReadString();
            Data.Players[id].Gender = buffer.ReadByte();
            Data.Players[id].Class = buffer.ReadInt32();
            Data.Players[id].Level = buffer.ReadInt32();
            Data.Players[id].Experience = buffer.ReadInt32();
            Data.Players[id].Sprite = buffer.ReadInt32();
            Data.Players[id].Map = buffer.ReadInt32();
            Data.Players[id].X = buffer.ReadInt32();
            Data.Players[id].Y = buffer.ReadInt32();
            Data.Players[id].Direction = buffer.ReadByte();
            for (var i = 0; i < (Int32)Enumerations.Stats.Stat_Count - 1; i++) {
                Data.Players[id].Statistic[i] = buffer.ReadInt32();
            }
        }

        internal static void HandlePlayerLocation(DataBuffer buffer) {
            var id = buffer.ReadInt32();
            if (!Data.Players.ContainsKey(id)) return;
            Data.Players[id].Map = buffer.ReadInt32();
            Data.Players[id].X = buffer.ReadInt32();
            Data.Players[id].Y = buffer.ReadInt32();
            Data.Players[id].Direction = buffer.ReadByte();
        }

        internal static void HandleChatMessage(DataBuffer buffer) {
            var msg = buffer.ReadString();
            if (!Data.InGame) return;
            Interface.GUI.Get<TGUI.ChatBox>("chat").AddLine(msg, SFML.Graphics.Color.Black);
        }

        internal static void HandleInGame(DataBuffer obj) {
            Interface.ChangeUI(Interface.Windows.Game);
            Data.InGame = true;
        }

        internal static void HandleLoadMap(DataBuffer buffer) {
            var mapnum = buffer.ReadInt32();
            var revision = buffer.ReadInt32();

            // Load our map.
            Data.LoadMap(mapnum);

            // Compare revisions
            if (Data.Map.Revision == revision) {
                Send.MapOK();
            } else {
                Send.RequestMap();
            }
        }

        internal static void HandleSelectCharacterData(DataBuffer buffer) {
            // Retrieve our data.
            for (var i = 0; i < Data.CharSelect.Length; i++) {
                Data.CharSelect[i] = new Character();
                Data.CharSelect[i].Name  = buffer.ReadString();
                Data.CharSelect[i].Level = buffer.ReadInt32();
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
