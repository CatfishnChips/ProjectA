using System.Collections.Generic;
using UnityEngine;
using System;

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
    [Serializable]
    public struct AttackMoveAttribution{
        public string name;
        public AttackMove attackMove;
    }
    [SerializeField]
    private AttackMoveAttribution[] _attackMoveAttribution;
    private Dictionary<string, AttackMove> _attackMoveDict;

    private CharacterController _charController;

    private Animator _animator;
    private AnimatorOverrideController _animOverrideCont;
    private AnimationClipOverrides _clipOverrides;

    private Animator _colBoxAnimator;
    private AnimatorOverrideController _colBoxOverrideCont;
    private AnimationClipOverrides _colBoxClipOverrides;

    private FighterStateFactory _states;
    private FighterBaseState _currentState;


    private Vector2 _velocity;
    private bool _isJumpPressed;
    private bool _isGrounded;
    private bool _attackPerformed;
    private string _attackName;
    [SerializeField] string _currentStateName;
    [SerializeField] string _currentSubStateName;
    [SerializeField] string _currentSuperStateName;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _moveSpeed;
    private float _deltaTarget;

    public bool IsJumpPressed{get{return _isJumpPressed;}}
    public bool IsGrounded{get{return _isGrounded;}}
    public bool AttackPerformed{get{return _attackPerformed;} set{_attackPerformed = value;}}
    public string AttackName{get{return _attackName;}}
    public CharacterController CharController{get{return _charController;}}
    public Vector2 Velocity{get{return _velocity;}}
    public float MoveSpeed{get{return _moveSpeed;}}
    public FighterBaseState CurrentState{get{return _currentState;} set{_currentState = value;}}

    public Animator Animator{get{return _animator;}}
    public AnimatorOverrideController AnimOverrideCont{get{return _animOverrideCont;} set{_animOverrideCont = value;}}
    public AnimationClipOverrides ClipOverrides{get{return _clipOverrides;}}

    public Animator ColBoxAnimator{get{return _colBoxAnimator;}}
    public AnimatorOverrideController ColBoxOverrideCont{get{return _colBoxOverrideCont;} set{_colBoxOverrideCont = value;}}
    public AnimationClipOverrides ColBoxClipOverrides{get{return _colBoxClipOverrides;}}

    public Dictionary<string, AttackMove> AttackMoveDict{get{return _attackMoveDict;}}
   

    void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _colBoxAnimator = transform.Find("Hurtboxes").GetComponent<Animator>();
        _isJumpPressed = false;
        _attackPerformed = false;
        _states = new FighterStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _animOverrideCont = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animOverrideCont;

        _clipOverrides = new AnimationClipOverrides(_animOverrideCont.overridesCount);
        _animOverrideCont.GetOverrides(_clipOverrides);

        _colBoxOverrideCont = new AnimatorOverrideController(_colBoxAnimator.runtimeAnimatorController);
        _colBoxAnimator.runtimeAnimatorController = _colBoxOverrideCont;

        _colBoxClipOverrides = new AnimationClipOverrides(_colBoxOverrideCont.overridesCount);
        _colBoxOverrideCont.GetOverrides(_colBoxClipOverrides);

        _attackMoveDict = new Dictionary<string, AttackMove>();

        foreach (AttackMoveAttribution attribution in _attackMoveAttribution)
        {
            Debug.Log(attribution.name);
            _attackMoveDict.Add(attribution.name, attribution.attackMove);
        }
    }

    void Start()
    {
        EventManager.Instance.Walk += OnWalk;
        EventManager.Instance.Dash += OnDash;
        EventManager.Instance.AttackMove += OnAttack;
    }

    private void OnDisable() 
    {
        EventManager.Instance.Walk -= OnWalk;
        EventManager.Instance.Dash -= OnDash;
        EventManager.Instance.AttackMove -= OnAttack;

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        _currentState.FixedUpdateStates();
        _currentStateName = _currentState.StateName;
        _currentSubStateName = _currentState.SubStateName();
        _currentSubStateName = _currentState.SuperStateName();
        _isGrounded = _charController.isGrounded;
    }

    private void Update(){
        _currentState.UpdateStates();
        _velocity.x = Mathf.MoveTowards(_velocity.x, _deltaTarget, 0.75f * Time.deltaTime);
    }

    public void ListenToJump(){
        _isJumpPressed = true;
    }

    public void OnWalk(float delta){
        _deltaTarget = delta;
    }

    public void OnAttack(string attackName){
        _attackName = attackName;
        _attackPerformed = true;
    }

    private void OnDash(Vector2 direction) 
    {
        //_velocity.x = direction.x * _dashDistance;
    }
}
