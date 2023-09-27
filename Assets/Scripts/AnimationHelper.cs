using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[ExecuteInEditMode]
public class AnimationHelper : MonoBehaviour
{
    //m_script = (AnimationPreview)serializedObject.targetObject;
    [SerializeField] private Animator animator;

    void CreateTPose() {
        var recorder = new GameObjectRecorder(animator.gameObject);
        recorder.BindComponentsOfType<Transform>(animator.gameObject, true);
        recorder.TakeSnapshot(0f);
        var tempClip = new AnimationClip();
        CurveFilterOptions filterOptions = new CurveFilterOptions();
        recorder.SaveToClip(tempClip, 30, filterOptions);
        AssetDatabase.CreateAsset(tempClip, EditorUtility.SaveFilePanel("Save", Application.dataPath, "test", "anim").Replace(Application.dataPath, "Assets"));
    }
}
