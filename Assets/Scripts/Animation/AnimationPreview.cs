using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class AnimationPreview : MonoBehaviour
{
    [SerializeField] private int m_frame;
    [SerializeField] private bool m_manual;
    [SerializeField] private bool m_preview;
    private AnimationWindow _animationWindow;

    [SerializeField] private List<AnimationObject> m_animationObjects;

    private void OnEnable(){
        if (EditorApplication.isPlaying) return;
        if (!m_preview) return;

        _animationWindow = EditorWindow.GetWindow<AnimationWindow>();
    }

    private void Update(){
        if (EditorApplication.isPlaying) return;
        if (!m_preview) return;

        if (_animationWindow != null && !m_manual){
            m_frame = _animationWindow.frame;
        }

        foreach (AnimationObject animationObject in m_animationObjects){
            if (animationObject.Object == null) return;
            if (animationObject.Clip == null) return;

            int frame = Mathf.Clamp(m_frame, 0, animationObject.FrameCount);
            float time = frame / animationObject.FrameRate;
            animationObject.Clip.SampleAnimation(animationObject.Object, time);
        }        
    }
}

[System.Serializable]
public struct AnimationObject
{
    public GameObject Object;
    public AnimationClip Clip;
    public float FrameRate { get {return Clip != null ? Clip.frameRate : 0;} }
    public float Lenght { get {return Clip != null ? Clip.length : 0;} }
    public int FrameCount { get {return Mathf.FloorToInt(Lenght * FrameRate);} }
}
