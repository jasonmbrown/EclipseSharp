using Extensions.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Database {

    public static class Data {
        #region Declarations
        //public static Dictionary<String, Object>    Temp       = new Dictionary<String, Object>();
        public static Map                           Map         = new Map();
        public static Dictionary<Int32, Class>      Classes     = new Dictionary<Int32, Class>();
        public static Dictionary<Int32, Player>     Players     = new Dictionary<Int32, Player>();
        public static Settings                      Settings    = new Settings();
        public static String                        AppPath;
        public static Int32                         MyId;
        public static Boolean                       InGame = false;
        #endregion

        #region Methods
        public static void CheckDirectories() {
            if (!Directory.Exists(Data.AppPath + "data files\\tilesets")) Directory.CreateDirectory(Data.AppPath + "data files\\tilesets");
            if (!Directory.Exists(Data.AppPath + "data files\\sprites")) Directory.CreateDirectory(Data.AppPath + "data files\\sprites");
            if (!Directory.Exists(Data.AppPath + "data files\\interface")) Directory.CreateDirectory(Data.AppPath + "data files\\interface");
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
        public static void LoadMap(Int32 id) {
            var filename = String.Format("{0}data files\\maps\\{1}.dat", Data.AppPath, id);

            // load our data.
            if (File.Exists(filename)) {
                // Destroy our existing list of layers.
                Data.Map.Layers.Clear();

                using (var fs = File.OpenRead(filename)) {
                    using (var re = new BinaryReader(fs)) {
                        Data.Map.Name = re.ReadString();
                        Data.Map.Music = re.ReadString();
                        Data.Map.Revision = re.ReadInt32();
                        Data.Map.SizeX = re.ReadInt32();
                        Data.Map.SizeY = re.ReadInt32();

                        var layers = re.ReadInt32();

                        for (var l = 0; l < layers; l++) {
                            Data.Map.Layers.Add(new LayerData(Data.Map.SizeX, Data.Map.SizeY));
                            Data.Map.Layers[l].Name = re.ReadString();
                            Data.Map.Layers[l].BelowPlayer = re.ReadBoolean();
                            for (var x = 0; x < Data.Map.SizeX; x++) {
                                for (var y = 0; y < Data.Map.SizeY; y++) {
                                    Data.Map.Layers[l].Tiles[x, y].Tileset = re.ReadInt32();
                                    Data.Map.Layers[l].Tiles[x, y].Tile = re.ReadInt32();
                                }
                            }
                        }
                    }
                }
            } else {
                Data.Map.Revision = 0;
            }
        }
            #endregion
    }
}
