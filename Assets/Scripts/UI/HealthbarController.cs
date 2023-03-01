using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    [SerializeField] private Slider m_healthbarF;
    [SerializeField] private Slider m_healthbarB;

    public void UpdateHealthbar(GameObject owner, int health){
        m_healthbarF.value = health;
    }

    private void Update(){
        float target = m_healthbarF.value;
        m_healthbarB.value = Mathf.MoveTowards(m_healthbarB.value, target, 10f * Time.deltaTime);
    }

    private void Start(){
        m_healthbarF.maxValue = 100f;
        m_healthbarB.maxValue = 100f;

        m_healthbarF.value = 100f;
        m_healthbarB.value = 100f;
    }
}
