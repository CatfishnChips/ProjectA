using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleHurtResponder : MonoBehaviour, IHurtResponder
{
    private List<Hurtbox> m_hurtboxes = new List<Hurtbox>();

    private void Start()
    {
        m_hurtboxes = new List<Hurtbox>(GetComponentsInChildren<Hurtbox>());
        foreach (Hurtbox hurtbox in m_hurtboxes) 
        {
            hurtbox.HurtResponder = this;
        }
    }

    bool IHurtResponder.CheckHit(CollisionData data)
    {
        return true;
    }

    void IHurtResponder.Response(CollisionData data)
    {
        Debug.Log("Hurt Response"); 
    }
}
