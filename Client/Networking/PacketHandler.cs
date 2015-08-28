using Extensions;
using Extensions.Networking;
using System;
using System.Collections.Generic;
using Client.Rendering;

namespace Client.Networking {
    public static class PacketHandler {

        // Set up our dictionary that'll contain the link between our enum and our actual methods.
        // It's a fairly simple system, enum in method out.
        private static Dictionary<Packets.Server, Action<DataBuffer>> Handlers = new Dictionary<Packets.Server, Action<DataBuffer>>() {
            { Packets.Server.AlertMsg,              HandleData.HandleAlertMessage },
            { Packets.Server.ErrorMsg,              HandleData.HandleErrorMessage },
            { Packets.Server.PlayerId,              HandleData.HandlePlayerId },
            { Packets.Server.LoginOk,               HandleData.HandleLoginOk },
            { Packets.Server.NewCharacterData,      HandleData.HandleCreateCharacterData },
            { Packets.Server.SelectCharacterData,   HandleData.HandleSelectCharacterData },
            { Packets.Server.LoadMap,               HandleData.HandleLoadMap },
            { Packets.Server.MapData,               HandleData.HandleMapData },
            { Packets.Server.InGame,                HandleData.HandleInGame },
            { Packets.Server.ChatMessage,           HandleData.HandleChatMessage },
            { Packets.Server.PlayerLocation,        HandleData.HandlePlayerLocation },
            { Packets.Server.PlayerData,            HandleData.HandlePlayerData },
            { Packets.Server.RemovePlayer,          HandleData.HandleRemovePlayer },
            { Packets.Server.PlayerMoving,          HandleData.HandlePlayerMoving },
            { Packets.Server.MapEditorData,         HandleData.HandleMapEditorData }
        };

        public static void Handle(DataBuffer buffer) {
            var packet = (Packets.Server)buffer.ReadInt32();
            Handlers.TryGet(packet, (b) => { /* Do Nothing */ })(buffer);
        }

        public static void ClientConnected(Boolean success) {
            if (!success) {
                Interface.ShowAlertbox("Error", "Unable to connect to server.\nTry again later.");
            }
        }

    }
}
