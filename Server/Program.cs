﻿using System;
using Server.Database;
using Server.Networking;
using Server.Logic;
using System.Threading;

namespace Server {
    static class Program {

        public static   Networking.Server   Server;
        public static   Boolean             Running             = true;
        private static  Timer               HandleMovement;
        private static  Timer               SyncPlayers;
        private static  Timer               MOTD;
        private static  Timer               SavePlayers;
        private static  Timer               PingClients;

        static void Main(string[] args) {

            Logger.Write("[ EclipseSharp Server ]");

            // Set up some basic runtime variables.
            Logger.Write("Setting up initial variables...");
            Data.AppPath = AppDomain.CurrentDomain.BaseDirectory;

            // Load our server settings!
            Logger.Write("Loading server settings...");
            Data.LoadSettings(Data.AppPath + "settings.xml");

            // Apply some settings to our window.
            Console.Title = String.Format("{0} - Running on Port {1}", Data.Settings.GameName, Data.Settings.Port);

            // Check and Create our data structure.
            Logger.Write("Checking data structure...");
            Data.CheckDirectories();
            Data.InitData();

            // Create our Server instance.
            // We're also setting up our event handlers that the server produces here.
            Logger.Write("Creating server objects...");
            Server = new Networking.Server(Data.Settings.MaxPlayers, Data.Settings.Port);
            Server.ConnectHandler       += PacketHandler.ClientConnected;
            Server.DisconnectHandler    += PacketHandler.ClientDisconnected;
            Server.PacketHandler        += PacketHandler.Handle;

            // Create our logic handlers.
            PingClients     = new Timer(new TimerCallback(PacketHandler.PingClients), null, 0, 2000);
            HandleMovement  = new Timer(new TimerCallback(Players.HandleMovement), null, 0, 10);
            SyncPlayers     = new Timer(new TimerCallback(Players.SyncPlayers), null, 0, 500);
            MOTD            = new Timer(new TimerCallback(Players.SendMOTD), null, 0, 1000);    // The excuse for this is that with the way the client loads and the sockets being a seperate thread an MOTD at MapOK does not always arrive after the UI loaded, but sometimes before. Eep.
            SavePlayers     = new Timer(new TimerCallback(Players.SavePlayers), null, 0, 300000);

            // Open the server to players!
            Logger.Write("Opening server...");
            Server.Open();
            Logger.Write("--- Normal Operations Below ---");

            // Keep the console open for as long as the program can run.
            while (Program.Running) {
                var input = Console.ReadLine().Trim().ToLower();
                if (input.Length > 0) Input.Process(input);
            }

            // Close down the server! We no longer need it.
            Server.Close();

            // Time to get rid of everything! .. Or well, save it to prevent data loss.
            Data.SavePlayers();
            Data.SaveClasses();
            Data.SaveMaps();
        }
    }
}
