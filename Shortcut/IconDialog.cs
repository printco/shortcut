using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shortcut
{
    public partial class IconDialog : Form
    {
        private int imageIndex;

        public IconDialog()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.imageIndex = treeViewPreview.SelectedNode.Index;
            //this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Hide();
        }

        public int getImageIndex()
        {
            return this.imageIndex;
        }
    }
}