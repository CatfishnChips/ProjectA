using UnityEngine;

[RequireComponent(typeof(FighterStateMachine))]
public class StaminaManager : MonoBehaviour
{
    [Tooltip("How much stamina is required to recover a single point of Block.")]
    [SerializeField] private float m_stamina;

    [Tooltip("How many hits can the character Auto-block before getting exhausted.")]
    [SerializeField] private int m_block;

    [Tooltip("How much stamina is recovered in a single frame.")]
    [SerializeField] private float m_staminaRegen;
    
    [SerializeField] [ReadOnly] private float m_currentStamina;
    [SerializeField] [ReadOnly] private int m_currentBlock;
    private Player m_player;

    public float Stamina {get => m_currentStamina;}
    public int Block {get => m_currentBlock;}
    public bool CanBlock {get {return m_currentBlock > 0;}}

    private void Start(){
        m_player = GetComponent<FighterStateMachine>().Player;
        Reset();
    }

    public void UpdateStamina(float value){
        if (m_currentBlock >= m_block) return;

        m_currentStamina += value;
    
        if (m_currentStamina >= m_stamina && m_currentBlock < m_block){
            m_currentStamina = 0f;
            m_currentBlock++;
            m_currentBlock = Mathf.Clamp(m_currentBlock, 0, m_block);

            switch(m_player)
            {
                case Player.P1:
                    EventManager.Instance.BlockChanged_P1?.Invoke(m_currentBlock, m_block);   
                break;

                case Player.P2:
                    EventManager.Instance.BlockChanged_P2?.Invoke(m_currentBlock, m_block);
                break;
            }
        }

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

    public void UpdateBlock(int value){
        m_currentBlock += value;
        m_currentBlock = Mathf.Clamp(m_currentBlock, 0, m_block);

        switch(m_player)
        {
            case Player.P1:
                EventManager.Instance.BlockChanged_P1?.Invoke(m_currentBlock, m_block);   
            break;

            case Player.P2:
                EventManager.Instance.BlockChanged_P2?.Invoke(m_currentBlock, m_block);
            break;
        }
        
        // Resetting the variable just to be safe;
        m_currentStamina = 0f;

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
        if (m_currentBlock >= m_block) return;

        m_currentStamina += m_staminaRegen;

        if (m_currentStamina >= m_stamina && m_currentBlock < m_block){
            m_currentStamina = 0f;
            m_currentBlock++;
            m_currentBlock = Mathf.Clamp(m_currentBlock, 0, m_block);

            switch(m_player)
            {
                case Player.P1:
                    EventManager.Instance.BlockChanged_P1?.Invoke(m_currentBlock, m_block);   
                break;

                case Player.P2:
                    EventManager.Instance.BlockChanged_P2?.Invoke(m_currentBlock, m_block);
                break;
            }
        }

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

    public void Reset(){
        m_currentStamina = 0f;
        m_currentBlock = m_block;

        switch(m_player)
        {
            case Player.P1:
                EventManager.Instance.StaminaChanged_P1?.Invoke(m_currentStamina, m_stamina);   
                EventManager.Instance.BlockChanged_P1?.Invoke(m_currentBlock, m_block);   
            break;

            case Player.P2:
                EventManager.Instance.StaminaChanged_P2?.Invoke(m_currentStamina, m_stamina);
                EventManager.Instance.BlockChanged_P2?.Invoke(m_currentBlock, m_block);
            break;
        }
    }
}
