using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace Shortcut
{
    public partial class Manual : Form
    {
        private Utils utils;
        private const string TreeDataFileName = "treeData.xml"; // Name of your save file
        // Dictionary to cache icon indices for performance and resource management
        private Dictionary<string, int> driveIconPathToIndexMap = new Dictionary<string, int>();
        private int nextIconIndex = 0; // Tracks the next available index in imageListTreeView


        public Manual()
        {
            utils = new Utils();

            InitializeComponent();

        }

        private void addFile_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;

            // Determine parent node
            if (selectedNode == null)
            {
                // No selection, show message
                MessageBox.Show("Please select the folder that you want to add files.", "Warning",
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
                    utils.AddFileToNode(nodeImageList, selectedNode, fileName);
                }
                selectedNode.Expand();
            }
        }

        private void Manual_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Only save if the window is not minimized.
            // If it's minimized, saving its size/location might result in strange behavior
            // when it's restored to a non-minimized state.
            if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
            {
                // Save the current window state (Normal or Maximized)
                Properties.Settings.Default.WindowState = this.WindowState;

                // Save the current location
                Properties.Settings.Default.WindowLocation = this.Location;

                // Save the current size
                Properties.Settings.Default.WindowSize = this.Size;
            }
            else // If minimized, save the Normal state position/size
            {
                Properties.Settings.Default.WindowState = FormWindowState.Normal;
                // You might want to save its last known normal state bounds,
                // but for simplicity, we'll just save the current location/size if not minimized.
                // If it was minimized, its .Location/.Size might be off-screen.
                // A more robust solution might use Form.RestoreBounds.
            }

            // Important: Call Save() to persist the changes to the user.config file.
            Properties.Settings.Default.Save();
        }

        private void Manual_Load(object sender, EventArgs e)
        {
            // Load the saved window location
            // Check if the saved location is valid (e.g., not all 0s if it's the default,
            // or if it's not off-screen).
            Point savedLocation = Properties.Settings.Default.WindowLocation;

            // Simple check to prevent window from appearing off-screen if monitors change
            if (savedLocation.X >= SystemInformation.VirtualScreen.Left &&
                savedLocation.Y >= SystemInformation.VirtualScreen.Top &&
                savedLocation.X < SystemInformation.VirtualScreen.Right &&
                savedLocation.Y < SystemInformation.VirtualScreen.Bottom)
            {
                this.Location = savedLocation;
            }
            else
            {
                // If saved location is invalid, center the window
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            // Load the saved window size
            Size savedSize = Properties.Settings.Default.WindowSize;
            // Ensure minimum size is respected if the saved size is too small
            if (savedSize.Width > this.MinimumSize.Width && savedSize.Height > this.MinimumSize.Height)
            {
                this.Size = savedSize;
            }

            // Load the saved window state
            FormWindowState savedWindowState = Properties.Settings.Default.WindowState;
            if (savedWindowState != FormWindowState.Minimized) // Never start minimized
            {
                this.WindowState = savedWindowState;
            }

            // --- (Your existing window location/size loading logic here) ---

            LoadTreeData(); // Call this to load TreeView data
        }

        private void addFolder_Click(object sender, EventArgs e)
        {
            
        }

        private void renameFolder_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("Please select the item you want to change the name.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string currentName = selectedNode.Text;
            InputDialog inputDialog = new InputDialog();
            if(inputDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string newName = inputDialog.getValue();

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

        private void deleteItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("Please select the item you want to delete.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string itemType = (selectedNode.Tag is FolderItem) ? "Folder" : "File";
            DialogResult result = MessageBox.Show(
                string.Format("Delete {0} '{1}'?", itemType, selectedNode.Text),
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                selectedNode.Remove();
            }
        }

        private void createFolder_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;

            InputDialog inputDlg = new InputDialog();

            if (selectedNode == null)
            {
                // Not selected, Add new parent node
                if (inputDlg.ShowDialog() == DialogResult.OK)
                {
                    TreeNode newParentNode = new TreeNode(inputDlg.getValue());
                    int imageIndex = inputDlg.getImageIndex();
                    newParentNode.ImageIndex = imageIndex;
                    newParentNode.SelectedImageIndex = imageIndex;
                    treeViewFiles.Nodes.Add(newParentNode);
                }
                this.BringToFront();
                return;

            }

            string inputName;
            if (inputDlg.ShowDialog() == DialogResult.OK)
            {
                inputName = inputDlg.getValue();
                if (!string.IsNullOrEmpty(inputName))
                {
                    TreeNode newFolderNode = new TreeNode(inputName);
                    newFolderNode.Tag = new FolderItem(inputName);
                    newFolderNode.ImageKey = "folder";
                    newFolderNode.SelectedImageKey = "folder_open";

                    if (selectedNode != null)
                    {
                        selectedNode.Nodes.Add(newFolderNode);
                        selectedNode.Expand();
                    }
                    else
                    {
                        treeViewFiles.Nodes.Add(newFolderNode);
                    }

                    treeViewFiles.SelectedNode = newFolderNode;
                }
            }
        }

        /// <summary>
        /// Loads TreeView structure from an XML file.
        /// </summary>
        private void LoadTreeData()
        {
            string filePath = Path.Combine(Application.LocalUserAppDataPath, TreeDataFileName);

            if (File.Exists(filePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableTreeNodeData>));
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        List<SerializableTreeNodeData> loadedData = (List<SerializableTreeNodeData>)serializer.Deserialize(reader);

                        // Clear current tree and icon cache before rebuilding
                        treeViewFiles.Nodes.Clear();
                        // (You would clear and re-populate your imageListTreeView.Images and driveIconPathToIndexMap here)
                        // imageListTreeView.Images.Clear();
                        // driveIconPathToIndexMap.Clear();
                        // nextIconIndex = 0;

                        BuildTreeNodesFromSerializableData(treeViewFiles.Nodes, loadedData);
                    }
                    System.Diagnostics.Debug.WriteLine("Tree data loaded from: {0}", filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading tree data: " + ex.Message + "\nStarting with a fresh tree.", "Load Error"
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Error);
                    PopulateDriveNodes(); // Fallback to initial population if load fails
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No saved tree data found at: {0}. Populating with drives.", filePath);
                PopulateDriveNodes(); // Initial population if no file exists
            }
        }

        /// <summary>
        /// Saves the current TreeView structure to an XML file.
        /// </summary>
        private void SaveTreeData()
        {
            string filePath = Path.Combine(Application.LocalUserAppDataPath, TreeDataFileName);
            List<SerializableTreeNodeData> dataToSave = ConvertTreeNodesToSerializableData(treeViewFiles.Nodes);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableTreeNodeData>));
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, dataToSave);
                }
                System.Diagnostics.Debug.WriteLine("Tree data saved to: {0}", filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving tree data: " + ex.Message
                    , "Save Error"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Converts the current TreeView nodes into a list of serializable data objects.
        /// </summary>
        private List<SerializableTreeNodeData> ConvertTreeNodesToSerializableData(TreeNodeCollection nodes)
        {
            List<SerializableTreeNodeData> serializableNodes = new List<SerializableTreeNodeData>();
            foreach (TreeNode node in nodes)
            {
                FolderItem folderItem = node.Tag as FolderItem;
                if (folderItem != null)
                {
                    SerializableTreeNodeData serializableNode = new SerializableTreeNodeData(folderItem);
                    serializableNode.Children = ConvertTreeNodesToSerializableData(node.Nodes);
                    serializableNodes.Add(serializableNode);
                }
            }
            return serializableNodes;
        }

        /// <summary>
        /// Reconstructs TreeView nodes from a list of serializable data objects.
        /// </summary>
        private void BuildTreeNodesFromSerializableData(TreeNodeCollection parentNodes, List<SerializableTreeNodeData> serializableData)
        {
            foreach (SerializableTreeNodeData data in serializableData)
            {
                TreeNode newNode = new TreeNode(data.ItemData.Name);
                newNode.Tag = data.ItemData;

                // You need a method here to re-assign icons based on data.ItemData.FullPath and data.ItemData.IsDrive
                // AssignNodeIcon(newNode, data.ItemData.FullPath, data.ItemData.IsDrive);

                if (data.Children != null && data.Children.Count > 0)
                {
                    BuildTreeNodesFromSerializableData(newNode.Nodes, data.Children);
                }
                else
                {
                    // Add dummy "Loading..." node if it's a directory/drive but wasn't expanded when saved
                    if (data.ItemData.IsDrive || Directory.Exists(data.ItemData.FullPath))
                    {
                        newNode.Nodes.Add("Loading...");
                    }
                }
                parentNodes.Add(newNode);
            }
        }

        // --- (Your existing PopulateDriveNodes and AssignNodeIcon methods would be here) ---
        // PopulateDriveNodes should be called if LoadTreeData fails or no save file exists.
        // AssignNodeIcon is a helper for setting icons after loading/rebuilding.


        /// Populates the TreeView with logical drives, retrieving system icons.
        /// This method is typically called on initial startup or if saved data is unavailable/corrupt.
        /// </summary>
        private void PopulateDriveNodes()
        {
            // Clear existing nodes and reset image cache
            treeViewFiles.Nodes.Clear();
            nodeImageList.Images.Clear(); // Clear all images from the ImageList
            driveIconPathToIndexMap.Clear();  // Clear our custom icon index map
            nextIconIndex = 0;                // Reset the next available index

            // Get all logical drives on the computer (e.g., C:\, D:\)
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                // Only process drives that are ready (e.g., connected hard drives, not empty CD/DVD drives)
                if (drive.IsReady)
                {
                    TreeNode driveNode = new TreeNode();
                    string driveName = drive.Name; // Full path, e.g., "C:\"

                    // Get a user-friendly display name (e.g., "Local Disk (C:)")
                    string displayName = "";
                    try
                    {
                        displayName = drive.VolumeLabel + " " + "(" + driveName.Substring(0, 1) + ")";
                    }
                    catch (IOException)
                    {
                        // Handle cases where VolumeLabel might not be accessible
                        displayName = "Local Disk (" + driveName.Substring(0, 1) + ")";
                    }
                    catch (Exception)
                    {
                        displayName = "Drive ("+ driveName.Substring(0, 1) + ")";
                    }

                    driveNode.Text = displayName;

                    // --- Assign a new FolderItem instance to the Tag property ---
                    // This stores custom data with the TreeNode, required for serialization.
                    driveNode.Tag = new FolderItem(displayName, driveName, true); // true indicates it's a drive
                    // -------------------------------------------------------------

                    // Assign the correct system icon for the drive to the node
                    AssignNodeIcon(driveNode, driveName, true); // true indicates it's a directory/drive

                    // Add a dummy child node ("Loading...") to make the drive node expandable.
                    // This is essential for lazy loading of subfolders when the user expands the node.
                    driveNode.Nodes.Add("Loading...");

                    // Add the populated drive node to the TreeView
                    treeViewFiles.Nodes.Add(driveNode);
                }
            }
        }

        // --- Helper Method to Assign Icons (as provided previously) ---
        /// <summary>
        /// Helper method to assign the correct system icon to a TreeView node.
        /// It uses the IconExtractor class and caches icons in imageListTreeView.
        /// </summary>
        /// <param name="node">The TreeNode to assign the icon to.</param>
        /// <param name="path">The full path of the item (file or directory).</param>
        /// <param name="isDirectory">True if the path represents a directory or drive.</param>
        private void AssignNodeIcon(TreeNode node, string path, bool isDirectory)
        {
            Icon nodeIcon = IconExtractor.GetPathIcon(path, true, isDirectory); // true for small icon

            if (nodeIcon != null)
            {
                // Cache the icon: If we don't have this icon (path) yet, add it to the ImageList
                if (!driveIconPathToIndexMap.ContainsKey(path))
                {
                    nodeImageList.Images.Add(nodeIcon); // Add the actual Icon object
                    driveIconPathToIndexMap[path] = nextIconIndex; // Store its index
                    nextIconIndex++; // Increment for the next unique icon
                }
                // Assign the cached icon's index to the TreeNode
                node.ImageIndex = driveIconPathToIndexMap[path];
                node.SelectedImageIndex = driveIconPathToIndexMap[path];
            }
            else
            {
                // Fallback if no icon could be retrieved (e.g., a default empty icon or -1)
                node.ImageIndex = -1; // No icon assigned
                node.SelectedImageIndex = -1; // No icon assigned
            }
        }

    }

    
}