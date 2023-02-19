using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects", fileName = "HitboxData")]
[System.Serializable]
public class SC_Hitbox : ScriptableObject
{
    [Header("Hitbox Properties")]
    [SerializeField] private Vector2 m_offset;

    [Header("Knockback Properties")]
    [SerializeField] private float m_angle;
    [SerializeField] private float m_knockback;

    [Header("SFX Properties")]
    [SerializeField] private AudioClip m_sound;
    [SerializeField] private float m_soundLevel;

    [Space]
    [SerializeField] private int m_priority;
    [SerializeField] private int m_part;
    [SerializeField] private float m_hitStun;
    //[SerializeField] private float m_blockStun;
    [SerializeField] private float m_damage;
}

[System.Serializable]
public struct HitboxData {
    public Vector2 m_offset;

    public float m_angle;
    public float m_knockback;


    //public AudioClip m_sound;
    public float m_soundLevel;

    public int m_priority;
    public int m_part;
    public float m_hitStun;
    //public float m_blockStun;
    public float m_damage;

    // public HitboxData(Vector2 Offset, float Angle, float Knockback, AudioClip Sound, float Level, int Priority, int Part, float Hitstun, float Damage) {
    //     m_offset = Offset;
    //     m_angle = Angle;
    //     m_knockback = Knockback;
    //     //m_sound = Sound;
    //     m_soundLevel = Level;
    //     m_priority = Priority;
    //     m_part = Part;
    //     m_hitStun = Hitstun;
    //     m_damage = Damage;
    // }
}
