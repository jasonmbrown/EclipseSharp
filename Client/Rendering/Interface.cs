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
        public enum Windows {
            None,
            Loading,
            MainMenu,
            Login,
            Register,
            CharacterSelect,
            CharacterCreate
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
            { Windows.CharacterCreate,  CreateCharacterCreate }
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
            GUI.RemoveAllWidgets();
        }
        public static void ChangeUI(Windows newui) {
            // If this UI isn't used right now, 
            if (newui != CurrentUI) {
                // Clear the old UI, and load the new one!
                ClearGUI();
                Interfaces.TryGet(newui, () => { CurrentUI = Windows.None; })();
            }
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

            var chr1 = window.Add(new Button(Theme), "chr1");
            chr1.Text = Data.Players[Data.MyId].Characters[0].Name.Length < 1 ? "Create" : String.Format("{0} Lv.{1}", Data.Players[Data.MyId].Characters[0].Name, Data.Players[Data.MyId].Characters[0].Level);
            chr1.Position = new Vector2f(5, (window.Size.Y / 2) - (chr1.Size.Y / 2));
            chr1.LeftMouseClickedCallback += UIHandlers.CharacterSelect_Char1Click;

            var chr2 = window.Add(new Button(Theme), "chr2");
            chr2.Text = Data.Players[Data.MyId].Characters[1].Name.Length < 1 ? "Create" : String.Format("{0} Lv.{1}", Data.Players[Data.MyId].Characters[1].Name, Data.Players[Data.MyId].Characters[1].Level);
            chr2.Position = new Vector2f((window.Size.X / 2) - (chr2.Size.X / 2), (window.Size.Y / 2) - (chr2.Size.Y / 2));
            chr2.LeftMouseClickedCallback += UIHandlers.CharacterSelect_Char2Click;

            var chr3 = window.Add(new Button(Theme), "chr3");
            chr3.Text = Data.Players[Data.MyId].Characters[2].Name.Length < 1 ? "Create" : String.Format("{0} Lv.{1}", Data.Players[Data.MyId].Characters[2].Name, Data.Players[Data.MyId].Characters[2].Level);
            chr3.Position = new Vector2f(window.Size.X - (chr3.Size.X + 5), (window.Size.Y / 2) - (chr2.Size.Y / 2));
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
        #endregion
    }
}
