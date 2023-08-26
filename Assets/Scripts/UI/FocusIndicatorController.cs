using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusIndicatorController : MonoBehaviour
{   
    [Header("Player1")]
    [SerializeField] private GameObject m_focusIndicator_P1;

    [Header("Player2")]
    [SerializeField] private GameObject m_focusIndicator_P2;

    private void Start(){
        m_focusIndicator_P1.SetActive(false);
        m_focusIndicator_P2.SetActive(false);
    }
    
    public void SetFocus_P1(bool value){
        m_focusIndicator_P1.SetActive(value);
    }

    public void SetFocus_P2(bool value){
        m_focusIndicator_P2.SetActive(value);
    }
}
