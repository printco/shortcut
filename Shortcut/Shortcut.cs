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
    public partial class Shortcut : Form
    {
        private Utils utils;
        private const string TreeDataFileName = "treeData.xml"; // Name of your save file
        // Dictionary to cache icon indices for performance and resource management
        private Dictionary<string, int> driveIconPathToIndexMap = new Dictionary<string, int>();
        private int nextIconIndex = 0; // Tracks the next available index in imageListTreeView
        // --- Add a class-level list to temporarily store nodes that need to be expanded ---
        private List<TreeNode> nodesToExpandOnShown = new List<TreeNode>();
        // Keep track of the node being dragged
        private TreeNode _draggedNode;
        private TreeNode _overNode;

        public Shortcut()
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

            SaveTreeData();
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
            TreeNode selectedNode = treeViewFiles.SelectedNode;

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select folder";
            folderBrowserDialog.ShowNewFolderButton = true; // Allow user to create new folders

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolderPath = folderBrowserDialog.SelectedPath; // Get the full path of the selected folder
                toolStripStatusLabel1.Text = selectedFolderPath;

                if (selectedNode == null)
                {
                    // Create new node as root
                    string folderName = Path.GetFileName(selectedFolderPath);
                    TreeNode nodeFromSelect = new TreeNode(folderName);
                    TreeItem treeItem = new TreeItem();
                    treeItem.IsFile = false;
                    treeItem.Name = folderName;
                    nodeFromSelect.Tag = treeItem;
                    treeViewFiles.Nodes.Add(nodeFromSelect);
                    selectedNode = nodeFromSelect;
                    treeViewFiles.SelectedNode = selectedNode;
                    RecursiveFolder(selectedNode, selectedFolderPath, 0);
                }

            }
        }

        /// <summary>
        /// Recursively scans a directory for all files and subdirectories.
        /// </summary>
        /// <param name="currentDirectory">The directory path to scan.</param>
        /// <param name="depth">The current recursion depth (for indentation).</param>
        private void RecursiveFolder(TreeNode parentNode, string currentDirectory, int depth)
        {
            // Create indentation for clearer output
            string indent = new string(' ', depth * 4);
            System.Diagnostics.Debug.WriteLine(string.Format("{0}Scanning Directory: {1}", indent, currentDirectory));
            // _foundPaths.Add(currentDirectory); // Add current directory to the list

            try
            {
                // --- Get and Process Files in the current directory ---
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (string file in files)
                {
                    TreeNode newNode = new TreeNode();
                    string baseName = Path.GetFileName(file);
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}    File: {1}", indent, baseName));
                    Icon icon = utils.GetFileIcon(file);
                    string imageKey = "folder";
                    if (icon != null)
                    {
                        string iconExtension = Path.GetExtension(file);
                        nodeImageList.Images.Add(iconExtension, icon);
                        imageKey = iconExtension;
                    }
                    TreeItem fileItem = new TreeItem();
                    fileItem.Name = baseName;
                    fileItem.IsFile = true;
                    fileItem.FullPath = file;

                    newNode.Text = baseName;
                    newNode.Tag = fileItem;
                    newNode.ImageKey = imageKey;
                    newNode.SelectedImageKey = imageKey;
                    parentNode.Nodes.Add(newNode);                    
                }

                // --- Get Subdirectories and Recurse ---
                string[] subDirectories = Directory.GetDirectories(currentDirectory);
                foreach (string subDir in subDirectories)
                {
                    string baseName = Path.GetFileName(subDir);
                    TreeItem folderItem = new TreeItem();
                    folderItem.IsFile = false;
                    folderItem.Name = baseName;

                    TreeNode nextNode = new TreeNode();
                    nextNode.Text = baseName;
                    nextNode.Name = baseName;
                    nextNode.Tag = folderItem;

                    string imageKey = "folder";

                    nextNode.ImageKey = imageKey;
                    nextNode.SelectedImageKey = imageKey;
                    parentNode.Nodes.Add(nextNode);
                    // Recursively call the method for each subdirectory
                    RecursiveFolder(nextNode, subDir, depth + 1);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle cases where the application does not have permission to access a directory
                System.Diagnostics.Debug.WriteLine(string.Format("{0}    ACCESS DENIED: Cannot access directory {1}", indent, currentDirectory));
            }
            catch (IOException ex)
            {
                // Handle other I/O errors (e.g., disk not ready, file in use)
                System.Diagnostics.Debug.WriteLine(string.Format("{0}    I/O Error in {1}: {2}", indent, currentDirectory, ex.Message));
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                System.Diagnostics.Debug.WriteLine(string.Format("{0}    Error in {1}: {2}", indent, currentDirectory, ex.Message));
            }
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
                TreeItem treeItem = selectedNode.Tag as TreeItem;
                if (treeItem.IsFile)
                {
                    TreeItem TreeItem = (TreeItem)selectedNode.Tag;
                    TreeItem.Name = newName;
                }
                else
                {
                    TreeItem TreeItem = (TreeItem)selectedNode.Tag;
                    TreeItem.Name = newName;
                }
            }
        }

        private void deleteItem_Click(object sender, EventArgs e)
        {
            deleteTreeNodeItem();
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
                    string nodeName = inputDlg.getValue();
                    TreeNode newParentNode = new TreeNode(nodeName);
                    newParentNode.Tag = new TreeItem(nodeName, nodeName, true);
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
                    newFolderNode.Tag = new TreeItem(inputName
                        , selectedNode.FullPath + treeViewFiles.PathSeparator + inputName
                        , false);
                    newFolderNode.ImageKey = "folder";
                    newFolderNode.SelectedImageKey = "open-folder";

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
            ReadFileXML(filePath);
        }

        private void ReadFileXML(string filePath)
        {
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
                    System.Diagnostics.Debug.WriteLine("Tree data loaded from: " + filePath);
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
                System.Diagnostics.Debug.WriteLine("No saved tree data found at: " + filePath + ". Populating with drives.");
                // PopulateDriveNodes(); // Initial population if no file exists
            }
        }

        private void SaveFileXML(string filePath)
        {
            try
            {
                List<SerializableTreeNodeData> dataToSave = ConvertTreeNodesToSerializableData(treeViewFiles.Nodes);
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
                System.Diagnostics.Debug.WriteLine("Tree data saved to: " + filePath);
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
        /// Saves the current TreeView structure to an XML file.
        /// </summary>
        private void SaveTreeData()
        {
            string filePath = Path.Combine(Application.LocalUserAppDataPath, TreeDataFileName);
            SaveFileXML(filePath);
        }

        /// <summary>
        /// Converts the current TreeView nodes into a list of serializable data objects.
        /// </summary>
        private List<SerializableTreeNodeData> ConvertTreeNodesToSerializableData(TreeNodeCollection nodes)
        {
            List<SerializableTreeNodeData> serializableNodes = new List<SerializableTreeNodeData>();
            foreach (TreeNode node in nodes)
            {
                // C# 2.0 style: Cast to BaseTreeItem using 'as' operator
                TreeItem itemData = node.Tag as TreeItem;
                if (itemData != null) // Check for null after 'as' cast
                {
                    // Recursive into children node
                    SerializableTreeNodeData serializableNode = new SerializableTreeNodeData(itemData);
                    serializableNode.IsExpanded = node.IsExpanded; // Capture the current expansion state
                    serializableNode.Children = ConvertTreeNodesToSerializableData(node.Nodes);
                    serializableNodes.Add(serializableNode);
                }
            }
            return serializableNodes;
        }


        /// <summary>
        /// Reconstructs TreeView nodes from a list of serializable data objects.
        /// </summary>
        private void BuildTreeNodesFromSerializableData(TreeNodeCollection parentNodes
            , List<SerializableTreeNodeData> serializableData)
        {
            foreach (SerializableTreeNodeData data in serializableData)
            {
                TreeNode newNode = new TreeNode(data.ItemData.Name);
                newNode.Tag = data.ItemData;
                TreeItem treeItem = newNode.Tag as TreeItem;
                // You need a method here to re-assign icons based on data.ItemData.FullPath and data.ItemData.IsDrive
                // AssignNodeIcon(newNode, data.ItemData.FullPath, data.ItemData.IsDrive);

                if (data.Children != null && data.Children.Count > 0)
                {
                    BuildTreeNodesFromSerializableData(newNode.Nodes, data.Children);
                }
                //else
                //{
                //    // Add dummy "Loading..." node if it's a directory/drive but wasn't expanded when saved
                //    if (Directory.Exists(data.ItemData.FullPath))
                //    {
                //        newNode.Nodes.Add("Loading...");
                //    }
                //}

                // Get icon and add to ImageList
                Icon icon = utils.GetFileIcon(treeItem.FullPath);
                string imageKey = "folder";
                if (icon != null && treeItem.IsFile)
                {
                    string iconExtension = Path.GetExtension(treeItem.FullPath);
                    nodeImageList.Images.Add(iconExtension, icon);
                    imageKey = iconExtension;
                } 
                else 
                {
                    if (data.IsExpanded)
                    {
                        nodesToExpandOnShown.Add(newNode);
                        imageKey = "open-folder";
                    }    
                }

                newNode.ImageKey = imageKey;
                newNode.SelectedImageKey = imageKey;
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

                    // --- Assign a new TreeItem instance to the Tag property ---
                    // This stores custom data with the TreeNode, required for serialization.
                    driveNode.Tag = new TreeItem(displayName, driveName, true); // true indicates it's a drive
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

        private void Manual_MouseClick(object sender, MouseEventArgs e)
        {
            treeViewFiles.SelectedNode = null;
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null || selectedNode.Tag is TreeItem)
            {
                MessageBox.Show("Please select the file you want to open.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeItem TreeItem = (TreeItem)selectedNode.Tag;
            OpenFile(TreeItem.FullPath);
        }

        private void treeViewFiles_DoubleClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is TreeItem)
            {
                TreeItem treeItem = (TreeItem)selectedNode.Tag;
                if (treeItem.IsFile)
                {
                    OpenFile(treeItem.FullPath);
                }
            }
        }

        private void OpenFile(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open file: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Manual_Shown(object sender, EventArgs e)
        {
            // --- Perform actual expansion after the form is fully visible and rendered ---
            foreach (TreeNode node in nodesToExpandOnShown)
            {
                if (node != null && node.TreeView != null) // Ensure node is still valid and in a TreeView
                {
                    node.Expand();
                }
            }
            nodesToExpandOnShown.Clear(); // Clear the list after expanding
        }

        private void treeViewFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Store the node being dragged
            _draggedNode = e.Item as TreeNode;

            if (_draggedNode != null)
            {
                System.Diagnostics.Debug.WriteLine("Item drag");
                // Start the drag-and-drop operation.
                // Allowed effects: Copy (Ctrl key down) or Move (default).
                // DoDragDrop() returns once the drop is complete or cancelled.
                DragDropEffects result = DoDragDrop(e.Item, DragDropEffects.Move | DragDropEffects.Copy);
                // Clear the dragged node reference after the drag operation, regardless of outcome.
                _draggedNode = null;
            }
        }

        private void treeViewFiles_DragEnter(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Drag enter()");
            System.Diagnostics.Debug.WriteLine(e.Data.GetDataPresent(typeof(TreeNode)));
            // Check if the data being dragged is a TreeNode
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                System.Diagnostics.Debug.WriteLine("typeof(TreeNode)");
                System.Diagnostics.Debug.WriteLine("KeyState " + (e.KeyState & 8));
                // Set the allowed drag effects based on modifier keys
                if ((e.KeyState & 8) == 8) // Ctrl key is down (check KeyState value for Ctrl)
                {
                    e.Effect = DragDropEffects.Copy; // Copy operation
                }
                else
                {
                    e.Effect = DragDropEffects.Move; // Move operation
                }
            }
            else
            {
                e.Effect = DragDropEffects.None; // Not a TreeNode, no drag allowed
            }
        }

        private void treeViewFiles_DragOver(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Drag over()");
            // Get the node under the mouse cursor
            Point targetPoint = treeViewFiles.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeViewFiles.GetNodeAt(targetPoint);

            // Determine the drag effect
            if ((e.KeyState & 8) == 8) // Ctrl key is down
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }

            // Provide visual feedback: highlight the target node
            if (targetNode != null && targetNode != _draggedNode && !IsDescendant(_draggedNode, targetNode))
            {
                treeViewFiles.SelectedNode = targetNode; // Highlight target node
                _overNode = targetNode; // Keep select node for parent
            }
            else
            {
                treeViewFiles.SelectedNode = null; // Clear highlight if over invalid target
                e.Effect = DragDropEffects.None; // No valid drop target
            }
        }

        private void treeViewFiles_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Drag drop()");
            // Get the node under the mouse cursor where the drop occurred
            Point targetPoint = treeViewFiles.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeViewFiles.GetNodeAt(targetPoint);

            // Get the node that was dragged
            _draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (_overNode != null && files != null)
            {
                foreach (string file in files){
                    string baseName = Path.GetFileName(file);
                    TreeNode newNode = new TreeNode(baseName);
                    Icon icon = utils.GetFileIcon(file);
                    string imageKey = "object";
                    if (icon != null)
                    {
                        string iconExtension = Path.GetExtension(file);
                        nodeImageList.Images.Add(iconExtension, icon);
                        imageKey = iconExtension;
                    }
                    TreeItem fileItem = new TreeItem();
                    fileItem.Name = baseName;
                    fileItem.IsFile = true;
                    fileItem.FullPath = file;

                    newNode.Text = baseName;
                    newNode.Tag = fileItem;
                    newNode.ImageKey = imageKey;
                    newNode.SelectedImageKey = imageKey;
                    _overNode.Nodes.Add(newNode); 
                }
            }
            // Validate drop operation
            else if (_draggedNode == null 
                || targetNode == null 
                || targetNode == _draggedNode 
                || IsDescendant(_draggedNode, targetNode))
            {
                return; // Invalid drop
            }

            TreeItem targetNodeItem = (TreeItem) targetNode.Tag;
            if (targetNodeItem.IsFile)
            {
                MessageBox.Show(this, "Could not drop in to file");
                return;
            }

            // Perform Move or Copy
            if (e.Effect == DragDropEffects.Move)
            {
                // Remove the node from its original parent
                _draggedNode.Remove();
                // Add the node to the new target node
                targetNode.Nodes.Add(_draggedNode);
            }
            else if (e.Effect == DragDropEffects.Copy)
            {
                // Create a clone of the dragged node for copying
                TreeNode clonedNode = (TreeNode)_draggedNode.Clone();
                targetNode.Nodes.Add(clonedNode);
            }

            targetNode.ExpandAll(); // Expand the target node to show the dropped item
            treeViewFiles.SelectedNode = _draggedNode; // Select the dropped/moved node
            _draggedNode = null; // Clear the dragged node reference
        }

        /// <summary>
        /// Helper method to check if a target node is a descendant of the dragged node.
        /// (Prevents dragging a parent node into its own child)
        /// </summary>
        /// <param name="parent">The potential parent node.</param>
        /// <param name="child">The potential child node.</param>
        /// <returns>True if child is a descendant of parent, false otherwise.</returns>
        private bool IsDescendant(TreeNode parent, TreeNode child)
        {
            System.Diagnostics.Debug.WriteLine("IsDescendant()");
            if (child.Parent == null)
            {
                return false; // Reached root, child is not a descendant
            }
            if (child.Parent == parent)
            {
                return true; // Child's immediate parent is 'parent'
            }
            return IsDescendant(parent, child.Parent); // Recursively check up the hierarchy
        }

        private void treeViewFiles_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteTreeNodeItem();
            }
            else if (e.KeyCode == Keys.F2)
            {
                // Ensure a node is currently selected
                if (treeViewFiles.SelectedNode != null)
                {
                    // Start editing the label of the selected node
                    treeViewFiles.SelectedNode.BeginEdit();
                }
            }

        }

        private void deleteTreeNodeItem()
        {
            TreeNode selectedNode = treeViewFiles.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("Please select the item you want to delete.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string itemType = (selectedNode.Tag is TreeItem) ? "Folder" : "File";
            DialogResult result = MessageBox.Show(
                string.Format("Delete {0} '{1}'?", itemType, selectedNode.Text),
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                toolStripStatusLabel1.Text = "Delete " + selectedNode.Text;
                selectedNode.Remove();
            }
        }

        private void treeViewFiles_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the TreeNode that was hit by the mouse click
            // GetNodeAt() returns null if no node is at the specified coordinates.
            TreeNode clickedNode = treeViewFiles.GetNodeAt(e.X, e.Y);

            // If clickedNode is null, it means the click was on an empty area
            if (clickedNode == null)
            {
                // Programmatically set SelectedNode to null to unselect any currently selected node.
                treeViewFiles.SelectedNode = null;
            }
            else
            {
                toolStripStatusLabel1.Text = clickedNode.FullPath;
            }
            
        }

        private void treeViewFiles_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // e.Label contains the new text entered by the user.
            // If e.Label is null, it means the user cancelled the edit (e.g., by pressing Esc).
            if (e.Label != null && e.Label.Trim().Length > 0)
            {
                // Update your underlying data (e.g., the Name property in your FolderItem/FileItem)
                // This is crucial for keeping your data model in sync with the UI.
                TreeItem item = e.Node.Tag as TreeItem;
                if (item != null)
                {
                    item.Name = e.Label.Trim(); // Update the Name in your data object

                    // Also update the Text property of the node (e.Node.Text = e.Label.Trim();)
                    // This is usually done automatically by the TreeView if e.CancelEdit is false.
                }

                // MessageBox.Show(string.Format("Node '{0}' renamed to '{1}'.", e.Node.Text, e.Label), "Renamed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                e.CancelEdit = true; // Cancel the edit if the new label is empty or null
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save tree node to XML file"; // Dialog window title
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml"; // File type filter
            saveFileDialog.FilterIndex = 1; // Default selected filter (Text Files)
            saveFileDialog.RestoreDirectory = true; // Restores the directory to the previously selected one

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string saveFilePath = saveFileDialog.FileName;
                SaveFileXML(saveFilePath);
                toolStripStatusLabel1.Text = string.Format("Save XML to {0}", saveFilePath);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // --- 1. Create an instance of OpenFileDialog ---
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // --- 2. Configure dialog properties ---
            openFileDialog.Title = "Select tree node XML"; // Dialog window title
            openFileDialog.Filter = "XML Files (*.xml)|*.xml"; // File type filter
            openFileDialog.FilterIndex = 1; // Default selected filter (Text Files)
            openFileDialog.RestoreDirectory = true; // Restores the directory to the previously selected one

            // --- 3. Show the dialog and check the result ---
            // ShowDialog() returns a DialogResult value indicating how the dialog was closed.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // User clicked OK and selected a file
                string selectedFilePath = openFileDialog.FileName; // Get the full path of the selected file
                ReadFileXML(selectedFilePath);
                toolStripStatusLabel1.Text = string.Format("Load XML from {0}", selectedFilePath);
            }
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewFiles.ExpandAll();
        }

        private void collapeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewFiles.CollapseAll();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }
    }
}