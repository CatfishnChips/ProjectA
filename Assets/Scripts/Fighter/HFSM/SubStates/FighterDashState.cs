using UnityEngine;

public class FighterDashState : FighterBaseState
{
    private ActionConditional _action;
    private int _currentFrame = 0;
    private float _direction;
    private float _time;
    private float _initialVelocity;
    private float _drag;

    private bool _listeningForChainInput;
    private bool _performedChainMove;

    public FighterDashState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    :base(currentContext, fighterStateFactory){
    }

    public override void CheckSwitchState()
    {
        if (_currentFrame >= _ctx.DashTime){
            SwitchState(_factory.GetSubState(FighterSubStates.Idle));
        }
        if(_currentFrame >= _action.CancelFrame && _ctx.ChainActionGesture != InputGestures.None){
            _performedChainMove = true;
            if(_ctx.ActionManager.PeekAction(_ctx.ChainActionGesture).GetType() == typeof(ActionFighterAttack)) SwitchState(_factory.GetSubState(FighterSubStates.Attack)); 
        }
    }

    public override void EnterState()
    {
        _currentFrame = 0;
        _ctx.IsGravityApplied = false;
        _drag = 0f;

        if(_ctx.ChainActionGesture != InputGestures.None) _action = _ctx.ActionManager.GetAction(InputGestures.SwipeRightL) as ActionConditional;
        else _action = _ctx.ActionManager.GetAction(InputGestures.SwipeRightL) as ActionConditional;

        if(_action == null) { // Safety check if all the precautions to reset Action Manager somehow failed.
            _ctx.ActionManager.Reset();
            _action = _ctx.ActionManager.GetAction(InputGestures.SwipeRightL) as ActionConditional;
        }

        _ctx.ChainActionGesture = InputGestures.None;

        AnimationClip clip;
        AnimationClip colClip;

        if (_ctx.DashDirection == -1)
        {
            clip = _action.Animations[0].meshAnimation;
            colClip = _action.Animations[0].boxAnimation;
        }
        else
        {
            clip = _action.Animations[1].meshAnimation;
            colClip = _action.Animations[1].boxAnimation;
        }

        _direction = _ctx.DashDirection;
        _time = _ctx.DashTime * Time.fixedDeltaTime;

        _drag = -2 * _ctx.DashDistance / Mathf.Pow(_time, 2);
        _drag *= _direction;

        _initialVelocity = 2 * _ctx.DashDistance / _time; // Initial horizontal velocity;
        _initialVelocity *= _direction;

        // Apply Calculated Variables
        _ctx.Drag = _drag;
        _ctx.Gravity = 0f;
        _ctx.CurrentMovement = new Vector2(_initialVelocity, _ctx.CurrentMovement.y);

        _ctx.AnimOverrideCont["Action"] = clip;
        _ctx.ColBoxOverrideCont["Box_Action"] = colClip;

        // For this action, DashTime variable is used instead of animation's Frame variable.
        float speedVar = AdjustAnimationTime(clip, _ctx.DashTime);
        _ctx.Animator.SetFloat("SpeedVar", speedVar);
        _ctx.ColBoxAnimator.SetFloat("SpeedVar", speedVar);
        
        _ctx.Animator.PlayInFixedTime("Action");
        _ctx.ColBoxAnimator.PlayInFixedTime("Action");
    }

    public override void ExitState()
    {
        _drag = 0f;
        _direction = 0f;
        _time = 0f;
        _initialVelocity = 0f;
        _currentFrame = 0;

        if(!_performedChainMove) {
            _ctx.ChainActionGesture = InputGestures.None;
            _ctx.ActionManager.Reset();
        }
        _performedChainMove = false;
        _listeningForChainInput = true;

        _ctx.IsGravityApplied = true;
        _ctx.Gravity = 0f;
        _ctx.Drag = 0f;
        _ctx.CurrentFrame = 0;
        _ctx.CurrentMovement = Vector2.zero;
    }

    public override void FixedUpdateState()
    {
        CancelCheck();
        CheckSwitchState();
        _currentFrame++;  
    }

    public void CancelCheck(){
        if(_currentFrame <= _action.CancelFrame && _currentFrame > _action.InputIgnoreFrames && _ctx.AttackInput.Read() && _listeningForChainInput){
            if(_ctx.ActionManager.CheckIfChain(_ctx.AttackInput.ReadContent())){
                _ctx.ChainActionGesture = _ctx.AttackInput.ReadContent();
            }else{
                _listeningForChainInput = false;
            }
        }
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
    }
}
