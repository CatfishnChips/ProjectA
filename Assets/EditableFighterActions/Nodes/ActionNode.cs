using System.Collections.Generic;

namespace EditableFighterActions{
    public abstract class ActionNode : BPNode
    {
        public InputGestures inputGesture;
        public ActionBase fighterAction;

        public override void InOrderTreeToList(BPNode node, ref List<BPNode> list)
        {
            if(node == null) return;

            int childrenCount = node.Children.Count;

            for(int i = 0; i < childrenCount - 1; i++){
                InOrderTreeToList(node.Children[i], ref list);
            }

            if(node.GetType() != typeof(RootNode)) list.Add(node);

            InOrderTreeToList(node.Children[childrenCount - 1], ref list);

        }

    }
}
