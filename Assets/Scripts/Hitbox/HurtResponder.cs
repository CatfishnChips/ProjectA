using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HurtResponder : MonoBehaviour, IHurtResponder
{
    public UnityAction<CollisionData> HurtResponse;
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
        HurtResponse?.Invoke(data);
    }
}
