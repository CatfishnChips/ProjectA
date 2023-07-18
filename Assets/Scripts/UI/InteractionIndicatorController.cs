using System.Collections;
using UnityEngine;
using TMPro;

public class InteractionIndicatorController : MonoBehaviour
{
    [SerializeField] private int m_time;
    [Header("Player 1")]
    [SerializeField] private TextMeshProUGUI m_interactionText_P1;
    [Header("Player 2")]
    [SerializeField] private TextMeshProUGUI m_interactionText_P2;

    private string m_text;

    private void Start(){
        m_interactionText_P1.text = "";
        m_interactionText_P2.text = "";
    }

    private string ChangeString(Interactions value){
        m_text = "";
        switch (value)
        {
            case Interactions.Counter:
                m_text = "Counter";
            break;

            case Interactions.Punish:
                m_text = "Punish";
            break;

            case Interactions.Break:
                m_text = "Break";
            break;
        }
        return m_text;
    }

    public void UpdateText_P1(Interactions value){
        StopCoroutine(SetText_P1(value));
        StartCoroutine(SetText_P1(value));
    }

    public void UpdateText_P2(Interactions value){
        StopCoroutine(SetText_P2(value));
        StartCoroutine(SetText_P2(value));
    }

    private IEnumerator SetText_P1(Interactions value){
        int t = 0;
        m_interactionText_P1.text = ChangeString(value);
        while (t < m_time)
        {
            t++;
            yield return new WaitForFixedUpdate();
        }
        m_interactionText_P1.text = "";
    }

    private IEnumerator SetText_P2(Interactions value){
        int t = 0;
        m_interactionText_P2.text = ChangeString(value);
        while (t < m_time)
        {
            t++;
            yield return new WaitForFixedUpdate();
        }
        m_interactionText_P2.text = "";
    }
}
