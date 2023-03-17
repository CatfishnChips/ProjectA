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
    [ReadOnly] [SerializeField] private FighterStates _currentRootState = default;
    [ReadOnly] [SerializeField] private FighterStates _currentSubState = default;

    private HitResponder _hitResponder;
    private HurtResponder _hurtResponder;
    private Rigidbody2D _rigidbody2D;

    private HealthManager _healthManager;
    private StaminaManager _staminaManager;

    private Vector2 _velocity;

    #region Input Variables
    
    private Input<bool> _isJumpPressed = new Input<bool>(false);
    private Input<bool> _isDashPressed = new Input<bool>(false);
    private Input<bool> _isDodgePressed = new Input<bool>(false);
    private Input<bool> _isAttackPerformed = new Input<bool>(false);
    private ContinuousInput<float> _movementInput = new ContinuousInput<float>(0);
    private ContinuousInput<bool> _holdTouchA = new ContinuousInput<bool>(false);
    private ContinuousInput<bool> _holdTouchB = new ContinuousInput<bool>(false);

    #endregion

    [SerializeField] private int _inputDelay = 2; // Amount of time before registering an input.
    [SerializeField] private int _inputTimeout = 10; // in frames
    [SerializeField] private float _dashDistance = 1f;
    [SerializeField] private int _dashTime; // in frames
    [SerializeField] private float _airMoveSpeed;
    [SerializeField] private float _jumpHeight = 1f;
    [SerializeField] private int _jumpTime; // in frames
    [SerializeField] private float _fallMultiplier = 1f;
    [SerializeField] private int _dodgeTime; // in frames

    private bool _isGrounded;
    private string _attackName;
    private float _gravity;
    private float _deltaTarget;
    private CollisionData _collisionData;
    private bool _isHurt;
    private bool _isGravityApplied;
    private Vector2 _swipeDirection;
    [SerializeField] private LayerMask _rayCastLayerMask;
    [SerializeField] private float _rayCastLenght = 1f;
    [SerializeField] private Vector2 _rayCastPosition;
    private Vector2 _currentMovement;
    [SerializeField] private float _jumpDistance = 1f;

    public FighterStates CurrentRootState{get{return _currentRootState;} set{_currentRootState = value;}}
    public FighterStates CurrentSubState{get{return _currentSubState;} set{_currentSubState = value;}}

    public bool IsJumpPressed{get{return _isJumpPressed.Value;} set{_isJumpPressed.Value = value;}}
    public bool IsDashPressed{get{return _isDashPressed.Value;} set{_isDashPressed.Value = value;}}
    public bool IsDodgePressed{get{return _isDodgePressed.Value;} set {_isDodgePressed.Value = value;}}
    public bool IsGrounded{get{return _isGrounded;}}
    public bool IsHoldingTouchA{get{return _holdTouchA.Value;}}
    public bool IsHoldingTouchB{get{return _holdTouchB.Value;}}
    public bool AttackPerformed{get{return _isAttackPerformed.Value;} set{_isAttackPerformed.Value = value;}}
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
    public bool IsGravityApplied {get{return _isGravityApplied;} set{_isGravityApplied = value;}}
    public Rigidbody2D Rigidbody2D {get{return _rigidbody2D;}}
    
    public float JumpHeight {get{return _jumpHeight;}}
    public int JumpTime {get{return _jumpTime;}}
    public float FallMultiplier {get{return _fallMultiplier;}}
    public float Gravity {get{return _gravity;} set {_gravity = value;}}
    public float MovementInput {get{return _movementInput.Value;}}
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
        _isHurt = false;
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
            if (attribution.action.GetType() == typeof(ActionAttack) || attribution.action.GetType() == typeof(ActionContinuousAttack))
            {
                ActionAttack action = Instantiate(attribution.action) as ActionAttack;
                _attackMoveDict.Add(action.name, action);
            }
            else 
            {
                _actionDictionary.Add(attribution.action.name, attribution.action);
            }
        }

        if (TryGetComponent<HitResponder>(out HitResponder hitResponder)) _hitResponder = hitResponder;
        if (TryGetComponent<HurtResponder>(out HurtResponder hurtResponder)) _hurtResponder = hurtResponder;
        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2D)) _rigidbody2D = rigidbody2D;
        if (TryGetComponent<HealthManager>(out HealthManager healthManager)) _healthManager = healthManager;
        if (TryGetComponent<StaminaManager>(out StaminaManager staminaManager)) _staminaManager = staminaManager;
    }

    void Start()
    {
        EventManager.Instance.Move += ListenToMove;
        EventManager.Instance.Swipe += OnDash;
        EventManager.Instance.AttackMove += ListenToAttack;
        EventManager.Instance.OnTap += OnTapA;
        EventManager.Instance.OnHoldA += OnHoldA;
        EventManager.Instance.OnHoldB += OnHoldB;

        if(_hitResponder) _hitResponder.HitResponse += OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse += OnHurt;

        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    private void OnDisable() 
    {
        EventManager.Instance.Move -= ListenToMove;
        EventManager.Instance.Swipe -= OnDash;
        EventManager.Instance.AttackMove -= ListenToAttack;
        EventManager.Instance.OnTap -= OnTapA;
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
        //Debug.Log(_isGrounded);

        if(_comboListener.isActive){
            _comboListener.FixedUpdate();
        }
        _currentState.FixedUpdateStates();
    }

    private void Update(){
        _currentState.UpdateStates();
        //_velocity.x = Mathf.MoveTowards(_velocity.x, _deltaTarget, 1f * Time.deltaTime);
    }

    public void ListenToJump(){
        StartCoroutine(InputDelay(_isJumpPressed));
    }

    public void ListenToDash(){
        StartCoroutine(InputDelay(_isDashPressed));
    }

    public void ListenToDodge(){
        StartCoroutine(InputDelay(_isDodgePressed));
    }

    public void ListenToMove(float value){
        if (value < 0) value = -1;
        else if (value > 0) value = 1;

        InputDelayContinuous(_movementInput, value);
    }

    public void ListenToAttack(string attackName){
        if (attackName == "L") return; // Temporary Bugfix
        _attackName = attackName;
        StartCoroutine(InputDelay(_isAttackPerformed));
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
        _swipeDirection = direction;
        //Debug.Log("Swipe Direction: " + direction);

        if (direction.y <= -0.5f) {
            ListenToJump();
        }
        else if (direction.y < 0.5f && direction.y > -0.5f){
            ListenToDash();
        }
        else if (direction.y >= 0.5f){
            ListenToDodge();
        }
    }

    private void OnTapA(){

    }

    private void OnHoldA(bool value){
        InputDelayContinuous(_holdTouchA, value);
    }

    private void OnHoldB(bool value){
        InputDelayContinuous(_holdTouchB, value);
    }

    private void InputDelayContinuous<T>(ContinuousInput<T> input, T value){
        if(!input.TargetValue.Equals(value)){
            input.TargetValue = value;
            StartCoroutine(InputDelay(input));
        }
    }

    private IEnumerator InputDelay<T>(ContinuousInput<T> input){
        for (int i = 0; i < _inputDelay; i++){
            yield return new WaitForFixedUpdate();
        }
        input.Value = input.TargetValue;
    }

    private IEnumerator InputDelay(Input<bool> input){
        for (int i = 0; i < _inputDelay; i++){
            yield return new WaitForFixedUpdate();
        }
        if (input.Value) yield break;
        StartCoroutine(InputBuffer(input));
    }

    private IEnumerator InputBuffer(Input<bool> input){
        int currentFrame = 0;
        input.Value = true;

        while (input.Value) 
        {
            currentFrame++;
            if (currentFrame >= _inputTimeout){
                input.Value = false;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.down * _rayCastLenght);
    }
}

public class Input<T>
{
    private T _value;
    public T Value {get{return _value;} set{_value = value;}}
    public Input(T value){
        _value = value;
    }
}

public class ContinuousInput<T>
{
    private T _value;
    private T _targetValue;
    public T Value {get{return _value;} set{_value = value;}}
    public T TargetValue {get{return _targetValue;} set{_targetValue = value;}}
    public ContinuousInput(T value){
        _value = value;
        _targetValue = _value;
    }
}