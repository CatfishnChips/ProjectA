using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EditableFighterActions{

    public abstract class BPNode : ScriptableObject
    {
        private string guid;
        public Node nodeView;
        public Vector2 positionOnGraph;

        [SerializeField]
        private List<BPNode> children = new List<BPNode>();

        public List<BPNode> Children { get => children; set => children = value; }
        public string Guid { get => guid; set => guid = value; }

        public abstract void InOrderTreeToList(BPNode node, ref List<BPNode> list);
        public abstract void InitializeDictionary();

    }
}
