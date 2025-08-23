using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Shortcut
{
    public class IconExtractor
    {
        // Define SHFILEINFO structure
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        // Define constants for uFlags parameter
        public const uint SHGFI_ICON = 0x000000100;     // Get icon
        public const uint SHGFI_LARGEICON = 0x000000000; // Get large icon (32x32)
        public const uint SHGFI_SMALLICON = 0x000000001; // Get small icon (16x16)
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010; // Use file attributes (even if file doesn't exist)
        public const uint SHGFI_TYPENAME = 0x000000400;  // Get type name

        // Declare SHGetFileInfo function
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);

        // File attributes (used with SHGFI_USEFILEATTRIBUTES)
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010; // Directory
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;    // Normal file

        /// <summary>
        /// Gets the system icon for a file or directory.
        /// </summary>
        /// <param name="path">The path to the file or directory (e.g., "C:\\", "C:\\Windows\\System32").</param>
        /// <param name="smallIcon">True for 16x16 icon, False for 32x32 icon.</param>
        /// <param name="isDirectory">True if the path represents a directory (or drive root).</param>
        /// <returns>The Icon object, or null if not found.</returns>
        public static Icon GetPathIcon(string path, bool smallIcon, bool isDirectory)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            uint flags = SHGFI_ICON;
            uint attributes = FILE_ATTRIBUTE_NORMAL; // Default for files

            if (smallIcon)
            {
                flags |= SHGFI_SMALLICON;
            }
            else
            {
                flags |= SHGFI_LARGEICON;
            }

            if (isDirectory)
            {
                attributes = FILE_ATTRIBUTE_DIRECTORY; // Set attribute for directory
                flags |= SHGFI_USEFILEATTRIBUTES;      // Essential when dealing with drive roots or non-existent paths
            }
            // If it's not a directory, we can let SHGetFileInfo try to find the actual file.
            // If the file might not exist, you might still use SHGFI_USEFILEATTRIBUTES with FILE_ATTRIBUTE_NORMAL.


            IntPtr result = SHGetFileInfo(
                path,
                attributes,
                ref shfi,
                (uint)Marshal.SizeOf(shfi),
                flags);

            if (result != IntPtr.Zero && shfi.hIcon != IntPtr.Zero)
            {
                // Create a copy of the icon, then destroy the original handle to prevent leaks.
                Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
                DestroyIcon(shfi.hIcon); // Destroy the HICON returned by SHGetFileInfo
                return icon;
            }
            return null;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);
    }
}
