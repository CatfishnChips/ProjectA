using UnityEngine;
using System.Collections.Generic;

public class ScoreIndicatorController : MonoBehaviour
{  [Header("Player1")]
   [SerializeField] private Transform m_scoreIndicators_P1;
   private List<GameObject> m_scoreIndicatorList_P1 = new List<GameObject>();

   [Header("Player2")]
   [SerializeField] private Transform m_scoreIndicators_P2;
   private List<GameObject> m_scoreIndicatorList_P2 = new List<GameObject>();

   private void Start(){
      foreach(RectTransform obj in m_scoreIndicators_P1.GetComponentsInChildren<RectTransform>()){
         if(obj != m_scoreIndicators_P1) 
         {
            m_scoreIndicatorList_P1.Add(obj.gameObject);
            obj.gameObject.SetActive(false);
         }
      }

      foreach(RectTransform obj in m_scoreIndicators_P2.GetComponentsInChildren<RectTransform>()){
         if(obj != m_scoreIndicators_P2) 
         {
            m_scoreIndicatorList_P2.Add(obj.gameObject);
            obj.gameObject.SetActive(false);
         }
      }
   }

   public void UpdateScore(Player player, int value){
      switch(player)
      {
         case Player.P1:
            m_scoreIndicatorList_P1[value-1].SetActive(true);
         break;

         case Player.P2:
            m_scoreIndicatorList_P2[value-1].SetActive(true);
         break;
      }
   }
}
