using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

namespace EditableFighterActions{

    public abstract class BPNode : ScriptableObject
    {
        private string guid;
        #if UNITY_EDITOR
        public Node nodeView;
        # endif
        public Vector2 positionOnGraph;

        [SerializeField]
        private List<BPNode> children = new List<BPNode>();

        public List<BPNode> Children { get => children; set => children = value; }
        public string Guid { get => guid; set => guid = value; }

        public abstract void InOrderTreeToList(BPNode node, ref List<BPNode> list);
        public abstract void InitializeDictionary();

    }
}
