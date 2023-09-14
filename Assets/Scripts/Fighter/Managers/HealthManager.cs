using UnityEngine;

[RequireComponent(typeof(FighterStateMachine))]
public class HealthManager : MonoBehaviour
{
    [Tooltip("Value that dictates how much damage the character can take before fainting.")]
    [SerializeField] private int m_health;
    [SerializeField] [ReadOnly] private int m_currentHealth;
    public int Health {get => m_currentHealth;}

    private Player m_player;

    private void Start(){
        m_player = GetComponent<FighterStateMachine>().Player;
        Reset();
    }

    public void UpdateHealth(int value){
        m_currentHealth += value;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_health);

        switch(m_player){
            case Player.P1:
                EventManager.Instance.HealthChanged_P1?.Invoke(m_currentHealth, m_health);
            break;

            case Player.P2:
                EventManager.Instance.HealthChanged_P2?.Invoke(m_currentHealth, m_health);
            break;
        }
    }

    public void Reset(){
        m_currentHealth = m_health;

        switch(m_player){
            case Player.P1:
                EventManager.Instance.HealthChanged_P1?.Invoke(m_currentHealth, m_health);
            break;

            case Player.P2:
                EventManager.Instance.HealthChanged_P2?.Invoke(m_currentHealth, m_health);
            break;
        }
    }
}
