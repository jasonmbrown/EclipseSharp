using Extensions.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Database {

    public static class Data {
        #region Declarations
        public static Dictionary<Int32, Class> Classes  = new Dictionary<Int32, Class>();
        public static Settings  Settings                = new Settings();
        public static String    AppPath;
        #endregion

        #region Methods
        public static void CheckDirectories() {
            if (!Directory.Exists(Data.AppPath + "data files\\tilesets")) Directory.CreateDirectory(Data.AppPath + "data files\\tilesets");
            if (!Directory.Exists(Data.AppPath + "data files\\sprites")) Directory.CreateDirectory(Data.AppPath + "data files\\sprites");
            if (!Directory.Exists(Data.AppPath + "data files\\interface")) Directory.CreateDirectory(Data.AppPath + "data files\\interface");
            if (!Directory.Exists(Data.AppPath + "data files\\sounds")) Directory.CreateDirectory(Data.AppPath + "data files\\sounds");
        }
        public static void LoadSettings() {
            var filename = String.Format("{0}data files\\settings.xml", Data.AppPath);

            // load our data.
            if (File.Exists(filename)) {
                var ser = new System.Xml.Serialization.XmlSerializer(Data.Settings.GetType());
                using (var fs = File.OpenRead(filename)) {
                    Data.Settings = (Settings)ser.Deserialize(fs);
                }
            } else {
                Data.Settings.GameName              = "EclipseSharp";
                Data.Settings.Graphics.Fullscreen   = false;
                Data.Settings.Graphics.ResolutionX  = 1280;
                Data.Settings.Graphics.ResolutionY  = 720;
                Data.Settings.Graphics.Theme        = "White.conf";
                Data.Settings.Network.IPAddress     = "127.0.0.1";
                Data.Settings.Network.Port          = 7001;
                Data.SaveSettings();
            }
        }

        private static void SaveSettings() {
            var filename = String.Format("{0}data files\\settings.xml", Data.AppPath);

            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            // Serialize our object and throw it to a file!
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Settings.GetType());
            using (var fs = File.OpenWrite(filename)) {
                ser.Serialize(fs, Data.Settings);
            }
        }
        #endregion
    }
}
