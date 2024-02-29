using System.Collections.Generic;

namespace EditableFighterActions{
    public class RootNode : BPNode
    {
        public new string name; 
        
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