using System;
using Client.Rendering;
using TGUI;
using Client.Networking;
using Extensions;

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

                    case Interface.Windows.CharacterCreate:
                    Interface.ChangeUI(Interface.Windows.CharacterCreate);
                    Interface.GUI.Get<TGUI.Panel>("window").Get<TGUI.EditBox>("name").Text = (String)Interface.LastData[0];
                    Interface.GUI.Get<TGUI.Panel>("window").Get<TGUI.ComboBox>("class").SetSelectedItem((Int32)Interface.LastData[1] - 1);
                    if ((Enumerations.Gender)Interface.LastData[2] == Enumerations.Gender.Male) {
                        Interface.GUI.Get<TGUI.Panel>("window").Get<TGUI.Checkbox>("male").Check();
                        Interface.GUI.Get<TGUI.Panel>("window").Get<TGUI.Checkbox>("female").Uncheck();
                    } else {
                        Interface.GUI.Get<TGUI.Panel>("window").Get<TGUI.Checkbox>("female").Check();
                        Interface.GUI.Get<TGUI.Panel>("window").Get<TGUI.Checkbox>("male").Uncheck();
                    }
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

        internal static void CreateCharacter_CancelClick(object sender, CallbackArgs e) {
            // Change our UI!
            Interface.ChangeUI(Interface.Windows.CharacterSelect);
        }

        internal static void CreateCharacter_CreateClick(object sender, CallbackArgs e) {
            var name = Interface.GUI.Get<Panel>("window").Get<EditBox>("name").Text;
            var pclass = Interface.GUI.Get<Panel>("window").Get<ComboBox>("class").GetSelectedItemIndex() + 1;
            var male = Interface.GUI.Get<Panel>("window").Get<Checkbox>("male").IsChecked();
            var female = Interface.GUI.Get<Panel>("window").Get<Checkbox>("female").IsChecked();
            var gender = male ? Enumerations.Gender.Male : Enumerations.Gender.Female;
            
            // Check some stuff.
            if (name.Length < 1) {
                Interface.ShowMessagebox("Error", "Please fill in every field.");
                return;
            }

            // Send our request.
            Send.AddCharacter(name, pclass, gender);

            // Set our screen to the loading screen since we are awaiting a response.
            // Make sure we store some data so we can come back to this.
            Interface.LastWindow = Interface.Windows.CharacterCreate;
            Interface.LastData.Clear();
            Interface.LastData.Add(name);
            Interface.LastData.Add(pclass);
            Interface.LastData.Add(gender);
            Interface.ChangeUI(Interface.Windows.Loading);
            Interface.GUI.Get<Panel>("loadpanel").Get<Label>("loadtext").Text = "Sending character data..";
        }

        internal static void CharacterSelect_LogoutClick(object sender, CallbackArgs e) {
            Send.Logout();
            Interface.ChangeUI(Interface.Windows.MainMenu);
        }

        internal static void CreateCharacter_MaleClick(object sender, CallbackArgs e) {
            Interface.GUI.Get<Panel>("window").Get<Checkbox>("male").Check();
            Interface.GUI.Get<Panel>("window").Get<Checkbox>("female").Uncheck();
        }

        internal static void CreateCharacter_FemaleClick(object sender, CallbackArgs e) {
            Interface.GUI.Get<Panel>("window").Get<Checkbox>("male").Uncheck();
            Interface.GUI.Get<Panel>("window").Get<Checkbox>("female").Check();
        }
    }
}
