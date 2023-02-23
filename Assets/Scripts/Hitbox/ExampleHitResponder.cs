using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleHitResponder : MonoBehaviour, IHitResponder
{
    [Header("Hitbox Properties")]
    [SerializeField] private bool m_attack;
    [SerializeField] private Hitbox _hitbox;
    [SerializeField] private int m_priority;
    [SerializeField] private int m_part;
    [SerializeField] private float m_hitStun;
    [SerializeField] private float m_blockStun;
    [SerializeField] private int m_damage = 10;

    [Header("Knockback Properties")]
    [SerializeField] private float m_angle;
    [SerializeField] private float m_knockback;

    [Header("SFX Properties")]
    [SerializeField] private AudioClip m_sound;
    [SerializeField] private float m_soundLevel;

    int IHitResponder.Damage { get => m_damage; }

    private void Start(){
        _hitbox.HitResponder = this;
    }

    private void Update(){
        if (m_attack) 
            _hitbox.CheckHit();
    }

    bool IHitResponder.CheckHit(CollisionData data){
        return true;
    }

    void IHitResponder.Response(CollisionData data){
        
    }
}
