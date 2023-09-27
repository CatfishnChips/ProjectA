using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationPreview), false)]
public class AnimationPreviewEditor : Editor
{
    private AnimationPreview m_script;
    private ActionAttack m_attack;
    private Vector2 m_location;
    private Vector2 m_characterPosition;
    private Vector2 m_origin;
    private bool m_axis3D = false;
    private Hitbox m_hitbox;

    private void OnEnable(){
        m_script = (AnimationPreview)serializedObject.targetObject;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Extras", EditorStyles.boldLabel);
        m_attack = (ActionAttack)EditorGUILayout.ObjectField(m_attack, typeof(ActionAttack), allowSceneObjects: false);
        m_axis3D = EditorGUILayout.Toggle("3D Axis? ", m_axis3D);
        EditorGUILayout.LabelField("3D Axis - Z is forward axis.    2D Axis - X is forward axis.", EditorStyles.miniLabel);
        if (GUILayout.Button("Override Property")){
            if (m_attack != null){
                
                m_hitbox = m_script.AnimationObjects[1].Object.transform.Find("Hitbox").GetComponent<Hitbox>();
                int frame = Mathf.Clamp(m_script.Frame, 0, m_script.AnimationObjects[0].FrameCount);
                float time = frame / m_script.AnimationObjects[0].FrameRate;
                m_script.AnimationObjects[0].Clip.SampleAnimation(m_script.AnimationObjects[0].Object, time);

                if (m_axis3D){
                    m_origin = new Vector2(m_script.transform.position.z, m_script.transform.position.y);
                    m_characterPosition = new Vector2(m_script.AnimationObjects[0].Object.transform.position.z, m_script.AnimationObjects[0].Object.transform.position.y);
                }
                else{
                    m_origin = new Vector2(m_script.transform.position.x, m_script.transform.position.y);
                    m_characterPosition = new Vector2(m_script.AnimationObjects[0].Object.transform.position.x, m_script.AnimationObjects[0].Object.transform.position.y);
                }

                m_location = m_characterPosition + m_hitbox.Offset - m_origin;
                
                m_attack.HitboxLocation = m_location;
                m_attack.HitboxOffset = m_hitbox.Offset;
                m_attack.HitboxSize = m_hitbox.Size;
                m_attack.HitboxFrame = m_script.Frame;
                Debug.Log(m_attack.name + "'s AI properties were set to Frame: " + "(" + m_script.Frame + ")" + " Location: " + m_location + " Offset: " + m_hitbox.Offset + " Size: " + m_hitbox.Size);
            }
        }
        if (GUILayout.Button("Reset to T-Pose")){
            if(m_script.TryGetAnimatorComponent(m_script.AnimationObjects[0].Object, out Animator animator)){
                bool a = false;
                foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips){
                    if (clip.name == "T_Pose"){
                        clip.SampleAnimation(m_script.AnimationObjects[0].Object, 0);
                        a = true;
                        break;
                    }
                }

                if (a) Debug.Log(m_script.AnimationObjects[0].Object.name + " is set to default T-Pose.");
                else Debug.Log(m_script.AnimationObjects[0].Object.name + " does not have any matching AnimationClip named T_Pose in its Animator.");
            }
            else{
                Debug.Log(m_script.AnimationObjects[0].Object.name + " has no attached Animator component.");
            }
        }
    }
}
