using System;

namespace Extensions {
    public static class Packets {

        public enum Client {
            NewAccount,
            Login,
            AddCharacter,
            Logout,
            RequestNewCharacter,
            UseCharacter
        }

        public enum Server {
            PlayerId,
            AlertMsg,
            ErrorMsg,
            LoginOk,
            SelectCharacterData,
            NewCharacterData
        }

    }
}
