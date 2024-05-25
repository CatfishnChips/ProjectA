using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Rendering;

[CustomEditor(typeof(ActionAttack), true)]
public class ActionAttackEditor : Editor
{
    public VisualTreeAsset m_InspectorXML;
    private ActionAttack m_script;
    private string[] m_categoryTools = new string[4]{
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
        var nameProperty = serializedObject.FindProperty("name");
        EditorGUILayout.PropertyField(nameProperty);

        var fighterStateProperty = serializedObject.FindProperty("stateName");
        EditorGUILayout.PropertyField(fighterStateProperty);

        var inputGestureProperty = serializedObject.FindProperty("inputGesture");
        EditorGUILayout.PropertyField(inputGestureProperty);

        var applyRootMotionProperty = serializedObject.FindProperty("applyRootMotion");
        EditorGUILayout.PropertyField(applyRootMotionProperty);

        var cancelFrameProperty = serializedObject.FindProperty("_cancelFrame");
        EditorGUILayout.PropertyField(cancelFrameProperty);

        var inputIgnoreFramesProperty = serializedObject.FindProperty("_inputIgnoreFrames");
        EditorGUILayout.PropertyField(inputIgnoreFramesProperty);

        var tagsProperty = serializedObject.FindProperty("m_tags");
        EditorGUILayout.PropertyField(tagsProperty);

        var damageProperty = serializedObject.FindProperty("m_damage");
        EditorGUILayout.PropertyField(damageProperty);

        var jugglePropery = serializedObject.FindProperty("m_juggle");
        EditorGUILayout.PropertyField(jugglePropery);

        var faceProperty = serializedObject.FindProperty("m_face");
        EditorGUILayout.PropertyField(faceProperty);

        var priorityProperty = serializedObject.FindProperty("m_priority");
        EditorGUILayout.PropertyField(priorityProperty);

        var partProperty = serializedObject.FindProperty("m_part");
        EditorGUILayout.PropertyField(partProperty);

        var gravityProperty = serializedObject.FindProperty("m_gravity");
        EditorGUILayout.PropertyField(gravityProperty);

        var staminaProperty = serializedObject.FindProperty("m_staminaRecovery");
        EditorGUILayout.PropertyField(staminaProperty);

        var spiritProperty = serializedObject.FindProperty("m_spiritRecovery");
        EditorGUILayout.PropertyField(spiritProperty);

        var soundProperty = serializedObject.FindProperty("m_sound");
        EditorGUILayout.PropertyField(soundProperty);

        var soundLevelProperty = serializedObject.FindProperty("m_soundLevel");
        EditorGUILayout.PropertyField(soundLevelProperty);

        var screenShakeVelocityProperty = serializedObject.FindProperty("m_screenShakeVelocity");
        EditorGUILayout.PropertyField(screenShakeVelocityProperty);

        var cameraPositionProperty = serializedObject.FindProperty("m_cameraPosition");
        EditorGUILayout.PropertyField(cameraPositionProperty);

        var cameraRotationProperty = serializedObject.FindProperty("m_cameraRotation");
        EditorGUILayout.PropertyField(cameraRotationProperty);

        var cameraEaseFactorProperty = serializedObject.FindProperty("m_cameraEaseFactor");
        EditorGUILayout.PropertyField(cameraEaseFactorProperty);

        var startFramesProperty = serializedObject.FindProperty("m_startFrames");
        EditorGUILayout.PropertyField(startFramesProperty);

        var activeFramesProperty = serializedObject.FindProperty("m_activeFrames");
        EditorGUILayout.PropertyField(activeFramesProperty);

        var recoveryFramesProperty = serializedObject.FindProperty("m_recoveryFrames");
        EditorGUILayout.PropertyField(recoveryFramesProperty);

        var hitboxFrameProperty = serializedObject.FindProperty("m_hitboxFrame");
        EditorGUILayout.PropertyField(hitboxFrameProperty);

        var hitboxOffsetProperty = serializedObject.FindProperty("m_hitboxOffset");
        EditorGUILayout.PropertyField(hitboxOffsetProperty);

        var hitboxLocationProperty = serializedObject.FindProperty("m_hitboxLocation");
        EditorGUILayout.PropertyField(hitboxLocationProperty);

        var hitboxSizeProperty = serializedObject.FindProperty("m_hitboxSize");
        EditorGUILayout.PropertyField(hitboxSizeProperty);

        var meshAnimationStartupProperty = serializedObject.FindProperty("m_meshAnimationS");
        EditorGUILayout.PropertyField(meshAnimationStartupProperty);

        var meshAnimationActiveProperty = serializedObject.FindProperty("m_meshAnimationA");
        EditorGUILayout.PropertyField(meshAnimationActiveProperty);

        var meshAnimationRecoveryProperty = serializedObject.FindProperty("m_meshAnimationR");
        EditorGUILayout.PropertyField(meshAnimationRecoveryProperty);

        var boxAnimationStartupProperty = serializedObject.FindProperty("m_boxAnimationS");
        EditorGUILayout.PropertyField(boxAnimationStartupProperty);

        var boxAnimationActiveProperty = serializedObject.FindProperty("m_boxAnimationA");
        EditorGUILayout.PropertyField(boxAnimationActiveProperty);

        var boxAnimationRecoveryProperty = serializedObject.FindProperty("m_boxAnimationR");
        EditorGUILayout.PropertyField(boxAnimationRecoveryProperty);

        EditorGUILayout.Space();
        m_toolbarInt = GUILayout.Toolbar(m_toolbarInt, m_categoryTools);
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
    }
}
