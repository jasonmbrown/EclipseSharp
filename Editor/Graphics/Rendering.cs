using Editor.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Editor.Graphics {
    public static class Rendering {

        public static Dictionary<Int32, TexData> Tileset = new Dictionary<Int32, TexData>();

        public static void InitGraphics() {
            InitTilesets();
        }

        private static void InitTilesets() {
            var id = 0;
            var done = false;
            while (!done) {
                id++;
                if (File.Exists(String.Format("{0}data files\\tilesets\\{1}.png", Data.AppPath, id))) {
                    var texdata = new TexData();
                    texdata.File = String.Format("{0}data files\\tilesets\\{1}.png", Data.AppPath, id);
                    Rendering.Tileset.Add(id, texdata);
                } else {
                    done = true;
                }
            }
        }
    }

    public class TexData {
        #region Declarations
        public String File { get; set; }
        public DateTime LastUse { get; set; }
        #endregion

        #region Constructors
        public TexData() {
            this.File = String.Empty;
            this.LastUse = DateTime.MinValue;
        }
        #endregion
    }
}
