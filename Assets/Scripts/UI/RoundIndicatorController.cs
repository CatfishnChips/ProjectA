using UnityEngine;
using TMPro;

public class RoundIndicatorController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_roundText;
    private const string m_text = "Round";

    public void UpdateText(int value, int maxValue){
        string text;

        if (value >= maxValue) text = $"Final {m_text}";
        else text = $"{m_text} {value.ToString()}";

        m_roundText.text = text;
    }
}
