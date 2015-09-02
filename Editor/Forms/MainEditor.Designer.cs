namespace Editor {
    partial class MainEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.scrlTileset = new System.Windows.Forms.VScrollBar();
            this.cmbTilesets = new System.Windows.Forms.ComboBox();
            this.lstMaps = new System.Windows.Forms.ListBox();
            this.scrlMapVertical = new System.Windows.Forms.VScrollBar();
            this.scrlMapHorizontal = new System.Windows.Forms.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1025, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 587);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1025, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.scrlMapVertical);
            this.splitContainer1.Panel2.Controls.Add(this.scrlMapHorizontal);
            this.splitContainer1.Size = new System.Drawing.Size(1025, 562);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.scrlTileset);
            this.splitContainer2.Panel1.Controls.Add(this.cmbTilesets);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lstMaps);
            this.splitContainer2.Size = new System.Drawing.Size(259, 560);
            this.splitContainer2.SplitterDistance = 384;
            this.splitContainer2.TabIndex = 0;
            // 
            // scrlTileset
            // 
            this.scrlTileset.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrlTileset.Location = new System.Drawing.Point(238, 24);
            this.scrlTileset.Name = "scrlTileset";
            this.scrlTileset.Size = new System.Drawing.Size(21, 360);
            this.scrlTileset.TabIndex = 2;
            // 
            // cmbTilesets
            // 
            this.cmbTilesets.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTilesets.FormattingEnabled = true;
            this.cmbTilesets.Location = new System.Drawing.Point(0, 0);
            this.cmbTilesets.Name = "cmbTilesets";
            this.cmbTilesets.Size = new System.Drawing.Size(259, 24);
            this.cmbTilesets.TabIndex = 1;
            // 
            // lstMaps
            // 
            this.lstMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMaps.FormattingEnabled = true;
            this.lstMaps.ItemHeight = 16;
            this.lstMaps.Location = new System.Drawing.Point(0, 0);
            this.lstMaps.Name = "lstMaps";
            this.lstMaps.Size = new System.Drawing.Size(259, 172);
            this.lstMaps.TabIndex = 0;
            // 
            // scrlMapVertical
            // 
            this.scrlMapVertical.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrlMapVertical.Location = new System.Drawing.Point(737, 0);
            this.scrlMapVertical.Name = "scrlMapVertical";
            this.scrlMapVertical.Size = new System.Drawing.Size(21, 539);
            this.scrlMapVertical.TabIndex = 2;
            // 
            // scrlMapHorizontal
            // 
            this.scrlMapHorizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrlMapHorizontal.Location = new System.Drawing.Point(0, 539);
            this.scrlMapHorizontal.Name = "scrlMapHorizontal";
            this.scrlMapHorizontal.Size = new System.Drawing.Size(758, 21);
            this.scrlMapHorizontal.TabIndex = 1;
            // 
            // MainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 609);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainEditor_FormClosing);
            this.Load += new System.EventHandler(this.MainEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.VScrollBar scrlTileset;
        private System.Windows.Forms.ComboBox cmbTilesets;
        private System.Windows.Forms.ListBox lstMaps;
        private System.Windows.Forms.VScrollBar scrlMapVertical;
        private System.Windows.Forms.HScrollBar scrlMapHorizontal;
    }
}