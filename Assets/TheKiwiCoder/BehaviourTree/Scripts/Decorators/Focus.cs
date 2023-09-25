// using System.Collections;
// using System.Collections.Generic;
// using TheKiwiCoder;
// using UnityEngine;

// public class Focus : DecoratorNode
// {
//     protected override void OnStart(){}

//     protected override void OnStop(){}

//     protected override State OnUpdate()
//     {
//         State state = child.Update();
//         if(state == State.Running)
//         {
//             FocusNode(child);
//             return State.Running;
//         }
//         return state;
//     }

//     private void FocusNode(Node node)
//     {
//         context.focusNode = node;
//     }

// }
