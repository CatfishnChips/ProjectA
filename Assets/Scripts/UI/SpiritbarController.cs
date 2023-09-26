using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritbarController : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] private Slider m_spiritbar_P1;

    [Header("Player 2")]
    [SerializeField] private Slider m_spiritbar_P2;

    void Start()
    {
        m_spiritbar_P1.maxValue = 1f;
        m_spiritbar_P2.maxValue = 1f;
    }

    public void UpdateSpiritbar_P1(float value){
        m_spiritbar_P1.value = value;
    }

    public void UpdateSpiritbar_P2(float value){
        m_spiritbar_P2.value = value;
    }
}
