﻿using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.IO;
using Client.Database;
using Extensions.Database;
using Extensions;
using System.Linq;
using System.Threading;

namespace Client.Rendering {
    static class Graphics {

        #region Declarations
        private static RenderWindow Screen;

        public static   Dictionary<Int32, TexData>   Tileset   = new Dictionary<Int32, TexData>();
        private static  Dictionary<Int32, TexData>   Sprite    = new Dictionary<Int32, TexData>();
        public static   ManualResetEvent             Loaded    = new ManualResetEvent(false);
        private static  Vector2i                     OffSet;
        private static  Font                         NameFont;
        #endregion

        #region Methods
        public static void InitWindow(Object obj) {
            Object[] data               = obj as Object[];
            var width                   = (Int32)data[0];
            var height                  = (Int32)data[1];
            var title                   = (String)data[2];
            var fullscreen              = (Boolean)data[3];

            // Set up our Window.
            Screen                      = new RenderWindow(new VideoMode((uint)width, (uint)height), title, fullscreen == true ? Styles.Fullscreen : Styles.Titlebar | Styles.Close);
            Screen.Closed               += new EventHandler(WindowClosed);
            Screen.KeyPressed           += new EventHandler<KeyEventArgs>(WindowKeyPressed);
            Screen.KeyReleased          += new EventHandler<KeyEventArgs>(WindowKeyReleased);
            Screen.MouseButtonPressed   += new EventHandler<MouseButtonEventArgs>(WindowMousePressed);
            Screen.MouseMoved           += new EventHandler<MouseMoveEventArgs>(WindowMouseMoved);
            Screen.SetFramerateLimit(60);

            // Show the screen.
            Screen.Display();

            // Initialize the UI.
            Interface.InitGUI(Screen);
            Interface.ChangeUI(Interface.Windows.Loading);
            Graphics.RenderScreenOnce();

            // Initialize our Graphics.
            Interface.GUI.Get<TGUI.Panel>("loadpanel").Get<TGUI.Label>("loadtext").Text = "Initializing Grahics...";
            Graphics.RenderScreenOnce();
            Graphics.NameFont = new Font(String.Format("{0}data files\\interface\\names.ttf", Data.AppPath));
            Graphics.InitSprites();
            Graphics.InitTilesets();

            Interface.GUI.Get<TGUI.Panel>("loadpanel").Get<TGUI.Label>("loadtext").Text = "Connecting...";

            Loaded.Set();

            // Move on to rendering the window.
            Graphics.RenderScreen();
        }
        public static void CloseScreen() {
            WindowClosed(null, null);
        }
        private static void RenderScreen() {
            while (Screen.IsOpen) {
                // Clear the screen of all data.
                Screen.Clear(Color.Black);

                // Only render this if we're in-game.
                if (Data.InGame) {

                    // Render our lower layers.
                    Graphics.DrawMap(true);

                    // Draw our players.
                    Graphics.DrawPlayers();

                    // Render our upper layers.
                    Graphics.DrawMap(false);

                    // If in the map editor, draw map outlines.
                    if (Interface.CurrentUI == Interface.Windows.MapEditor) {
                        Graphics.DrawMapEditorOutline();
                    }

                    // Draw player names.
                    Graphics.DrawPlayerNames();
                }

                // Render the UI on top of everything!
                Interface.Draw();

                // If we're in the map editor, draw our map editor items on top of everything else!
                if (Interface.CurrentUI == Interface.Windows.MapEditor) {
                    Graphics.DrawMapEditorTileset();
                }

                // Handle all screen events and render changes.
                Screen.DispatchEvents();
                Screen.Display();
            }
        }

        private static void DrawMapEditorOutline() {
            var left = new RectangleShape();
            left.FillColor = Color.Magenta;
            left.Size = new Vector2f(1, Data.Map.SizeY * 32);
            left.Position = new Vector2f(Graphics.OffSet.X, Graphics.OffSet.Y);
            Screen.Draw(left);
            var right = new RectangleShape();
            right.FillColor = Color.Magenta;
            right.Size = new Vector2f(1, Data.Map.SizeY * 32);
            right.Position = new Vector2f(Graphics.OffSet.X + Data.Map.SizeX * 32, Graphics.OffSet.Y);
            Screen.Draw(right);
            var top = new RectangleShape();
            top.FillColor = Color.Magenta;
            top.Size = new Vector2f(Data.Map.SizeX *32, 1);
            top.Position = new Vector2f(Graphics.OffSet.X, Graphics.OffSet.Y);
            Screen.Draw(top);
            var bottom = new RectangleShape();
            bottom.FillColor = Color.Magenta;
            bottom.Size = new Vector2f(Data.Map.SizeX * 32, 1);
            bottom.Position = new Vector2f(Graphics.OffSet.X, Graphics.OffSet.Y + Data.Map.SizeY * 32);
            Screen.Draw(bottom);
        }

        private static void DrawMapEditorTileset() {
            var window = Interface.GUI.Get<TGUI.ChildWindow>("tileset");
            var select = Interface.GUI.Get<TGUI.Panel>("panel").Get<TGUI.ComboBox>("tileselect");
            var curset = select.GetSelectedItemIndex() + 1;
            var spr = Graphics.GetTileset(curset);
            var xoffset = window.Position.X + window.Borders.Left;
            var yoffset = window.Position.Y + window.TitleBarHeight + window.Borders.Top;

            spr.Position = new Vector2f(xoffset, yoffset);
            spr.TextureRect = new IntRect(new Vector2i(0, 0), new Vector2i((Int32)spr.Texture.Size.X, (Int32)window.Size.Y - (Int32)window.Borders.Top));
            Screen.Draw(spr);
        }

        private static void RenderScreenOnce() {
            // Clear the screen of all data.
            Screen.Clear(Color.Green);

            // Render the UI on top of everything!
            Interface.Draw();

            // Handle all screen events and render changes.
            Screen.DispatchEvents();
            Screen.Display();
        }
        private static void WindowMouseMoved(object sender, MouseMoveEventArgs e) {
            
        }
        private static void WindowMousePressed(object sender, MouseButtonEventArgs e) {
            
        }
        private static void WindowKeyPressed(object sender, KeyEventArgs e) {
            Logic.Input.WindowKeyPressed(e);
        }
        private static void WindowKeyReleased(object sender, KeyEventArgs e) {
            Logic.Input.WindowKeyReleased(e);
        }
        private static void WindowClosed(object sender, EventArgs e) {
            // Destroy all graphical elements!.
            // TODO: Destroy Graphics.
            Interface.Destroy();

            // Close the screen and start shutting down the client.
            Screen.Close();
            Program.Close();
        }
        public static void UpdateWindow() {
            Screen.DispatchEvents();
        }
        private static void InitSprites() {
            var id = 0;
            var done = false;
            while (!done) {
                id++;
                if (File.Exists(String.Format("{0}data files\\sprites\\{1}.png", Data.AppPath, id))) {
                    var texdata = new TexData();
                    texdata.File = String.Format("{0}data files\\sprites\\{1}.png", Data.AppPath, id);
                    Graphics.Sprite.Add(id, texdata);
                } else {
                    done = true;
                }
            }
        }
        private static void InitTilesets() {
            var id = 0;
            var done = false;
            while (!done) {
                id++;
                if (File.Exists(String.Format("{0}data files\\tilesets\\{1}.png", Data.AppPath, id))) {
                    var texdata = new TexData();
                    texdata.File = String.Format("{0}data files\\tilesets\\{1}.png", Data.AppPath, id);
                    Graphics.Tileset.Add(id, texdata);
                } else {
                    done = true;
                }
            }
        }
        private static Boolean LoadTileset(Int32 id) {
            // make sure it exists.
            if (!Graphics.Tileset.ContainsKey(id)) return false;

            // Is it loaded already? If not load it, otherwise return true and update the time.
            if (Tileset[id].Texture != null && Tileset[id].Sprite != null) {
                Tileset[id].LastUse = DateTime.UtcNow;
                return true;
            } else {
                using (var fs = File.OpenRead(Tileset[id].File)) {
                    Tileset[id].Texture = new Texture(fs);
                    Tileset[id].Sprite  = new Sprite(Tileset[id].Texture);
                    Tileset[id].LastUse = DateTime.UtcNow;
                    return true;
                }
            }
        }
        private static Boolean LoadSprite(Int32 id) {
            // make sure it exists.
            if (!Graphics.Sprite.ContainsKey(id)) return false;

            // Is it loaded already? If not load it, otherwise return true and update the time.
            if (Sprite[id].Texture != null && Sprite[id].Sprite != null) {
                Sprite[id].LastUse = DateTime.UtcNow;
                return true;
            } else {
                using (var fs = File.OpenRead(Sprite[id].File)) {
                    Sprite[id].Texture  = new Texture(fs);
                    Sprite[id].Sprite   = new Sprite(Sprite[id].Texture);
                    Sprite[id].LastUse  = DateTime.UtcNow;
                    return true;
                }
            }
        }
        public static Sprite GetSprite(Int32 id) {
            if (!Graphics.Sprite.ContainsKey(id)) return null;
            Graphics.LoadSprite(id);
            return Graphics.Sprite[id].Sprite;
        }
        public static Sprite GetTileset(Int32 id) {
            if (!Graphics.Tileset.ContainsKey(id)) return null;
            Graphics.LoadTileset(id);
            return Graphics.Tileset[id].Sprite;
        }
        public static void DrawMap(Boolean belowplayer) {
            foreach (var layer in Data.Map.Layers) {
                if (layer.BelowPlayer = belowplayer) {
                    for (var x = 0; x < Data.Map.SizeX; x++) {
                        for (var y = 0; y < Data.Map.SizeY; y++) {
                            Graphics.DrawTile(layer.Tiles[x,y], x, y);
                        }
                    }
                }
            }
        }
        public static void DrawTile(TileData tile, Int32 x, Int32 y) {
            var spr = Graphics.GetTileset(tile.Tileset);
            if (spr == null) return;
            spr.TextureRect = new IntRect(new Vector2i(tile.TileX * 32, tile.TileY * 32), new Vector2i(32, 32));
            spr.Position = new Vector2f(Graphics.OffSet.X + (x * 32), Graphics.OffSet.Y + (y * 32));
            Screen.Draw(spr);
        }
        public static void UpdateOffset() {
            Int32 x;
            Int32 y;
            if (Data.Map.SizeX * 32 < Data.Settings.Graphics.ResolutionX) {
                x = (Data.Settings.Graphics.ResolutionX / 2) - ((Data.Map.SizeX * 32) / 2);
            } else {
                x = (Data.Settings.Graphics.ResolutionX / 2) - ((Data.Players[Data.MyId].X + (((Int32)Graphics.GetSprite(Data.Players[Data.MyId].Sprite).Texture.Size.X / 4) / 2)));
            }
            if (Data.Map.SizeY * 32 < Data.Settings.Graphics.ResolutionY) {
                y = (Data.Settings.Graphics.ResolutionY / 2) - ((Data.Map.SizeY * 32) / 2);
            } else {
                y = (Data.Settings.Graphics.ResolutionY / 2) - ((Data.Players[Data.MyId].Y) + (((Int32)Graphics.GetSprite(Data.Players[Data.MyId].Sprite).Texture.Size.Y / 4)));
            }
            Graphics.OffSet = new Vector2i(x, y);
        }
        public static void DrawPlayers() {
            for (var y = 0; y < Data.Map.SizeY * 32; y += 5) {
                for (var i = 0; i < Data.Players.Count; i++) {
                    var key = Data.Players.ElementAt(i).Key;
                    if (Data.Players[key].Map == Data.Players[Data.MyId].Map && Data.Players[key].Y >= y && Data.Players[key].Y < y + 5) {
                        Graphics.DrawSprite(Data.Players[key].Sprite, (Enumerations.Direction)Data.Players[key].Direction, Data.TempPlayers[key].Frame, Data.Players[key].X, Data.Players[key].Y);
                    }
                }
            }
        }
        public static void DrawSprite(Int32 spr, Enumerations.Direction dir, Int32 frame, Int32 x, Int32 y) {
            var sp = Graphics.GetSprite(spr);
            if (sp.Texture == null) return;
            Int32 yoffset = 0;
            Int32 xoffset = (Int32)(sp.Texture.Size.X / 4) * frame;
            switch (dir) {
                case Enumerations.Direction.Up:
                    yoffset = (Int32)(sp.Texture.Size.Y / 4) * 3;
                break;
                case Enumerations.Direction.Right:
                    yoffset = (Int32)(sp.Texture.Size.Y / 4) * 2;
                break;
                case Enumerations.Direction.Down:
                    yoffset = 0;
                break;
                case Enumerations.Direction.Left:
                yoffset = (Int32)(sp.Texture.Size.Y / 4);
                break;
            }
            sp.TextureRect = new IntRect(new Vector2i(xoffset, yoffset), new Vector2i((Int32)sp.Texture.Size.X / 4, (Int32)sp.Texture.Size.Y / 4));
            sp.Position = new Vector2f(Graphics.OffSet.X + (x - ((Int32)sp.Texture.Size.X / 4) / 2), Graphics.OffSet.Y + (y - ((Int32)sp.Texture.Size.X / 4)));
            Screen.Draw(sp);
        }
        public static void DrawPlayerNames() {
            for (var i = 0; i < Data.Players.Count; i++) {
                var key = Data.Players.ElementAt(i).Key;
                if (Data.Players[key].Map == Data.Players[Data.MyId].Map) {
                    Graphics.DrawPlayerName(key, Data.Players[key].X, Data.Players[key].Y);
                }
            }
        }
        public static void DrawPlayerName(Int32 id, Int32 x, Int32 y) {
            var tex = Graphics.GetSprite(Data.Players[id].Sprite);
            if (tex == null || Data.Players[id].Name.Length < 1) return;
            var name = new Text(Data.Players[id].Name, Graphics.NameFont);
            name.CharacterSize = 12;
            name.Color = Color.Yellow;
            name.Position = new Vector2f(Graphics.OffSet.X + (x - (name.DisplayedString.Length * 3)), Graphics.OffSet.Y + (y - (tex.Texture.Size.Y / 4)));
            Screen.Draw(name);
        }
        public static void CheckSpriteFrames(Object e) {
            // Player Sprites
            for (var i = 0; i < Data.TempPlayers.Count; i++) {
                var key = Data.TempPlayers.ElementAt(i).Key;
                if (Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Up] ||
                    Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Down] ||
                    Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Left] ||
                    Data.TempPlayers[key].IsMoving[(Int32)Enumerations.Direction.Right]) {
                    Data.TempPlayers[key].Frame += 1;
                    if (Data.TempPlayers[key].Frame > 3) Data.TempPlayers[key].Frame = 0; ;
                } else {
                    Data.TempPlayers[key].Frame = 0;
                }
            }
        }
        #endregion
    }
    class TexData {
        #region Declarations
        public String File         { get; set; }
        public DateTime LastUse    { get; set; }
        public Texture Texture     { get; set; }
        public Sprite Sprite       { get; set; }
        #endregion

        #region Constructors
        public TexData() {
            this.File       = String.Empty;
            this.LastUse    = DateTime.MinValue;
            this.Texture    = null;
            this.Sprite     = null;
        }
        #endregion
    }
}
