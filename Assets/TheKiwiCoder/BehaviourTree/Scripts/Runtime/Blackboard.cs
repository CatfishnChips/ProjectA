using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard {
        public Vector3 moveToPosition;
        public string choosenAgressiveAction;
        public string choosenDefensiveAction;
        public ActionAttack enemyAttackAction;

        [ReadOnly] public bool enemyAttackStarted;
        [ReadOnly] public bool enemyAttackEnded;

        [ReadOnly] public bool selfAttackStarted;
        [ReadOnly] public bool selfAttackEnded;

        public void OnEnemyAttackStart()
        {
            enemyAttackStarted = true;
            enemyAttackEnded = false;
        }

        public void OnEnemyAttackEnd()
        {
            enemyAttackStarted = false;
            enemyAttackEnded = true;
        }

        public void OnSelfAttackStart()
        {
            selfAttackStarted = true;
            selfAttackEnded = false;
        }

        public void OnSelfAttackEnd()
        {
            selfAttackStarted = false;
            selfAttackEnded = true;
        }
    }
}