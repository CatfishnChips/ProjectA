using UnityEngine;

[RequireComponent(typeof(FighterStateMachine))]
public class StaminaManager : MonoBehaviour
{
    [Tooltip("How many hits can the character Auto-block before getting exhausted.")]
    [SerializeField] private int m_stamina;

    [Tooltip("Time needed to recover a single unit of stamina (in frames).")]
    [SerializeField] private int m_recoveryTime; // in frames
    
    [SerializeField] [ReadOnly] private int m_currentStamina;
    private int m_currentFrame = 0;
    private Player m_player;

    public int Stamina {get => m_currentStamina;}
    public bool CanBlock {get{ return m_currentStamina > 0;}}

    private void Start(){
        m_player = GetComponent<FighterStateMachine>().Player;
        m_currentStamina = m_stamina;

        switch(m_player)
        {
            case Player.P1:
                EventManager.Instance.StaminaChanged_P1?.Invoke(m_currentStamina, m_stamina);   
            break;

            case Player.P2:
                EventManager.Instance.StaminaChanged_P2?.Invoke(m_currentStamina, m_stamina);
            break;
        }
    }

    public void UpdateStamina(){
        m_currentStamina--;
        m_currentStamina = Mathf.Clamp(m_currentStamina, 0, m_stamina);

        switch(m_player)
        {
            case Player.P1:
                EventManager.Instance.StaminaChanged_P1?.Invoke(m_currentStamina, m_stamina);   
            break;

            case Player.P2:
                EventManager.Instance.StaminaChanged_P2?.Invoke(m_currentStamina, m_stamina);
            break;
        }
    }

    private void FixedUpdate(){
        m_currentFrame++;
        if (m_currentFrame >= m_recoveryTime){
            m_currentFrame = 0;
            m_currentStamina++;
            m_currentStamina = Mathf.Clamp(m_currentStamina, 0, m_stamina);

            switch(m_player)
            {
                case Player.P1:
                    EventManager.Instance.StaminaChanged_P1?.Invoke(m_currentStamina, m_stamina);   
                break;

                case Player.P2:
                    EventManager.Instance.StaminaChanged_P2?.Invoke(m_currentStamina, m_stamina);
                break;
            }
        }
    }
}
