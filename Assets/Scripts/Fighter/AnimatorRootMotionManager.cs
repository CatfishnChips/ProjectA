using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorRootMotionManager : MonoBehaviour
{
    [SerializeField] private bool m_applyRootMotion;
    private Animator m_animator;
    private FighterStateMachine m_stateMachine;

    private void Awake(){
        m_animator = GetComponent<Animator>();
        m_stateMachine = GetComponentInParent<FighterStateMachine>();
    }

    private void Start(){
        m_animator.applyRootMotion = m_applyRootMotion;
        m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void OnAnimatorMove(){
        //m_rigidbody.MovePosition(transform.position + m_animator.deltaPosition);

        m_stateMachine.RootMotion = m_animator.deltaPosition / Time.fixedDeltaTime;
    }
}
