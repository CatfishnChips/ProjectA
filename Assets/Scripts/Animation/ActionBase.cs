using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBase : ScriptableObject
{
    public new string name;

    public static float AdjustAnimationTime(AnimationClip clip, int frames){
        float length;
        float time;
        float speed;

        length = clip.length;
        time = frames * Time.fixedDeltaTime; 
        speed = length / time;

        return speed;
    }
}
