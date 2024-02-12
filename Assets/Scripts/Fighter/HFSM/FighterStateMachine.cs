using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;

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

public abstract class FighterStateMachine : MonoBehaviour
{
    [SerializeField] protected FighterManager _fighterManager;
    public Dictionary<InputGestures, ActionAttack> AttackMoveDict{get{return _fighterManager.AttackMoveDict;}}
    public Dictionary<string, ActionBase> ActionDictionary{get{return _fighterManager.ActionDictionary;}}

    [SerializeField] protected Player _player;
    protected Dictionary<string, ActionAttack> _aerialAttackMoveDict;
    protected Dictionary<string, ActionBase> _actionDictionary;

    protected ComboListener _comboListener;

    protected Transform _mesh;

    protected Animator _animator;
    protected AnimatorOverrideController _animOverrideCont;
    protected AnimationClipOverrides _clipOverrides;

    protected Animator _colBoxAnimator;
    protected AnimatorOverrideController _colBoxOverrideCont;
    protected AnimationClipOverrides _colBoxClipOverrides;

    protected FighterStateFactory _states;
    protected FighterBaseState _currentState;
    [ReadOnly] [SerializeField] protected FighterStates _currentRootState = default;
    [ReadOnly] [SerializeField] protected FighterStates _currentSubState = default;
    [ReadOnly] [SerializeField] protected FighterStates _previousRootState = default;
    [ReadOnly] [SerializeField] protected FighterStates _previousSubState = default;
    [ReadOnly] [SerializeField] protected ActionStates _actionState = default;
    [ReadOnly] [SerializeField] protected int _currentFrame = 0;

    protected HitResponder _hitResponder;
    protected HurtResponder _hurtResponder;
    protected Rigidbody2D _rigidbody2D;

    protected HealthManager _healthManager;
    protected StaminaManager _staminaManager;
    protected ParticleEffectManager _particleEffectManager;
    protected SpiritManager _spiritManager;
    protected ProjectileManager m_projectileManager;
    protected Vector2 _velocity;
    protected Vector2 _rootMotion; // Provided by AnimatorRootMotionManager

    #region Public Events
    public UnityAction OnAttackStart;
    public UnityAction OnAttackEnd;
    #endregion

    #region Input Variables
    
    private TouchInput<bool> _jumpInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Jump);
    private TouchInput<bool> _dashInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Dash);
    private TouchInput<bool> _dodgeInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Dodge);
    private TouchInput<bool> _blockInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Dodge);
    private TouchContinuousInput<int> _movementInput = new TouchContinuousInput<int>(0, InputTypes.Drag, SubInputTypes.None);
    private TouchContinuousInput<bool> _holdAInput = new TouchContinuousInput<bool>(false, InputTypes.Hold, SubInputTypes.None);
    private TouchContinuousInput<bool> _holdBInput = new TouchContinuousInput<bool>(false, InputTypes.Hold, SubInputTypes.None);
    private TouchQueueInput<ActionAttack> _attackInput = new TouchQueueInput<ActionAttack>(InputTypes.Gesture, SubInputTypes.None);
    protected List<ITouchInput> _inputsList;

    #endregion

    [SerializeField] protected int _comboBuffer = 5;
    [SerializeField] protected float _dashDistance = 1f;
    [SerializeField] protected int _dashTime; // in frames
    [SerializeField] protected Vector2Int _dodgeTime; // X: Startup Y: Active // in frames

    [Header("Airborne State")]
    [SerializeField] protected float _airMoveSpeed;
    [SerializeField] protected float _jumpHeight = 1f;
    [SerializeField] protected float _jumpDistance = 1f;
    [SerializeField] protected int _jumpTime; // in frames
    [SerializeField] protected int _fallTime; // in frames

    [Header("Raycast")]
    [SerializeField] [ReadOnly] bool _isGrounded;
    [SerializeField] protected LayerMask _rayCastLayerMask;
    [SerializeField] protected float _rayCastLenght = 1f;
    [SerializeField] protected Vector2 _rayCastOffset;

    [Space]
    [SerializeField] [ReadOnly] protected Vector2 _currentMovement;
    [SerializeField] [ReadOnly] protected int _faceDirection; // -1 Left, +1 Right
    [SerializeField] [ReadOnly] protected float _gravity;
    [SerializeField] [ReadOnly] protected float _drag;

    protected CollisionData _hurtCollisionData;
    protected CollisionData _hitCollisionData;
    protected bool _isHit;
    protected bool _isHurt;
    protected bool _isInvulnerable;
    protected bool _isGravityApplied;
    protected Vector2 _swipeDirection;
    protected int _dashDirection;
    protected bool _validAttackInputInterval;
    protected ActionFighterAttack _fighterAttackAction;

    public Player Player {get{return _player;}}
    public Transform Mesh {get{return _mesh;}}

    public FighterStates CurrentRootState{get{return _currentRootState;} set{_currentRootState = value;}}
    public FighterStates CurrentSubState{get{return _currentSubState;} set{_currentSubState = value;}}
    public FighterStates PreviousRootState{get{return _previousRootState;} set{_previousRootState = value;}}
    public FighterStates PreviousSubState{get{return _previousSubState;} set{_previousSubState = value;}}
    public ActionStates ActionState{get{return _actionState;} set{_actionState = value;}}
    public int CurrentFrame{get{return _currentFrame;} set{_currentFrame = value;}}

    public TouchInput<bool> JumpInput { get => _jumpInput; }
    public TouchInput<bool> DashInput { get => _dashInput; }
    public TouchInput<bool> DodgeInput { get => _dodgeInput; }
    public TouchContinuousInput<int> MovementInput { get => _movementInput; }
    public TouchContinuousInput<bool> HoldAInput { get => _holdAInput; }
    public TouchContinuousInput<bool> HoldBInput { get => _holdBInput; }
    public TouchQueueInput<ActionAttack> AttackInput { get => _attackInput; }

    //public string AttackName{get{return _isAttackPerformed.Queue.Peek();}}
    public ActionFighterAttack AttackAction{get{return _fighterAttackAction;}}
    public Vector2 Velocity{get{return _velocity;} set{_velocity = value;}}
    public Vector2 RootMotion{get{return _rootMotion;} set{_rootMotion = value;}}
    public float AirMoveSpeed{get{return _airMoveSpeed;}}
    public FighterBaseState CurrentState{get{return _currentState;} set{_currentState = value;}}

    public Animator Animator{get{return _animator;}}
    public AnimatorOverrideController AnimOverrideCont{get{return _animOverrideCont;} set{_animOverrideCont = value;}}
    public AnimationClipOverrides ClipOverrides{get{return _clipOverrides;}}

    public Animator ColBoxAnimator{get{return _colBoxAnimator;}}
    public AnimatorOverrideController ColBoxOverrideCont{get{return _colBoxOverrideCont;} set{_colBoxOverrideCont = value;}}
    public AnimationClipOverrides ColBoxClipOverrides{get{return _colBoxClipOverrides;}}

    public ComboListener ComboListener{get{return _comboListener;} set{_comboListener = value;}}
    public ComboMove[] CombosArray{get{return _fighterManager.CombosArray;}}
    public int ComboBuffer{get{return _comboBuffer;}}

    public HitResponder HitResponder {get{return _hitResponder;}}
    public HurtResponder HurtResponder {get{return _hurtResponder;}}
    public CollisionData HurtCollisionData {get{return _hurtCollisionData;}}
    public CollisionData HitCollisionData {get{return _hitCollisionData;}}
    public HealthManager HealthManager {get{return _healthManager;}}
    public StaminaManager StaminaManager {get{return _staminaManager;}}
    public ParticleEffectManager ParticleEffectManager {get{return _particleEffectManager;}}
    public ProjectileManager ProjectileManager {get => m_projectileManager;}
    public bool IsHit {get{return _isHit;} set{_isHit = value;}}
    public bool IsHurt {get{return _isHurt;} set{_isHurt = value;}}
    public bool CanBlock {get{return _staminaManager.CanBlock && (_currentRootState == FighterStates.Grounded || _previousRootState == FighterStates.Grounded) && (_currentSubState == FighterStates.Idle || _currentSubState == FighterStates.Walk);}}
    public bool IsInvulnerable {get{return _isInvulnerable;} set{_isInvulnerable = value;}}
    public bool IsGravityApplied {get{return _isGravityApplied;} set{_isGravityApplied = value;}}
    public Rigidbody2D Rigidbody2D {get{return _rigidbody2D;}}
    private FighterController _fighterController; 
    public FighterController FighterController {get{return _fighterController;}}   

    public bool IsGrounded{get{return _isGrounded;}}
    public float JumpHeight {get{return _jumpHeight;}}
    public int JumpTime {get{return _jumpTime;}}
    public int FallTime {get{return _fallTime;}}
    public float Gravity {get{return _gravity;} set{_gravity = value;}}
    public float Drag {get{return _drag;} set{_drag = value;}}
    public Vector2 SwipeDirection {get{return _swipeDirection;}} // SwipeDirection is affected by FaceDirection
    public int DashDirection {get {return _dashDirection;}}
    public Vector2 CurrentMovement {get{return _currentMovement;} set{_currentMovement = value;}}
    public float JumpDistance {get{return _jumpDistance;}}
    public float DashDistance {get{return _dashDistance;}}
    public int DashTime {get{return _dashTime;}}
    public Vector2Int DodgeTime {get{return _dodgeTime;}}
    public int FaceDirection {get{return _faceDirection;}}
    public bool ValidAttackInputInterval { get { return _validAttackInputInterval; } set { _validAttackInputInterval = value; } }

    #region Unity Monobehaviour Functions

    private void Awake() { AwakeFunction(); }
    private void Start() { StartFunction(); }
    private void Update(){ UpdateFunction(); }
    private void FixedUpdate() { FixedUpdateFunction(); }
    private void OnDisable() { OnDisableFunction(); }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.down * _rayCastLenght);
    }

    #endregion

    #region Virtual Monobehaviour Functions

    protected virtual void AwakeFunction(){

        _inputsList = new List<ITouchInput>
        {
            _jumpInput,
            _dashInput,
            _dodgeInput,
            _movementInput,
            _holdAInput,
            _holdBInput,
            _attackInput
        };

        _mesh = transform.Find("Mesh");

        if (TryGetComponent(out Animator animator)){
            _animator = animator;
        }
        else _animator = _mesh.GetComponent<Animator>();

        _colBoxAnimator = transform.Find("Hurtboxes").GetComponent<Animator>();
        
        _states = new FighterStateFactory(this);
        _comboListener = new ComboListener(this);
        _faceDirection = (int)Mathf.Sign(transform.forward.x);
        
        _animOverrideCont = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animOverrideCont;

        _clipOverrides = new AnimationClipOverrides(_animOverrideCont.overridesCount);
        _animOverrideCont.GetOverrides(_clipOverrides);

        _colBoxOverrideCont = new AnimatorOverrideController(_colBoxAnimator.runtimeAnimatorController);
        _colBoxAnimator.runtimeAnimatorController = _colBoxOverrideCont;

        _colBoxClipOverrides = new AnimationClipOverrides(_colBoxOverrideCont.overridesCount);
        _colBoxOverrideCont.GetOverrides(_colBoxClipOverrides);

        GetComponents();
    }

    protected virtual void GetComponents(){
        if (TryGetComponent(out FighterController fighterController)) _fighterController = fighterController;
        if (TryGetComponent(out HitResponder hitResponder)) _hitResponder = hitResponder;
        if (TryGetComponent(out HurtResponder hurtResponder)) _hurtResponder = hurtResponder;
        if (TryGetComponent(out Rigidbody2D rigidbody2D)) _rigidbody2D = rigidbody2D;
        if (TryGetComponent(out HealthManager healthManager)) _healthManager = healthManager;
        if (TryGetComponent(out StaminaManager staminaManager)) _staminaManager = staminaManager;
        if (TryGetComponent(out ParticleEffectManager particleEffectManager)) _particleEffectManager = particleEffectManager;
        if (TryGetComponent(out SpiritManager spiritManager)) _spiritManager = spiritManager;
        if (TryGetComponent(out ProjectileManager projectileManager)) m_projectileManager = projectileManager;
    }

    private bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
           || potentialDescendant == potentialBase;
    }

    protected virtual void StartFunction(){

        _fighterManager.fighterEvents.OnFighterAttack += OnFighterAttackInput;
        _fighterManager.fighterEvents.OnSpiritAttack += OnSpiritAttackInput;
        _fighterManager.fighterEvents.OnMove += OnMove;
        _fighterManager.fighterEvents.OnDash += OnDash;

        // Subscribe to component based events.
        if(_hitResponder) _hitResponder.HitResponse += OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse += OnHurt;

        ResetVariables();

        // Start default state.
        _currentState = _states.GetRootState(FighterRootStates.Airborne);
        _currentState.EnterState();
    }

    protected virtual void UpdateFunction(){
        //_currentState.UpdateStates();
    }

    protected virtual void FixedUpdateFunction(){
        foreach(ITouchInput input in _inputsList)
        {
            input.OnTick();
        }

        _isGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) + _rayCastOffset, Vector2.down, _rayCastLenght, _rayCastLayerMask);
        _faceDirection = (int)Mathf.Sign(transform.forward.x);

        if(_comboListener.isActive){
            _comboListener.FixedUpdate();
        }
        _currentState.FixedUpdateStates();

        _fighterController.Simulate();

        foreach(ITouchInput input in _inputsList)
        {
            input.OnLateTick();
        }
    }

    protected virtual void OnDisableFunction(){
        
        _fighterManager.fighterEvents.OnFighterAttack -= OnFighterAttackInput;
        _fighterManager.fighterEvents.OnSpiritAttack -= OnSpiritAttackInput;
        _fighterManager.fighterEvents.OnMove -= OnMove;
        _fighterManager.fighterEvents.OnDash -= OnDash;

        // Component based events.
        if(_hitResponder) _hitResponder.HitResponse -= OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse -= OnHurt;

        StopAllCoroutines();
    }

    #endregion

    #region Input Functions

    protected virtual void OnDash(int direction){
        _dashDirection = direction * _faceDirection;
        _dashInput.Write(true);
    }

    protected virtual void OnMove(int value){
        _movementInput.Write(value);
    }

    protected virtual void OnFighterAttackInput(ActionFighterAttack attackAction){
        _attackInput.Write(attackAction);
    }

    protected virtual void OnSpiritAttackInput(ActionSpiritAttack attackAction){
        _spiritManager?.SpawnSpirit(attackAction);
    }

    #endregion

    #region Event Functions

    protected virtual void OnHit(CollisionData data){
        if (_isHit) return;
        _hitCollisionData = data;
        _isHit = true;

        FighterStateMachine target = data.hurtbox.Owner;

        //if (target.StaminaManager.CanBlock && target.StaminaManager) return; // If hit opponent blocked the attack.
        //Debug.Log("Script: FighterStateMachine" + "Time: " + Time.timeSinceLevelLoad + " Target Can Block?: " + target.CanBlock);
         // Hit VFX
        if (_particleEffectManager != null){
            GameObject obj;
            if (target.CanBlock){
                obj = _particleEffectManager.DequeueObject(_particleEffectManager.PoolableObjects[1].Prefab, _particleEffectManager.PoolableObjects[1].QueueReference);
            }
            else{
                obj = _particleEffectManager.DequeueObject(_particleEffectManager.PoolableObjects[0].Prefab, _particleEffectManager.PoolableObjects[0].QueueReference);
            }
            
            obj.transform.position = new Vector3(data.collisionPoint.x, data.collisionPoint.y, -1f);
            ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        }

        if (target.CanBlock || target.IsInvulnerable) return; // If hit opponent blocked/can block the attack.

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
        CameraController.Instance.ScreenShake(data.action.ScreenShakeVelocity * Mathf.Sign(data.hitbox.Transform.right.x));
        
        // Stamina Recovery upon a successful hit.
        _staminaManager.UpdateStamina(data.action.StaminaRecovery);

        // SpiritManager Functions
        if (_spiritManager != null){
            _spiritManager.Setup(this, data);
            //_spiritManager.DrawPath(); // DEBUG
            _spiritManager.UpdateSpirit(data.action.SpiritRecovery);
        }
    }

    protected virtual void OnHurt(CollisionData data){
        if (_isHurt) return;
        _hurtCollisionData = data;
        _isHurt = true;
        Debug.Log("Script: FighterStateMachine - OnHurt " + " Time: " + Time.timeSinceLevelLoad);

        if (_isInvulnerable) return;
    }

    #endregion

    #region Public Functions

    public virtual void SetFaceDirection(int value){
        _faceDirection = value;
        transform.rotation = Quaternion.Euler(0f, 90f * _faceDirection, 0f);

        _mesh.localRotation = Quaternion.Euler(0f, 5f, 0f);
        
        // Chaning scale at runtime creates problems with MagicaCloth
        //_mesh.localScale = new Vector3(_faceDirection, 1f, 1f);
    }

    public virtual void ResetVariables(){
        StopAllCoroutines();

        switch(_player)
        {
            case Player.P1:
                SetFaceDirection(-1);
                if (_spiritManager) EventManager.Instance.RecoveredFromStun_P1?.Invoke();
            break;

            case Player.P2:
                SetFaceDirection(1);
                if (_spiritManager) EventManager.Instance.RecoveredFromStun_P2?.Invoke();
            break;
        }

        _isHit = false;
        _isHurt = false;
        _isInvulnerable = false;
        _isGravityApplied = true;
        _gravity = 0f;
        _drag = 0f;

        _currentMovement = Vector2.zero;
        _velocity = Vector2.zero;
        _rigidbody2D.velocity = Vector2.zero;
        
        // Reset Inputs

        _staminaManager?.Reset();
        _healthManager?.Reset();
        _spiritManager?.Reset();

        _currentState = _states.GetRootState(FighterRootStates.Grounded);
        _currentState.EnterState();
    }

    #endregion

}