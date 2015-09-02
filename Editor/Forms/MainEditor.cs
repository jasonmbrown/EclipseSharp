using Editor.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Editor.Graphics;

namespace Editor {
    public partial class MainEditor : Form {

        public MainEditor() {
            InitializeComponent();
        }

        private void MainEditor_FormClosing(object sender, FormClosingEventArgs e) {
            Program.FrmLogin.BeginInvoke((Action)(()=> {
                Program.FrmLogin.Close();
            }));
        }

        private void MainEditor_Load(object sender, EventArgs e) {
            // Request our map list.
            Send.RequestMapList();

            // Add our tilesets.
            cmbTilesets.Items.Clear();
            for (var i = 0; i < Rendering.Tileset.Count; i++) {
                cmbTilesets.Items.Add(String.Format("Tileset {0}", i + 1));
            }
            cmbTilesets.SelectedIndex = 0;


        }

        public Int32 GetTileSet() {
            return cmbTilesets.SelectedIndex + 1;
        }
        public void ClearMaps() {
            lstMaps.Items.Clear();
        }
        public void AddMap(String name) {
            lstMaps.Items.Add(name);
        }
    }
}
