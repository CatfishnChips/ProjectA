using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugEventCaller))]
public class DebugEventCallerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DebugEventCaller eventCaller = (DebugEventCaller)target;

        base.OnInspectorGUI();
        
        if (GUILayout.Button("Set Time")){
            eventCaller.SetTime();
        }
        if (GUILayout.Button("Increase Round")){
            eventCaller.IncreaseRound();
        }
        if (GUILayout.Button("Increase Score (P1)")){
            eventCaller.IncreaseScore_P1();
        }
        if (GUILayout.Button("Set Health (P1)")){
            eventCaller.SetHealth_P1();
        }
        if (GUILayout.Button("Set Stamina (P1)")){
            eventCaller.SetStamina_P1();
        }
    }
}
