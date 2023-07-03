using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{   
    [Header("Settings")]
    [SerializeField] [Range(0.1f, 1f)] private float m_frontbarInterpolation = 1f;
    [SerializeField] [Range(0.1f, 1f)] private float m_backbarInterpolation = 1f;

    [Header("Player 1")]
    [SerializeField] private Slider m_healthbarF_P1;
    [SerializeField] private Slider m_healthbarB_P1;

    [Header("Player 2")]
    [SerializeField] private Slider m_healthbarF_P2;
    [SerializeField] private Slider m_healthbarB_P2;

    private void Start(){
        m_healthbarF_P1.maxValue = 1f;
        m_healthbarB_P1.maxValue = 1f;
        m_healthbarF_P1.maxValue = 1f;
        m_healthbarB_P2.maxValue = 1f;
    }

    public void UpdateHealthbar_P1(float value){
        m_healthbarF_P1.value = value;
        StopCoroutine(HealthbarAnimation_P1());
        StartCoroutine(HealthbarAnimation_P1());
    }

    public void UpdateHealthbar_P2(float value){
        m_healthbarF_P2.value = value;
        StopCoroutine(HealthbarAnimation_P2());
        StartCoroutine(HealthbarAnimation_P2());
    }

    private IEnumerator HealthbarAnimation_P1(){
        float target = m_healthbarF_P1.value;
        while (m_healthbarB_P1.value != target)
        {
            m_healthbarB_P1.value = Mathf.MoveTowards(m_healthbarB_P1.value, target, m_backbarInterpolation * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator HealthbarAnimation_P2(){
        float target = m_healthbarF_P2.value;
        while (m_healthbarB_P2.value != target)
        {
            m_healthbarB_P2.value = Mathf.MoveTowards(m_healthbarB_P2.value, target, m_backbarInterpolation * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
