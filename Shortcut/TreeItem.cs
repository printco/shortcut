using System;
using System.Collections.Generic;
using System.Text;

namespace Shortcut
{
    [Serializable]
    public class TreeItem
    {
        private string _name;
        private string _fullPath;
        private bool _isFile;

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

        public bool IsFile
        {
            get { return _isFile; }
            set { _isFile = value; }
        }

        public TreeItem() { /* Parameterless constructor for serialization */ }

        public TreeItem(string name, string fullPath, bool isFile)
        {
            Name = name;
            FullPath = fullPath;
            IsFile = isFile;
        }

    }
}
