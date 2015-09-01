using System;
using Client.Rendering;
using Client.Database;
using System.Threading;
using Client.Networking;
using Client.Logic;

namespace Client {
    public static class Program {

        #region Declares
        public static Thread GThread;
        public static Networking.Client NetworkClient;

        private static ManualResetEvent KeepAlive = new ManualResetEvent(false);
        private static Timer            HandleMovement;
        private static Timer            HandleMouseClicks;
        private static Timer            CheckSpriteFrames;
        public static  Timer            PingServer;
        #endregion

        #region Methods
        static void Main() {

            // Set up some basic runtime variables.
            Data.AppPath = AppDomain.CurrentDomain.BaseDirectory;

            // Check our directory structures.
            Data.CheckDirectories();

            // Load our settings.
            Data.LoadSettings();

            // Set up our window we'll be using to render our game in.
            // Handling the rendering and window events in a seperate thread from everything else.
            Object[] obj = new Object[4];
            obj[0] = Data.Settings.Graphics.ResolutionX;
            obj[1] = Data.Settings.Graphics.ResolutionY;
            obj[2] = Data.Settings.GameName;
            obj[3] = Data.Settings.Graphics.Fullscreen;
            GThread = new Thread(new ParameterizedThreadStart(Graphics.InitWindow));
            GThread.Start(obj);

            // Set up our Network stuff.
            NetworkClient = new Networking.Client();
            NetworkClient.ConnectedHandler      += PacketHandler.ClientConnected;
            NetworkClient.PacketHandler         += PacketHandler.Handle;
            NetworkClient.DisconnectedHandler   += PacketHandler.ClientDisconnected;

            // Open our connection!
            NetworkClient.Open(Data.Settings.Network.IPAddress, Data.Settings.Network.Port);

            // Set up our timers with Logic handlers.
            // These little engines will be the lifeblood of the client. Anything that is not handled by UI or input from the server
            // will be done or correctd by these.
            PingServer          = new Timer(new TimerCallback(PacketHandler.PingServer), null, 0, 2000);
            HandleMovement      = new Timer(new TimerCallback(Input.HandleMovement), null, 0, 10);
            HandleMouseClicks   = new Timer(new TimerCallback(Input.HandleMouseClicks), null, 0, 30);
            CheckSpriteFrames   = new Timer(new TimerCallback(Graphics.CheckSpriteFrames), null, 0, 250);

            // Stops the program from closing until a signal is received.
            KeepAlive.WaitOne();

            // Destroy the client and its data!
        }

        public static void Close() {
            // Send our lovely thread a signal..
            // Time to shut down!
            KeepAlive.Set();
        }
        #endregion
    }
}
