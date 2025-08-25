using System;
using System.Collections.Generic;
using System.Text;

namespace Shortcut
{
    [Serializable]
    public class FileItem : BaseTreeItem // Inherit from BaseTreeItem
    {
        private long _size; // Example: file size
        private string _extension; // Example: file extension
        private string _filePath;

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public FileItem() : base() { /* Parameterless constructor for serialization */ }

        public FileItem(string name, string fullPath, long size, string extension)
            : base(name, fullPath) // Call base constructor
        {
            Size = size;
            Extension = extension;
        }
    }
}
