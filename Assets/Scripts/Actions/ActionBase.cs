using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBase : ScriptableObject
{
    public new string name;
    public FighterStates stateName;
    public InputGestures inputGesture;
    public bool applyRootMotion;

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
