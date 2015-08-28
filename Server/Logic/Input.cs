using System;
using System.Linq;
using Extensions;
using System.Collections.Generic;
using Server.Database;
using Server.Networking;

namespace Server.Logic {
    public static class Input {

        private static Dictionary<String, Action<Object[]>> Commands = new Dictionary<String, Action<Object[]>>() {
            { "close",  Shutdown}, { "exit",  Shutdown}, { "shutdown",  Shutdown},
            { "help", Help }
        };
        private static Dictionary<String, String> HelpList = new Dictionary<String, String>() {
            { "close", "Shuts down the server and saves all currently loaded information to disk." },
            { "exit", "Shuts down the server and saves all currently loaded information to disk." },
            { "shutdown", "Shuts down the server and saves all currently loaded information to disk." },
            { "help", "Provides help for every command available in this program.\n- Use 'help' to get a list of available commands.\n- Use 'help command' to get more detailed information about a command." },
        };

        public static void Process(String input) {
            // Parse our input command and pass it on to the appropriate method.
            var data = input.Split(' ');
            Object[] arguments;
            if (data.Length > 1) {
                arguments = new Object[data.Length - 1];
                for (var i = 0; i < data.Length - 1; i++) {
                    arguments[i] = data[i + 1];
                }
            } else {
                arguments = new Object[0];
            }
            Commands.TryGet(data[0], (o) => { Logger.Write("Unknown Command."); })(arguments);
        }

        private static void Shutdown(Object[] args) {
            Logger.Write("Received Shutdown Command.");
            Program.Running = false;
        }

        internal static void HandleMovement(object state) {
            for (var i = 0; i < Data.TempPlayers.Count; i++) {
                var key = Data.TempPlayers.ElementAt(i).Key;
                if (Data.TempPlayers[key].InGame) {
                    if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Up]) {
                        Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Direction = (Int32)Enumerations.Direction.Up;
                        if (Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Y > 0) Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Y -= 1;
                    }
                    if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Down]) {
                        Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Direction = (Int32)Enumerations.Direction.Down;
                        if (Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Y < Data.Map[Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Map].SizeY * 32) Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Y += 1;
                    }
                    if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Left]) {
                        Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Direction = (Int32)Enumerations.Direction.Left;
                        if (Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].X > 0) Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].X -= 1;
                    }
                    if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Right]) {
                        Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Direction = (Int32)Enumerations.Direction.Right;
                        if (Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].X < Data.Map[Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Map].SizeX * 32) Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].X += 1;
                    }
                }
            }
        }

        internal static void SyncPlayers(object state) {
            // Send each player their own actual location and their location to everyone on their map.
            // This keeps them in-sync, even if they are lagging behind.
            for (var i = 0; i < Data.Players.Count; i++) {
                var id = Data.Players.ElementAt(i).Key;
                if (Data.TempPlayers[id].InGame) {
                    Send.PlayerLocation(id, id);
                    for (var n = 0; n < Data.Players.Count; n++) {
                        var player = Data.Players.ElementAt(n).Key;
                        if (Data.TempPlayers[player].InGame) {
                            if (Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map == Data.Players[player].Characters[Data.TempPlayers[player].CurrentCharacter].Map) {
                                Send.PlayerLocation(player, id);
                            }
                        }
                    }
                }
            }
        }

        private static void Help(Object[] args) {
            // Show a generic help when there's no arguments provided.
            // Otherwise show the command's help where available.
            if (args.Length > 0) {
                Logger.Write(HelpList.TryGet((String)args[0], "Unknown Command."));
            } else {
                Logger.Write(String.Format("All available commands follow. Please use 'help command' to get a more detailed overview.\n{0}", (from c in Commands orderby c.Key select c.Key).ToArray().Aggregate((i,j) => i + ", " + j )));
            }
        }
    }
}
