using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchConducter : MonoBehaviour
{   
    [SerializeField] private FighterStateMachine m_fighterSlot1;
    [SerializeField] private FighterStateMachine m_fighterSlot2;
    [SerializeField] private UIManager m_uiManager;

    private void Awake(){
        List<FighterStateMachine> fighters = new List<FighterStateMachine>();
        fighters.AddRange(FindObjectsOfType<FighterStateMachine>());

        foreach(FighterStateMachine fighter in fighters){
            switch(fighter.Player)
            {
                case Player.P1:
                    m_fighterSlot1 = fighter;
                break;

                case Player.P2:
                    m_fighterSlot2 = fighter;
                break;

                case Player.AI:
                    m_fighterSlot2 = fighter;
                break;
            }
        }

        m_uiManager = FindObjectOfType<UIManager>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
