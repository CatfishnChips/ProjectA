#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[ExecuteInEditMode]
public class AnimationHelper : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void CreateTPose() {
        var recorder = new GameObjectRecorder(animator.gameObject);
        recorder.BindComponentsOfType<Transform>(animator.gameObject, true);
        recorder.TakeSnapshot(0f);
        var tempClip = new AnimationClip();
        CurveFilterOptions filterOptions = new CurveFilterOptions();
        recorder.SaveToClip(tempClip, 30, filterOptions);
        AssetDatabase.CreateAsset(tempClip, EditorUtility.SaveFilePanel("Save", Application.dataPath, "test", "anim").Replace(Application.dataPath, "Assets"));
    }
}
#endif