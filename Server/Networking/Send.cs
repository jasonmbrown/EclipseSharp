﻿using System;
using Server.Database;
using Extensions;
using Extensions.Networking;
using System.Linq;

namespace Server.Networking {
    static class Send {

        #region Data Sending - Do not touch
        private static void SendDataTo(Int32 id, DataBuffer buffer) {
            Program.Server.SendDataTo(id, buffer.ToArray());
        }
        private static void SendDataToAll(Int32 id, DataBuffer buffer) {
            Program.Server.SendDataToAll(buffer.ToArray());
        }
        #endregion

        #region Game Data
        public static void AlertMessage(Int32 id, String message) {
            // Write our alert into a buffer and send it along.
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.AlertMsg);
                buffer.WriteString(message);
                SendDataTo(id, buffer);
            }
        }
        public static void ErrorMessage(Int32 id, String message) {
            // Write our alert into a buffer and send it along.
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.ErrorMsg);
                buffer.WriteString(message);
                SendDataTo(id, buffer);
            }
        }
        public static void NewCharacterData(Int32 id) {
            // Write our classlist and send it.
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.NewCharacterData);

                // Write our class count.
                buffer.WriteInt32(Data.Settings.MaxClasses);

                // We're going to have to write data for each class.
                for (var i = 1; i <= Data.Settings.MaxClasses; i++) {

                    // Name
                    buffer.WriteString(Data.Classes[i].Name);

                    // Sprites
                    buffer.WriteInt32(Data.Classes[i].MaleSprite);
                    buffer.WriteInt32(Data.Classes[i].FemaleSprite);

                }
                // Send our data!
                SendDataTo(id, buffer);
            }
        }

        public static void SelectCharacterData(Int32 id) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.SelectCharacterData);
                for (var i = 0; i < Data.Players[id].Characters.Length; i++) {
                    buffer.WriteString(Data.Players[id].Characters[i].Name);
                    buffer.WriteInt32(Data.Players[id].Characters[i].Level);
                }
                SendDataTo(id, buffer);
            }
        }

        public static void LoginOK(Int32 id) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.LoginOk);
                SendDataTo(id, buffer);
            }
        }
        public static void PlayerID(Int32 id) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.PlayerId);
                buffer.WriteInt32(id);
                SendDataTo(id, buffer);
            }
        }
        public static void LoadMap(Int32 id, Int32 map) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.LoadMap);
                buffer.WriteInt32(map);
                buffer.WriteInt32(Data.Map[map].Revision);
                SendDataTo(id, buffer);
            }
        }
        public static void MapData(Int32 id, Int32 map) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.MapData);
                buffer.WriteInt32(map);
                buffer.WriteBytes(Data.MapCache[map]);
                SendDataTo(id, buffer);
            }
        }
        public static void InGame(Int32 id) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.InGame);
                SendDataTo(id, buffer);
            }
        }
        public static void ChatMessageMap(Int32 sender, Int32 map, String msg) {
            using(var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.ChatMessage);
                buffer.WriteString(String.Format("[{0}]: {1}", Data.Players[sender].Characters[Data.TempPlayers[sender].CurrentCharacter].Name, msg));
                for (var i = 0; i < Data.Players.Count; i++) {
                    if (Data.Players.ContainsKey(Data.Players.ElementAt(i).Key)) {
                        var key = Data.Players.ElementAt(i).Key;
                        if (Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Map == map && Data.TempPlayers[key].InGame) {
                            SendDataTo(key, buffer);
                        }
                    }
                }
            }
        }
        public static void PlayerLocation(Int32 id, Int32 player) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.PlayerLocation);
                buffer.WriteInt32(player);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Map);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].X);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Y);
                buffer.WriteByte(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Direction);
            }
        }
        public static void PlayerData(Int32 id, Int32 player) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.PlayerData);
                buffer.WriteInt32(player);
                buffer.WriteString(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Name);
                buffer.WriteByte(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Gender);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Class);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Level);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Experience);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Sprite);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Map);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].X);
                buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Y);
                buffer.WriteByte(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Direction);
                for (var i = 0; i < (Int32)Enumerations.Stats.Stat_Count - 1; i++) {
                    buffer.WriteInt32(Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Statistic[i]);
                }
                SendDataTo(id, buffer);
            }
        }
        public static void RemovePlayer(Int32 id, Int32 player) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.RemovePlayer);
                buffer.WriteInt32(player);
                SendDataTo(id, buffer);
            }
        }
        #endregion
    }
}
