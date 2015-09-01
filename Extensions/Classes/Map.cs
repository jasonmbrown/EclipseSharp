﻿using System;
using System.Collections.Generic;
using Extensions;

namespace Extensions.Database {
    public class Map {

        #region Declarations
        public String Name      { get; set; }
        public String Music     { get; set; }
        public Int32 Revision   { get; set; }
        public Int32[] Links    = new Int32[(Int32)Enumerations.Direction.Direction_Count - 1];
        public Int32 SizeX      { get; set; }
        public Int32 SizeY      { get; set; }
        public List<LayerData> Layers = new List<LayerData>();
        #endregion

        #region Constructors
        public Map() {
            this.Name           = "New Map";
            this.Music          = String.Empty;
            this.SizeX          = 30;
            this.SizeY          = 24;
            this.Revision       = 1;
            var l1 = new LayerData(this.SizeX, this.SizeY);
            l1.Name = "Ground";
            l1.BelowPlayer = true;
            var l2 = new LayerData(this.SizeX, this.SizeY);
            l2.Name = "Mask";
            l2.BelowPlayer = true;
            var l3 = new LayerData(this.SizeX, this.SizeY);
            l3.Name = "Fringe";
            l2.BelowPlayer = false;
            this.Layers.Add(l1);
            this.Layers.Add(l2);
            this.Layers.Add(l3);
        }
        #endregion
    }

    public class LayerData {
        #region Declarations
        public String Name { get; set; }
        public Boolean BelowPlayer { get; set; }
        public TileData[,] Tiles = new TileData[0,0];
        #endregion

        #region Constructors
        public LayerData(Int32 sizex, Int32 sizey) {
            this.Name = String.Empty;
            this.BelowPlayer = true;
            this.Tiles = new TileData[sizex, sizey];
            for (var x = 0; x < sizex; x++) {
                for (var y = 0; y < sizey; y++) {
                    this.Tiles[x, y] = new TileData();
                }
            }
        }
        #endregion
    }

    public class TileData {

        #region Declarations
        public Int32 Tileset    { get; set; }
        public Int32 TileX      { get; set; }
        public Int32 TileY      { get; set; }
        #endregion

        #region Constructors
        public TileData() {
            this.TileX      = 0;
            this.TileY      = 0;
            this.Tileset    = 0;
        }
        #endregion
    }
}
