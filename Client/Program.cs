using System;
using Client.Rendering;
using Client.Database;
using System.Threading;

namespace Client {
    static class Program {

        #region Declares
        public static Thread GThread;

        private static ManualResetEvent KeepAlive = new ManualResetEvent(false);
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

            // Set up our timers with Logic handlers.
            

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
