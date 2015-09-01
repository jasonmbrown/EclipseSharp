using Client.Database;
using Client.Networking;
using Client.Rendering;
using Extensions;
using SFML.System;
using SFML.Window;
using System;
using System.Linq;
using TGUI;

namespace Client.Logic {
    public static class Input {

        public static Boolean[]     DirectionPressed    = new Boolean[(Int32)Enumerations.Direction.Direction_Count];
        public static Boolean       MousePressed;
        public static Boolean[]     MouseButton         = new Boolean[(Int32)SFML.Window.Mouse.Button.ButtonCount];
        public static Boolean       MousePressedOnMap;
        public static Vector2f      Mouse;
        public static Vector2i      SelectedTile        = new Vector2i(0, 0);

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

        internal static void HandleMouseClicks(object state) {
            switch (Interface.CurrentUI) {

                case Interface.Windows.MapEditor:
                if (Input.MousePressedOnMap) {
                    if (Input.MouseButton[(Int32)SFML.Window.Mouse.Button.Left]) {
                        var loc = Input.GetCurrentTile(Input.Mouse.X, Input.Mouse.Y);
                        var layer = Interface.GUI.Get<Panel>("panel").Get<ComboBox>("layerselect").GetSelectedItemIndex();
                        var tileset = Interface.GUI.Get<Panel>("panel").Get<ComboBox>("tileselect").GetSelectedItemIndex() + 1;
                        if (loc.X >= 0 && loc.X < Data.Map.SizeX && loc.Y >= 0 && loc.Y < Data.Map.SizeY) {
                            Data.Map.Layers.ElementAt(layer).Tiles[loc.X, loc.Y].TileX = Input.SelectedTile.X;
                            Data.Map.Layers.ElementAt(layer).Tiles[loc.X, loc.Y].TileY = Input.SelectedTile.Y;
                            Data.Map.Layers.ElementAt(layer).Tiles[loc.X, loc.Y].Tileset = tileset;
                        }
                    }
                    if (Input.MouseButton[(Int32)SFML.Window.Mouse.Button.Right]) {
                        var loc = Input.GetCurrentTile(Input.Mouse.X, Input.Mouse.Y);
                        var layer = Interface.GUI.Get<Panel>("panel").Get<ComboBox>("layerselect").GetSelectedItemIndex();
                        var tileset = Interface.GUI.Get<Panel>("panel").Get<ComboBox>("tileselect").GetSelectedItemIndex() + 1;
                        if (loc.X >= 0 && loc.X < Data.Map.SizeX && loc.Y >= 0 && loc.Y < Data.Map.SizeY) {
                            Data.Map.Layers.ElementAt(layer).Tiles[loc.X, loc.Y].TileX = 0;
                            Data.Map.Layers.ElementAt(layer).Tiles[loc.X, loc.Y].TileY = 0;
                            Data.Map.Layers.ElementAt(layer).Tiles[loc.X, loc.Y].Tileset = 0;
                        }
                    }
                }
                break;

            }
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
        public static void WindowMousePressed(MouseButtonEventArgs e) {
            Input.MouseButton[(Int32)e.Button] = true;
            Input.MousePressed = true;
            switch (Interface.CurrentUI) {

                case Interface.Windows.MapEditor:
                var tileselect  = Interface.GUI.Get<ChildWindow>("tileset");
                var layereditor = Interface.GUI.Get<ChildWindow>("layers");
                if (Input.Mouse.X > tileselect.Position.X && Input.Mouse.X < tileselect.Position.X + tileselect.Size.X &&
                    Input.Mouse.Y > tileselect.Position.Y && Input.Mouse.Y < tileselect.Position.Y + tileselect.Size.Y) {
                    Input.MousePressedOnMap = false;
                } else 
                if (Input.Mouse.X > layereditor.Position.X && Input.Mouse.X < layereditor.Position.X + layereditor.Size.X &&
                    Input.Mouse.Y > layereditor.Position.Y && Input.Mouse.Y < layereditor.Position.Y + layereditor.Size.Y) {
                    Input.MousePressedOnMap = false;
                } else {
                    Input.MousePressedOnMap = true;
                }
                break;

            }
        }
        public static void WindowMouseReleased(MouseButtonEventArgs e) {
            Input.MousePressed                  = false;
            Input.MousePressedOnMap             = false;
            Input.MouseButton[(Int32)e.Button]  = false;
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

        internal static void WindowMouseMoved(MouseMoveEventArgs e) {
            Input.Mouse.X = e.X;
            Input.Mouse.Y = e.Y;
        }

        public static Vector2i GetCurrentTile(float mousex, float mousey) {
            var tx = Graphics.OffSet.X > 0 ? mousex - Graphics.OffSet.X : mousex - Graphics.OffSet.X;
            var ty = Graphics.OffSet.Y > 0 ? mousey - Graphics.OffSet.Y : mousey - Graphics.OffSet.Y;
            return new Vector2i((Int32)tx / 32, (Int32)ty / 32);
        }
    }
}
