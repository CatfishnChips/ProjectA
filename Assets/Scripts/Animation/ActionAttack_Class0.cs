using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "ScriptableObject/Action/Class0/Attack")]
public class ActionAttack_Class0 : ActionFighterAttack
{
    [Header("Focus Properties")]
    [SerializeField] protected int m_focusDamage;
    [SerializeField] protected int m_focusChipDamage;
    [SerializeField] protected bool m_focusIgnoreBlock;
    [SerializeField] protected int m_focusBlockStun;
    [SerializeField] protected int m_focusHitStop;
    [SerializeField] protected int m_focusKnockbackStun;
    [SerializeField] protected int m_focusKnockdownStun;
    [SerializeField] protected Vector2Int m_focusKnockupStun;
    [SerializeField] protected float m_focusKnockup;
    [SerializeField] protected float m_focusKnockback;

    [SerializeField] [ReadOnly] protected bool m_focus = false; 

    public override int Damage {get => m_focus ? m_focusDamage : m_damage;}
    public override int ChipDamage {get => m_focus ? m_focusChipDamage : m_chipDamage;}
    public override bool IgnoreBlock {get => m_focus ? m_focusIgnoreBlock : m_ignoreBlock;}
    public override int BlockStun {get => m_focus ? m_focusBlockStun : m_blockStun;}
    public override int HitStop {get => m_focus ? m_focusHitStop : m_hitStop;}
    public override int KnockbackStun {get => m_focus ? m_focusKnockbackStun : m_knockbackStun;}
    public override Vector2Int KnockupStun {get => m_focus ? m_focusKnockupStun : m_knockupStun;}
    public override int KnockdownStun {get => m_focus ? m_focusKnockdownStun : m_knockdownStun;}
    public override float Knockup {get => m_focus ? m_focusKnockup : m_knockup;}
    public override float Knockback {get => m_focus ? m_focusKnockback : m_knockback;}

    public override void EnterStateFunction(FighterStateMachine ctx, FighterAttackState state){
        base.EnterStateFunction(ctx, state);
        m_focus = ((FighterStateMachine_Class0)ctx).Focus;
    }
    
    public override void ExitStateFunction(FighterStateMachine ctx, FighterAttackState state){
        ((FighterStateMachine_Class0)ctx).SetFocus(false);
    }
}
