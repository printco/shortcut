using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Shortcut
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            // Use a unique name for your mutex to prevent conflicts

            using (Mutex mutex = new Mutex(true, "Shortcut-48B4-4D4B", out createdNew))
            {
                if (createdNew)
                {
                    // This is the first instance, run the application
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Shortcut());
                }
                else
                {
                    // Another instance is already running, show a message and exit
                    MessageBox.Show("Another instance of this application is already running.", "Application Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            
        }
    }
}