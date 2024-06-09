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
        if (_ctx.ActionInput.Read()){
            if(_ctx.GestureActionDict.ContainsKey(_ctx.ActionInput.PeekContent())){
                FighterStates state = _ctx.GestureActionDict[_ctx.ActionInput.PeekContent()].stateName;
                if(state == FighterStates.Dash){
                    if(_ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
                        SwitchState(_factory.GetSubState(FighterSubStates.Dash));
                        return true;
                    }
                    return false;
                }
                else{
                    SwitchState(_factory.GetSubState((FighterSubStates)state));
                    return true;
                }
            }
            else{
                _ctx.ActionInput.Remove();
            }
        }

        if (_ctx.DodgeInput.Read() && _ctx.IsGrounded && _ctx.CurrentRootState == FighterStates.Grounded){
            SwitchState(_factory.GetSubState(FighterSubStates.Dodge));
            return true;
        }

        if(_ctx.BlockInput.Read() && _ctx.StaminaManager.Block > 0){
            Debug.Log("Block Input is read.");
            SwitchState(_factory.GetSubState(FighterSubStates.Block));
            return true;
        }

        if (_ctx.MovementInput.Read() != 0){
            SwitchState(_factory.GetSubState(FighterSubStates.Walk));
            return true;
        }

        return false;
        
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
