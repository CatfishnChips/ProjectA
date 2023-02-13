using UnityEngine;

public class FighterStateMachine : MonoBehaviour
{

    private CharacterController _charController;
    private Animator _animator;

    private FighterStateFactory _states;
    private FighterBaseState _currentState;


    private bool _isJumpPressed;
    private bool _isGrounded;

    public bool IsJumpPressed{get{return _isJumpPressed;}}
    public bool IsGrounded{get{return _isGrounded;}}
    public FighterBaseState CurrentState{get{return _currentState;} set{_currentState = value;}}

    void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _isJumpPressed = false;
        _states = new FighterStateFactory(this);
        _currentState = _states.Grounded();

    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateState();
        _isGrounded = _charController.isGrounded;
    }

    private void ApplyGravity(){

    }

    public void ListenToJump(){
        _isJumpPressed = true;
    }
}
