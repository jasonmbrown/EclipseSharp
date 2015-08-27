using System;
using Extensions.Database;
using System.Collections.Generic;
using System.IO;
using Server.Logic;

namespace Server.Database {
    static class Data {

        #region Declarations
        public static Dictionary<Int32, Player>     Players     = new Dictionary<Int32, Player>();
        public static Dictionary<Int32, TempPlayer> TempPlayers = new Dictionary<Int32, TempPlayer>();
        public static Dictionary<Int32, Class>      Classes     = new Dictionary<Int32, Class>();
        public static Dictionary<Int32, Map>        Map         = new Dictionary<Int32, Map>();
        public static List<String>                  Characters  = new List<String>();
        public static Settings                      Settings    = new Settings();
        public static String                        AppPath;
        #endregion

        #region Methods
        public static void LoadSettings(string filename) {
            // load our data.
            if (File.Exists(filename)) {
                var ser = new System.Xml.Serialization.XmlSerializer(Data.Settings.GetType());
                using (var fs = File.OpenRead(filename)) {
                    Data.Settings = (Settings)ser.Deserialize(fs);
                }
            } else {
                Data.Settings.GameName          = "EclipseSharp";
                Data.Settings.Port              = 7001;
                Data.Settings.MaxPlayers        = 100;
                Data.Settings.MOTD              = "Welcome online.";
                Data.Settings.MinUsernameChar   = 3;
                Data.Settings.MaxUsernameChar   = 20;
                Data.Settings.MinPasswordChar   = 3;
                Data.Settings.MaxPasswordChar   = 20;
                Data.Settings.MaxClasses        = 3;
                Data.Settings.MaxMaps           = 100;             
                Data.SaveSettings(filename);
            }
        }

        private static void SaveSettings(String filename) {
            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            // Serialize our object and throw it to a file!
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Settings.GetType());
            using (var fs = File.OpenWrite(filename)) {
                ser.Serialize(fs, Data.Settings);
            }
        }
        public static void InitData() {
            // This method loads our data into the server.

            // Load Character List
            Logger.Write("Loading character list...");
            Data.LoadCharacterList();

            // Load Classes
            Logger.Write(String.Format("Loading {0} Classes...", Data.Settings.MaxClasses));
            for (var i = 1; i <= Data.Settings.MaxClasses; i++) {
                Data.LoadClass(i);
            }

            // Load Maps
            Logger.Write(String.Format("Loading {0} Maps...", Data.Settings.MaxMaps));
            for (var i = 1; i <= Data.Settings.MaxMaps; i++) {
                Data.LoadMap(i);
            }

        }
        public static void CheckDirectories() {
            if (!Directory.Exists(Data.AppPath + "data files\\accounts")) Directory.CreateDirectory(Data.AppPath + "data files\\accounts");
            if (!Directory.Exists(Data.AppPath + "data files\\classes")) Directory.CreateDirectory(Data.AppPath + "data files\\classes");
            if (!Directory.Exists(Data.AppPath + "data files\\maps")) Directory.CreateDirectory(Data.AppPath + "data files\\maps");
        }
        public static void SaveClasses() {
            for (var i = 0; i < Data.Settings.MaxClasses; i++) {
                Data.SaveClass(i);
            }
        }
        public static void SaveMaps() {
            for (var i = 0; i < Data.Settings.MaxMaps; i++) {
                Data.SaveMap(i);
            }
        }
        public static void SaveClass(Int32 id) {
            var filename = String.Format("{0}data files\\classes\\{1}.xml", Data.AppPath, id);

            // Make sure we don't try to save a non-existant class.
            if (!Data.Classes.ContainsKey(id)) return;

            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            // Serialize our object and throw it to a file!
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Classes[id].GetType());
            using (var fs = File.OpenWrite(filename)) {
                ser.Serialize(fs, Data.Classes[id]);
            }
            Logger.Write(String.Format("Saved Class {0}.", id));
        }
        public static void LoadClass(Int32 id) {
            var filename = String.Format("{0}data files\\classes\\{1}.xml", Data.AppPath, id);
            // Make sure we created this class before moving on.
            if (!Data.Classes.ContainsKey(id)) {
                var c = new Class();
                Data.Classes.Add(id, c);
            }

            // load our data.
            if (File.Exists(filename)) {
                var ser = new System.Xml.Serialization.XmlSerializer(Data.Classes[id].GetType());
                using (var fs = File.OpenRead(filename)) {
                    Data.Classes[id] = (Class)ser.Deserialize(fs);
                }
                if (Data.Classes[id].Name.Length > 0) Logger.Write(String.Format("Loaded Class: {0}", Data.Classes[id].Name));
            } else {
                Data.SaveClass(id);
            }
        }
        public static void SavePlayers() {
            foreach (var player in Data.Players) {
                Data.SavePlayer(player.Key);
            }
        }
        public static void SavePlayer(Int32 id) {
            var filename = String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, Data.Players[id].Username.ToLower());

            // Make sure we don't try to save a non-existant player.
            if (!Data.Players.ContainsKey(id)) return;

            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            // Serialize our object and throw it to a file!
            var ser = new System.Xml.Serialization.XmlSerializer(new Player().GetType());
            using (var fs = File.OpenWrite(filename)) {
                ser.Serialize(fs, Data.Players[id]);
            }
        }
        public static void LoadPlayer(Int32 id, String name) {
            var filename = String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, Data.Players[id].Username.ToLower());

            // load our data.
            var ser = new System.Xml.Serialization.XmlSerializer(new Player().GetType());
            using (var fs = File.OpenRead(String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, name))) {
                Data.Players[id] = (Player)ser.Deserialize(fs);
            }
        }
        public static void SaveCharacterList() {
            var filename = String.Format("{0}data files\\accounts\\charlist.xml", Data.AppPath);

            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            // Serialize our object and throw it to a file!
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Characters.GetType());
            using (var fs = File.OpenWrite(filename)) {
                ser.Serialize(fs, Data.Characters);
            }
        }
        public static void LoadCharacterList() {
            var filename = String.Format("{0}data files\\accounts\\charlist.xml", Data.AppPath);

            // No point in loading non-existant data.
            if (!File.Exists(filename)) return;

            // load our data.
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Characters.GetType());
            using (var fs = File.OpenRead(filename)) {
                Data.Characters = (List<String>)ser.Deserialize(fs);
            }
        }
        public static void SaveMap(Int32 id) {
            var filename = String.Format("{0}data files\\maps\\{1}.dat", Data.AppPath, id);

            // Make sure we don't try to save a non-existant map.
            if (!Data.Map.ContainsKey(id)) return;

            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            using (var fs = File.OpenWrite(filename)) {
                using (var wr = new BinaryWriter(fs)) {
                    wr.Write(Data.Map[id].Name);
                    wr.Write(Data.Map[id].Music);
                    wr.Write(Data.Map[id].Revision);
                    wr.Write(Data.Map[id].SizeX);
                    wr.Write(Data.Map[id].SizeY);

                    wr.Write(Data.Map[id].Layers.Count);

                    foreach (var l in Data.Map[id].Layers) {
                        wr.Write(l.Name);
                        wr.Write(l.BelowPlayer);
                        for (var x = 0; x < Data.Map[id].SizeX; x++) {
                            for (var y = 0; y < Data.Map[id].SizeY; y++) {
                                wr.Write(l.Tiles[Data.Map[id].Translate(x, y)].Tileset);
                                wr.Write(l.Tiles[l.Translate(x, y)].Tile);
                            }
                        }
                    }
                }
            }
            Logger.Write(String.Format("Saved Map {0}.", id));
        }
        public static void LoadMap(Int32 id) {
            var filename = String.Format("{0}data files\\maps\\{1}.dat", Data.AppPath, id);
            // Make sure we created this class before moving on.
            if (!Data.Map.ContainsKey(id)) {
                var c = new Map();
                Data.Map.Add(id, c);
            }

            // load our data.
            if (File.Exists(filename)) {
                // Destroy our existing list of layers.
                Data.Map[id].Layers.Clear();

                using (var fs = File.OpenRead(filename)) {
                    using (var re = new BinaryReader(fs)) {
                        Data.Map[id].Name = re.ReadString();
                        Data.Map[id].Music = re.ReadString();
                        Data.Map[id].Revision = re.ReadInt32();
                        Data.Map[id].SizeX = re.ReadInt32();
                        Data.Map[id].SizeY = re.ReadInt32();

                        var layers = re.ReadInt32();

                        for (var l = 0; l < layers; l++) {
                            Data.Map[id].Layers.Add(new LayerData(Data.Map[id].SizeX, Data.Map[id].SizeY));
                            Data.Map[id].Layers[l].Name = re.ReadString();
                            Data.Map[id].Layers[l].BelowPlayer = re.ReadBoolean();
                            for (var x = 0; x < Data.Map[id].SizeX; x++) {
                                for (var y = 0; y < Data.Map[id].SizeY; y++) {
                                    Data.Map[id].Layers[l].Tiles[Data.Map[id].Translate(x, y)].Tileset = re.ReadInt32();
                                    Data.Map[id].Layers[l].Tiles[Data.Map[id].Translate(x, y)].Tile = re.ReadInt32();
                                }
                            }
                        }
                    }
                }

                if (Data.Map[id].Name.Length > 0) Logger.Write(String.Format("Loaded Map: {0}", Data.Map[id].Name));

            } else {
                Data.SaveMap(id);
            }
        }
        #endregion

    }
}
