using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace Shortcut
{
    class Utils
    {

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

        public Icon GetFileIcon(string filePath)
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

        public void AddFileToNode(ImageList imageList, TreeNode parentNode, string filePath)
        {
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                TreeItem item = new TreeItem(fileInfo.Name, filePath, true);
                //item.DisplayName = fileInfo.Name;
                //item.FileSize = fileInfo.Length;
                //item.FileType = GetFileType(filePath);

                // Get icon and add to ImageList
                Icon icon = GetFileIcon(filePath);
                string imageKey = "file_" + imageList.Images.Count.ToString();
                if (icon != null)
                {
                    imageList.Images.Add(imageKey, icon);
                }

                TreeNode fileNode = new TreeNode(item.Name);
                fileNode.Tag = item;
                fileNode.ImageKey = imageKey;
                fileNode.SelectedImageKey = imageKey;
                //fileNode.ToolTipText = string.Format("{0}\nประเภท: {1}\nขนาด: {2}\nเส้นทาง: {3}",
                //    item.FileName, item.FileType, FormatFileSize(item.FileSize), item.FilePath);

                parentNode.Nodes.Add(fileNode);
            }
        }

        public string GetFileType(string filePath)
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

        public string FormatFileSize(long bytes)
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

        public Icon GetFolderIcon()
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

        public Icon GetOpenFolderIcon()
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
    }
}
