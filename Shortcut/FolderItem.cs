// FolderItem.cs
using System;

namespace Shortcut // Replace with your actual project namespace
{
    [Serializable]
    public class FolderItem
    {
        // --- Explicit Backing Fields ---
        private string _name;
        private string _fullPath;
        private bool _isDrive;
        public string _folderName;
        // -------------------------------

        // --- Full Properties with Get/Set Bodies ---
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

        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }
        }

        public bool IsDrive
        {
            get { return _isDrive; }
            set { _isDrive = value; }
        }
        // -------------------------------------------

        public FolderItem() { /* Parameterless constructor for XML serialization */ }

        public FolderItem(string name, string fullPath, bool isDrive)
        {
            Name = name;      // Assign through the property setter
            FullPath = fullPath;
            IsDrive = isDrive;
        }

        public FolderItem(string name)
        {
            _name = name;
        }
    }
}