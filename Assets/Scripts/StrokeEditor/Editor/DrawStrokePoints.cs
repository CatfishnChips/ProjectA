using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StrokeVisualizer))]
public class DrawStrokePoints : Editor
{
    private readonly GUIStyle style = new GUIStyle();
    private Transform transform;

    private void OnEnable () {
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        transform = (target as MonoBehaviour).transform;
    }

    void OnSceneGUI(){
        if (Application.isPlaying) return;

        Handles.color = Color.green;
        StrokeVisualizer visualizer = (StrokeVisualizer)target;
        

        //SerializedProperty property = serializedObject.GetIterator();
        //CheckProperty(property);
        SerializedProperty property = serializedObject.FindProperty("_points");

        
       for (int i = 0; i < property.arraySize; i++) {
            SerializedProperty element = property.GetArrayElementAtIndex(i);
            if (element.propertyType == SerializedPropertyType.Vector2 || element.propertyType == SerializedPropertyType.Vector3) {
                DrawPoint(element, true, property.displayName + " " + i);
            }
        }

        // for (int i = 0; i < visualizer.Points.Count; i++){
        //     DrawPoint(visualizer.Points[i]);
        // }
    }

    private void DrawPoint(SerializedProperty property, bool local, string name){
        Vector2 drawPos = local ? (Vector2)transform.TransformPoint(property.vector2Value) : property.vector2Value;
        Handles.Label(drawPos, name, style);
        EditorGUI.BeginChangeCheck();
        //Vector2 position = Handles.PositionHandle(drawPos, Quaternion.identity);
        Vector2 position = Handles.FreeMoveHandle(drawPos, Quaternion.identity, 0.5f, Vector3.one, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck()) {
            if (local) position = transform.InverseTransformPoint(position);
            property.vector2Value = position;
            serializedObject.ApplyModifiedProperties();
        }
    }

    // private void CheckProperty (SerializedProperty property) {
    //     while (property.Next(true)) {
    //         if (property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector3) {
    //             FieldInfo field = GetParent(property).GetType().GetField(property.name);
    //             if (field == null) continue;
    //             object[] draggablePoints = field.GetCustomAttributes(typeof(DraggablePointAttribute), false);
    //             if (draggablePoints.Length > 0) {
    //                 DraggablePointAttribute attribute = draggablePoints[0] as DraggablePointAttribute;
    //                 DrawPoint(property, attribute.local, property.displayName);
    //             }
    //         }
    //         else if (property.propertyType == SerializedPropertyType.Generic && property.isArray) {
    //             FieldInfo field = GetParent(property).GetType().GetField(property.name);
    //             if (field == null) continue;
    //             object[] draggablePoints = field.GetCustomAttributes(typeof(DraggablePointAttribute), false);
    //             if (draggablePoints.Length > 0) {
    //                 DraggablePointAttribute attribute = draggablePoints[0] as DraggablePointAttribute;
    //                 for (int i = 0; i < property.arraySize; i++) {
    //                     SerializedProperty element = property.GetArrayElementAtIndex(i);
    //                     if (element.propertyType == SerializedPropertyType.Vector2 || element.propertyType == SerializedPropertyType.Vector3) {
    //                         DrawPoint(element, attribute.local, property.displayName + " " + i);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    // private void DrawPoint(SerializedProperty property, bool local, string name){
    //     if (property.propertyType == SerializedPropertyType.Vector2) {
    //         Vector2 drawPos = local ? (Vector2)transform.TransformPoint(property.vector2Value) : property.vector2Value;
    //         Handles.Label(drawPos, name, style);
    //         EditorGUI.BeginChangeCheck();
    //         Vector2 position = Handles.PositionHandle(drawPos, Quaternion.identity);
    //         if (EditorGUI.EndChangeCheck()) {
    //             if (local) position = transform.InverseTransformPoint(position);
    //             property.vector2Value = position;
    //             serializedObject.ApplyModifiedProperties();
    //         }
    //     }
    //     else {
    //         Vector3 drawPos = local ? transform.TransformPoint(property.vector3Value) : property.vector3Value;
    //         Handles.Label(drawPos, name, style);
    //         EditorGUI.BeginChangeCheck();
    //         Vector3 position = Handles.PositionHandle(drawPos, Quaternion.identity);
    //         if (EditorGUI.EndChangeCheck()) {
    //             if (local) position = transform.InverseTransformPoint(position);
    //             property.vector3Value = position;
    //             serializedObject.ApplyModifiedProperties();
    //         }
    //     }
    // }
}
