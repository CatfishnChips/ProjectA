using UnityEngine;
using UnityEngine.UI;

public class StaminabarController : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] private Slider m_staminabarF_P1;
    [SerializeField] private Slider m_blockbarB_P1;

    [Header("Player 2")]
    [SerializeField] private Slider m_staminabarF_P2;
    [SerializeField] private Slider m_blockbarB_P2;


    void Start()
    {
        m_staminabarF_P1.maxValue = 1f;
        m_staminabarF_P2.maxValue = 1f;
        m_blockbarB_P1.maxValue = 1f;
        m_blockbarB_P2.maxValue = 1f;
    }

    public void UpdateStaminabar_P1(float value){
        m_staminabarF_P1.value = value;
    }

    public void UpdateBlock_P1(float value){
        m_blockbarB_P1.value = value;
    }

    public void UpdateStaminabar_P2(float value){
        m_staminabarF_P2.value = value;
    }

    public void UpdateBlock_P2(float value){
        m_blockbarB_P2.value = value;
    }
}
