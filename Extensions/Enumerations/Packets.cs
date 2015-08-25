using System;

namespace Extensions {
    public static class Packets {

        public enum Client {
            NewAccount,
            Login,
            AddCharacter,
            Logout
        }

        public enum Server {
            AlertMsg,
            ErrorMsg,
            LoginOk,
            NewCharacterData
        }

    }
}
