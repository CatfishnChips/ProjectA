using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthbarController m_healthbarController;

    void Start(){
        EventManager.Instance.HealthChanged += OnHealthChanged;
    }

    private void OnDisable(){
        EventManager.Instance.HealthChanged -= OnHealthChanged;
    }

    #region Healthbar Controller

    private void OnHealthChanged(GameObject owner, int health){
        if (!m_healthbarController) return;
        m_healthbarController.UpdateHealthbar(owner, health);
    }

    private void SetupHealthbar(){
        if (!m_healthbarController) return;
        // Before the game starts setup the values of the healthbar.
        //m_healthbar.maxValue = 
    }

    #endregion
}
