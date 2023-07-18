using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_StageSelect : MonoBehaviour
{
    private Element m_selectedElement;
    private Element m_lastSelectedElement;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private DynamicScrollView m_dynamicScrollView;
    private bool m_fadingOut;

    void Update()
    {
        m_selectedElement = m_dynamicScrollView.m_selectedElement;
        if (m_selectedElement != m_lastSelectedElement){
            m_lastSelectedElement = m_selectedElement;

            if (!m_fadingOut) {
                StopAllCoroutines();
                StartCoroutine(FadeOut(0.25f));
            }
            
        }
    }

     private IEnumerator FadeIn(float duration){
        float time = 0f;
        m_backgroundImage.sprite = m_selectedElement.Sprite;
        while(time < duration)
        {
            m_backgroundImage.CrossFadeAlpha(1, duration, false);
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FadeOut(float duration){
        float time = 0f;
        m_fadingOut = true;
        while(time < duration)
        {
            m_backgroundImage.CrossFadeAlpha(0, duration, false);
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        m_fadingOut = false;
        StartCoroutine(FadeIn(duration));
    }
}
