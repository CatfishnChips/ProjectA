using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public AIDifficultySettings difficultySettings;
        public FighterStateMachine selfFSM;
        public FighterStateMachine enemyFSM;

        public Dictionary<string, ActionAttack> hittingAttacks = new Dictionary<string, ActionAttack>();

        public Vector2 distanceToOpponent = new Vector2();
        public AIPositionMethod optimalDistanceMethod;
        public float distanceMargin;
        public float optimalDistance;

        public float attackBoxFlexibilityMargin;
        // Add other game specific systems here

        public Context (GameObject gameObject) {
            // Fetch all commonly used components
            BehaviourTreeRunner btr = gameObject.GetComponent<BehaviourTreeRunner>();
            difficultySettings = btr.DifficultySettings;
            selfFSM = gameObject.GetComponent<FighterStateMachine>();
            enemyFSM = GameObject.FindWithTag("Player").GetComponent<FighterStateMachine>();

            attackBoxFlexibilityMargin = btr.AttackBoxFlexibilityMargin;
            distanceMargin = btr.DistanceMargin;
            optimalDistanceMethod = btr.OptimalDistanceMethod;
            // Add whatever else you need here...
        }
    }
}