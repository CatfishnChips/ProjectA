using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionAttack))]
public class AttackMoveEditor : Editor
{
    private SerializedProperty _meshAnimationS, _meshAnimationA, _meshAnimationR, _boxAnimationS, _boxAnimationA, _boxAnimationR;
    private AnimationInfo _meshSClip, _meshAClip, _meshRClip, _boxSClip, _boxAClip, _boxRClip;

    private bool _status = false;

    private void OnEnable(){
        _meshAnimationS = serializedObject.FindProperty("m_meshAnimationS");
        _meshSClip.Clip = _meshAnimationS.objectReferenceValue as AnimationClip;

        _meshAnimationA = serializedObject.FindProperty("m_meshAnimationA");
        _meshAClip.Clip = _meshAnimationA.objectReferenceValue as AnimationClip;

        _meshAnimationR = serializedObject.FindProperty("m_meshAnimationR");
        _meshRClip.Clip = _meshAnimationR.objectReferenceValue as AnimationClip;
        
        _boxAnimationS = serializedObject.FindProperty("m_boxAnimationS");
        _boxSClip.Clip = _boxAnimationS.objectReferenceValue as AnimationClip;

        _boxAnimationA = serializedObject.FindProperty("m_boxAnimationA");
        _boxAClip.Clip = _boxAnimationA.objectReferenceValue as AnimationClip;

        _boxAnimationR = serializedObject.FindProperty("m_boxAnimationR");
        _boxRClip.Clip = _boxAnimationR.objectReferenceValue as AnimationClip;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorGUILayout.Space();
        _status = EditorGUILayout.BeginFoldoutHeaderGroup(_status, "Animation Info");
        using (new EditorGUI.DisabledScope(true))
        {
            if (_status)
            {
                DrawInfoPanel(_meshAnimationS, _meshSClip);
                DrawInfoPanel(_meshAnimationA, _meshAClip);
                DrawInfoPanel(_meshAnimationR, _meshRClip);
                DrawInfoPanel(_boxAnimationS, _boxSClip);
                DrawInfoPanel(_boxAnimationA, _boxAClip);
                DrawInfoPanel(_boxAnimationR, _boxRClip);
            }
        }
        
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawInfoPanel(SerializedProperty property, AnimationInfo animation){
        EditorGUILayout.PropertyField(property);

        if (animation.Clip != null) 
        {
            EditorGUILayout.LabelField("Frame Count: " + animation.FrameCount +  " Lenght: " + animation.Lenght + " Frame Rate: " + animation.FrameRate);
        }

        EditorGUILayout.Space();
    }
}

public struct AnimationInfo
{
    public AnimationClip Clip;
    public float FrameRate { get {return Clip != null ? Clip.frameRate : 0;} }
    public float Lenght { get {return Clip != null ? Clip.length : 0;} }
    public int FrameCount { get {return Mathf.FloorToInt(Lenght * FrameRate);} }
}
