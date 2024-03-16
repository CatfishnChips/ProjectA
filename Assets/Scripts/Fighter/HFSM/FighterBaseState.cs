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
