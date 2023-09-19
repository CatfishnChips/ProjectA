using UnityEngine;


namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node _root = null;

        protected void Awake()
        {
            _root = SetupTree();
        }

        protected void Start(){
            SetupTreeFields();
        }

        private void FixedUpdate()
        {
            if(_root != null){
                _root.Evaluate();
            }
        }

        protected abstract Node SetupTree();
        protected abstract void SetupTreeFields();
    }
}
