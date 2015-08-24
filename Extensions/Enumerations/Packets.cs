using System;

namespace Extensions {
    public static class Packets {

        public enum Client {
            NewAccount,
            Login,
            AddCharacter
        }

        public enum Server {
            AlertMsg = 1,
            LoginOk,
            CreateCharacterData
        }

    }
}
