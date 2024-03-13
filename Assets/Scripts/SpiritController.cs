using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritController : MonoBehaviour
{
    [SerializeField] protected Player _player;
    protected SpiritManager _spiritManager;
    protected ActionSpiritAttack _action;
    protected bool _isActive = false;

    [ReadOnly] [SerializeField] protected ActionStates _actionState = default;
    [ReadOnly] [SerializeField] protected int _currentFrame = 0;
    [SerializeField] [ReadOnly] protected int _faceDirection; // -1 Left, +1 Right

    protected Animator _animator;
    protected AnimatorOverrideController _animOverrideCont;
    protected AnimationClipOverrides _clipOverrides;

    protected Animator _colBoxAnimator;
    protected AnimatorOverrideController _colBoxOverrideCont;
    protected AnimationClipOverrides _colBoxClipOverrides;

    protected HitResponder _hitResponder;
    protected HurtResponder _hurtResponder;
    protected Rigidbody2D _rigidbody2D;
    protected ParticleEffectManager _particleEffectManager;

    protected CollisionData _hurtCollisionData;
    protected CollisionData _hitCollisionData;
    protected bool _isHit;
    protected bool _isHurt;

    public Player Player {get{return _player;}}
    public SpiritManager Manager {get{return _spiritManager;} set{_spiritManager = value;}}
    public ActionAttack Action {get{return _action as ActionAttack;}}  
    public ActionSpiritAttack Action_Spirit {get{return _action;} set{_action = value;}}  
    public ActionStates ActionState {get{return _actionState;} set{_actionState = value;}}
    public int CurrentFrame {get{return _currentFrame;} set{_currentFrame = value;}}
    public bool IsActive {get{return _isActive;} set{_isActive = value;}}

    public Animator Animator{get{return _animator;}}
    public AnimatorOverrideController AnimOverrideCont{get{return _animOverrideCont;} set{_animOverrideCont = value;}}
    public AnimationClipOverrides ClipOverrides{get{return _clipOverrides;}}

    public Animator ColBoxAnimator{get{return _colBoxAnimator;}}
    public AnimatorOverrideController ColBoxOverrideCont{get{return _colBoxOverrideCont;} set{_colBoxOverrideCont = value;}}
    public AnimationClipOverrides ColBoxClipOverrides{get{return _colBoxClipOverrides;}}

    public HitResponder HitResponder {get{return _hitResponder;}}
    public HurtResponder HurtResponder {get{return _hurtResponder;}}
    public CollisionData HurtCollisionData {get{return _hurtCollisionData;}}
    public CollisionData HitCollisionData {get{return _hitCollisionData;}}

    public ParticleEffectManager ParticleEffectManager {get{return _particleEffectManager;}}
    public bool IsHit {get{return _isHit;} set{_isHit = value;}}
    public bool IsHurt {get{return _isHurt;} set{_isHurt = value;}}
    public Rigidbody2D Rigidbody2D {get{return _rigidbody2D;}}

    public int FaceDirection {get{return _faceDirection;}}

    public void Activate(){
        _isActive = true;
        _currentFrame = 0;

        _action.EnterState(this);
        
        // ClipOverrides["DirectPunchA"] = _action.MeshAnimationA;
        // ClipOverrides["DirectPunchR"] = _action.MeshAnimationR;
        // ClipOverrides["DirectPunchS"] = _action.MeshAnimationS;

        ColBoxClipOverrides["Uppercut_Startup"] = _action.BoxAnimationS;
        ColBoxClipOverrides["Uppercut_Active"] = _action.BoxAnimationA;
        ColBoxClipOverrides["Uppercut_Recovery"] = _action.BoxAnimationR;

        //AnimOverrideCont.ApplyOverrides(ClipOverrides);
        ColBoxOverrideCont.ApplyOverrides(ColBoxClipOverrides);

        HitResponder.UpdateData(_action);
    }

    private void Awake(){
        _animator = GetComponent<Animator>();
        _colBoxAnimator = transform.Find("Hurtboxes").GetComponent<Animator>();
        
        // _animOverrideCont = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        // _animator.runtimeAnimatorController = _animOverrideCont;

        // _clipOverrides = new AnimationClipOverrides(_animOverrideCont.overridesCount);
        // _animOverrideCont.GetOverrides(_clipOverrides);

        _colBoxOverrideCont = new AnimatorOverrideController(_colBoxAnimator.runtimeAnimatorController);
        _colBoxAnimator.runtimeAnimatorController = _colBoxOverrideCont;

        _colBoxClipOverrides = new AnimationClipOverrides(_colBoxOverrideCont.overridesCount);
        _colBoxOverrideCont.GetOverrides(_colBoxClipOverrides);

        if (TryGetComponent(out HitResponder hitResponder)) _hitResponder = hitResponder;
        if (TryGetComponent(out HurtResponder hurtResponder)) _hurtResponder = hurtResponder;
        if (TryGetComponent(out Rigidbody2D rigidbody2D)) _rigidbody2D = rigidbody2D;
        if (TryGetComponent(out ParticleEffectManager particleEffectManager)) _particleEffectManager = particleEffectManager;
    }

    private void OnEnable(){
        if(_hitResponder) _hitResponder.HitResponse += OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse += OnHurt;
    }

    private void OnDisable(){
        // Component based events.
        if(_hitResponder) _hitResponder.HitResponse -= OnHit;
        if (_hurtResponder) _hurtResponder.HurtResponse -= OnHurt;
    }

    private void FixedUpdate(){
        if (!_isActive) return;

        _action.FixedUpdateState();
        
        //Debug.Log("Frame L: " + _action.FrameLenght + " Current F: " + _currentFrame + " Buffer: " + _ctx.GetInputBuffer + " Bool: " + _ctx.ValidAttackInputInterval);
        //_ctx.ValidAttackInputInterval = _action.FrameLenght - _currentFrame < _ctx.GetInputBuffer;

        _action.SwitchActionStateFunction();

        CheckSwitchState();
    }

    private void CheckSwitchState()
    {
        if (_actionState == ActionStates.None){
            //Despawn
            _action.ExitStateFunction(this);
            _isActive = false;
            _spiritManager.IsDeployed = false;
            gameObject.SetActive(false);
        }

        // if (_ctx.IsHurt){
        //     EventManager.Instance.FighterAttackInterrupted?.Invoke();
        //     SwitchState(_factory.Stunned());
        // }
    }

    #region Event Functions

    private void OnHit(CollisionData data){
        if (_isHit) return;
        _hitCollisionData = data;
        _isHit = true;

        FighterStateMachine target = data.hurtbox.Owner;

        //if (target.StaminaManager.CanBlock && target.StaminaManager) return; // If hit opponent blocked the attack.
        //Debug.Log("Script: FighterStateMachine" + "Time: " + Time.timeSinceLevelLoad + " Target Can Block?: " + target.CanBlock);
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

        // Hit VFX
        if (_particleEffectManager != null){
            var obj = _particleEffectManager.DequeueObject(_particleEffectManager.PoolableObjects[0].Prefab, _particleEffectManager.PoolableObjects[0].QueueReference);
            obj.transform.position = data.collisionPoint;
            ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        }
    }

    private void OnHurt(CollisionData data){
        if (_isHurt) return;
        _hurtCollisionData = data;
        _isHurt = true;

        //if (_isInvulnerable) return;
    }

    #endregion
}
