using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Shortcut
{
    public partial class ShortcutForm : Form
    {
        private TreeView treeViewFiles;
        private ImageList imageList;
        private Button btnAddFile;
        private Button btnRemoveItem;
        private Button btnOpenFile;
        private Button btnCreateFolder;
        private Button btnRenameItem;
        private ContextMenuStrip contextMenu;

        public ShortcutForm()
        {
            InitializeComponent();
            InitializeImageList();
            InitializeFileTree();
            SetupContextMenu();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortcutForm));
            this.treeViewFiles = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnAddFile = new System.Windows.Forms.Button();
            this.btnRemoveItem = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnCreateFolder = new System.Windows.Forms.Button();
            this.btnRenameItem = new System.Windows.Forms.Button();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewFiles.ForeColor = System.Drawing.SystemColors.WindowText;
            this.treeViewFiles.HideSelection = false;
            this.treeViewFiles.HotTracking = true;
            this.treeViewFiles.ImageIndex = 0;
            this.treeViewFiles.ImageList = this.imageList;
            this.treeViewFiles.Indent = 19;
            this.treeViewFiles.ItemHeight = 18;
            this.treeViewFiles.Location = new System.Drawing.Point(12, 12);
            this.treeViewFiles.Name = "treeViewFiles";
            this.treeViewFiles.SelectedImageIndex = 0;
            this.treeViewFiles.ShowNodeToolTips = true;
            this.treeViewFiles.Size = new System.Drawing.Size(578, 500);
            this.treeViewFiles.TabIndex = 1;
            this.treeViewFiles.DoubleClick += new System.EventHandler(this.treeViewFiles_DoubleClick);
            this.treeViewFiles.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewFiles_NodeMouseClick);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnAddFile
            // 
            this.btnAddFile.Location = new System.Drawing.Point(130, 530);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(100, 30);
            this.btnAddFile.TabIndex = 3;
            this.btnAddFile.Text = "เพิ่มไฟล์";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnRemoveItem
            // 
            this.btnRemoveItem.Location = new System.Drawing.Point(370, 530);
            this.btnRemoveItem.Name = "btnRemoveItem";
            this.btnRemoveItem.Size = new System.Drawing.Size(100, 30);
            this.btnRemoveItem.TabIndex = 5;
            this.btnRemoveItem.Text = "ลบ";
            this.btnRemoveItem.UseVisualStyleBackColor = true;
            this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(490, 530);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(100, 30);
            this.btnOpenFile.TabIndex = 6;
            this.btnOpenFile.Text = "เปิดไฟล์";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnCreateFolder
            // 
            this.btnCreateFolder.Location = new System.Drawing.Point(12, 530);
            this.btnCreateFolder.Name = "btnCreateFolder";
            this.btnCreateFolder.Size = new System.Drawing.Size(100, 30);
            this.btnCreateFolder.TabIndex = 2;
            this.btnCreateFolder.Text = "สร้างโฟลเดอร์";
            this.btnCreateFolder.UseVisualStyleBackColor = true;
            this.btnCreateFolder.Click += new System.EventHandler(this.btnCreateFolder_Click);
            // 
            // btnRenameItem
            // 
            this.btnRenameItem.Location = new System.Drawing.Point(250, 530);
            this.btnRenameItem.Name = "btnRenameItem";
            this.btnRenameItem.Size = new System.Drawing.Size(100, 30);
            this.btnRenameItem.TabIndex = 4;
            this.btnRenameItem.Text = "เปลี่ยนชื่อ";
            this.btnRenameItem.UseVisualStyleBackColor = true;
            this.btnRenameItem.Click += new System.EventHandler(this.btnRenameItem_Click);
            // 
            // contextMenu
            // 
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // ShortcutForm
            // 
            this.ClientSize = new System.Drawing.Size(606, 561);
            this.Controls.Add(this.treeViewFiles);
            this.Controls.Add(this.btnCreateFolder);
            this.Controls.Add(this.btnAddFile);
            this.Controls.Add(this.btnRenameItem);
            this.Controls.Add(this.btnRemoveItem);
            this.Controls.Add(this.btnOpenFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShortcutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shortcut Manager - Tree View";
            this.ResumeLayout(false);

        }

        private void InitializeImageList()
        {
            // Add folder icons to ImageList
            Icon folderIcon = GetFolderIcon();
            Icon openFolderIcon = GetOpenFolderIcon();

            if (folderIcon != null)
            {
                this.imageList.Images.Add("folder", folderIcon);
            }

            if (openFolderIcon != null)
            {
                this.imageList.Images.Add("folder_open", openFolderIcon);
            }
            else if (folderIcon != null)
            {
                // Use same icon if open folder icon not available
                this.imageList.Images.Add("folder_open", folderIcon);
            }
        }

        private void InitializeFileTree()
        {
            // Create root nodes with Windows-style icons
            TreeNode documentsNode = new TreeNode("เอกสาร");
            documentsNode.Tag = new FolderItem("เอกสาร");
            documentsNode.ImageKey = "folder";
            documentsNode.SelectedImageKey = "folder_open";
            treeViewFiles.Nodes.Add(documentsNode);

            TreeNode applicationsNode = new TreeNode("โปรแกรม");
            applicationsNode.Tag = new FolderItem("โปรแกรม");
            applicationsNode.ImageKey = "folder";
            applicationsNode.SelectedImageKey = "folder_open";
            treeViewFiles.Nodes.Add(applicationsNode);

            TreeNode mediaNode = new TreeNode("สื่อ");
            mediaNode.Tag = new FolderItem("สื่อ");
            mediaNode.ImageKey = "folder";
            mediaNode.SelectedImageKey = "folder_open";
            treeViewFiles.Nodes.Add(mediaNode);

            // Add event handlers for folder expand/collapse
            treeViewFiles.BeforeExpand += new TreeViewCancelEventHandler(treeViewFiles_BeforeExpand);
            treeViewFiles.BeforeCollapse += new TreeViewCancelEventHandler(treeViewFiles_BeforeCollapse);

            // Don't expand all initially - let user expand as needed
        }

        private void SetupContextMenu()
        {
            ToolStripMenuItem createFolderItem = new ToolStripMenuItem("สร้างโฟลเดอร์");
            createFolderItem.Click += new EventHandler(this.btnCreateFolder_Click);

            ToolStripMenuItem addFileItem = new ToolStripMenuItem("เพิ่มไฟล์");
            addFileItem.Click += new EventHandler(this.btnAddFile_Click);

            ToolStripMenuItem renameItem = new ToolStripMenuItem("เปลี่ยนชื่อ");
            renameItem.Click += new EventHandler(this.btnRenameItem_Click);

            ToolStripMenuItem deleteItem = new ToolStripMenuItem("ลบ");
            deleteItem.Click += new EventHandler(this.btnRemoveItem_Click);

            ToolStripMenuItem openItem = new ToolStripMenuItem("เปิด");
            openItem.Click += new EventHandler(this.btnOpenFile_Click);

            contextMenu.Items.Add(createFolderItem);
            contextMenu.Items.Add(addFileItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(deleteItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(openItem);

            treeViewFiles.ContextMenuStrip = contextMenu;
        }

        private void btnCreateFolder_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            TreeNode parentNode = null;

            // Determine parent node
            if (selectedNode == null)
            {
                // No selection, add to root
                parentNode = null;
            }
            else if (selectedNode.Tag is FolderItem)
            {
                // Selected node is a folder, add as child
                parentNode = selectedNode;
            }
            else
            {
                // Selected node is a file, add as sibling
                parentNode = selectedNode.Parent;
            }

            string folderName = "โฟลเดอร์ใหม่";
            string inputName = ShowInputDialog("สร้างโฟลเดอร์ใหม่", "ชื่อโฟลเดอร์:", folderName);

            if (!string.IsNullOrEmpty(inputName))
            {
                TreeNode newFolderNode = new TreeNode(inputName);
                newFolderNode.Tag = new FolderItem(inputName);
                newFolderNode.ImageKey = "folder";
                newFolderNode.SelectedImageKey = "folder_open";

                if (parentNode != null)
                {
                    parentNode.Nodes.Add(newFolderNode);
                    parentNode.Expand();
                }
                else
                {
                    treeViewFiles.Nodes.Add(newFolderNode);
                }

                treeViewFiles.SelectedNode = newFolderNode;
            }
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            TreeNode parentNode = null;

            // Determine parent node
            if (selectedNode == null)
            {
                // No selection, show message
                MessageBox.Show("กรุณาเลือกโฟลเดอร์ที่ต้องการเพิ่มไฟล์", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (selectedNode.Tag is FolderItem)
            {
                // Selected node is a folder, add as child
                parentNode = selectedNode;
            }
            else
            {
                // Selected node is a file, add as sibling
                parentNode = selectedNode.Parent;
            }

            if (parentNode == null)
            {
                MessageBox.Show("กรุณาเลือกโฟลเดอร์ที่ต้องการเพิ่มไฟล์", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    AddFileToNode(parentNode, fileName);
                }
                parentNode.Expand();
            }
        }

        private void btnRenameItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการเปลี่ยนชื่อ", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string currentName = selectedNode.Text;
            string newName = ShowInputDialog("เปลี่ยนชื่อ", "ชื่อใหม่:", currentName);

            if (!string.IsNullOrEmpty(newName) && newName != currentName)
            {
                selectedNode.Text = newName;

                if (selectedNode.Tag is FolderItem)
                {
                    FolderItem folderItem = (FolderItem)selectedNode.Tag;
                    folderItem.FolderName = newName;
                }
                else if (selectedNode.Tag is FileItem)
                {
                    FileItem fileItem = (FileItem)selectedNode.Tag;
                    fileItem.DisplayName = newName;
                }
            }
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการลบ", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string itemType = (selectedNode.Tag is FolderItem) ? "โฟลเดอร์" : "ไฟล์";
            DialogResult result = MessageBox.Show(
                string.Format("คุณต้องการลบ{0} '{1}' หรือไม่?", itemType, selectedNode.Text),
                "ยืนยันการลบ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                selectedNode.Remove();
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null || selectedNode.Tag is FolderItem)
            {
                MessageBox.Show("กรุณาเลือกไฟล์ที่ต้องการเปิด", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FileItem fileItem = (FileItem)selectedNode.Tag;
            OpenFile(fileItem.FilePath);
        }

        private void treeViewFiles_DoubleClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is FileItem)
            {
                FileItem fileItem = (FileItem)selectedNode.Tag;
                OpenFile(fileItem.FilePath);
            }
        }

        private void treeViewFiles_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewFiles.SelectedNode = e.Node;
            }
        }

        private void treeViewFiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // Change folder icon to open folder when expanding
            if (e.Node.Tag is FolderItem)
            {
                e.Node.ImageKey = "folder_open";
            }
        }

        private void treeViewFiles_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            // Change folder icon to closed folder when collapsing
            if (e.Node.Tag is FolderItem)
            {
                e.Node.ImageKey = "folder";
            }
        }

        private void AddFileToNode(TreeNode parentNode, string filePath)
        {
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                FileItem item = new FileItem();
                item.FilePath = filePath;
                item.FileName = fileInfo.Name;
                item.DisplayName = fileInfo.Name;
                item.FileSize = fileInfo.Length;
                item.FileType = GetFileType(filePath);

                // Get icon and add to ImageList
                Icon icon = GetFileIcon(filePath);
                string imageKey = "file_" + imageList.Images.Count.ToString();
                if (icon != null)
                {
                    imageList.Images.Add(imageKey, icon);
                }

                TreeNode fileNode = new TreeNode(item.DisplayName);
                fileNode.Tag = item;
                fileNode.ImageKey = imageKey;
                fileNode.SelectedImageKey = imageKey;
                fileNode.ToolTipText = string.Format("{0}\nประเภท: {1}\nขนาด: {2}\nเส้นทาง: {3}",
                    item.FileName, item.FileType, FormatFileSize(item.FileSize), item.FilePath);

                parentNode.Nodes.Add(fileNode);
            }
        }

        private string GetFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension);
                if (key != null)
                {
                    string fileType = key.GetValue("").ToString();
                    key.Close();

                    RegistryKey typeKey = Registry.ClassesRoot.OpenSubKey(fileType);
                    if (typeKey != null)
                    {
                        string description = typeKey.GetValue("").ToString();
                        typeKey.Close();
                        return description;
                    }
                }
            }
            catch
            {
                // Fallback to extension
            }

            return extension.ToUpper() + " File";
        }

        private Icon GetFileIcon(string filePath)
        {
            try
            {
                // Use Shell32.dll to get file icon
                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hImgSmall = SHGetFileInfo(filePath, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON | SHGFI_SMALLICON);

                if (shinfo.hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(shinfo.hIcon);
                    Icon clonedIcon = (Icon)icon.Clone();
                    DestroyIcon(shinfo.hIcon); // Clean up
                    return clonedIcon;
                }
            }
            catch
            {
                // Return null if failed
            }

            return null;
        }

        private Icon GetFolderIcon()
        {
            try
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hImg = SHGetFileInfo("", FILE_ATTRIBUTE_DIRECTORY, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);

                if (shinfo.hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(shinfo.hIcon);
                    Icon clonedIcon = (Icon)icon.Clone();
                    DestroyIcon(shinfo.hIcon);
                    return clonedIcon;
                }
            }
            catch
            {
                // Return null if failed
            }

            return null;
        }

        private Icon GetOpenFolderIcon()
        {
            try
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hImg = SHGetFileInfo("", FILE_ATTRIBUTE_DIRECTORY, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES | SHGFI_OPENICON);

                if (shinfo.hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(shinfo.hIcon);
                    Icon clonedIcon = (Icon)icon.Clone();
                    DestroyIcon(shinfo.hIcon);
                    return clonedIcon;
                }
            }
            catch
            {
                // Return null if failed
            }

            return null;
        }

        private void OpenFile(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถเปิดไฟล์ได้: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }

        private string ShowInputDialog(string title, string prompt, string defaultValue)
        {
            Form inputForm = new Form();
            inputForm.Text = title;
            inputForm.Size = new Size(350, 150);
            inputForm.StartPosition = FormStartPosition.CenterParent;
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;

            Label lblPrompt = new Label();
            lblPrompt.Text = prompt;
            lblPrompt.Location = new Point(10, 20);
            lblPrompt.Size = new Size(300, 20);

            TextBox txtInput = new TextBox();
            txtInput.Text = defaultValue;
            txtInput.Location = new Point(10, 45);
            txtInput.Size = new Size(300, 20);
            txtInput.SelectAll();

            Button btnOK = new Button();
            btnOK.Text = "ตกลง";
            btnOK.Location = new Point(155, 75);
            btnOK.Size = new Size(75, 25);
            btnOK.DialogResult = DialogResult.OK;

            Button btnCancel = new Button();
            btnCancel.Text = "ยกเลิก";
            btnCancel.Location = new Point(235, 75);
            btnCancel.Size = new Size(75, 25);
            btnCancel.DialogResult = DialogResult.Cancel;

            inputForm.Controls.Add(lblPrompt);
            inputForm.Controls.Add(txtInput);
            inputForm.Controls.Add(btnOK);
            inputForm.Controls.Add(btnCancel);

            inputForm.AcceptButton = btnOK;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                return txtInput.Text;
            }

            return string.Empty;
        }

        // Win32 API declarations
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath,
            uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        // Constants
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint SHGFI_OPENICON = 0x2;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        // Structure
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
    }

    // Helper classes for tree items
    public class FileItem
    {
        private string filePath;
        private string fileName;
        private string displayName;
        private long fileSize;
        private string fileType;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        public string FileType
        {
            get { return fileType; }
            set { fileType = value; }
        }
    }

}