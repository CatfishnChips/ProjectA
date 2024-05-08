using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TrajectoryProperties))]
public class TrajectoryPropertyCustomDrawer : PropertyDrawer
{
    private int m_toolbarInt;

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLine = 4;

        var trajectoryProp = property.FindPropertyRelative("trajectory");
       
        int value = trajectoryProp.enumValueIndex;

        switch (value){
            case 0:
                var arcProp = property.FindPropertyRelative("Arc");
                if(arcProp.isExpanded)
                    totalLine += 6;
            break;

            case 1:
                var lineProp = property.FindPropertyRelative("Line");
                if(lineProp.isExpanded)
                    totalLine += 2;
            break;
        }
    
        return EditorGUIUtility.singleLineHeight * totalLine + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect rect;

        var trajectoryProp = property.FindPropertyRelative("trajectory");

        rect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(trajectoryProp, label, true));
    
        EditorGUI.PropertyField(rect, trajectoryProp, false);
        m_toolbarInt = trajectoryProp.enumValueIndex;

        switch(m_toolbarInt){
            case 0: 
                var arcProp = property.FindPropertyRelative("Arc");
                rect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(trajectoryProp, label, true), position.width, EditorGUI.GetPropertyHeight(arcProp, label, true));
                EditorGUI.PropertyField(rect, arcProp, true);
            break;

            case 1:
                var lineProp = property.FindPropertyRelative("Line");
                rect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(trajectoryProp, label, true), position.width, EditorGUI.GetPropertyHeight(lineProp, label, true));
                EditorGUI.PropertyField(rect, lineProp, true);
            break;
        }
        EditorGUI.EndProperty();
    }
}
