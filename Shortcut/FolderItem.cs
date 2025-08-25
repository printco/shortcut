// FolderItem.cs
using System;

namespace Shortcut // Replace with your actual project namespace
{
    [Serializable] // IMPORTANT: Required for XML serialization
    public class FolderItem
    {
        // --- Explicit Backing Fields ---
        // These private fields will actually store the data for each property.
        private string _name;
        private string _fullPath;
        private bool _isDrive;
        // -------------------------------

        // --- Full Properties with Get/Set Bodies ---
        // Each property now explicitly defines its 'get' and 'set' accessors.
        public string Name
        {
            get { return _name; } // 'get' accessor returns the value from the backing field
            set { _name = value; } // 'set' accessor assigns the incoming 'value' to the backing field
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
        // -------------------------------------------

        // Parameterless constructor (required for XML serialization)
        public FolderItem() { }

        // Parameterized constructor
        public FolderItem(string name, string fullPath, bool isDrive)
        {
            // Assign values through the property setters (good practice)
            Name = name;
            FullPath = fullPath;
            IsDrive = isDrive;
        }

        // You might add other methods here if needed.
    }
}