using System;
using Server.Database;
using Server.Logic;
using System.Text.RegularExpressions;
using System.IO;
using Extensions;

namespace Server.Networking {
    static class HandleData {

        public static void HandleLogin(Int32 id, DataBuffer buffer) {
            // TODO
            throw new NotImplementedException();
        }

        public static void HandleNewAccount(Int32 id, DataBuffer buffer) {
            // Handles a user's request to register an account.
            var username = buffer.ReadString().Trim();
            var password = buffer.ReadString().Trim();
            var legal = new Regex("^[a-zA-Z0-9]*$");

            // Check if the user isn't sending too long/short data.
            if (username.Trim().Length < Data.Settings.MinUsernameChar || username.Trim().Length > Data.Settings.MaxUsernameChar || password.Trim().Length < Data.Settings.MinPasswordChar || password.Trim().Length > Data.Settings.MaxPasswordChar) {
                Send.AlertMessage(id, String.Format("Your username must be between {0} and {1} characters long. Your password must be between {2} and {3} characters long.", Data.Settings.MinUsernameChar, Data.Settings.MaxUsernameChar, Data.Settings.MinPasswordChar, Data.Settings.MaxPasswordChar));
                buffer.Dispose();
                return;
            }

            // Check if the user isn't sending any unallowed characters.
            if (!legal.IsMatch(username)) {
                Send.AlertMessage(id, "Invalid username, only letters and numbers are allowed in names.");
                buffer.Dispose();
                return;
            }

            // Check if this account already exists.
            if (File.Exists(String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, username))) {
                Send.AlertMessage(id, "Sorry, that account name is already taken!");
                buffer.Dispose();
                return;
            }

            // Now we can finaly start creating the account!
            Data.Players[id].Username = username;
            Data.Players[id].SetPassword(password);     // Always use this over setting the password directly! This hashes and salts it.

            // Save the player!
            Data.SavePlayer(id);

            // Send our player the data required to create a new character!
            Send.NewCharacterClasses(id);
        }
        public static void HandleAddCharacter(Int32 id, DataBuffer buffer) {
            var legal   = new Regex("^[a-zA-Z0-9]*$");
            var slot    = buffer.ReadByte();
            var name    = buffer.ReadString().Trim();
            var gender  = buffer.ReadInt32();
            var classn  = buffer.ReadInt32();
            var sprite  = buffer.ReadInt32();

            // Make sure the name is at least X characters long.
            if (name.Length < Data.Settings.MinUsernameChar) {
                Send.AlertMessage(id, String.Format("Character name must be at least {0} characters in length.", Data.Settings.MinUsernameChar));
                buffer.Dispose();
                return;
            }

            // Make sure they only entered regular characters.
            if (!legal.IsMatch(name)) {
                Send.AlertMessage(id, "Invalid name, only letters and numbers are allowed in names.");
                buffer.Dispose();
                return;
            }

            // Check if the name is already in use or not.
            if (Data.Characters.Contains(name)) {
                Send.AlertMessage(id, "A character with this name already exists!");
                buffer.Dispose();
                return;
            }

            // Make sure this player exists.
            if (!Data.Players.ContainsKey(id)) return;

            // We've come this far, we can add this character!
            Data.Characters.Add(name);
            Data.Players[id].Characters[slot].Name                                    = name;
            Data.Players[id].Characters[slot].Gender                                  = (Byte)gender;
            Data.Players[id].Characters[slot].Class                                   = classn;
            Data.Players[id].Characters[slot].Sprite                                  = gender == (Int32)Enumerations.Gender.Male ? Data.Classes[classn].MaleSprite[sprite] : Data.Classes[classn].FemaleSprite[sprite];
            Data.Players[id].Characters[slot].Level                                   = 1;
            Data.Players[id].Characters[slot].Map                                     = Data.Settings.StartMap;
            Data.Players[id].Characters[slot].X                                       = Data.Settings.StartX;
            Data.Players[id].Characters[slot].Y                                       = Data.Settings.StartY;
            Data.Players[id].Characters[slot].Direction                               = (Int32)Enumerations.Direction.Down;
            for (var i = 0; i < (Int32)Enumerations.Stats.Stat_Count; i++) {
                Data.Players[id].Characters[slot].Statistic[i] = Data.Classes[classn].Statistic[i];
            }

            // Save our data!
            Data.SaveCharacterList();
            Data.SavePlayer(id);

            // Notify our user!
            Logger.Write(String.Format("ID: {0} has created character '{1}'.", id, name));
            HandleData.HandleUseCharacter(id);
        }
        public static void HandleUseCharacter(Int32 id) {
            // Make sure the user exists.
            if (!Data.Players.ContainsKey(id)) return;

            // If the user isn't already in-game, send them all the appropriate data!
            if (!Data.TempPlayers[id].InGame) {

                // Send our client the go-ahead!
                Send.LoginOK(id);

                // Send our client our game data!

                // Notify the Console.
                Logger.Write(String.Format("ID: {0} has logged on.", id));
            }
        }

    }
}
