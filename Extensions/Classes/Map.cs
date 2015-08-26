using System;
using System.Collections.Generic;

namespace Extensions.Database {
    public class Map {

        #region Declarations
        public String Name      { get; set; }
        public String Music     { get; set; }
        public Int32 Revision   { get; set; }
        public Int32[] Links    = new Int32[(Int32)Enumerations.Direction.Direction_Count - 1];
        public Int32 SizeX      { get; }
        public Int32 SizeY      { get; }
        public List<TileData[][]> Layers = new List<TileData[][]>();
        #endregion

        #region Constructors
        public Map() {
            this.Name           = "New Map";
            this.Music          = String.Empty;
            this.SizeX          = 30;
            this.SizeY          = 24;
            this.Revision       = 1;
            var t = new TileData[this.SizeX][];
            for (var x = 0; x < this.SizeX; x++) {
                t[x] = new TileData[this.SizeY];
                for (var y = 0; y < this.SizeY; y++) {
                    t[x][y] = new TileData();
                }
            }
            // Add three layers by default.
            for (var i = 0; i < 3; i++) {
                Layers.Add(t);
            }
        }
        #endregion

    }

    public class TileData {

        #region Declarations
        public Int32 Tileset    { get; set; }
        public Int32 Tile       { get; set; }
        #endregion

        #region Constructors
        public TileData() {
            this.Tile       = -1;
            this.Tileset    = 0;
        }
        #endregion
    }
}
