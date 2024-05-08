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
    private string[] m_tools = new string[4]{
        "Ground",
        "Air",
        "Knockdown",
        "Block"
    };

    private int m_toolbarInt = 0;

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
        m_toolbarInt = GUILayout.Toolbar(m_toolbarInt, m_tools);
        switch (m_toolbarInt)
        {
            case 0:
                var groundProperty = serializedObject.FindProperty("_ground");
                EditorGUILayout.PropertyField(groundProperty);
            break;

            case 1:
                var airProperty = serializedObject.FindProperty("_air");
                EditorGUILayout.PropertyField(airProperty);
            break;
            
            case 2:
                var knockdownProperty = serializedObject.FindProperty("_knockdown");
                EditorGUILayout.PropertyField(knockdownProperty);
            break;

            case 3:
                var blockProperty = serializedObject.FindProperty("_block");
                EditorGUILayout.PropertyField(blockProperty);
            break;
        }
        serializedObject.ApplyModifiedProperties();


        // if (GUILayout.Button("Calculate Variables")){
        //     if (m_script != null){
        //        m_script.Calculate();
        //     }
        // }
    }
}
