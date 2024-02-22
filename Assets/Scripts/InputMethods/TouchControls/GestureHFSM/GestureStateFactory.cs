// using System.Collections.Generic;
// using UnityEngine;

// public class GestureStateFactory : MonoBehaviour
// {
//     GestureStateMachine _ctx;
//     Dictionary<TouchState, GestureBaseState> _states = new Dictionary<TouchState, GestureBaseState>();

//     public GestureStateFactory(GestureStateMachine currentContext){
//         _ctx = currentContext;

//         _states[TouchState.None] = new GestureNoTouchState(_ctx, this);
//         _states[TouchState.Stationary] = new GestureStationaryState(_ctx, this);

//     }

// }
