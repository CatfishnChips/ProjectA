using System.Collections.Generic;
using UnityEngine;

namespace EditableFighterActions{
    public class RootNode : BPNode
    {
        public new string name; 
        [SerializeField]
        public Dictionary<InputGestures, ActionNode> childrenDict;

        public override void InitializeDictionary(){
            childrenDict = new Dictionary<InputGestures, ActionNode>();
            Children.ForEach(c => {
                ActionNode childActionNode = c as ActionNode;
                if(childActionNode && !childrenDict.ContainsKey(childActionNode.inputGesture)) childrenDict.Add(childActionNode.inputGesture, childActionNode);
            });
        }
        
        public override void InOrderTreeToList(BPNode node, ref List<BPNode> list)
        {
            if(node == null) return;

            int childrenCount = node.Children.Count;

            for(int i = 0; i < childrenCount; i++){
                InOrderTreeToList(node.Children[i], ref list);
            }

            if(node.GetType() != typeof(RootNode)) list.Add(node);

        }

    }
}