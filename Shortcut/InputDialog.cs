using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shortcut
{
    public partial class InputDialog : Form
    {
        private string value;
        private int imageIndex = 0; // default 0

        public InputDialog()
        {
            InitializeComponent();

        }

        public string getValue()
        {
            return this.value;
        }

        public int getImageIndex()
        {
            return imageIndex;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            submit();
        }

        private void InputDialog_Shown(object sender, EventArgs e)
        {
            textBox1.Focus(); // Keep focus on the textbox
        }

        private void submit()
        {
            // --- Input Validation Logic ---
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.Trim().Length == 0)
            {
                // Input is empty or just whitespace
                MessageBox.Show("Please enter some data before pressing OK.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.DialogResult = DialogResult.None; // Prevent the form from closing
                // (This overrides the DialogResult set in the designer)
            }
            else
            {
                // Input is valid, allow the form to close with OK result
                this.DialogResult = DialogResult.OK; // Explicitly set DialogResult to OK
                this.value = textBox1.Text;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Enter key was pressed
            if (e.KeyCode == Keys.Enter)
            {
                // This simulates a click on the OK button
                okButton.PerformClick();

                // Prevents the "ding" sound that often occurs with the Enter key
                e.SuppressKeyPress = true;
            }
        }
    }
}