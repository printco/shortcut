using System;
using System.Collections.Generic;
using System.Text;

namespace Shortcut
{
    [Serializable] // MUST be serializable
    public class SerializableTreeNodeData
    {
        private FolderItem _itemData;
        private List<SerializableTreeNodeData> _children;

        public FolderItem ItemData { get { return _itemData; } set { _itemData = value; } }
        public List<SerializableTreeNodeData> Children { get { return _children; } set { _children = value; } }

        public SerializableTreeNodeData()
        {
            Children = new List<SerializableTreeNodeData>();
        }
        public SerializableTreeNodeData(FolderItem itemData)
        {
            ItemData = itemData; Children = new List<SerializableTreeNodeData>();
        }
    }
}
