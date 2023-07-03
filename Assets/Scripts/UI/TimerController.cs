using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerText;

    // Currently only supports numbers up to 2 decimals.
    public void UpdateTimer(int value){
        string time;
        if (value == 100) time = "∞";
        else if (value < 10) time = "0" + value;
        else time = value.ToString();
        m_timerText.text = time;
    }
}
