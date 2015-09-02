using System;
using System.IO;

namespace Editor.Database {
    public static class Data {

        #region Declarations
        public static Settings  Settings = new Settings();
        public static String    AppPath;
        public static Int32     MyId;
        #endregion

        #region Methods
        public static void CheckDirectories() {
            if (!Directory.Exists(Data.AppPath + "data files\\tilesets")) Directory.CreateDirectory(Data.AppPath + "data files\\tilesets");
            if (!Directory.Exists(Data.AppPath + "data files\\sprites")) Directory.CreateDirectory(Data.AppPath + "data files\\sprites");
            if (!Directory.Exists(Data.AppPath + "data files\\sounds")) Directory.CreateDirectory(Data.AppPath + "data files\\sounds");
            if (!Directory.Exists(Data.AppPath + "data files\\maps")) Directory.CreateDirectory(Data.AppPath + "data files\\maps");
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
                Data.Settings.IPAddress = "127.0.0.1";
                Data.Settings.Port = 7001;
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
