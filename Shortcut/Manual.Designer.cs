namespace Shortcut
{
    partial class Manual
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manual));
            this.treeViewFiles = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.addFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.renameFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeImageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewFiles.ContextMenuStrip = this.contextMenuStrip1;
            this.treeViewFiles.ImageIndex = 0;
            this.treeViewFiles.ImageList = this.nodeImageList;
            this.treeViewFiles.Location = new System.Drawing.Point(12, 12);
            this.treeViewFiles.Name = "treeViewFiles";
            this.treeViewFiles.SelectedImageIndex = 0;
            this.treeViewFiles.Size = new System.Drawing.Size(238, 303);
            this.treeViewFiles.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFolder,
            this.toolStripSeparator3,
            this.addFolder,
            this.addFile,
            this.toolStripSeparator1,
            this.renameFolder,
            this.deleteItem,
            this.toolStripSeparator2,
            this.toolStripMenuItem4});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(143, 154);
            // 
            // createFolder
            // 
            this.createFolder.Name = "createFolder";
            this.createFolder.Size = new System.Drawing.Size(142, 22);
            this.createFolder.Text = "Create folder";
            this.createFolder.Click += new System.EventHandler(this.createFolder_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(139, 6);
            // 
            // addFolder
            // 
            this.addFolder.Name = "addFolder";
            this.addFolder.Size = new System.Drawing.Size(142, 22);
            this.addFolder.Text = "Add Folder";
            this.addFolder.Click += new System.EventHandler(this.addFolder_Click);
            // 
            // addFile
            // 
            this.addFile.Name = "addFile";
            this.addFile.Size = new System.Drawing.Size(142, 22);
            this.addFile.Text = "Add File";
            this.addFile.Click += new System.EventHandler(this.addFile_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // renameFolder
            // 
            this.renameFolder.Name = "renameFolder";
            this.renameFolder.Size = new System.Drawing.Size(142, 22);
            this.renameFolder.Text = "Rename";
            this.renameFolder.Click += new System.EventHandler(this.renameFolder_Click);
            // 
            // deleteItem
            // 
            this.deleteItem.Name = "deleteItem";
            this.deleteItem.Size = new System.Drawing.Size(142, 22);
            this.deleteItem.Text = "Delete";
            this.deleteItem.Click += new System.EventHandler(this.deleteItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(139, 6);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem4.Text = "Open";
            // 
            // nodeImageList
            // 
            this.nodeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("nodeImageList.ImageStream")));
            this.nodeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.nodeImageList.Images.SetKeyName(0, "folder2.png");
            this.nodeImageList.Images.SetKeyName(1, "root.png");
            this.nodeImageList.Images.SetKeyName(2, "open-folder.png");
            this.nodeImageList.Images.SetKeyName(3, "shortcut.png");
            this.nodeImageList.Images.SetKeyName(4, "file.png");
            // 
            // Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 327);
            this.Controls.Add(this.treeViewFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Manual";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Manual";
            this.Load += new System.EventHandler(this.Manual_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Manual_MouseClick);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Manual_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewFiles;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addFolder;
        private System.Windows.Forms.ToolStripMenuItem addFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem renameFolder;
        private System.Windows.Forms.ToolStripMenuItem deleteItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ImageList nodeImageList;
        private System.Windows.Forms.ToolStripMenuItem createFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}