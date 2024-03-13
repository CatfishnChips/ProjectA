using System;
using UnityEngine;

public abstract class FighterBaseState : StateMachineBaseState
{
    public new string name;
    public FighterStates stateName;
    public InputGestures inputGesture;
    public bool applyRootMotion;

    protected FighterStateMachine _ctx;
    protected FighterStateFactory _factory;

    protected int _currentFrame;
    protected int CurrentFrame { get => _currentFrame; set => _currentFrame = value; }

    public FighterBaseState(){
        name = GetType().ToString();
        if(stateName == default){
            foreach(FighterStates state in (FighterStates[])Enum.GetValues(typeof(FighterStates))){
                if(name.Contains(state.ToString())){
                    stateName = state;
                }
            }
        }
    }

    public virtual void Initialize(IStateMachineRunner ctx, FighterStateFactory factory){
        base.Initialize(ctx);
        _ctx = _context as FighterStateMachine;
        _factory = factory;
    }

    public FighterBaseState Clone(){
        FighterBaseState cloneState = Instantiate(this);
        return cloneState;
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
