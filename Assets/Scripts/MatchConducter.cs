using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchConducter : MonoBehaviour
{   
    #region Variables

    [SerializeField] private int m_round;
    [SerializeField] private int m_time;
    [SerializeField] private int m_score;
    private int m_currentRound;
    private float m_currentTime;
    private int m_currentScore_P1;
    private int m_currentScore_P2;
    [SerializeField] private MatchStates m_state;

    #endregion

    #region References

    [SerializeField] [ReadOnly] private FighterStateMachine m_fighterSlot1;
    [SerializeField] [ReadOnly] private FighterStateMachine m_fighterSlot2;

    #endregion

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
            }
        }
    }

    private void Start(){
        EventManager.Instance.HealthChanged_P1 += OnHealthChanged_P1;
        EventManager.Instance.HealthChanged_P2 += OnHealthChanged_P2;

        m_currentTime = m_time;
    }

    private void OnDisable(){
        EventManager.Instance.HealthChanged_P1 -= OnHealthChanged_P1;
        EventManager.Instance.HealthChanged_P2 -= OnHealthChanged_P2;
    }

    private void Update(){
        if (m_state != MatchStates.InRound) return;

        HandleTimer();
        HandleFighterDirections();
    }

    private void OnHealthChanged_P1(float value, float maxValue){
        if (value <= 0){
            // Player 1 Lose
            HandleRoundEnd(Player.P2);
        }
    }

    private void OnHealthChanged_P2(float value, float maxValue){
        if (value <= 0){
            // Player 2 Lose
           HandleRoundEnd(Player.P1);
        }
    }

    private void HandleTimer(){
        if (!m_fighterSlot1) return;
        if (!m_fighterSlot2) return;
        
        m_currentTime -= Time.deltaTime;
        EventManager.Instance.TimeChanged?.Invoke(Mathf.FloorToInt(Mathf.Clamp(m_currentTime, 0f, m_time)));
        if (m_currentTime <= 0f){
            if (m_fighterSlot1.HealthManager.Health > m_fighterSlot2.HealthManager.Health){
                HandleRoundEnd(Player.P1);
            }
            else if (m_fighterSlot1.HealthManager.Health < m_fighterSlot2.HealthManager.Health){
                HandleRoundEnd(Player.P2);
            }
            else{
                HandleRoundEnd(Player.None);
            }
        }
    }

    private void HandleFighterDirections(){
        if (!m_fighterSlot1) return;
        if (!m_fighterSlot2) return;
        
        float direction = (m_fighterSlot1.transform.position - m_fighterSlot2.transform.position).normalized.x;
        int value = (int)Mathf.Sign(direction);

        if (m_fighterSlot1.FaceDirection != value) m_fighterSlot1.SetFaceDirection(value);
        if (m_fighterSlot2.FaceDirection != -value) m_fighterSlot2.SetFaceDirection(-value);
    }

    private void HandleRoundEnd(Player player){
        m_state = MatchStates.Standby;
        m_currentTime = m_time;
        switch(player)
        {
            case Player.P1:
                m_currentScore_P1++;
                EventManager.Instance?.ScoreChanged(Player.P1, m_currentScore_P1);
            break;

            case Player.P2:
                m_currentScore_P2++;
                EventManager.Instance?.ScoreChanged(Player.P2, m_currentScore_P2);
            break;

            case Player.None:
                m_currentScore_P1++;
                m_currentScore_P2++;
                EventManager.Instance?.ScoreChanged(Player.P1, m_currentScore_P1);
                EventManager.Instance?.ScoreChanged(Player.P2, m_currentScore_P2);
            break;
        }

        if (m_currentScore_P1 >= m_score && m_currentScore_P2 >= m_score){
            // Match Draw
        }
        else if (m_currentScore_P1 >= m_score){
            // Match P1 Victory
        }
        else if (m_currentScore_P2 >= m_score){
            // Match P2 Victory
        }
        else
        {
            EventManager.Instance?.RoundChanged(m_currentRound, m_round);
        }
    }
}

public enum MatchStates
{
    Standby,
    InRound
}