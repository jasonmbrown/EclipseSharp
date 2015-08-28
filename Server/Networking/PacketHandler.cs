using System;
using Extensions;
using Server.Database;
using Server.Logic;
using System.Collections.Generic;
using Extensions.Networking;
using Extensions.Database;
using System.Linq;

namespace Server.Networking {
    static class PacketHandler {

        // Set up our dictionary that'll contain the link between our enum and our actual methods.
        // It's a fairly simple system, enum in method out.
        private static  Dictionary<Packets.Client, Action<Int32, DataBuffer>> Handlers = new Dictionary<Packets.Client, Action<Int32, DataBuffer>>() {
            { Packets.Client.Login, HandleData.HandleLogin },
            { Packets.Client.NewAccount, HandleData.HandleNewAccount },
            { Packets.Client.AddCharacter, HandleData.HandleAddCharacter },
            { Packets.Client.Logout, HandleData.HandleLogout },
            { Packets.Client.RequestNewCharacter, HandleData.HandleRequestNewCharacter },
            { Packets.Client.UseCharacter, HandleData.HandleUseCharacter },
            { Packets.Client.RequestMap, HandleData.HandleRequestMap },
            { Packets.Client.MapOK, HandleData.HandleMapOK },
            { Packets.Client.ChatMessage, HandleData.HandleChatMessage },
            { Packets.Client.PlayerMoving, HandleData.HandlePlayerMoving }
        };

        public static void Handle(Int32 id, DataBuffer buffer) {
            var packet = (Packets.Client)buffer.ReadInt32();
            Handlers.TryGet(packet, (i, b) => { /* Do Nothing */ })(id, buffer);   
        }

        public static void ClientConnected(Int32 id) {
            if (Data.Players.ContainsKey(id)) return;

            // Create ourselves a brand new Player class and assign it to the appropriate ID.
            var temp = new Player();
            var tempp = new TempPlayer();
            Data.Players.Add(id, temp);
            Data.TempPlayers.Add(id, tempp);

            // Send the ID to our player.
            Send.PlayerID(id);
        }

        public static void ClientDisconnected(Int32 id) {
            // Save our player data
            if (Data.Players[id].Username.Length > 0) {
                Data.SavePlayer(id);
            }

            // Remove our player from other clients.
            for (var i = 0; i < Data.Players.Count; i++) {
                var key = Data.Players.ElementAt(i).Key;
                if (Data.TempPlayers[key].InGame && key != id) {
                    Send.RemovePlayer(key, id);
                }
            }

            // Remove our player from our system!
            Data.Players.Remove(id);
            Data.TempPlayers.Remove(id);

            Logger.Write(String.Format("ID: {0} has disconnected.", id));
        }

    }
}
