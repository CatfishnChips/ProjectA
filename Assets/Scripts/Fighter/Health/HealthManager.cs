using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int m_health;
    private int _health;
    private HurtResponder _hurtResponder;

    public int Health {get => _health; }

    private void Awake(){
        if (TryGetComponent<HurtResponder>(out HurtResponder hurtResponder)) _hurtResponder = hurtResponder;
    }

    private void Start(){
        if (_hurtResponder) _hurtResponder.HurtResponse += HurtResponse;
        _health = m_health;
    }

    private void OnDisable(){
        if (_hurtResponder) _hurtResponder.HurtResponse -= HurtResponse;
    }

    private void HurtResponse(CollisionData data){
        _health -= data.damage;
        _health = Mathf.Clamp(_health, 0, m_health);
        EventManager.Instance.HealthChanged?.Invoke(gameObject, _health);
    }
}
