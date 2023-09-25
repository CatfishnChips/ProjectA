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
        public int dodgeFrame; // In frames. If Attempt defensive action decided to dodge it also tells how much to wait for the dogdge to work.
    }
}