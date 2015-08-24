using System;
using Client.Rendering;
using TGUI;

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
                Interface.ShowMessagebox("Oopsie!", "Please fill in every field.");
                return;
            }

            // TODO: Registration
        }

        public static void RegisterMenu_CancelClick(object sender, CallbackArgs e) {
            // Change our UI over to the Registration interface.
            Interface.ChangeUI(Interface.Windows.MainMenu);
        }

    }
}
