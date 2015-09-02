using System;
using System.Windows.Forms;
using Editor.Networking;
using Editor.Database;
using System.Threading;
using Editor.Graphics;

namespace Editor {
    static class Program {

        public static Login         FrmLogin;
        public static MainEditor    FrmEditor;
        public static Client        NetworkClient = new Client();

        public static System.Threading.Timer PingServer;

        static void Main() {

            // Set up standard WinForm nonsense.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Program.FrmLogin = new Login();

            // Set up some core settings.
            Data.AppPath = AppDomain.CurrentDomain.BaseDirectory;
            Data.CheckDirectories();

            // Load our application settings.
            Data.LoadSettings();

            // Initialize local data.
            Rendering.InitGraphics();

            // Set up our networking.
            Program.NetworkClient.ConnectedHandler      += PacketHandler.HandleConnected;
            Program.NetworkClient.PacketHandler         += PacketHandler.HandlePacket;
            Program.NetworkClient.DisconnectedHandler   += PacketHandler.HandleDisconnected;

            // Timers
            PingServer = new System.Threading.Timer(new TimerCallback(PacketHandler.PingServer), null, 0, 2000);

            // Connect to the server!
            Program.NetworkClient.Open(Data.Settings.IPAddress, Data.Settings.Port);


            Application.Run(Program.FrmLogin);
        }
    }
}
