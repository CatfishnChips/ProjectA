using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public BehaviourTree tree;
        [SerializeField] private AIDifficultySettings difficultySettings;

        [Tooltip("Enlarges the hitboxes to allow AI to take action while the hitbox is not precisely hitting the opponent.")]
        [SerializeField] private float _attackBoxFlexibilityMargin;
        [SerializeField] private float _distanceMargin;
        [SerializeField] private AIPositionMethod _optimalDistanceMethod;
        
        private Dictionary<string, ActionAttack> _hittingAttacks;

        private System.Type _currentState;
        private System.Type _previousState;
        private bool _stateChange;
        private bool _attackMoveEnded; // When an attack Move is finished, this bool gets set to true by an event to let the script know that next desicion for attack move can be made.
        private bool _drivenByContext; // This boolean may need to be changed to an enum later on. It indicates whether the fixed update method of the AI is driven by it's ctx FighterState or not.

        private Vector2 _distanceToOpponent;
        private float _optimalDistance;

        public Dictionary<string, ActionAttack> HittingAttacks { get => _hittingAttacks; set => _hittingAttacks = value; }
        public AIDifficultySettings DifficultySettings { get => difficultySettings; }
        public float AttackBoxFlexibilityMargin { get => _attackBoxFlexibilityMargin; }
        public float DistanceMargin { get => _distanceMargin; }
        public AIPositionMethod OptimalDistanceMethod { get => _optimalDistanceMethod; }

        // Storage container object to hold game object subsystems
        Context context;

        // Start is called before the first frame update
        void Start() {
            context = new Context(gameObject);
            tree = tree.Clone();
            tree.Bind(context);
        }

        // Update is called once per frame
        void FixedUpdate() {
            if (tree) {
                tree.Update();
            }
        }

        // Context CreateBehaviourTreeContext() {
        //     return Context.CreateFromGameObject(gameObject);
        // }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            BehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}