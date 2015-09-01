﻿using Client.Database;
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
        public enum Windows {
            None,
            Loading,
            MainMenu,
            Login,
            Register,
            CharacterSelect,
            CharacterCreate,
            Game,
            MapEditor
        }

        public static   Gui                                                   GUI;
        public static   Windows                                               LastWindow;
        public static   List<Object>                                          LastData = new List<Object>();
        public static   Windows                                               CurrentUI;
        private static  String                                                Theme;
        private static  Dictionary<Windows, Action>    Interfaces = new Dictionary<Windows, Action>() {
            { Windows.Loading,          CreateLoadMenu },
            { Windows.MainMenu,         CreateMainMenu },
            { Windows.Login,            CreateLoginMenu },
            { Windows.Register,         CreateRegisterMenu },
            { Windows.CharacterSelect,  CreateCharacterSelect },
            { Windows.CharacterCreate,  CreateCharacterCreate },
            { Windows.Game,             CreateGame },
            { Windows.MapEditor,        CreateMapEditor }
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
            CurrentUI = Windows.None;
            try {
                if (GUI != null) GUI.RemoveAllWidgets();
            } catch { }
        }
        public static void ChangeUI(Windows newui) {
            // If this UI isn't used right now, load it!
            try {
                if (newui != CurrentUI) {
                    // Clear the old UI, and load the new one!
                    ClearGUI();
                    Interfaces.TryGet(newui, () => { CurrentUI = Windows.None; })();
                }
            } catch { }
        }
        public static void Draw() {
            // Draw the UI!
            // This can go wrong since we can wreck the internal arrays, threading ahoy!
            try {
                if (CurrentUI != Windows.None) GUI.Draw();
            } catch { }
        }
        public static void Destroy() {
            ClearGUI();
            GUI.CleanUp();
        }
        public static void ShowMessagebox(String title, String text) {
            var resx                = Data.Settings.Graphics.ResolutionX;
            var resy                = Data.Settings.Graphics.ResolutionY;

            var mbox                = GUI.Add(new MessageBox(Theme), "mbox");
            mbox.Title              = title;
            var label               = mbox.Add(new Label(Theme));
            label.Text              = text;
            label.TextSize          = 16;
            label.TextColor         = Color.Black;
            mbox.Size               = new Vector2f(label.Size.X + 10, label.Size.Y + 50);
            label.Position          = new Vector2f(5, 5);
            mbox.Position           = new Vector2f((resx / 2) - (mbox.Size.X / 2), (resy / 2) - (mbox.Size.Y / 2));
            mbox.ClosedCallback += UIHandlers.Messagebox_OKClick;
            var button              = mbox.Add(new Button(Theme));
            button.Text             = "OK";
            button.Position         = new Vector2f((mbox.Size.X / 2) - (button.Size.X / 2), mbox.Size.Y - (button.Size.Y + 5));
            button.LeftMouseClickedCallback += UIHandlers.Messagebox_OKClick;
        }
        public static void ShowAlertbox(String title, String text) {
            var resx                = Data.Settings.Graphics.ResolutionX;
            var resy                = Data.Settings.Graphics.ResolutionY;

            var mbox                = GUI.Add(new MessageBox(Theme), "mbox");
            mbox.Title              = title;
            var label               = mbox.Add(new Label(Theme));
            label.Text              = text;
            label.TextSize          = 16;
            label.TextColor         = Color.Black;
            mbox.Size               = new Vector2f(label.Size.X + 10, label.Size.Y + 50);
            label.Position          = new Vector2f(5, 5);
            mbox.Position           = new Vector2f((resx / 2) - (mbox.Size.X / 2), (resy / 2) - (mbox.Size.Y / 2));
            mbox.ClosedCallback += UIHandlers.Alertbox_OKClick;
            var button              = mbox.Add(new Button(Theme));
            button.Text             = "OK";
            button.Position         = new Vector2f((mbox.Size.X / 2) - (button.Size.X / 2), mbox.Size.Y - (button.Size.Y + 5));
            button.LeftMouseClickedCallback += UIHandlers.Alertbox_OKClick;
        }
        private static void CreateLoadMenu() {
            var resx                = Data.Settings.Graphics.ResolutionX;
            var resy                = Data.Settings.Graphics.ResolutionY;

            var backpic             = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size            = new Vector2f((float)resx,(float)resy);
            backpic.Position        = new Vector2f(0f, 0f);

            var panel               = GUI.Add( new Panel(), "loadpanel");
            panel.Size              = new Vector2f(resx, 30);
            panel.Position          = new Vector2f(0, resy - 30);
            panel.BackgroundColor   = Color.White;

            var label               = panel.Add(new Label(Theme), "loadtext");
            label.Text              = "Initializing Rendering Engine";
            label.TextSize          = 14;
            label.TextColor         = Color.Black;
            label.Position          = new Vector2f((resx / 2) - (label.Size.X / 2), (panel.Size.Y / 2) - (label.Size.Y / 2));

            // Set our current UI!
            CurrentUI               = Windows.Loading;
       }
        private static void CreateMainMenu() {
            var resx            = Data.Settings.Graphics.ResolutionX;
            var resy            = Data.Settings.Graphics.ResolutionY;

            var backpic         = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size        = new Vector2f((float)resx, (float)resy);
            backpic.Position    = new Vector2f(0f, 0f);

            var window          = GUI.Add(new Panel(), "mainmenu");
            window.Size         = new Vector2f(500, 300);
            window.Position     = new Vector2f((resx / 2) - (window.Size.X / 2), (resy / 2) - (window.Size.Y / 2));
            window.Transparency = 200;

            var gamename        = window.Add(new Label(Theme), "gamename");
            gamename.TextColor  = Color.Black;
            gamename.TextSize   = 60;
            gamename.Text       = Data.Settings.GameName;
            gamename.Position   = new Vector2f((window.Size.X / 2) - (gamename.Size.X / 2), 10);

            var login           = window.Add(new Button(Theme), "login");
            login.Text          = "Login";
            login.Position      = new Vector2f(5, 260);
            login.LeftMouseClickedCallback += UIHandlers.MainMenu_LoginClick;

            var register        = window.Add(new Button(Theme), "register");
            register.Text       = "Register";
            register.Position   = new Vector2f(window.Size.X - (5 + register.Size.X), 260);
            register.LeftMouseClickedCallback += UIHandlers.MainMenu_RegisterClick;

            // Set our current UI!
            CurrentUI           = Windows.MainMenu;
        }
        private static void CreateRegisterMenu() {
            var resx            = Data.Settings.Graphics.ResolutionX;
            var resy            = Data.Settings.Graphics.ResolutionY;

            var backpic         = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size        = new Vector2f((float)resx, (float)resy);
            backpic.Position    = new Vector2f(0f, 0f);

            var window          = GUI.Add(new Panel(), "mainmenu");
            window.Size         = new Vector2f(300, 300);
            window.Position     = new Vector2f((resx / 2) - (window.Size.X / 2), (resy / 2) - (window.Size.Y / 2));
            window.Transparency = 200;

            var title           = window.Add(new Label(Theme), "title");
            title.TextColor     = Color.Black;
            title.TextSize      = 60;
            title.Text          = "Register";
            title.Position      = new Vector2f((window.Size.X / 2) - (title.Size.X / 2), 10);

            var label1          = window.Add(new Label(Theme));
            label1.Text         = "Username:";
            label1.TextColor    = Color.Black;
            label1.TextSize     = 14;
            label1.Position     = new Vector2f(10, 100);

            var username        = window.Add(new EditBox(Theme), "username");
            username.Size       = new Vector2f(280, 20);
            username.Position   = new Vector2f(10, 115);

            var label2          = window.Add(new Label(Theme));
            label2.Text         = "Password:";
            label2.TextColor    = Color.Black;
            label2.TextSize     = 14;
            label2.Position     = new Vector2f(10, 140);

            var password        = window.Add(new EditBox(Theme), "password");
            password.Size       = new Vector2f(280, 20);
            password.Position   = new Vector2f(10, 155);
            password.PasswordCharacter = "*";

            var label3          = window.Add(new Label(Theme));
            label3.Text         = "Confirm Password:";
            label3.TextColor    = Color.Black;
            label3.TextSize     = 14;
            label3.Position     = new Vector2f(10, 180);

            var password2       = window.Add(new EditBox(Theme), "password2");
            password2.Size      = new Vector2f(280, 20);
            password2.Position  = new Vector2f(10, 195);
            password2.PasswordCharacter = "*";

            var cancel          = window.Add(new Button(Theme), "cancel");
            cancel.Text         = "cancel";
            cancel.Position     = new Vector2f(5, 260);
            cancel.LeftMouseClickedCallback += UIHandlers.RegisterMenu_CancelClick;

            var register        = window.Add(new Button(Theme), "register");
            register.Text       = "Register";
            register.Position   = new Vector2f(window.Size.X - (5 + register.Size.X), 260);
            register.LeftMouseClickedCallback += UIHandlers.RegisterMenu_RegisterClick;

            CurrentUI           = Windows.Register;
        }
        private static void CreateLoginMenu() {
            var resx            = Data.Settings.Graphics.ResolutionX;
            var resy            = Data.Settings.Graphics.ResolutionY;

            var backpic         = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size        = new Vector2f((float)resx, (float)resy);
            backpic.Position    = new Vector2f(0f, 0f);

            var window          = GUI.Add(new Panel(), "mainmenu");
            window.Size         = new Vector2f(300, 300);
            window.Position     = new Vector2f((resx / 2) - (window.Size.X / 2), (resy / 2) - (window.Size.Y / 2));
            window.Transparency = 200;

            var title           = window.Add(new Label(Theme), "title");
            title.TextColor     = Color.Black;
            title.TextSize      = 60;
            title.Text          = "Login";
            title.Position      = new Vector2f((window.Size.X / 2) - (title.Size.X / 2), 10);

            var label1          = window.Add(new Label(Theme));
            label1.Text         = "Username:";
            label1.TextColor    = Color.Black;
            label1.TextSize     = 14;
            label1.Position     = new Vector2f(10, 100);

            var username        = window.Add(new EditBox(Theme), "username");
            username.Size       = new Vector2f(280, 20);
            username.Position   = new Vector2f(10, 115);

            var label2          = window.Add(new Label(Theme));
            label2.Text         = "Password:";
            label2.TextColor    = Color.Black;
            label2.TextSize     = 14;
            label2.Position     = new Vector2f(10, 140);

            var password        = window.Add(new EditBox(Theme), "password");
            password.Size       = new Vector2f(280, 20);
            password.Position   = new Vector2f(10, 155);
            password.PasswordCharacter = "*";

            var cancel          = window.Add(new Button(Theme), "cancel");
            cancel.Text         = "cancel";
            cancel.Position     = new Vector2f(5, 260);
            cancel.LeftMouseClickedCallback += UIHandlers.LoginMenu_CancelClick;

            var register        = window.Add(new Button(Theme), "login");
            register.Text       = "login";
            register.Position   = new Vector2f(window.Size.X - (5 + register.Size.X), 260);
            register.LeftMouseClickedCallback += UIHandlers.LoginMenu_LoginClick;

            CurrentUI           = Windows.Login;
        }
        private static void CreateCharacterSelect() {
            var resx            = Data.Settings.Graphics.ResolutionX;
            var resy            = Data.Settings.Graphics.ResolutionY;

            var backpic         = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size        = new Vector2f((float)resx, (float)resy);
            backpic.Position    = new Vector2f(0f, 0f);

            var window          = GUI.Add(new Panel(), "register");
            window.Size         = new Vector2f(500, 300);
            window.Position     = new Vector2f((resx / 2) - (window.Size.X / 2), (resy / 2) - (window.Size.Y / 2));
            window.Transparency = 200;

            var label           = window.Add(new Label(Theme), "labelselect");
            label.TextColor     = Color.Black;
            label.TextSize      = 60;
            label.Text          = "Select Character";
            label.Position      = new Vector2f((window.Size.X / 2) - (label.Size.X / 2), 10);

            var chr1            = window.Add(new Button(Theme), "chr1");
            chr1.Text           = Data.CharSelect[0].Name.Length < 1 ? "Create" : String.Format("{0} Lv.{1}", Data.CharSelect[0].Name, Data.CharSelect[0].Level);
            chr1.Position       = new Vector2f(5, (window.Size.Y / 2) - (chr1.Size.Y / 2));
            chr1.LeftMouseClickedCallback += UIHandlers.CharacterSelect_Char1Click;

            var chr2            = window.Add(new Button(Theme), "chr2");
            chr2.Text           = Data.CharSelect[1].Name.Length < 1 ? "Create" : String.Format("{0} Lv.{1}", Data.CharSelect[1].Name, Data.CharSelect[1].Level);
            chr2.Position       = new Vector2f((window.Size.X / 2) - (chr2.Size.X / 2), (window.Size.Y / 2) - (chr2.Size.Y / 2));
            chr2.LeftMouseClickedCallback += UIHandlers.CharacterSelect_Char2Click;

            var chr3            = window.Add(new Button(Theme), "chr3");
            chr3.Text           = Data.CharSelect[2].Name.Length < 1 ? "Create" : String.Format("{0} Lv.{1}", Data.CharSelect[2].Name, Data.CharSelect[2].Level);
            chr3.Position       = new Vector2f(window.Size.X - (chr3.Size.X + 5), (window.Size.Y / 2) - (chr2.Size.Y / 2));
            chr3.LeftMouseClickedCallback += UIHandlers.CharacterSelect_Char3Click;

            var logout          = window.Add(new Button(Theme), "logout");
            logout.Text         = "Logout";
            logout.Position     = new Vector2f((window.Size.X / 2) - (logout.Size.X / 2), 260);
            logout.LeftMouseClickedCallback += UIHandlers.CharacterSelect_LogoutClick;

            // Set our current UI!
            CurrentUI           = Windows.CharacterSelect;
        }
        private static void CreateCharacterCreate() {
            var resx            = Data.Settings.Graphics.ResolutionX;
            var resy            = Data.Settings.Graphics.ResolutionY;

            var backpic         = GUI.Add(new Picture(String.Format("{0}data files\\interface\\background.png", Data.AppPath)), "background");
            backpic.Size        = new Vector2f((float)resx, (float)resy);
            backpic.Position    = new Vector2f(0f, 0f);

            var window          = GUI.Add(new Panel(), "window");
            window.Size         = new Vector2f(500, 300);
            window.Position     = new Vector2f((resx / 2) - (window.Size.X / 2), (resy / 2) - (window.Size.Y / 2));
            window.Transparency = 200;

            var label           = window.Add(new Label(Theme));
            label.TextColor     = Color.Black;
            label.TextSize      = 48;
            label.Text          = "Create Character";
            label.Position      = new Vector2f((window.Size.X / 2) - (label.Size.X / 2), 10);

            var label1          = window.Add(new Label(Theme));
            label1.Text         = "Name:";
            label1.TextColor    = Color.Black;
            label1.TextSize     = 14;
            label1.Position     = new Vector2f(10, 100);

            var name            = window.Add(new EditBox(Theme), "name");
            name.Size           = new Vector2f(280, 20);
            name.Position       = new Vector2f(10, 115);

            var label3 = window.Add(new Label(Theme));
            label3.Text = "Class:";
            label3.TextColor = Color.Black;
            label3.TextSize = 14;
            label3.Position = new Vector2f(10, 135);

            var selclass        = window.Add(new ComboBox(Theme), "class");
            selclass.Position   = new Vector2f(10, 150);
            selclass.Size       = new Vector2f(name.Size.X, name.Size.Y);
            for (var i = 1; i < Data.Classes.Count + 1; i++) {
                selclass.AddItem(Data.Classes[i].Name);
            }
            selclass.SetSelectedItem(Data.Classes[1].Name);

            var label2          = window.Add(new Label(Theme));
            label2.Text         = "Gender:";
            label2.TextColor    = Color.Black;
            label2.TextSize     = 14;
            label2.Position     = new Vector2f(window.Size.X - (label2.Size.X + 15), 100);

            var male            = window.Add(new Checkbox(Theme), "male");
            male.Text           = "Male";
            male.Position       = new Vector2f(window.Size.X - (label2.Size.X + 15), 115);
            male.TextColor      = Color.Black;
            male.LeftMouseClickedCallback += UIHandlers.CreateCharacter_MaleClick;
            male.Check();

            var female          = window.Add(new Checkbox(Theme), "female");
            female.Text         = "Female";
            female.Position     = new Vector2f(window.Size.X - (label2.Size.X + 15), 130);
            female.TextColor    = Color.Black;
            female.LeftMouseClickedCallback += UIHandlers.CreateCharacter_FemaleClick;

            var cancel          = window.Add(new Button(Theme), "cancel");
            cancel.Text         = "Cancel";
            cancel.Position     = new Vector2f(5, 260);
            cancel.LeftMouseClickedCallback += UIHandlers.CreateCharacter_CancelClick;

            var create          = window.Add(new Button(Theme), "create");
            create.Text         = "Create";
            create.Position     = new Vector2f(window.Size.X - (create.Size.X + 5), 260);
            create.LeftMouseClickedCallback += UIHandlers.CreateCharacter_CreateClick;

            // Set our current UI!
            CurrentUI           = Windows.CharacterCreate;
        }
        private static void CreateGame() {

            var chatbox = GUI.Add(new ChatBox(Theme), "chat");
            chatbox.Transparency = 128;
            chatbox.Size = new Vector2f(400, 150);
            chatbox.Position = new Vector2f(5, Data.Settings.Graphics.ResolutionY - (chatbox.Size.Y + 30));
            chatbox.TextSize = 12;

            var chatinput = GUI.Add(new EditBox(Theme), "chatinput");
            chatinput.Size = new Vector2f(chatbox.Size.X, 20);
            chatinput.Position = new Vector2f(5, Data.Settings.Graphics.ResolutionY - (chatinput.Size.Y + 5));
            chatinput.Visible = false;

            CurrentUI = Windows.Game;
            LastData.Clear();
            LastWindow = Windows.None;
        }
        private static void CreateMapEditor() {

            var panel = GUI.Add(new Panel(), "panel");
            panel.Size = new Vector2f(Data.Settings.Graphics.ResolutionX, 20);
            panel.Position = new Vector2f(0, 0);

            var label1 = panel.Add(new Label(Theme));
            label1.Text = "Tileset:";
            label1.Position = new Vector2f(0, 5);
            label1.TextColor = Color.Black;
            label1.TextSize = 14;

            var tileselect = panel.Add(new ComboBox(Theme), "tileselect");
            foreach (var t in Graphics.Tileset) {
                tileselect.AddItem(String.Format("Tileset {0}", t.Key));
            }
            tileselect.SetSelectedItem(0);
            tileselect.Position = new Vector2f(label1.Size.X + 3, 0);
            tileselect.Size = new Vector2f(120, panel.Size.Y);

            var opentwindow = panel.Add(new Button(Theme), "opentwindow");
            opentwindow.Text = "Tile Select";
            opentwindow.Size = new Vector2f(opentwindow.Size.X, panel.Size.Y);
            opentwindow.Position = new Vector2f(tileselect.Position.X + tileselect.Size.X + 5, 0);
            opentwindow.LeftMouseClickedCallback += UIHandlers.MapEditor_OpenTileWindowClick;

            var label2 = panel.Add(new Label(Theme));
            label2.Text = "| Layer:";
            label2.Position = new Vector2f(opentwindow.Position.X + opentwindow.Size.X + 10, 5);
            label2.TextColor = Color.Black;
            label2.TextSize = 14;

            var layerselect = panel.Add(new ComboBox(Theme), "layerselect");
            foreach (var t in Data.Map.Layers) {
                layerselect.AddItem(t.Name);
            }
            layerselect.SetSelectedItem(0);
            layerselect.Position = new Vector2f(label2.Position.X + label2.Size.X + 3, 0);
            layerselect.Size = new Vector2f(120, panel.Size.Y);

            var openlwindow = panel.Add(new Button(Theme), "openlwindow");
            openlwindow.Text = "Layer Editor";
            openlwindow.Size = new Vector2f(openlwindow.Size.X, panel.Size.Y);
            openlwindow.Position = new Vector2f(layerselect.Position.X + openlwindow.Size.X + 5, 0);
            openlwindow.LeftMouseClickedCallback += UIHandlers.MapEditor_OpenLayerWindowClick;

            var save = panel.Add(new Button(Theme), "save");
            save.Text = "Save Map";
            save.Size = new Vector2f(save.Size.X, panel.Size.Y);
            save.Position = new Vector2f(panel.Size.X - save.Size.X, 0);
            save.LeftMouseClickedCallback += UIHandlers.MapEditor_SaveClick;

            var cancel = panel.Add(new Button(Theme), "cancel");
            cancel.Text = "Cancel";
            cancel.Size = new Vector2f(save.Size.X, panel.Size.Y);
            cancel.Position = new Vector2f(save.Position.X - (cancel.Size.X + 5), 0);
            cancel.LeftMouseClickedCallback += UIHandlers.MapEditor_CancelClick;

            var tileset = GUI.Add(new ChildWindow(Theme), "tileset");
            tileset.Title = "Tile Selection";
            tileset.Size = new Vector2f(Graphics.GetTileset(1).Texture.Size.X + 20, ((Int32)(Data.Settings.Graphics.ResolutionY * 0.75f) / 32) * 32);
            tileset.Position = new Vector2f(20, 40);
            tileset.Visible = false;
            tileset.ClosedCallback += (s, e) => { tileset.Visible = false; };

            var tilescroll = tileset.Add(new Scrollbar(Theme), "tilescroll");
            tilescroll.Size = new Vector2f(20, tileset.Size.Y - 4);
            tilescroll.Position = new Vector2f(tileset.Size.X - 20, 2);
            tilescroll.Maximum = (Int32)((Graphics.GetTileset(1).Texture.Size.Y / 32) - (Data.Settings.Graphics.ResolutionY * 0.75f) / 32);

            var layers = GUI.Add(new ChildWindow(Theme), "layers");
            layers.Title = "Layer Editor";
            layers.Size = new Vector2f(400, 310);
            layers.Position = new Vector2f(Data.Settings.Graphics.ResolutionX - (layers.Size.X + 20), 40);
            layers.Visible = false;
            layers.ClosedCallback += (s, e) => { layers.Visible = false; };

            var layerlist = layers.Add(new ListBox(Theme), "layerlist");
            layerlist.Position = new Vector2f(2, 2);
            layerlist.Size = new Vector2f(120, 300); 
            
            CurrentUI = Windows.MapEditor;
        }
        #endregion
    }
}
