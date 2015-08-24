using System;
using Extensions;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using Server.Logic;

namespace Server.Database {
    static class Data {

        #region Declarations
        public static Dictionary<Int32, Player>     Players     = new Dictionary<Int32, Player>();
        public static Dictionary<Int32, TempPlayer> TempPlayers = new Dictionary<Int32, TempPlayer>();
        public static Dictionary<Int32, Class>      Classes     = new Dictionary<Int32, Class>();
        public static List<String>                  Characters  = new List<String>();
        public static Settings                      Settings    = new Settings();
        public static String                        AppPath;
        #endregion

        #region Methods
        public static void LoadSettings(String filename) {
            // Load up our XML file and parse the settings!
            var xml = XElement.Load(filename);
            var dic = new Dictionary<String, String>();
            foreach (var setting in xml.Elements()) {
                dic.Add(setting.Attribute("key").Value, setting.Attribute("value").Value);
            }

            // Now apply them to the actual settings object.
            Settings.Port               = dic["port"].ToInt32();
            Settings.MaxPlayers         = dic["maxplayers"].ToInt32();

            Settings.GameName           = dic["gamename"];
            Settings.MOTD               = dic["motd"];
            Settings.Website            = dic["website"];

            Settings.MaxPasswordChar    = dic["maxpasswordchar"].ToInt32();
            Settings.MinPasswordChar    = dic["minpasswordchar"].ToInt32();
            Settings.MaxUsernameChar    = dic["maxusernamechar"].ToInt32();
            Settings.MinUsernameChar    = dic["minusernamechar"].ToInt32();
            Settings.StartMap           = dic["startmap"].ToInt32();
            Settings.StartX             = (Byte)dic["startx"].ToInt32();
            Settings.StartY             = (Byte)dic["starty"].ToInt32();

            Settings.MaxClasses         = dic["maxclasses"].ToInt32();
            Settings.MaxCharacters      = dic["maxcharacters"].ToInt32();
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
        }
        public static void CheckDirectories() {
            if (!Directory.Exists(Data.AppPath + "data files\\accounts")) Directory.CreateDirectory(Data.AppPath + "data files\\accounts");
            if (!Directory.Exists(Data.AppPath + "data files\\classes")) Directory.CreateDirectory(Data.AppPath + "data files\\classes");
        }
        public static void SaveClasses() {
            for (var i = 0; i < Data.Settings.MaxClasses; i++) {
                Data.SaveClass(i);
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
            var filename = String.Format("{0}data files\\accounts\\{1}.xml", Data.AppPath, Data.Players[id].Username);

            // Make sure we don't try to save a non-existant player.
            if (!Data.Players.ContainsKey(id)) return;

            // Delete a file should it already exist.
            if (File.Exists(filename)) File.Delete(filename);

            // Serialize our object and throw it to a file!
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Players[id].GetType());
            using (var fs = File.OpenWrite(filename)) {
                ser.Serialize(fs, Data.Players[id]);
            }
        }
        public static void LoadPlayer(Int32 id, String name) {

            // load our data.
            var ser = new System.Xml.Serialization.XmlSerializer(Data.Classes[id].GetType());
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
        #endregion

    }
}
