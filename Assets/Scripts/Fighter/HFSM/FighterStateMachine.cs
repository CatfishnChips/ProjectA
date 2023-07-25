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
    [SerializeField] private Player _player;
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
    [ReadOnly] [SerializeField] private FighterStates _previousRootState = default;
    [ReadOnly] [SerializeField] private FighterStates _previousSubState = default;
    [ReadOnly] [SerializeField] private ActionStates _actionState = default;

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
    private ContinuousInput<float> _movementInput = new ContinuousInput<float>(0);
    private ContinuousInput<bool> _holdTouchA = new ContinuousInput<bool>(false);
    private ContinuousInput<bool> _holdTouchB = new ContinuousInput<bool>(false);
    private QueueInput<bool, string> _isAttackPerformed = new QueueInput<bool, string>(false);

    #endregion

    [SerializeField] private int _comboBuffer = 15;
    [SerializeField] private int _inputDelay = 2; // Amount of time before registering an input.
    [SerializeField] private int _inputBuffer = 10; // in frames
    [SerializeField] private float _dashDistance = 1f;
    [SerializeField] private int _dashTime; // in frames
    [SerializeField] private int _dodgeTime; // in frames

    [Header("Airborne State")]
    [SerializeField] private float _airMoveSpeed;
    [SerializeField] private float _jumpHeight = 1f;
    [SerializeField] private float _jumpDistance = 1f;
    [SerializeField] private int _jumpTime; // in frames
    [SerializeField] private int _fallTime; // in frames

    [Header("Raycast")]
    [SerializeField] [ReadOnly] bool _isGrounded;
    [SerializeField] private LayerMask _rayCastLayerMask;
    [SerializeField] private float _rayCastLenght = 1f;
    [SerializeField] private Vector2 _rayCastOffset;

    [Space]
    [SerializeField] [ReadOnly] private Vector2 _currentMovement;
    [SerializeField] [ReadOnly] private int _faceDirection; // -1 Left, +1 Right
    [SerializeField] [ReadOnly] private float _gravity;
    [SerializeField] [ReadOnly] private float _drag;

    private CollisionData _hurtCollisionData;
    private CollisionData _hitCollisionData;
    private bool _isHit;
    private bool _isHurt;
    private bool _isGravityApplied;
    private Vector2 _swipeDirection;
    private bool _validAttackInputInterval;

    public Player Player {get{return _player;}}

    public FighterStates CurrentRootState{get{return _currentRootState;} set{_currentRootState = value;}}
    public FighterStates CurrentSubState{get{return _currentSubState;} set{_currentSubState = value;}}
    public FighterStates PreviousRootState{get{return _previousRootState;} set{_previousRootState = value;}}
    public FighterStates PreviousSubState{get{return _previousSubState;} set{_previousSubState = value;}}
    public ActionStates ActionState{get{return _actionState;} set{_actionState = value;}}

    public bool IsJumpPressed{get{return _isJumpPressed.Value;} set{_isJumpPressed.Value = value;}}
    public bool IsDashPressed{get{return _isDashPressed.Value;} set{_isDashPressed.Value = value;}}
    public bool IsDodgePressed{get{return _isDodgePressed.Value;} set {_isDodgePressed.Value = value;}}
    public bool IsGrounded{get{return _isGrounded;}}
    public bool IsHoldingTouchA{get{return _holdTouchA.Value;}}
    public bool IsHoldingTouchB{get{return _holdTouchB.Value;}}
    public bool AttackPerformed{get{return _isAttackPerformed.Value;} set{_isAttackPerformed.Value = value;}}
    public string AttackName{get{return _isAttackPerformed.Queue.Peek();}}
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
    public int ComboBuffer{get{return _comboBuffer;}}

    public HitResponder HitResponder {get{return _hitResponder;}}
    public HurtResponder HurtResponder {get{return _hurtResponder;}}
    public CollisionData HurtCollisionData {get{return _hurtCollisionData;}}
    public CollisionData HitCollisionData {get{return _hitCollisionData;}}
    public HealthManager HealthManager {get{return _healthManager;}}
    public StaminaManager StaminaManager {get{return _staminaManager;}}
    public bool IsHit {get{return _isHit;} set{_isHit = value;}}
    public bool IsHurt {get{return _isHurt;} set{_isHurt = value;}}
    public bool CanBlock {get{return _staminaManager.CanBlock && ( (_currentRootState == FighterStates.Grounded || _previousRootState == FighterStates.Grounded) && (_currentSubState == FighterStates.Idle || _currentSubState == FighterStates.Walk));}}
    public bool IsGravityApplied {get{return _isGravityApplied;} set{_isGravityApplied = value;}}
    public Rigidbody2D Rigidbody2D {get{return _rigidbody2D;}}
    
    public int GetInputBuffer { get { return _inputBuffer; } }
    public float JumpHeight {get{return _jumpHeight;}}
    public int JumpTime {get{return _jumpTime;}}
    public int FallTime {get{return _fallTime;}}
    public float Gravity {get{return _gravity;} set{_gravity = value;}}
    public float Drag {get{return _drag;} set{_drag = value;}}
    public float MovementInput {get{return _movementInput.Value;}}
    public Vector2 SwipeDirection {get{return _swipeDirection * _faceDirection;}} // SwipeDirection is affected by FaceDirection
    public Vector2 CurrentMovement {get{return _currentMovement;} set{_currentMovement = value;}}
    public float JumpDistance {get{return _jumpDistance;}}
    public float DashDistance {get{return _dashDistance;}}
    public int DashTime {get{return _dashTime;}}
    public int DodgeTime {get{return _dodgeTime;}}
    public int FaceDirection {get{return _faceDirection;}}
    public bool ValidAttackInputInterval { get { return _validAttackInputInterval; } set { _validAttackInputInterval = value; } }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _colBoxAnimator = transform.Find("Hurtboxes").GetComponent<Animator>();
        
        _states = new FighterStateFactory(this);
        _comboListener = new ComboListener(this);
        //_faceDirection = (int)Mathf.Sign(transform.forward.x);
        
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

        Reset();
    }

    void Start()
    {
        // Subscribe to Event Manager base events.
        switch(_player){
            case Player.P1:
                EventManager.Instance.Move += OnMoveA;
                EventManager.Instance.Swipe += OnDash;
                EventManager.Instance.AttackMove += OnGestureB;
                EventManager.Instance.OnTap += OnTapA;
                EventManager.Instance.OnHoldA += OnHoldA;
                EventManager.Instance.OnHoldB += OnHoldB;
            break;

            case Player.P2:
                EventManager.Instance.P2Move += OnMoveA;
                EventManager.Instance.P2Swipe += OnDash;
                EventManager.Instance.P2AttackMove += OnGestureB;
                EventManager.Instance.P2OnTap += OnTapA;
                EventManager.Instance.P2OnHoldA += OnHoldA;
                EventManager.Instance.P2OnHoldB += OnHoldB;
            break;

            default:
                // Do no subscribe to any input events.
            break;
        }

        // Subscribe to component based events.
        if(_hitResponder) _hitResponder.HitResponse += OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse += OnHurt;

        // Start default state.
        _currentState = _states.Airborne();
        _currentState.EnterState();
    }

    private void OnDisable() 
    {
        switch(_player){
            case Player.P1:
                EventManager.Instance.Move -= OnMoveA;
                EventManager.Instance.Swipe -= OnDash;
                EventManager.Instance.AttackMove -= OnGestureB;
                EventManager.Instance.OnTap -= OnTapA;
                EventManager.Instance.OnHoldA -= OnHoldA;
                EventManager.Instance.OnHoldB -= OnHoldB;
            break;

            case Player.P2:
                EventManager.Instance.P2Move -= OnMoveA;
                EventManager.Instance.P2Swipe -= OnDash;
                EventManager.Instance.P2AttackMove -= OnGestureB;
                EventManager.Instance.P2OnTap -= OnTapA;
                EventManager.Instance.P2OnHoldA -= OnHoldA;
                EventManager.Instance.P2OnHoldB -= OnHoldB;
            break;

            default:
                EventManager.Instance.Move -= OnMoveA;
                EventManager.Instance.Swipe -= OnDash;
                EventManager.Instance.AttackMove -= OnGestureB;
                EventManager.Instance.OnTap -= OnTapA;
                EventManager.Instance.OnHoldA -= OnHoldA;
                EventManager.Instance.OnHoldB -= OnHoldB;
            break;
        }
        
        // Component based events.
        if(_hitResponder) _hitResponder.HitResponse -= OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse -= OnHurt;

        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        _isGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + _rayCastOffset, Vector2.down, _rayCastLenght, _rayCastLayerMask);
        _faceDirection = (int)Mathf.Sign(transform.forward.x);

        if(_comboListener.isActive){
            _comboListener.FixedUpdate();
        }
        _currentState.FixedUpdateStates();
    }

    private void Update(){
        //_currentState.UpdateStates();
    }

    #region Input Functions

    private void OnDash(Vector2 direction){
        _swipeDirection = direction;
        _swipeDirection.x *= _faceDirection;
        //Debug.Log("Swipe Direction: " + direction);

        if (direction.y <= -0.5f) {
            if (direction.y <= -0.9f) _swipeDirection.x = 0f;
            //if (_isJumpPressed.Value) return;
            StartCoroutine(InputDelay(_isJumpPressed));
        }
        else if (direction.y < 0.5f && direction.y > -0.5f){
            //if (_isDashPressed.Value) return;
            StartCoroutine(InputDelay(_isDashPressed));
        }
        else if (direction.y >= 0.5f){
            //if(_isDodgePressed.Value) return;
            StartCoroutine(InputDelay(_isDodgePressed));
        }
    }

    private void OnTapA(){
    }

    private void OnHoldA(bool value){
        StartCoroutine(InputDelay(_holdTouchA, value));
    }

    private void OnHoldB(bool value){
        StartCoroutine(InputDelay(_holdTouchB, value));
    }

    public void OnMoveA(float value){
        value = value == 0 ? 0 : Mathf.Sign(value);
        value *= _faceDirection;

        StartCoroutine(InputDelay(_movementInput, value));
    }

    public void OnGestureB(string attackName){
        if (attackName == "L") return; // Temporary Bugfix
        //if (_isAttackPerformed.Value) return;

        StartCoroutine(InputDelay(_isAttackPerformed, attackName));
    }

    #endregion

    public void OnHit(CollisionData data){
        if (_isHit) return;
        _hitCollisionData = data;
        _isHit = true;

        FighterStateMachine target = data.hurtbox.Owner;

        //if (target.StaminaManager.CanBlock && target.StaminaManager) return; // If hit opponent blocked the attack.
        Debug.Log("Script: FighterStateMachine" + "Time: " + Time.timeSinceLevelLoad + " Target Can Block?: " + target.CanBlock);
        if (target.CanBlock) return; // If hit opponent blocked/can block the attack.

        if (target.CurrentSubState == FighterStates.Block){
            // Break
            switch (_player){
                    case Player.P1:
                        EventManager.Instance.Interaction_P1(Interactions.Break);
                    break;

                    case Player.P2:
                        EventManager.Instance.Interaction_P2(Interactions.Break);
                    break;
                }
        }

        switch(target.ActionState){
            case ActionStates.Start:
            // Counter
                TimeController.Instance.SlowDown();

                switch (_player){
                    case Player.P1:
                        EventManager.Instance.Interaction_P1(Interactions.Counter);
                    break;

                    case Player.P2:
                        EventManager.Instance.Interaction_P2(Interactions.Counter);
                    break;
                }
            break;

            case ActionStates.Active:
            //Whiff Punish
                TimeController.Instance.SlowDown();

                switch (_player){
                    case Player.P1:
                        EventManager.Instance.Interaction_P1(Interactions.Punish);
                    break;

                    case Player.P2:
                        EventManager.Instance.Interaction_P2(Interactions.Punish);
                    break;
                }
            break;

            case ActionStates.Recovery:
            // Whiff Punish
                TimeController.Instance.SlowDown();

                switch (_player){
                    case Player.P1:
                        EventManager.Instance.Interaction_P1(Interactions.Punish);
                    break;

                    case Player.P2:
                        EventManager.Instance.Interaction_P2(Interactions.Punish);
                    break;
                }
            break;
        }

        // Screen Shake upon a successful hit.
        if (data.action.ScreenShakeVelocity != Vector3.zero)
        CameraController.Instance.ScreenShake(data.action.ScreenShakeVelocity);
        
        // Stmaina Recovery upon a successful hit.
        _staminaManager.UpdateStamina(data.action.StaminaRecovery);
    }

    public void OnHurt(CollisionData data){
        if (_isHurt) return;
        _hurtCollisionData = data;
        _isHurt = true;
    }

    #region Input Coroutines
    // These coroutines cannot be located in a static class due to Input Delay coroutine calling Input Buffer coroutine
    // which can only be done in a MonoBehaviour.

    private IEnumerator InputDelay<T>(ContinuousInput<T> input, T value){
        if(input.TargetValue.Equals(value)) yield break;
        input.TargetValue = value;
        
        for (int i = 0; i < GameManager.Instance.Config.InputDelay; i++){
            yield return new WaitForFixedUpdate();
        }

        input.Value = value;
    }

    private IEnumerator InputDelay(Input<bool> input){
        for (int i = 0; i < GameManager.Instance.Config.InputDelay; i++){
            yield return new WaitForFixedUpdate();
        }
        if (input.Value) input.Frame = GameManager.Instance.Config.InputBuffer; // Refresh the frame timer if Input Buffer coroutine is already running.
        else StartCoroutine(InputBuffer(input));
    }

    private IEnumerator InputDelay<T>(QueueInput<bool, T> input, T queue){
        for (int i = 0; i < GameManager.Instance.Config.InputDelay; i++){
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(InputBuffer(input, queue));
    }

    private IEnumerator InputBuffer(Input<bool> input){
        input.Value = true;
        input.Frame = GameManager.Instance.Config.InputBuffer;
        
        while (input.Frame >= 0){
            input.Frame--;
            yield return new WaitForFixedUpdate();
            if (!input.Value) break; // Exit coroutine if the input had been consumed by another source.
        }

        input.Value = false;
    }

    private IEnumerator InputBuffer<T>(QueueInput<bool, T> input , T queue){
        input.Value = true;
        input.Queue.Enqueue(queue);

        for (int i = 0; i < GameManager.Instance.Config.InputBuffer; i++){
            yield return new WaitForFixedUpdate();
            if (!input.Value) break;
        }

        if (input.Queue.Count == 1) input.Value = false;
        input.Queue.Dequeue();
    }

    #endregion

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.down * _rayCastLenght);
    }

    public void SetFaceDirection(int value){
        _faceDirection = value;
        transform.rotation = Quaternion.Euler(0f, 90f * _faceDirection, 0f);
    }

    public void Reset(){
        StopAllCoroutines();

        switch(_player)
        {
            case Player.P1:
                SetFaceDirection(-1);
            break;

            case Player.P2:
                SetFaceDirection(1);
            break;
        }

        _isHit = false;
        _isHurt = false;
        _isGravityApplied = true;
        _gravity = Physics2D.gravity.y;
        _drag = 0f;
        
        _isJumpPressed.Reset();
        _isAttackPerformed.Reset();
        _movementInput.Reset();
        _holdTouchA.Reset();
        _holdTouchB.Reset();
        _isDashPressed.Reset();
        _isDodgePressed.Reset();

        _staminaManager.Reset();
        _healthManager.Reset();
    }
}