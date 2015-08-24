using System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;

namespace Client.Rendering {
    static class Graphics {

        #region Declarations
        private static RenderWindow Screen;

        private static Dictionary<String, Texture> Tileset = new Dictionary<String, Texture>();
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

            // Done loading! Start the real menu.
            Interface.ChangeUI(Interface.Windows.MainMenu);

            // Move on to rendering the window.
            Graphics.RenderScreen();
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

        #endregion
    }
}
