using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hitbox : MonoBehaviour
{
    [SerializeField] private SC_Hitbox m_hitbox;
    [Header("Hitbox Properties")]
    [SerializeField] private Vector2 m_offset;

    [Header("Knockback Properties")]
    [SerializeField] private float m_angle;
    [SerializeField] private float m_knockback;

    [Header("SFX Properties")]
    [SerializeField] private AudioClip m_sound;
    [SerializeField] private float m_soundLevel;


    [SerializeField] private int m_priority;
    [SerializeField] private int m_part;
    [SerializeField] private float m_hitStun;
    //[SerializeField] private float m_blockStun;
    [SerializeField] private float m_damage;
    private int _part;

    
    private void Awake() 
    {
        // Get the required references here.
    }

    // Using OnTriggerEnter / OnTriggerExit can cause performance issues.
    // May need to find an alternative way to handle collision events.

    private void OnTriggerEnter(Collider other) 
    {
        //other.GetComponent<>;
    }

    private void OnTiggerExit(Collider other) 
    {

    }
}
