using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AirProperties))]
public class AirPropertiesCustomDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLine = 64;

        var trajectoryProp = property.FindPropertyRelative("HitTrajectory");
        int value = trajectoryProp.enumValueIndex;

        switch (value){
            case 1:
                totalLine += 6; 
            break;

            case 2:
                totalLine += 12;
            break;
        }

        var hitFlagProp = property.FindPropertyRelative("HitTrajectory");
        value = hitFlagProp.enumValueFlag;
        GroundedHitFlags hitFlags = (GroundedHitFlags)value;

        // BOUNCE_WALL
        if (hitFlags.HasFlag(GroundedHitFlags.BOUNCE_WALL)){
            var wallBounceProp = property.FindPropertyRelative("WallBounce");
            if (wallBounceProp.isExpanded){
                totalLine += 24;    
            }
        }

        // BOUNCE_GROUND
        if (hitFlags.HasFlag(GroundedHitFlags.BOUNCE_GROUND)){
            var groundBounceProp = property.FindPropertyRelative("GroundBounce");
            if (groundBounceProp.isExpanded){
                totalLine += 12;
            }
            
        }

        // SPLAT_WALL
        if (hitFlags.HasFlag(GroundedHitFlags.SPLAT_WALL)){
            var wallSplatProp = property.FindPropertyRelative("WallSplat");
            if (wallSplatProp.isExpanded){
                totalLine += 12;
            }
        }
    
        return EditorGUIUtility.singleLineHeight * totalLine + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        EditorGUI.BeginProperty(position, label, property);
        Rect rect;

       var stunProp = property.FindPropertyRelative("Stun");
        rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(stunProp, label, true));
        height += EditorGUI.GetPropertyHeight(stunProp, label, true) + EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, stunProp, true);

        var trajectoryProp = property.FindPropertyRelative("HitTrajectory");
        rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(trajectoryProp, label, true));
        height += EditorGUI.GetPropertyHeight(trajectoryProp, label, false);
        EditorGUI.PropertyField(rect, trajectoryProp, false);
        int trajectoryInt = trajectoryProp.enumValueIndex;
      
        switch (trajectoryInt){
            case 2: 
                var slideProp = property.FindPropertyRelative("Trajectory").FindPropertyRelative("Arc");
                rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(slideProp, label, true));
                height += EditorGUI.GetPropertyHeight(slideProp, label, true);
                EditorGUI.PropertyField(rect, slideProp, true);
            break;

            case 1:
                var trajProp = property.FindPropertyRelative("Trajectory").FindPropertyRelative("Line");
                rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(trajProp, label, true));
                height += EditorGUI.GetPropertyHeight(trajProp, label, true);
                EditorGUI.PropertyField(rect, trajProp, true);
            break;
        }

        height += EditorGUIUtility.singleLineHeight;
       
        var hitFlagProp = property.FindPropertyRelative("HitFlags");
        rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(hitFlagProp, label, false));
        height += EditorGUI.GetPropertyHeight(hitFlagProp, label, false) + EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, hitFlagProp, false);
        int hitFlagsInt = hitFlagProp.enumValueFlag;
        GroundedHitFlags hitFlags = (GroundedHitFlags)hitFlagsInt;

        // BOUNCE_WALL
        if (hitFlags.HasFlag(GroundedHitFlags.BOUNCE_WALL)){
            var wallBounceProp = property.FindPropertyRelative("WallBounce");
            rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(wallBounceProp, label, true));
            height += EditorGUI.GetPropertyHeight(wallBounceProp, label, true) + EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, wallBounceProp, true);
        }

        // BOUNCE_GROUND
        if (hitFlags.HasFlag(GroundedHitFlags.BOUNCE_GROUND)){
            var groundBounceProp = property.FindPropertyRelative("GroundBounce");
            rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(groundBounceProp, label, true));
            height += EditorGUI.GetPropertyHeight(groundBounceProp, label, true) + EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, groundBounceProp, true);
            
        }

        // SPLAT_WALL
        if (hitFlags.HasFlag(GroundedHitFlags.SPLAT_WALL)){
            var wallSplatProp = property.FindPropertyRelative("WallSplat");
            rect = new Rect(position.x, position.y + height, position.width, EditorGUI.GetPropertyHeight(wallSplatProp, label, true));
            height += EditorGUI.GetPropertyHeight(wallSplatProp, label, true) +EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, wallSplatProp, true);
        }
    
        EditorGUI.EndProperty();
    }
}
