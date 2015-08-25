using System;

namespace Extensions {
    public static class Packets {

        public enum Client {
            NewAccount,
            Login,
            AddCharacter,
            RequestMaxData
        }

        public enum Server {
            AlertMsg,
            ErrorMsg,
            LoginOk,
            NewCharacterData
        }

    }
}
