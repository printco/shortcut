// BaseTreeItem.cs
using System;
using System.Xml.Serialization; // Required for [XmlInclude]

namespace Shortcut
{
    [Serializable]
    // IMPORTANT: Use [XmlInclude] to tell XmlSerializer about derived types
    [XmlInclude(typeof(FolderItem))]
    [XmlInclude(typeof(FileItem))]
    public abstract class BaseTreeItem
    {
        private string _name;
        private string _fullPath;
        private bool _isDrive;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        public bool IsDrive
        {
            get { return _isDrive; }
            set { _isDrive = value; }
        }

        public BaseTreeItem() { /* Parameterless constructor for serialization */ }

        public BaseTreeItem(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
        }
    }
}
