using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;


namespace BehaviourTree
{
    
    public enum NodeState{
        RUNNING,
        SUCCESS,
        FAIULRE
    }

    public class Node 
    {
        protected Tree tree;
        protected NodeState state;

        protected Node parent;
        protected List<Node> children;

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(Tree tree)
        {
            this.tree = tree;
        }

        public Node(Tree tree, List<Node> children)
        {
            this.tree = tree;
            foreach(Node child in children) _Attach(child);
        }

        public virtual NodeState Evaluate() => NodeState.FAIULRE;

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public object GetData(string key)
        {
            object value = null;
            if(_dataContext.TryGetValue(key, out value))
            {
                return value;
            }

            Node node = parent;
            while(node != null)
            {
                value = node.GetData(key);
                if(value != null){
                    return value;
                }
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if(_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while(node != null){
                bool cleared = node.ClearData(key);
                if(cleared){
                    return true;
                }
                node = node.parent;
            }
            return false;
        }
    }
}
