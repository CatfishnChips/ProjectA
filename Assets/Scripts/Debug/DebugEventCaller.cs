using UnityEngine;

public class DebugEventCaller : MonoBehaviour
{
    [SerializeField] private int m_time;
    [SerializeField] private int m_round;
    private int m_currentRound = 0;
    private int m_currentScore = 0;
    [SerializeField] private int m_health;
    [SerializeField] private int m_currentHealth;
    [SerializeField] private int m_stamina;
    [SerializeField] private int m_currentStamina;
    public void SetTime(){
        EventManager.Instance?.TimeChanged(m_time);
    }

    public void IncreaseRound(){
        m_currentRound++;
        EventManager.Instance?.RoundChanged(m_currentRound, m_round);
    }

    public void IncreaseScore_P1(){
        m_currentScore++;
        EventManager.Instance?.ScoreChanged(Player.P1, m_currentScore);
    }

    public void SetHealth_P1(){
        float health = m_currentHealth / m_health;
        EventManager.Instance?.HealthChanged_P1(m_currentHealth, m_health);
    }

    public void SetStamina_P1(){
        EventManager.Instance?.StaminaChanged_P1(m_currentStamina, m_stamina);
    }
}
