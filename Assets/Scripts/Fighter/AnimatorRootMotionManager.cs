using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorRootMotionManager : MonoBehaviour
{
    [SerializeField] private bool m_applyRootMotion;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody;
    private FighterStateMachine m_stateMachine;

    private void Awake(){
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponentInParent<Rigidbody2D>();
        m_stateMachine = GetComponentInParent<FighterStateMachine>();
    }

    private void Start(){
        m_animator.applyRootMotion = m_applyRootMotion;
    }

    private void OnAnimatorMove(){
        //m_rigidbody.MovePosition(transform.position + m_animator.deltaPosition);

        // Instead of directly changing the velocity of rigidbody, set the velocity of the StateMachine.
        // In order to do this, FighterWalkState must be modified.
        m_stateMachine.RootMotion = m_animator.deltaPosition / Time.fixedDeltaTime;
        //m_rigidbody.velocity = m_animator.deltaPosition / Time.fixedDeltaTime;
    }
}
