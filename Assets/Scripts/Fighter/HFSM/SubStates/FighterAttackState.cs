using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Attack State", menuName = "FighterStates/Sub/AttackState")]
public class FighterAttackState : ActionAttack, ICancellableAction
{
    protected ActionStates _actionState = default;
    protected bool _hadHit = false;

    protected bool isReading;

    public bool HadHit { get {return _hadHit;} set{ _hadHit = value;} }

    protected List<FrameEvent> _frameEvents = new List<FrameEvent>();
    protected virtual List<FrameEvent> Events {get {return _frameEvents;}}

    public override void Initialize(IStateMachineRunner ctx, FighterStateFactory factory)
    {
        base.Initialize(ctx, factory);
    }

    public override void CheckSwitchState(){
        base.CheckSwitchState();
        if (_actionState == ActionStates.None){

            if(_ctx.Player == Player.P1) EventManager.Instance.FighterAttackEnded?.Invoke();
            else if(_ctx.Player == Player.P2) EventManager.Instance.P2FighterAttackEnded?.Invoke();

            _ctx.ActionState = default;
            _ctx.ValidAttackInputInterval = false;

            if (_ctx.AttackInput.Read()){
                // SwitchState(_factory.GetSubState(_ctx.AttackInput.ReadContent()));
            }
            else{
                SwitchState(_factory.GetSubState(FighterStates.Idle));
            }
        }
    }

    public virtual void SwitchActionStateFunction(){
        if (_currentFrame <= m_startFrames){
            _actionState = ActionStates.Start;
        }
        else if (_currentFrame > m_startFrames && _currentFrame <= m_startFrames + m_activeFrames){
            _actionState = ActionStates.Active;
        }
        else if (_currentFrame > m_startFrames + m_activeFrames && 
        _currentFrame <= m_startFrames + m_activeFrames + RecoveryFrames){
            _actionState = ActionStates.Recovery;
        }
        else _actionState = ActionStates.None;

        //Debug.Log("ActionFighterAttack(SwitchActionStateFunction) - Frame: " + state._currentFrame + " State: " + state._actionState);
    }

    public override void EnterState()
    {
        _firstFrameStartup = true;
        _firstFrameActive = true;
        _firstFrameRecovery = true;
        _firstTimePause = true;
        _pause = false;
        _pauseFrames = 0;
        _ctx.IsGravityApplied = m_gravity;

        //Debug.Log(name);
        _currentFrame = 0;
        _ctx.OnAttackStart?.Invoke();
        _hadHit = false;
        _ctx.CurrentFrame =_currentFrame;
        _actionState = ActionStates.Start;
        _ctx.ChainActionGesture = InputGestures.None;
        
        _ctx.ClipOverrides["AttackStart"] = MeshAnimationS;
        _ctx.ClipOverrides["AttackActive"] = MeshAnimationA;
        _ctx.ClipOverrides["AttackRecover"] = MeshAnimationR;

        _ctx.ColBoxClipOverrides["Box_AttackStart"] = BoxAnimationS;
        _ctx.ColBoxClipOverrides["Box_AttackActive"] = BoxAnimationA;
        _ctx.ColBoxClipOverrides["Box_AttackRecover"] = BoxAnimationR;

        _ctx.AnimOverrideCont.ApplyOverrides(_ctx.ClipOverrides);
        _ctx.ColBoxOverrideCont.ApplyOverrides(_ctx.ColBoxClipOverrides);

        _ctx.HitResponder.UpdateData(this);

        if (_ctx.Player == Player.P2) EventManager.Instance.FighterAttackStarted?.Invoke(name);
        else EventManager.Instance.P2FighterAttackStarted?.Invoke(name);
    }

    public override void FixedUpdateState()
    {
        switch(_actionState)
        {
            case ActionStates.Start:
                if(_firstFrameStartup){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedS);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedS);
                    _ctx.Animator.PlayInFixedTime("AttackStart");
                    _ctx.ColBoxAnimator.PlayInFixedTime("AttackStart");
                    _firstFrameStartup = false;
                }
            break;

            case ActionStates.Active:
                if(_firstFrameActive){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedA);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedA);
                    _ctx.Animator.PlayInFixedTime("AttackActive");
                    _ctx.ColBoxAnimator.PlayInFixedTime("AttackActive");
                    _firstFrameActive = false;
                }
            break;

            case ActionStates.Recovery:
                if(_firstFrameRecovery){
                    _ctx.Animator.SetFloat("SpeedVar", AnimSpeedR);
                    _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedR);
                    _ctx.Animator.PlayInFixedTime("AttackRecover");
                    _ctx.ColBoxAnimator.PlayInFixedTime("AttackRecover");
                    _firstFrameRecovery = false;
                }
            break;
        }
       
        // Invoke events.
        foreach(FrameEvent e in Events){
            if (_currentFrame == e.Frame){
                e.Event(_ctx, _ctx.CurrentState as ActionAttack);
            }
        }

        if (_ctx.IsHit) {
            _ctx.IsHit = false;
            HadHit = true;

            if (_firstTimePause){
                _firstTimePause = false;
                _pause = true;
                _pauseFrames = m_hitStop;
                
                _ctx.Animator.SetFloat("SpeedVar", 0);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", 0);
            }
        } 


        if (_pause){
            if (_pauseFrames <= 0){
                _pause = false;
                _ctx.Animator.SetFloat("SpeedVar", AnimSpeedA);
                _ctx.ColBoxAnimator.SetFloat("SpeedVar", AnimSpeedA);
            }
            _pauseFrames--;
        } 
        else _currentFrame++;

        _ctx.CurrentMovement = applyRootMotion ? _ctx.RootMotion : _ctx.CurrentMovement;

        SwitchActionStateFunction();
        _ctx.ActionState = _actionState;

        _ctx.CurrentFrame =_currentFrame;
    }

    public override void ExitState()
    {
        //Debug.Log("Attack ended at frame: " + _currentFrame);
        _ctx.CurrentFrame = 0;
        _ctx.IsGravityApplied = true;
        _ctx.ActionState = default;
        _ctx.OnAttackEnd?.Invoke();
        
        _ctx.Drag = 0f;
        _ctx.Gravity = 0f;
        _ctx.CurrentMovement = Vector2.zero;
    }

    public override void InitializeSubState(){}

    public override void UpdateState(){}
}
