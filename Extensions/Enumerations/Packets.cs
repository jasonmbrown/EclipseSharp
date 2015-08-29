using System;

namespace Extensions {
    public static class Packets {

        public enum Client {
            Ping,
            NewAccount,
            Login,
            AddCharacter,
            Logout,
            RequestNewCharacter,
            UseCharacter,
            MapOK,
            RequestMap,
            ChatMessage,
            PlayerMoving
        }

        public enum Server {
            Ping,
            PlayerId,
            AlertMsg,
            ErrorMsg,
            LoginOk,
            SelectCharacterData,
            NewCharacterData,
            LoadMap,
            MapData,
            InGame,
            ChatMessage,
            PlayerLocation,
            PlayerData,
            RemovePlayer,
            PlayerMoving,
            MapEditorData
        }

    }
}
