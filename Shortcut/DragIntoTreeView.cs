using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shortcut
{
    public partial class DragIntoTreeView : Form
    {

        private TreeNode overNode;

        public DragIntoTreeView()
        {
            InitializeComponent();

            treeView1.ExpandAll();
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DragDrop");

            // Get the file paths from the drop event
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            // Process each file
            foreach (string file in files)
            {
                // You can add the file path directly or extract the file name
                // For example:
                string fileName = System.IO.Path.GetFileName(file);
                if (overNode == null)
                {
                    treeView1.Nodes.Add(fileName); // Adds the new node to the root
                }
                else
                {
                    overNode.Nodes.Add(fileName);
                    overNode.Expand();
                }
            }
        }


        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DragEnter");

            // Check if the data being dragged is a file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Or DragDropEffects.Move
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            // Convert screen coordinates to client coordinates
            Point targetPoint = treeView1.PointToClient(new Point(e.X, e.Y));

            // Get the node at the current mouse position
            TreeNode targetNode = treeView1.GetNodeAt(targetPoint);

            // Set the found node as the selected node
            treeView1.SelectedNode = targetNode;

            // Optional: Determine the drop effect based on the data being dragged
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Or DragDropEffects.Move, etc.
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
            
            overNode = treeView1.SelectedNode;
            System.Diagnostics.Debug.WriteLine("DragOver : " + overNode.Text);
        }

    }
}