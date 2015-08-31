using Client.Database;
using Client.Networking;
using Client.Rendering;
using Extensions;
using SFML.Window;
using System;
using System.Linq;
using TGUI;

namespace Client.Logic {
    public static class Input {

        public static Boolean[] DirectionPressed = new Boolean[(Int32)Enumerations.Direction.Direction_Count];

        private static Boolean ChatVisible() {
            if (Interface.CurrentUI == Interface.Windows.Game) {
                return Interface.GUI.Get<EditBox>("chatinput").Visible;
            } else {
                return false;
            }
        }

        public static void HandleChat() {
            if (ChatVisible()) {
                var msg = Interface.GUI.Get<EditBox>("chatinput").Text.Trim();
                if (msg.Length < 1) return;
                Send.ChatMessage(msg);
                Interface.GUI.Get<EditBox>("chatinput").Text = "";
                Interface.GUI.Get<EditBox>("chatinput").Visible = false;
            } else {
                Interface.GUI.Get<EditBox>("chatinput").Visible = true;
                Interface.GUI.Get<EditBox>("chatinput").Focused = true;
                // We've opened chat, stop moving!
                for (var i = 0; i < (Int32)Enumerations.Direction.Direction_Count; i++) {
                    DirectionPressed[i] = false;
                }
                Send.PlayerMoving();
            }
        }
        public static void WindowKeyPressed(KeyEventArgs e) {
            if (!Data.InGame) return;
            // Handle our keystates and set our movement to be true if they are pressed.
            switch (e.Code) {
                case Keyboard.Key.Return:
                        HandleChat();
                    break;
                case Keyboard.Key.W:
                    if (!ChatVisible()) DirectionPressed[(Int32)Enumerations.Direction.Up] = true;
                    break;
                case Keyboard.Key.S:
                    if (!ChatVisible()) DirectionPressed[(Int32)Enumerations.Direction.Down] = true;
                    break;
                case Keyboard.Key.A:
                    if (!ChatVisible()) DirectionPressed[(Int32)Enumerations.Direction.Left] = true;
                    break;
                case Keyboard.Key.D:
                    if (!ChatVisible()) DirectionPressed[(Int32)Enumerations.Direction.Right] = true;
                    break;
            }
            // Send the server an update.
            if (!ChatVisible()) Send.PlayerMoving();
        }
        public static void WindowKeyReleased(KeyEventArgs e) {
            if (!Data.InGame) return;
            // Handle our keystates and stop our movement if applicable.
            switch (e.Code) {
                case Keyboard.Key.W:
                    DirectionPressed[(Int32)Enumerations.Direction.Up] = false;
                    break;
                case Keyboard.Key.S:
                    DirectionPressed[(Int32)Enumerations.Direction.Down] = false;
                    break;
                case Keyboard.Key.A:
                    DirectionPressed[(Int32)Enumerations.Direction.Left] = false;
                    break;
                case Keyboard.Key.D:
                    DirectionPressed[(Int32)Enumerations.Direction.Right] = false;
                    break;
            }
            // Send the server an update.
            Send.PlayerMoving();
        }
        public static void HandleMovement(Object e) {
            if (!Data.InGame) return;

            for (var i = 0; i < Data.TempPlayers.Count; i++) {
                var key = Data.TempPlayers.ElementAt(i).Key;
                if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Up]) {
                    Data.Players[key].Direction = (Int32)Enumerations.Direction.Up;
                    if (Data.Players[key].Y > 0) Data.Players[key].Y -= 1;
                }
                if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Down]) {
                    Data.Players[key].Direction = (Int32)Enumerations.Direction.Down;
                    if (Data.Players[key].Y < Data.Map.SizeY * 32) Data.Players[key].Y += 1;
                }
                if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Left]) {
                    Data.Players[key].Direction = (Int32)Enumerations.Direction.Left;
                    if (Data.Players[key].X > 0) Data.Players[key].X -= 1;
                }
                if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Right]) {
                    Data.Players[key].Direction = (Int32)Enumerations.Direction.Right;
                    if (Data.Players[key].X < Data.Map.SizeX * 32) Data.Players[key].X += 1;
                }
            }

            // Calculate our screen Offset.
            Graphics.UpdateOffset();
        }

    }
}
