using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DynamicScrollView : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private ScrollRect m_scrollRect;
    [SerializeField] private RectTransform m_scrollView;
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private List<Element> m_stageList;
    [SerializeField] private AnimationCurve m_curve;
    [SerializeField] private AnimationCurve m_curve2;
    private Vector3[] m_worldCorners = new Vector3[4];

    private float m_highestPercentage;
    public Element m_selectedElement;

    private void Awake(){
        m_scrollRect = m_scrollView.GetComponent<ScrollRect>();
    }

    private void Start(){
        for (int i = 0; i < m_stageList.Count; i++){;
            m_stageList[i].Object.GetComponentInChildren<Image>().sprite = m_stageList[i].Sprite;
        }
    }

    void Update()
    {
        m_scrollView.GetWorldCorners(m_worldCorners);

        m_highestPercentage = 0;
        float percentage;
        for (int i = 0; i < m_stageList.Count; i++){

            percentage = (m_scrollView.position.y - m_stageList[i].Slot.position.y)/(m_worldCorners[1].y - m_worldCorners[0].y);
            float curveValue = m_curve.Evaluate(Mathf.Clamp01(1 - Mathf.Abs(percentage)));
            float curveValue2 = m_curve2.Evaluate( Mathf.Sign(percentage) * Mathf.Clamp01(1 - Mathf.Abs(percentage)));

            m_stageList[i].Object.position = m_stageList[i].Slot.position;
            m_stageList[i].Object.GetChild(0).localScale = new Vector3(curveValue, curveValue, 1f);
            m_stageList[i].Object.GetChild(0).localPosition = new Vector3(0f, Mathf.Sign(percentage) * curveValue2, 0f);

            m_stageList[i].Percentage = (Mathf.Clamp01(1 - Mathf.Abs(percentage)));

            // USED FOR DEBUG
            //m_stageList[i].Text.text = Mathf.Clamp01(1 - Mathf.Abs(percentage)).ToString();
            //m_stageList[i].Text.text = (Mathf.Sign(percentage) * Mathf.Clamp01(1 - Mathf.Abs(percentage))).ToString();

            if (Mathf.Clamp01(1 - Mathf.Abs(percentage)) <= 0.5f){
                m_stageList[i].Object.gameObject.SetActive(false);
            }
            else{
                m_stageList[i].Object.gameObject.SetActive(true);
            }
            if (Mathf.Clamp01(1 - Mathf.Abs(percentage)) > m_highestPercentage){
                m_highestPercentage = Mathf.Clamp01(1 - Mathf.Abs(percentage));
                m_selectedElement = m_stageList[i];
            }
        }
        m_selectedElement.Object.SetAsLastSibling();
    }

    private IEnumerator ScrollTo(){
        bool run = true;
        bool firstTime = true;
        float startingPos = 0f;
        float closestValue = 0f;
        float value = 0f;
        float time = 0f;
        Element closestElement = m_stageList[0];
        while(run)
        {
            if (Mathf.Abs(m_scrollRect.velocity.y) <= 45f && firstTime){
                m_scrollRect.velocity = Vector2.zero;
                firstTime = false;
                startingPos = m_scrollRect.content.position.y;
                
                foreach(Element element in m_stageList){
                    if (element.Percentage > closestValue){
                        closestValue = element.Percentage;
                        closestElement = element;
                    }
                }
                value = closestElement.Slot.position.y;
            } 
            if (!firstTime){
                //m_scrollRect.content.position = new Vector3(m_scrollRect.content.position.x, Mathf.MoveTowards(m_scrollRect.content.position.y, startingPos + (m_scrollView.position.y - value), 5f), m_scrollRect.content.position.z);
                m_scrollRect.content.position = new Vector3(m_scrollRect.content.position.x, Mathf.Lerp(startingPos, startingPos + (m_scrollView.position.y - value), time), m_scrollRect.content.position.z);
                time += Time.fixedDeltaTime * 2.5f;
                time = Mathf.Clamp01(time);
                if (Mathf.Approximately(m_scrollRect.content.position.y, value)){
                    run = false;
                }
            }
           
            yield return new WaitForFixedUpdate();
        } 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UP!");
        StartCoroutine(ScrollTo());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("DOWN!");
        StopAllCoroutines();
    }
}

[System.Serializable]
public class Element
{
    public RectTransform Slot;
    public RectTransform Object;
    [SerializeField] private float _percentage;
    public float Percentage {get{return _percentage;} set{_percentage = value;}}
    public TextMeshProUGUI Text;

    public Sprite Sprite;
    
}
