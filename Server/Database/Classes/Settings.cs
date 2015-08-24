using System;

namespace Server.Database {
    public class Settings {
        // Server Settings
        public Int32    Port                { get; set; }
        public Int32    MaxPlayers          { get; set; }

        // Game Settings
        public String   GameName            { get; set; }
        public String   MOTD                { get; set; }
        public String   Website             { get; set; }

        // Account Settings
        public Int32    MinUsernameChar     { get; set; }
        public Int32    MaxUsernameChar     { get; set; }
        public Int32    MinPasswordChar     { get; set; }
        public Int32    MaxPasswordChar     { get; set; }
        public Int32    StartMap            { get; set; }
        public Byte     StartX              { get; set; }
        public Byte     StartY              { get; set; }

        // Max Settings
        public Int32    MaxCharacters       { get; set; }
        public Int32    MaxClasses          { get; set; }
    }
}
