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
    [SerializeField] private bool m_lockZ;
    [SerializeField] private Vector3 m_positionOffset;
    [SerializeField] private Vector3 m_rotationOffset;
    private AnimationWindow _animationWindow;
    private int m_prevFrame;

    [SerializeField] private List<AnimationObject> m_animationObjects;
    
    public int Frame {get => m_frame;}
    public List<AnimationObject> AnimationObjects {get => m_animationObjects;}

    private void OnEnable(){
        if (EditorApplication.isPlaying) return;
        if (!m_preview) return;
        if (m_manual) return;
        _animationWindow = EditorWindow.GetWindow<AnimationWindow>();
    }

    private void Update(){
        if (EditorApplication.isPlaying) return;
        if (!m_preview) return;

        if (m_prevFrame != m_frame){
            if (_animationWindow != null && !m_manual){
                m_frame = _animationWindow.frame;
            }

            foreach (AnimationObject animationObject in m_animationObjects){
                if (animationObject.Object == null) return;
                if (animationObject.Clip == null) return;

                int frame = Mathf.Clamp(m_frame, 0, animationObject.FrameCount);
                float time = frame / animationObject.FrameRate;
                animationObject.Clip.SampleAnimation(animationObject.Object, time);
                if (m_lockZ) animationObject.Object.transform.position = new Vector3(animationObject.Object.transform.position.x, animationObject.Object.transform.position.y, 0f);
                if (animationObject.ApplyPositionOffset) animationObject.Object.transform.position += m_positionOffset;
                if (animationObject.ApplyRotationOffset) animationObject.Object.transform.Rotate(m_rotationOffset);
            }  
        }

        m_prevFrame = m_frame;      
    }
}

[System.Serializable]
public struct AnimationObject
{
    public GameObject Object;
    public AnimationClip Clip;
    public bool ApplyPositionOffset;
    public bool ApplyRotationOffset;
    public float FrameRate { get {return Clip != null ? Clip.frameRate : 0;} }
    public float Lenght { get {return Clip != null ? Clip.length : 0;} }
    public int FrameCount { get {return Mathf.FloorToInt(Lenght * FrameRate);} }
}
