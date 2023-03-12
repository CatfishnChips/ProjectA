using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int m_health;
    private int _health;
    public int Health {get => _health;}

    private void Start(){
        _health = m_health;
    }

    public void UpdateHealth(CollisionData data){
        _health -= data.action.Damage;
        _health = Mathf.Clamp(_health, 0, m_health);
        EventManager.Instance.HealthChanged?.Invoke(gameObject, _health);
    }
}
