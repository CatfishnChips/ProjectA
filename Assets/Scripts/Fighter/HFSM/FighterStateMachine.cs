using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) {}

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}

public class FighterStateMachine : MonoBehaviour
{

    [SerializeField] private ActionAttribution[] _actionAttribution;
    private Dictionary<string, ActionAttack> _attackMoveDict;
    private Dictionary<string, ActionBase> _actionDictionary;

    [SerializeField] private ComboMove[] _combosArray;
    private ComboListener _comboListener;

    private Animator _animator;
    private AnimatorOverrideController _animOverrideCont;
    private AnimationClipOverrides _clipOverrides;

    private Animator _colBoxAnimator;
    private AnimatorOverrideController _colBoxOverrideCont;
    private AnimationClipOverrides _colBoxClipOverrides;

    private FighterStateFactory _states;
    private FighterBaseState _currentState;

    private HitResponder _hitResponder;
    private HurtResponder _hurtResponder;
    private Rigidbody2D _rigidbody2D;

    private HealthManager _healthManager;
    private StaminaManager _staminaManager;

    private Vector2 _velocity;
    private bool _isJumpPressed;
    private bool _isDashPressed;
    private bool _isDodgePressed;
    private bool _isGrounded;
    private bool _attackPerformed;
    private string _attackName;
    [SerializeField] string _currentStateName;
    [SerializeField] string _currentSubStateName;
    [SerializeField] string _currentSuperStateName;
    [SerializeField] private float _dashDistance = 1f;
    [SerializeField] private int _dashTime; // in frames
    [SerializeField] private float _airMoveSpeed;
    [SerializeField] private int _inputTimeoutTime = 10; // in frames
    [SerializeField] private float _jumpHeight = 1f;
    [SerializeField] private int _jumpTime; // in frames
    [SerializeField] private float _fallMultiplier = 1f;
    [SerializeField] private int _dodgeTime; // in frames
    private float _gravity;
    private float _deltaTarget;
    private CollisionData _collisionData;
    private bool _isHurt;
    private bool _isInputLocked;
    private bool _isGravityApplied;
    private Vector2 _swipeDirection;
    [SerializeField] private LayerMask _rayCastLayerMask;
    [SerializeField] private float _rayCastLenght = 1f;
    [SerializeField] private Vector2 _rayCastPosition;
    private Vector2 _currentMovement;
    [SerializeField] private float _jumpDistance = 1f;

    public bool IsJumpPressed{get{return _isJumpPressed;} set{_isJumpPressed = value;}}
    public bool IsDashPressed{get{return _isDashPressed;} set{_isDashPressed = value;}}
    public bool IsDodgePressed{get{return _isDodgePressed;} set {_isDodgePressed = value;}}
    public bool IsGrounded{get{return _isGrounded;}}
    public bool AttackPerformed{get{return _attackPerformed;} set{_attackPerformed = value;}}
    public string AttackName{get{return _attackName;} set{_attackName = value;}}
    public Vector2 Velocity{get{return _velocity;} set{_velocity = value;}}
    public float AirMoveSpeed{get{return _airMoveSpeed;}}
    public FighterBaseState CurrentState{get{return _currentState;} set{_currentState = value;}}

    public Animator Animator{get{return _animator;}}
    public AnimatorOverrideController AnimOverrideCont{get{return _animOverrideCont;} set{_animOverrideCont = value;}}
    public AnimationClipOverrides ClipOverrides{get{return _clipOverrides;}}

    public Animator ColBoxAnimator{get{return _colBoxAnimator;}}
    public AnimatorOverrideController ColBoxOverrideCont{get{return _colBoxOverrideCont;} set{_colBoxOverrideCont = value;}}
    public AnimationClipOverrides ColBoxClipOverrides{get{return _colBoxClipOverrides;}}

    public Dictionary<string, ActionAttack> AttackMoveDict{get{return _attackMoveDict;}}
    public Dictionary<string, ActionBase> ActionDictionary{get{return _actionDictionary;}}
    public ComboListener ComboListener{get{return _comboListener;} set{_comboListener = value;}}
    public ComboMove[] CombosArray{get{return _combosArray;}}

    public HitResponder HitResponder {get{return _hitResponder;}}
    public HurtResponder HurtResponder {get{return _hurtResponder;}}
    public CollisionData CollisionData {get{return _collisionData;}}
    public HealthManager HealthManager {get{return _healthManager;}}
    public StaminaManager StaminaManager {get{return _staminaManager;}}
    public bool IsHurt {get{return _isHurt;} set{_isHurt = value;}}
    public bool IsInputLocked {get{return _isInputLocked;} set{_isInputLocked = value;}}
    public bool IsGravityApplied {get{return _isGravityApplied;} set{_isGravityApplied = value;}}
    public Rigidbody2D Rigidbody2D {get{return _rigidbody2D;}}
    
    public float JumpHeight {get{return _jumpHeight;}}
    public int JumpTime {get{return _jumpTime;}}
    public float FallMultiplier {get{return _fallMultiplier;}}
    public float Gravity {get{return _gravity;} set {_gravity = value;}}
    public float DeltaTarget {get{return _deltaTarget;}}
    public Vector2 SwipeDirection {get{return _swipeDirection;}}
    public Vector2 CurrentMovement {get{return _currentMovement;} set{_currentMovement = value;}}
    public float JumpDistance {get{return _jumpDistance;}}
    public float DashDistance {get{return _dashDistance;}}
    public int DashTime {get{return _dashTime;}}
    public int DodgeTime {get{return _dodgeTime;}}

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _colBoxAnimator = transform.Find("Hurtboxes").GetComponent<Animator>();
        _isJumpPressed = false;
        _isDodgePressed = false;
        _isDashPressed = false;
        _attackPerformed = false;
        _isHurt = false;
        _isInputLocked = false;
        _isGravityApplied = true;
        _gravity = Physics2D.gravity.y;
        _states = new FighterStateFactory(this);
        _comboListener = new ComboListener(this);
        
        _animOverrideCont = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animOverrideCont;

        _clipOverrides = new AnimationClipOverrides(_animOverrideCont.overridesCount);
        _animOverrideCont.GetOverrides(_clipOverrides);

        _colBoxOverrideCont = new AnimatorOverrideController(_colBoxAnimator.runtimeAnimatorController);
        _colBoxAnimator.runtimeAnimatorController = _colBoxOverrideCont;

        _colBoxClipOverrides = new AnimationClipOverrides(_colBoxOverrideCont.overridesCount);
        _colBoxOverrideCont.GetOverrides(_colBoxClipOverrides);

        _attackMoveDict = new Dictionary<string, ActionAttack>();
        _actionDictionary = new Dictionary<string, ActionBase>();

        foreach (ActionAttribution attribution in _actionAttribution)
        {
            if (attribution.action.GetType() == typeof(ActionAttack))
            {
                _attackMoveDict.Add(attribution.action.name, attribution.action as ActionAttack);
            }
            else 
            {
                _actionDictionary.Add(attribution.action.name, attribution.action);
            }

            Debug.Log("Action " + attribution.action.name + " has been added to the dictionary.");
            
        }

        if (TryGetComponent<HitResponder>(out HitResponder hitResponder)) _hitResponder = hitResponder;
        if (TryGetComponent<HurtResponder>(out HurtResponder hurtResponder)) _hurtResponder = hurtResponder;
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2D)) _rigidbody2D = rigidbody2D;
        if (TryGetComponent<HealthManager>(out HealthManager healthManager)) _healthManager = healthManager;
        if (TryGetComponent<StaminaManager>(out StaminaManager staminaManager)) _staminaManager = staminaManager;
    }

    void Start()
    {
        EventManager.Instance.Walk += OnMove;
        EventManager.Instance.Dash += OnDash;
        EventManager.Instance.AttackMove += OnAttack;
        EventManager.Instance.OnTap += OnTap;
        EventManager.Instance.OnHoldA += OnHoldA;
        EventManager.Instance.OnHoldB += OnHoldB;

        if(_hitResponder) _hitResponder.HitResponse += OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse += OnHurt;

        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void OnDisable() 
    {
        EventManager.Instance.Walk -= OnMove;
        EventManager.Instance.Dash -= OnDash;
        EventManager.Instance.AttackMove -= OnAttack;
        EventManager.Instance.OnTap -= OnTap;
        EventManager.Instance.OnHoldA -= OnHoldA;
        EventManager.Instance.OnHoldB -= OnHoldB;

        if(_hitResponder) _hitResponder.HitResponse -= OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse -= OnHurt;

        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        _isGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + _rayCastPosition, Vector2.down, _rayCastLenght, _rayCastLayerMask);
        //_isGrounded = _rigidbody2D.IsTouchingLayers(_rayCastLayerMask);
        Debug.Log(_isGrounded);

        if(_comboListener.isActive){
            _comboListener.FixedUpdate();
        }
        _currentState.FixedUpdateStates();
        _currentStateName = _currentState.StateName;
        _currentSubStateName = _currentState.SubStateName();
        _currentSubStateName = _currentState.SuperStateName();
    }

    private void Update(){
        _currentState.UpdateStates();

        //_velocity.x = Mathf.MoveTowards(_velocity.x, _deltaTarget, 1f * Time.deltaTime);
    }

    public void ListenToJump(){
        if (_isInputLocked) return;
        if (_isJumpPressed) return;
        if (!_isGrounded) return;
        //if (_currentSubStateName == "Stunned" || _currentSubStateName == "Attack") return;
        _isJumpPressed = true;

        //StartCoroutine(InputTimeout(ref _isJumpPressed));
    }

    public void ListenToDash(){
        if (_isInputLocked) return;
        if (_isDashPressed) return;
        _isDashPressed = true;
    }

    public void ListenToDodge(){
        if (_isInputLocked) return;
        if (_isDodgePressed) return;
        _isDodgePressed = true;
    }

    public void OnMove(float delta){
        _deltaTarget = delta;
    }

    public void OnAttack(string attackName){
        if (_isInputLocked) return;
        if (_attackPerformed) return;
        //if (_currentSubStateName == "Stunned" || _currentSubStateName == "Attack") return;
        if (attackName == "L") return; // Temporary Bugfix
        _attackName = attackName;
        _attackPerformed = true;
        //StartCoroutine(InputTimeout(ref _attackPerformed));
    }

    public void OnHit(CollisionData data){
        TimeController.Instance.SlowDown();
        CameraController.Instance.ScreenShake(data.action.ScreenShakeVelocity);
    }

    public void OnHurt(CollisionData data){
        if (_isHurt) return;
        _collisionData = data;
        _isHurt = true;
    }

    private void OnDash(Vector2 direction){
        if (_isInputLocked) return;
        _swipeDirection = direction;
        Debug.Log("Swipe Direction: " + direction);

        if (direction.y <= -0.5f) {
            ListenToJump();
        }
        else if (direction.y < 0.5f && direction.y > -0.5f){
            ListenToDash();
        }
        else if (direction.y >= 0.5f){
            ListenToDodge();
        }

        //if (_currentSubStateName == "Stunned" || _currentSubStateName == "Attack") return;

        //StartCoroutine(InputTimeout());
    }

    private void OnTap(){

    }

    private void OnHoldA(){

    }

    private void OnHoldB(){

    }

    // private IEnumerator InputTimeout(ref bool inputPerformed){
    //     int currentFrame = 0;

    //     while (inputPerformed) 
    //     {
    //         yield return new WaitForFixedUpdate();

    //         if (currentFrame >= _inputTimeoutTime){
    //             inputPerformed = false;
    //             break;
    //         }
    //         currentFrame++;
    //     }
    //     yield return null;
    // }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.down * _rayCastLenght);
    }
}
