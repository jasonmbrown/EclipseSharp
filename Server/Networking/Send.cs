using System;
using Server.Database;
using Extensions;

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
        public static void NewCharacterClasses(Int32 id) {
            // Write our classlist and send it.
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.CreateCharacterData);

                // Write our class count.
                buffer.WriteInt32(Data.Settings.MaxClasses);

                // We're going to have to write data for each class.
                for (var i = 1; i <= Data.Settings.MaxClasses; i++) {
                    // Name
                    buffer.WriteString(Data.Classes[i].Name);

                    // Vitals
                    buffer.WriteInt32(Data.Classes[i].GetMaxVital(Enumerations.Vitals.HP));
                    buffer.WriteInt32(Data.Classes[i].GetMaxVital(Enumerations.Vitals.MP));

                    // Sprites
                    buffer.WriteInt32(Data.Classes[i].MaleSprite);
                    buffer.WriteInt32(Data.Classes[i].FemaleSprite);

                    //Stats
                    for (var s = 0; s < (Int32)Enumerations.Stats.Stat_Count; s++) {
                        buffer.WriteInt32(Data.Classes[i].Statistic[s]);
                    }
                }
                // Send our data!
                SendDataTo(id, buffer);
            }
        }
        public static void LoginOK(Int32 id) {
            using (var buffer = new DataBuffer()) {
                buffer.WriteInt32((Int32)Packets.Server.LoginOk);
                buffer.WriteInt32(id);
                buffer.WriteInt32(Data.Players.Count);
                SendDataTo(id, buffer);
            }
        }
        #endregion
    }
}
