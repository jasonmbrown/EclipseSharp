using System;
using Client.Rendering;
using TGUI;
using Client.Networking;

namespace Client.Logic {
    public static class UIHandlers {

        public static void MainMenu_LoginClick(object sender, CallbackArgs e) {
            // Change our UI over to the Login interface.
            Interface.ChangeUI(Interface.Windows.Login);
        }

        public static void MainMenu_RegisterClick(object sender, CallbackArgs e) {
            // Change our UI over to the Registration interface.
            Interface.ChangeUI(Interface.Windows.Register);
        }

        public static void RegisterMenu_RegisterClick(object sender, CallbackArgs e) {
            // Retrieve our data from the UI.
            var username    = Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("username").Text;
            var password    = Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("password").Text;
            var password2   = Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("password2").Text;

            // Perform some preliminary checks.
            if (username.Length < 1 || password.Length < 1 || password2.Length < 1) {
                Interface.ShowMessagebox("Error", "Please fill in every field.");
                return;
            }
            if (!password.Equals(password2)) {
                Interface.ShowMessagebox("Error", "Passwords do not match.");
                return;
            }

            // We've passed the client-side checks! On to the server we go!
            Send.NewAccount(username, password);

            // Set our screen to the loading screen since we are awaiting a response.
            // Make sure we store some data so we can come back to this.
            Interface.LastWindow = Interface.Windows.Register;
            Interface.LastData.Clear();
            Interface.LastData.Add(username);
            Interface.LastData.Add(password);
            Interface.LastData.Add(password2);
            Interface.ChangeUI(Interface.Windows.Loading);
            Interface.GUI.Get<Panel>("loadpanel").Get<Label>("loadtext").Text = "Sending account data..";
        }

        public static void RegisterMenu_CancelClick(object sender, CallbackArgs e) {
            // Change our UI over to the Main interface.
            Interface.ChangeUI(Interface.Windows.MainMenu);
        }

        internal static void Messagebox_OKClick(object sender, CallbackArgs e) {
            // Get rid of the messagebox.
            Interface.GUI.Remove("mbox");

            // Check if we came from another UI, if so move back to it with its data intact!
            if (Interface.LastWindow != Interface.Windows.None) {
                switch (Interface.LastWindow) {
                    case Interface.Windows.Register:
                    Interface.ChangeUI(Interface.Windows.Register);
                    Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("username").Text = (String)Interface.LastData[0];
                    Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("password").Text = (String)Interface.LastData[1];
                    Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("password2").Text = (String)Interface.LastData[2];
                    break;

                    case Interface.Windows.Login:
                    Interface.ChangeUI(Interface.Windows.Login);
                    Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("username").Text = (String)Interface.LastData[0];
                    Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("password").Text = (String)Interface.LastData[1];
                    break;
                }
            }
        }

        internal static void Alertbox_OKClick(object sender, CallbackArgs e) {
            // Close our screen, and thus our program!
            Graphics.CloseScreen();
        }

        internal static void LoginMenu_CancelClick(object sender, CallbackArgs e) {
            // Change our UI over to the Main interface.
            Interface.ChangeUI(Interface.Windows.MainMenu);
        }

        internal static void LoginMenu_LoginClick(object sender, CallbackArgs e) {
            // Retrieve our data from the UI.
            var username = Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("username").Text;
            var password = Interface.GUI.Get<TGUI.Panel>("mainmenu").Get<TGUI.EditBox>("password").Text;

            // Perform some preliminary checks.
            if (username.Length < 1 || password.Length < 1) {
                Interface.ShowMessagebox("Error", "Please fill in every field.");
                return;
            }

            // We've passed the client-side checks! On to the server we go!
            Send.Login(username, password);

            // Set our screen to the loading screen since we are awaiting a response.
            // Make sure we store some data so we can come back to this.
            Interface.LastWindow = Interface.Windows.Login;
            Interface.LastData.Clear();
            Interface.LastData.Add(username);
            Interface.LastData.Add(password);
            Interface.ChangeUI(Interface.Windows.Loading);
            Interface.GUI.Get<Panel>("loadpanel").Get<Label>("loadtext").Text = "Sending account data..";
        }
    }
}
