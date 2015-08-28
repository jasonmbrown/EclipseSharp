using Extensions;
using Server.Database;
using Server.Networking;
using System;
using System.Linq;

namespace Server.Logic {
    public static class Players {
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
    }
}
