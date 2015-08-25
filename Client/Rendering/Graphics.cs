using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;
using System.IO;
using Client.Database;

namespace Client.Rendering {
    static class Graphics {

        #region Declarations
        private static RenderWindow Screen;

        private static Dictionary<Int32, TexData> Tileset   = new Dictionary<Int32, TexData>();
        private static Dictionary<Int32, TexData> Sprite    = new Dictionary<Int32, TexData>();
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
            Interface.GUI.Get<TGUI.Panel>("loadpanel").Get<TGUI.Label>("loadtext").Text = "Initializing Graphics...";
            Graphics.RenderScreenOnce();
            Graphics.InitSprites();

            // Done loading! Start the real menu.
            Interface.ChangeUI(Interface.Windows.MainMenu);

            // Move on to rendering the window.
            Graphics.RenderScreen();
        }
        public static void CloseScreen() {
            WindowClosed(null, null);
        }
        private static void RenderScreen() {
            while (Screen.IsOpen) {
                // Clear the screen of all data.
                Screen.Clear(Color.Green);

                // Render the UI on top of everything!
                Interface.Draw();

                // Handle all screen events and render changes.
                Screen.DispatchEvents();
                Screen.Display();
            }
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
        private static Boolean LoadSprite(Int32 id) {
            // make sure it exists.
            if (!Graphics.Sprite.ContainsKey(id)) return false;
            if (Sprite[id].Data != null) {
                Sprite[id].LastUse = DateTime.UtcNow;
                return true;
            } else {
                using (var fs = File.OpenRead(Sprite[id].File)) {
                    Sprite[id].Data = new Texture(fs);
                    Sprite[id].LastUse = DateTime.UtcNow;
                    return true;
                }
            }
        }
        public static Texture GetSprite(Int32 id) {
            if (!Graphics.Sprite.ContainsKey(id)) return null;
            Graphics.LoadSprite(id);
            return Graphics.Sprite[id].Data;
        }
        #endregion
    }
    class TexData {
        #region Declarations
        public String File         { get; set; }
        public DateTime LastUse    { get; set; }
        public Texture Data        { get; set; }
        #endregion

        #region Constructors
        public TexData() {
            this.File       = String.Empty;
            this.LastUse    = DateTime.MinValue;
            this.Data       = null;
        }
        #endregion
    }
}
