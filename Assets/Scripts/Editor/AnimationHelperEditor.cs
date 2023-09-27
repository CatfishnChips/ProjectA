using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationHelper), false)]
public class AnimationHelperEditor : Editor
{
    private AnimationHelper m_script;

    private void OnEnable(){
        m_script = (AnimationHelper)serializedObject.targetObject;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Create T Pose")){
            if (m_script != null){
               m_script.CreateTPose();
            }
        }
    }
}
