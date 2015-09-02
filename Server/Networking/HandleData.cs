﻿using System;
using Server.Database;
using Server.Logic;
using System.Text.RegularExpressions;
using System.IO;
using Extensions;
using Extensions.Networking;
using System.Linq;

namespace Server.Networking {
    static class HandleData {

        public static void HandleLogin(Int32 id, DataBuffer buffer) {
            // Handles a user's request to login.
            var username = buffer.ReadString().Trim();
            var password = buffer.ReadString().Trim();
            var isEditor = buffer.ReadBoolean();

            // Check if the user isn't sending too long/short data.
            if (username.Trim().Length < Data.Settings.MinUsernameChar || username.Trim().Length > Data.Settings.MaxUsernameChar || password.Trim().Length < Data.Settings.MinPasswordChar || password.Trim().Length > Data.Settings.MaxPasswordChar) {
                Send.AlertMessage(id, String.Format("Your username must be between {0} and {1} characters long. Your password must be between {2} and {3} characters long.", Data.Settings.MinUsernameChar, Data.Settings.MaxUsernameChar, Data.Settings.MinPasswordChar, Data.Settings.MaxPasswordChar));
                return;
            }

            // Check if this account exists.
            if (!File.Exists(String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, username.ToLower()))) {
                Send.AlertMessage(id, "Invalid Username/Password!");
                return;
            }

            // Load our player!
            Data.LoadPlayer(id, username.ToLower());

            // Compare their passwords!
            if (!Data.Players[id].ComparePassword(password)) {
                Send.AlertMessage(id, "Invalid Username/Password!");
                Data.Players[id] = new Extensions.Database.Player();
                return;
            }

            // Is the account allowed to log in?
            if (isEditor && Data.Players[id].UserRank < (Int32)Enumerations.Ranks.Developer) {
                Send.AlertMessage(id, "Invalid Username/Password!");    // We're not going to tell them the real reason.
                Data.Players[id] = new Extensions.Database.Player();
                return;
            }

            // Send our OK.
            Logger.Write(String.Format("ID: {0} has logged in as {1}", id, Data.Players[id].Username));
            Send.LoginOK(id);

            // Disconnect anyone else logged into this account.
            var oldclient = 0;
            for (var i = 0; i < Data.Players.Count; i++) {
                var key = Data.Players.ElementAt(i).Key;
                    if (Data.Players[key].Username.ToLower().Equals(username.ToLower()) && key != id) {
                        oldclient = key;
                    }
            }
            if (oldclient != 0) {
                if (!isEditor && Data.TempPlayers[oldclient].InGame) {
                    Send.AlertMessage(oldclient, "Someone else has logged onto your account!");
                    Logger.Write(String.Format("ID: {0} conflicts with a login at ID: {1}, Disconnecting ID: {1}", id, oldclient));
                    Data.SavePlayer(oldclient);
                    // NOTE: the user is still logged on until they get this message, we're saving their data on purpose.
                    // Their data will now be saved twice, but we can safely load it.
                    Data.LoadPlayer(id, username.ToLower());
                    // Now remove the old player from the world by force.
                    Program.Server.DisconnectClient(oldclient);
                } else if (isEditor && Data.TempPlayers[oldclient].InEditor) {
                    Send.AlertMessage(oldclient, "Someone else has logged onto your account!");
                    Logger.Write(String.Format("ID: {0} conflicts with a login at ID: {1}, Disconnecting ID: {1}", id, oldclient));
                    Data.SavePlayer(oldclient);
                    // NOTE: the user is still logged on until they get this message, we're saving their data on purpose.
                    // Their data will now be saved twice, but we can safely load it.
                    Data.LoadPlayer(id, username.ToLower());
                    // Now remove the old player from the world by force.
                    Program.Server.DisconnectClient(oldclient);
                } else {
                    Logger.Write(String.Format("ID: {0} conflicts with a login at ID: {1}, but are in an editor and client", id, oldclient));
                }
            }

            if (!isEditor) {
                // Check if they have at least one character.
                var haschars = false;
                foreach (var chr in Data.Players[id].Characters) {
                    if (chr.Name.Length > 0) haschars = true;
                }

                // If we have characters, show character select.
                // Otherwise force the user to make a new character.
                if (haschars) {
                    Send.SelectCharacterData(id);
                } else {
                    Send.NewCharacterData(id);
                }
            } else {
                Data.TempPlayers[id].InEditor = true;
            }

        }

        internal static void HandlePlayerMoving(Int32 id, DataBuffer buffer) {
            for (var i = 0; i < (Int32)Enumerations.Direction.Direction_Count; i++) {
                Data.TempPlayers[id].IsMoving[i] = buffer.ReadBoolean();
            }
            for (var i = 0; i < Data.Players.Keys.Count; i++) {
                var key = Data.Players.ElementAt(i).Key;
                if (Data.TempPlayers.ContainsKey(key)) {
                    if (Data.TempPlayers[key].InGame) {
                        if (Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map == Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Map) {
                            Send.PlayerMoving(key, id);
                        }
                    }
                }
            }
        }

        internal static void HandleChatMessage(Int32 id, DataBuffer buffer) {
            var msg = buffer.ReadString();
            if (msg[0] == '/') {
                var data = msg.ToLower().Split(' ');
                switch (data[0]) {

                    case "/editmap":
                        if (Data.Players[id].UserRank >= (Byte)Enumerations.Ranks.Developer) {
                            Send.MapEditorData(id);
                        }
                    break;
                    case "/w":
                        Send.ChatMessageWorld(id, msg.Substring(2), Enumerations.MessageType.World);
                    break;
                    case "/me":
                    case "/em":
                        Send.ChatMessageMap(id, Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map, msg.Substring(3), Enumerations.MessageType.Emote);
                    break;
                    default:
                        Send.ChatMessage(id, id, "Unknown command.", Enumerations.MessageType.Error);
                    break;

                }
            } else {
                Send.ChatMessageMap(id, Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map, msg, Enumerations.MessageType.Map);
            }
        }

        internal static void HandleMapOK(Int32 id, DataBuffer buffer) {
            Data.TempPlayers[id].InGame = true;

            Send.InGame(id);
            Logger.Write(String.Format("ID: {0} has entered the world as '{1}'.", id, Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Name));            
        }

        internal static void HandleRequestMap(Int32 id, DataBuffer buffer) {
            Send.MapData(id, Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map);
        }

        internal static void HandleRequestNewCharacter(Int32 id, DataBuffer buffer) {
            Send.NewCharacterData(id);
        }

        internal static void HandleLogout(Int32 id, DataBuffer buffer) {
            Data.SavePlayer(id);
            Data.Players[id] = new Extensions.Database.Player();
            Data.TempPlayers[id] = new TempPlayer();
        }

        public static void HandleNewAccount(Int32 id, DataBuffer buffer) {
            // Handles a user's request to register an account.
            var username = buffer.ReadString().Trim();
            var password = buffer.ReadString().Trim();
            var legal = new Regex("^[a-zA-Z0-9]*$");

            // Check if the user isn't sending too long/short data.
            if (username.Trim().Length < Data.Settings.MinUsernameChar || username.Trim().Length > Data.Settings.MaxUsernameChar || password.Trim().Length < Data.Settings.MinPasswordChar || password.Trim().Length > Data.Settings.MaxPasswordChar) {
                Send.AlertMessage(id, String.Format("Your username must be between {0} and {1} characters long. Your password must be between {2} and {3} characters long.", Data.Settings.MinUsernameChar, Data.Settings.MaxUsernameChar, Data.Settings.MinPasswordChar, Data.Settings.MaxPasswordChar));
                return;
            }

            // Check if the user isn't sending any unallowed characters.
            if (!legal.IsMatch(username)) {
                Send.AlertMessage(id, "Invalid username, only letters and numbers are allowed in names.");
                return;
            }

            // Check if this account already exists.
            if (File.Exists(String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, username.ToLower()))) {
                Send.AlertMessage(id, "Sorry, that account name is already taken!");
                return;
            }

            // Now we can finaly start creating the account!
            Data.Players[id].Username = username;
            Data.Players[id].SetPassword(password);     // Always use this over setting the password directly! This hashes and salts it.

            // Save the player!
            Data.SavePlayer(id);
            Logger.Write(String.Format("ID: {0} has created a new account named '{1}'.", id, username));

            // Send them our OK!
            Send.LoginOK(id);

            // Send our player the data required to create a new character!
            Send.NewCharacterData(id);
        }
        public static void HandleAddCharacter(Int32 id, DataBuffer buffer) {
            var legal   = new Regex("^[a-zA-Z0-9]*$");
            var name    = buffer.ReadString().Trim();
            var classn  = buffer.ReadInt32();
            var gender = buffer.ReadByte();

            // Make sure this player exists.
            if (!Data.Players.ContainsKey(id)) return;

            // Make sure the name is at least X characters long.
            if (name.Length < Data.Settings.MinUsernameChar) {
                Send.AlertMessage(id, String.Format("Character name must be at least {0} characters in length.", Data.Settings.MinUsernameChar));
                return;
            }

            // Make sure they only entered regular characters.
            if (!legal.IsMatch(name)) {
                Send.AlertMessage(id, "Invalid name, only letters and numbers are allowed in names.");
                return;
            }

            // Check if the name is already in use or not.
            if (Data.Characters.Contains(name)) {
                Send.AlertMessage(id, "A character with this name already exists!");
                return;
            }

            // Check if there's a slot left to use!
            var slot = -1;
            for (var i = 0; slot < Data.Players[id].Characters.Length; i++) {
                if (Data.Players[id].Characters[i].Name.Length < 1) {
                    slot = i;
                    break;
                }
            }
            if (slot < 0) {
                Send.AlertMessage(id, "Unable to create character.");
                return;
            }

            // We've come this far, we can add this character!
            Data.Characters.Add(name);
            Data.Players[id].Characters[slot].Name                                    = name;
            Data.Players[id].Characters[slot].Gender                                  = (Byte)gender;
            Data.Players[id].Characters[slot].Class                                   = classn;
            Data.Players[id].Characters[slot].Sprite                                  = gender == (Int32)Enumerations.Gender.Male ? Data.Classes[classn].MaleSprite : Data.Classes[classn].FemaleSprite;
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
            Logger.Write(String.Format("ID: {0} has created a character named '{1}'.", id, name));
            HandleData.HandleUseCharacter(id, slot);
        }
        public static void HandleUseCharacter(Int32 id, Int32 slot) {
            // Make sure the user exists.
            if (!Data.Players.ContainsKey(id)) return;

            // If the user isn't already in-game, send them all the appropriate data!
            if (!Data.TempPlayers[id].InGame) {

                // Player is using this character!
                Data.TempPlayers[id].CurrentCharacter = slot;

                // Send our client the go-ahead!
                Send.LoginOK(id);

                // Send our clients their game data!
                // This includes everyone on the same map as our lovely friend.
                Send.PlayerData(id, id);
                for (var i = 0; i < Data.Players.Count; i++) {
                    var key = Data.Players.ElementAt(i).Key;
                    if (Data.TempPlayers[key].InGame && Data.Players[key].Characters[Data.TempPlayers[key].CurrentCharacter].Map == Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map) {
                        Send.PlayerData(key, id);
                        Send.PlayerData(id, key);
                        Send.PlayerMoving(id, key);
                    }
                }

                // Now send our friendly friend our map!
                Send.LoadMap(id, Data.Players[id].Characters[Data.TempPlayers[id].CurrentCharacter].Map);

            }
        }
        public static void HandleUseCharacter(Int32 id, DataBuffer buffer) {
            // Make sure the user exists.
            if (!Data.Players.ContainsKey(id)) return;

            var slot = buffer.ReadInt32();
            HandleData.HandleUseCharacter(id, slot);
        }

    }
}
