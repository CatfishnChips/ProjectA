using System.Collections.Generic;

namespace BehaviourTree{

    public class Sequence : Node
    {
        public Sequence() : base() {}
        public Sequence(Tree tree, List<Node> children) : base(tree, children) {}

        public override NodeState Evaluate(){
            bool anyChildIsRunning = false;

            foreach(Node child in children)
            {
                switch(child.Evaluate())
                {
                    case NodeState.FAIULRE:
                        state = NodeState.FAIULRE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }

}
