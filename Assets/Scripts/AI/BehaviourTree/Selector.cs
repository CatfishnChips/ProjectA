using System.Collections.Generic;

namespace BehaviourTree{

    public class Selector : Node
    {
        public Selector() : base() {}
        public Selector(Tree tree, List<Node> children) : base(tree, children) {}

        public override NodeState Evaluate(){
            foreach(Node child in children)
            {
                switch(child.Evaluate())
                {
                    case NodeState.FAIULRE:
                        continue;
                    case NodeState.SUCCESS: 
                        return NodeState.SUCCESS;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    default:
                        continue;
                }
            }

            return NodeState.FAIULRE;
        }
    }

}
