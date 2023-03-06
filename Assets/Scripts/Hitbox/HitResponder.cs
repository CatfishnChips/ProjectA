using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitResponder : MonoBehaviour, IHitResponder
{
    public UnityAction<CollisionData> HitResponse;

    [Header("Hitbox Properties")]
    [SerializeField] private Hitbox _hitbox;
    [SerializeField] private int m_priority;
    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]
    [SerializeField] private int m_part = 1;
    [SerializeField] private int m_hitStun;
    [SerializeField] private int m_blockStun;
    [SerializeField] private int m_damage = 10;

    [Header("Knockback Properties")]
    [SerializeField] private float m_knockup;
    [SerializeField] private float m_knockback;

    [Header("SFX Properties")]
    [SerializeField] private AudioClip m_sound;
    [SerializeField] private float m_soundLevel;

    int IHitResponder.Damage { get => m_damage; }
    int IHitResponder.StunDuration { get => m_hitStun; }
    float IHitResponder.Knockback { get => m_knockback; }
    float IHitResponder.Knockup { get => m_knockup; }

    private int _part;

    public void UpdateData(ActionAttack action){
        m_part = action.Part;
        m_hitStun = action.HitStun;
        m_blockStun = action.BlockStun;
        m_damage = action.Damage;
        m_knockup = action.Knockup;
        m_knockback = action.Knockback;
        m_sound = action.Sound;
        m_soundLevel = action.SoundLevel;

        _part = m_part;
    }

    private void Start(){
        _hitbox.HitResponder = this;
        //StartCoroutine(UpdateHitbox());
        _part = m_part;
    }

    private void Update(){
        if (!_hitbox.Active) return;
        if (_part <= 0) return;

        _hitbox.CheckHit();
    }

    // private IEnumerator UpdateHitbox(){
    //     yield return new WaitForFixedUpdate();
    //     if (_hitbox.Active)
    //         _hitbox.CheckHit();
    // }

    bool IHitResponder.CheckHit(CollisionData data){
        return true;
    }

    void IHitResponder.Response(CollisionData data){
        _part--;
        HitResponse?.Invoke(data);
    }
}
