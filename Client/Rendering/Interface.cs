using Client.Database;
using Client.Logic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TGUI;
using Extensions;


namespace Client.Rendering {
    public static class Interface {

        #region Declarations
        public static Gui                                                   GUI;
        private static Database.Enumerations.Windows                        CurrentUI;
        private static String                                               Theme;
        private static Dictionary<Database.Enumerations.Windows, Action>    Interfaces = new Dictionary<Database.Enumerations.Windows, Action>() {
            { Database.Enumerations.Windows.Loading, CreateLoadMenu },
            {Database.Enumerations.Windows.MainMenu, CreateMainMenu }
        };
        #endregion

        #region Methods
        public static void InitGUI(RenderWindow screen) {
            // Init our screen with new settings.
            GUI = new Gui(screen);
            GUI.GlobalFont = new Font(String.Format("{0}data files\\interface\\interface.ttf", Data.AppPath));
            Theme = String.Format("{0}data files\\interface\\{1}", Data.AppPath, Data.Settings.Graphics.Theme);
        }
        public static void ClearGUI() {
            // Remove all current Widgets.
            CurrentUI = Database.Enumerations.Windows.None;
            GUI.RemoveAllWidgets();
        }
        public static void ChangeUI(Database.Enumerations.Windows newui) {
            // If this UI isn't used right now, 
            if (newui != CurrentUI) {
                // Clear the old UI, and load the new one!
                ClearGUI();
                Interfaces.TryGet(newui, () => { CurrentUI = Database.Enumerations.Windows.None; })();
            }
        }
        public static void Draw() {
            // Draw the UI!
            if (CurrentUI != Database.Enumerations.Windows.None) GUI.Draw();
        }
        public static void Destroy() {
            ClearGUI();
            GUI.CleanUp();
        }
        private static void CreateLoadMenu() {
            var resx = Data.Settings.Graphics.ResolutionX;
            var resy = Data.Settings.Graphics.ResolutionY;

            var backpic         = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size        = new Vector2f((float)resx,(float)resy);
            backpic.Position    = new Vector2f(0f, 0f);

            var panel = GUI.Add( new Panel(), "loadpanel");
            panel.Size = new Vector2f(resx, 30);
            panel.Position = new Vector2f(0, resy - 30);
            panel.BackgroundColor = Color.White;

            var label = panel.Add(new Label(Theme), "loadtext");
            label.Text = "Initializing Rendering Engine";
            label.TextSize = 14;
            label.TextColor = Color.Black;
            label.Position = new Vector2f((resx / 2) - (label.Size.X / 2), (panel.Size.Y / 2) - (label.Size.Y / 2));

            // Set our current UI!
            CurrentUI = Database.Enumerations.Windows.Loading;
       }
        private static void CreateMainMenu() {
            var resx = Data.Settings.Graphics.ResolutionX;
            var resy = Data.Settings.Graphics.ResolutionY;

            var backpic = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size = new Vector2f((float)resx, (float)resy);
            backpic.Position = new Vector2f(0f, 0f);

            var window = GUI.Add(new Panel(), "mainmenu");
            window.Size = new Vector2f(500, 300);
            window.Position = new Vector2f((resx / 2) - (window.Size.X / 2), (resy / 2) - (window.Size.Y / 2));
            window.Transparency = 200;

            var textbox = window.Add(new Label(Theme), "gamename");
            textbox.TextColor = Color.Black;
            textbox.TextSize = 60;
            textbox.Text = Data.Settings.GameName;
            textbox.Position = new Vector2f((window.Size.X / 2) - (textbox.Size.X / 2), 10);

            var login = window.Add(new Button(Theme), "login");
            login.Text = "Login";
            login.Position = new Vector2f(5, 260);
            login.LeftMouseClickedCallback += UIHandlers.MainMenu_LoginClick;

            var register = window.Add(new Button(Theme), "register");
            register.Text = "Register";
            register.Position = new Vector2f(window.Size.X - (5 + register.Size.X), 260);
            register.LeftMouseClickedCallback += UIHandlers.MainMenu_RegisterClick;

            // Set our current UI!
            CurrentUI = Database.Enumerations.Windows.MainMenu;
        }
        #endregion
    }
}
