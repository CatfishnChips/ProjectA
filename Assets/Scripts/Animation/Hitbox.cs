using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private float m_hitStun;
    [SerializeField] private float m_blockStun;
    [SerializeField] private float m_damage;
    [SerializeField] private float m_knockback;
    
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
