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
        public InputEvents inputEvents;

        public Dictionary<InputGestures, ActionAttack> hittingAttacks = new Dictionary<InputGestures, ActionAttack>();

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
            //Match conducter singleton gerektiriyor.
            selfFSM = MatchConducter.Instance.FighterSlot2.GetComponent<FighterStateMachine>();
            enemyFSM = GameObject.FindWithTag("Player").GetComponent<FighterStateMachine>();

            attackBoxFlexibilityMargin = btr.AttackBoxFlexibilityMargin;
            distanceMargin = btr.DistanceMargin;
            optimalDistanceMethod = btr.OptimalDistanceMethod;

            optimalDistance = CalcOptimalXDistance(optimalDistanceMethod);
            // Add whatever else you need here...
        }

        private float CalcOptimalXDistance(AIPositionMethod method)
        {
            switch (method)
            {
                case AIPositionMethod.ArithmeticMean:
                    float xTotal = 0.0f;
                    foreach (KeyValuePair<InputGestures, ActionAttack> attack in selfFSM.AttackMoveDict)
                    {
                        // Debug.Log("Attack Name: " + attack.Key + ", Attack Distance: " + attack.Value.HitboxOffset.x);
                        xTotal += attack.Value.HitboxOffset.x;
                    }
                    return xTotal / selfFSM.AttackMoveDict.Count;
                default:
                    return 10.0f; // Place Holder
            }
            
        }
    }
}