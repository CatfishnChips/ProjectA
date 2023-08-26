using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboIndicatorController : MonoBehaviour
{
    // NOTE: THIS SCRIPT IS READY TO USE BUT MISSING THE REQUIRED EVENTS TO BE SETUP IN EVENT MANAGER, FIGHTER STATE MACHINE AND UI MANAGER.
    [Header("Player1")]
    [SerializeField] private TextMeshProUGUI m_comboText_P1;
    [Header("Player2")]
    [SerializeField] private TextMeshProUGUI m_comboText_P2;

    private int m_combo_P1 = 0; // Combo is maintained as long as the attacks hit the opponent whilst they are still in Stunned state, else the combo breaks.
    private int m_combo_P2 = 0;

    void Start()
    {
        m_comboText_P1.text = "";
        m_comboText_P2.text = "";
        m_combo_P1 = 0;
        m_combo_P2 = 0;
    }

    public void UpdateCombo_P1(int value){
        m_combo_P1++;
        StopCoroutine(SetText_P1(value));
        StartCoroutine(SetText_P1(value));
    }

    public void UpdateCombo_P2(int value){
        m_combo_P2++;
        StopCoroutine(SetText_P2(value));
        StartCoroutine(SetText_P2(value));
    }


    private IEnumerator SetText_P1(int value){
        int t = 0;
        while (t < value)
        {
            t++;
            yield return new WaitForFixedUpdate();
        }
        m_comboText_P1.text = "";
        m_combo_P1 = 0;
    }

    private IEnumerator SetText_P2(int value){
        int t = 0;
        while (t < value)
        {
            t++;
            yield return new WaitForFixedUpdate();
        }
        m_comboText_P2.text = "";
        m_combo_P2 = 0;
    }
}
