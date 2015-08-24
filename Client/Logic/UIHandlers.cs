using System;
using Client.Rendering;
using TGUI;

namespace Client.Logic {
    public static class UIHandlers {

        public static void MainMenu_LoginClick(object sender, CallbackArgs e) {
            // Change our UI over to the Login interface.
            Interface.ChangeUI(Database.Enumerations.Windows.Login);
        }

        public static void MainMenu_RegisterClick(object sender, CallbackArgs e) {
            // Change our UI over to the Registration interface.
            Interface.ChangeUI(Database.Enumerations.Windows.Register);
        }

    }
}
