using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionData
{
    public ActionAttack action;
    public IHurtbox hurtbox;
    public IHitDetector hitbox;
    public Vector2 collisionPoint;

    public bool Validate(){

        if (hurtbox != null) 
            if (hurtbox.CheckHit(this)) 
                if (hurtbox.HurtResponder == null || hurtbox.HurtResponder.CheckHit(this)) 
                    if (hitbox.HitResponder == null || hitbox.HitResponder.CheckHit(this)) 
                        return true;
        return false;
    }
}

public enum HurtboxPart
{
    High,
    Mid,
    Low
}

public enum HurtboxType
{
    Player = 1 << 0,
    Enemy = 1 << 1,
    Ally = 1 << 2
}
[System.Flags]
public enum HurtboxMask 
{
    None = 0, //0000b
    Player = 1 << 0, //0001b
    Enemy = 1 << 1, //0010b
    Ally = 1 << 2 //0100b
}

public interface IHitResponder 
{
    ActionAttack Action { get; }
    public bool CheckHit(CollisionData data);
    public void Response(CollisionData data);
}

public interface IHitDetector 
{
    public bool Active { get; }
    public Transform Transform { get; }
    public IHitResponder HitResponder { get; set; }
    public void CheckHit();
}

public interface IHurtResponder 
{
    public bool CheckHit(CollisionData data);
    public void Response(CollisionData data);
}

public interface IHurtbox
{
    public bool Active { get; }
    public FighterStateMachine Owner { get; }
    public Transform Transform { get; }
    public HurtboxType Type { get; }
    public HurtboxPart Part { get; }
    public IHurtResponder HurtResponder { get; set; }
    public bool CheckHit(CollisionData data);
}