using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(ActionAttack), true)]
public class ActionAttackEditor : Editor
{
    public VisualTreeAsset m_InspectorXML;
    private ActionAttack m_script;

    private void OnEnable(){
        m_script = (ActionAttack)serializedObject.targetObject;
    }

    // public override VisualElement CreateInspectorGUI()
    // {
    //     // Create a new VisualElement to be the root of our Inspector UI.
    //     VisualElement inspector = new VisualElement();

    //     // Add a simple label.
    //     inspector.Add(new Label("This is a custom Inspector"));

    //     // Load from default reference.
    //     m_InspectorXML.CloneTree(inspector);

    //     // Return the finished Inspector UI.
    //     return inspector;
    // }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        if (GUILayout.Button("Calculate Variables")){
            if (m_script != null){
               m_script.Calculate();
            }
        }
    }
}
