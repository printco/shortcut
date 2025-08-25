using System;
using System.Collections.Generic;
using System.Text;

namespace Shortcut
{
    [Serializable] // MUST be serializable
    public class SerializableTreeNodeData
    {
        private TreeItem _itemData;
        private List<SerializableTreeNodeData> _children;
        private bool _isExpanded; // Add this backing field for IsExpanded

        public TreeItem ItemData { 
            get { return _itemData; } 
            set { _itemData = value; } 
        }
        
        public List<SerializableTreeNodeData> Children { 
            get { return _children; } 
            set { _children = value; } 
        }

        public bool IsExpanded // Add this property
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        public SerializableTreeNodeData()
        {
            Children = new List<SerializableTreeNodeData>();
        }
        public SerializableTreeNodeData(TreeItem itemData)
        {
            ItemData = itemData; 
            Children = new List<SerializableTreeNodeData>();
        }
    }
}
