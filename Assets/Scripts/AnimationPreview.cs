#if UNITY_EDITOR
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
    [SerializeField] private bool m_lockX;
    [SerializeField] private bool m_lockY;
    [SerializeField] private Vector3 m_positionOffset;
    [SerializeField] private Vector3 m_rotationOffset;
    private UnityEditor.AnimationWindow _animationWindow;
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

                Vector3 position;
                position.x = m_lockX ? 0f : animationObject.Object.transform.position.x;
                position.y = m_lockY ? 0f : animationObject.Object.transform.position.y;
                position.z = m_lockZ ? 0f : animationObject.Object.transform.position.z;
                animationObject.Object.transform.position = position;
  
                if (animationObject.ApplyPositionOffset) animationObject.Object.transform.position += m_positionOffset;
                if (animationObject.ApplyRotationOffset) animationObject.Object.transform.Rotate(m_rotationOffset);
            }  
        }

        m_prevFrame = m_frame;      
    }

    public bool TryGetAnimatorComponent(GameObject obj, out Animator animator){
        animator = null;
        if (obj == null) return false;
        return obj.TryGetComponent(out animator);
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
#endif