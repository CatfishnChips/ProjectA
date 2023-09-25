// using System;
// using TheKiwiCoder;

// public class ExecuteOnlyRunningTask : ActionNode
// {

//     protected override void OnStart(){}

//     protected override void OnStop(){}

//     protected override State OnUpdate()
//     {
//         if(context.focusedTask == null) return State.Failure;

//         State state = context.focusNode.Update();
//         if(state != State.Running) context.focusNode = null;

//         return state;
//     }
// }
