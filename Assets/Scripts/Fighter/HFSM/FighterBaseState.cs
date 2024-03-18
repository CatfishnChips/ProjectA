using UnityEngine;

public abstract class FighterBaseState : StateMachineBaseState
{
    protected new FighterStateMachine _ctx;
    protected FighterStateFactory _factory;
    protected int _currentFrame;

    public int CurrentFrame { get => _currentFrame; set => _currentFrame = value; }

    public FighterBaseState(IStateMachineRunner ctx, FighterStateFactory factory) : base(ctx)
    {
        _ctx = ctx as FighterStateMachine;
        _factory = factory;
    }

    public bool IdleStateSwitchCheck(){
        if (_ctx.ActionInput.Read() && _ctx.GestureActionDict.ContainsKey(_ctx.ActionInput.PeekContent())){
            FighterStates state = _ctx.GestureActionDict[_ctx.ActionInput.PeekContent()].stateName;
            if(state == FighterStates.Attack){
                SwitchState(_factory.GetSubState(FighterSubStates.Attack));
                return true;
            }
            else if(state == FighterStates.Dash && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
                SwitchState(_factory.GetSubState(FighterSubStates.Dash));
                return true;
            }
            else{
                return false;
            }
        }
        else if (_ctx.DodgeInput.Read() && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.GetSubState(FighterSubStates.Dodge));
            return true;
        }
        else if (_ctx.MovementInput.Read() != 0){
            SwitchState(_factory.GetSubState(FighterSubStates.Walk));
            return true;
        }
        else{
            return false;
        }
    }

    public static float AdjustAnimationTime(AnimationClip clip, int frames){
        float length;
        float time;
        float speed;

        length = clip.length;
        //time = frames / clip.frameRate;
        time = frames * Time.deltaTime;
        speed = length / time;
        //Debug.Log("Calculated Lenght: " + time + " Actual Lenght: " + length  + " Frame Rate: " + clip.frameRate + " Calculated Speed: " + speed);

        return speed;
    }
}
