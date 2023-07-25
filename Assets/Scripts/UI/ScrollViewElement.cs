using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollViewElement : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private DynamicScrollView m_dynamicScrollView;

    private void Awake(){
        m_dynamicScrollView = GetComponentInParent<DynamicScrollView>();
    }   

    public void OnPointerUp(PointerEventData eventData){
        //m_dynamicScrollView.OnUp();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //m_dynamicScrollView.OnDown();
    }
}
