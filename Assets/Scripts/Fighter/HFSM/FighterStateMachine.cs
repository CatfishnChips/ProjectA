using UnityEngine;

public class FighterStateMachine : MonoBehaviour
{

    private CharacterController _charController;
    private Animator _animator;

    private FighterStateFactory _states;
    private FighterBaseState _currentState;


    private Vector2 _velocity;
    private bool _isJumpPressed;
    private bool _isGrounded;
    private bool _uppercutPerformed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _moveSpeed;
    private float _deltaTarget;

    public bool IsJumpPressed{get{return _isJumpPressed;}}
    public bool IsGrounded{get{return _isGrounded;}}
    public bool UppercutPerformed{get{return _uppercutPerformed;} set{_uppercutPerformed = value;}}
    public CharacterController CharController{get{return _charController;}}
    public Vector2 Velocity{get{return _velocity;}}
    public float MoveSpeed{get{return _moveSpeed;}}
    public FighterBaseState CurrentState{get{return _currentState;} set{_currentState = value;}}
    public Animator Animator{get{return _animator;}}

    void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _isJumpPressed = false;
        _uppercutPerformed = false;
        _states = new FighterStateFactory(this);
        _currentState = _states.Grounded();

    }

    void Start()
    {
        EventManager.Instance.Walk += OnWalk;
        EventManager.Instance.Dash += OnDash;
        EventManager.Instance.Uppercut += OnUppercut;
    }

    private void OnDisable() 
    {
        EventManager.Instance.Walk -= OnWalk;
        EventManager.Instance.Dash -= OnDash;
        EventManager.Instance.Uppercut -= OnUppercut;

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        _currentState.UpdateState();
        _isGrounded = _charController.isGrounded;
    }

    private void Update(){
        _velocity.x = Mathf.MoveTowards(_velocity.x, _deltaTarget, 0.75f * Time.deltaTime);
    }


    public void ListenToJump(){
        _isJumpPressed = true;
    }

    public void OnWalk(float delta){
        _deltaTarget = delta;
    }

    public void OnUppercut(){
        _uppercutPerformed = true;
    }

    private void OnDash(Vector2 direction) 
    {
        //_velocity.x = direction.x * _dashDistance;
    }
}
