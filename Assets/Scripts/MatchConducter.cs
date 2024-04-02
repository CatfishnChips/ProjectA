using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MatchConducter : MonoBehaviour
{   
    #region Singleton

    public static MatchConducter Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FillFighterSlots();

        float xL = m_stageConfineBorders.points[0].x;
        float yU = m_stageConfineBorders.points[0].y;
        float xR = m_stageConfineBorders.points[2].x;
        float yD = m_stageConfineBorders.points[2].y;
        m_stageBorders = new Vector4(xL, xR, yD, yU);
    }

    #endregion

    #region Variables

    [Header("Settings")]
    [SerializeField] [ReadOnly] private MatchStates m_state;
    [SerializeField] [Range(1, 100)] private int m_time;
    [SerializeField] [Range(1, 3)] private int m_score;
    [SerializeField] private bool m_isTraining;
    private int m_currentRound;
    private float m_currentTime;
    private int m_currentScore_P1;
    private int m_currentScore_P2;
    private int m_round {get{
        switch (m_score){
            case 1:
                return 1;
            case 2:
                return 3;
            case 3:
                return 5;  
            default:
                return 1;
            }
        }
    }

    #endregion

    #region References

    [Header("References")]
    [SerializeField] private Transform m_spawnPoint1;
    [SerializeField] private Transform m_spawnPoint2;

    [SerializeField] [ReadOnly] private FighterStateMachine m_fighterSlot1;
    [SerializeField] [ReadOnly] private FighterStateMachine m_fighterSlot2;

    [SerializeField] private PolygonCollider2D m_stageConfineBorders;
    private Vector4 m_stageBorders;

    public PolygonCollider2D StageConfineBorders {get => m_stageConfineBorders;}
    public Vector4 StageBorders {get => m_stageBorders;}
    public FighterStateMachine FighterSlot1 { get => m_fighterSlot1; set => m_fighterSlot1 = value; }
    public FighterStateMachine FighterSlot2 { get => m_fighterSlot2; set => m_fighterSlot2 = value; }

    #endregion

    private void FillFighterSlots(){
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

        EventManager.Instance.ResetMatch += OnReset;
        EventManager.Instance.PauseMatch += OnPause;

        m_currentTime = m_time;
        EventManager.Instance.TimeChanged?.Invoke(Mathf.FloorToInt(m_currentTime));
    }

    private void OnDisable(){
        EventManager.Instance.HealthChanged_P1 -= OnHealthChanged_P1;
        EventManager.Instance.HealthChanged_P2 -= OnHealthChanged_P2;

        EventManager.Instance.ResetMatch -= OnReset;
        EventManager.Instance.PauseMatch -= OnPause;
    }

    #region Event Methods

    private void OnReset(){
        HandleRoundStart();
    }

    private void OnPause(){
        HandlePause();
    }

    #endregion

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
        if (m_time == 100) return; // 99 is the highest 2 decimal number, 100 indicates the timer is disabled.
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
        
        float direction = (m_fighterSlot2.transform.position - m_fighterSlot1.transform.position).normalized.x;
        int value = (int)Mathf.Sign(direction);

        if (m_fighterSlot1.FaceDirection != value) m_fighterSlot1.SetFaceDirection(value);
        if (m_fighterSlot2.FaceDirection != -value) m_fighterSlot2.SetFaceDirection(-value);
    }

    private void HandleRoundEnd(Player player){
        if (m_isTraining) {
            switch(player)
            {
                case Player.P1:
                    m_fighterSlot2.HealthManager.Reset();
                break;

                case Player.P2:
                    m_fighterSlot1.HealthManager.Reset();
                break;

                case Player.None:
                    m_fighterSlot1.HealthManager.Reset();
                    m_fighterSlot2.HealthManager.Reset();
                break;
            }
        }
        else{
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
                EventManager.Instance.MatchEnded?.Invoke(Player.None);
            }
            else if (m_currentScore_P1 >= m_score){
                EventManager.Instance.MatchEnded?.Invoke(Player.P1);
            }
            else if (m_currentScore_P2 >= m_score){
                EventManager.Instance.MatchEnded?.Invoke(Player.P2);
            }
            else
            {
                EventManager.Instance?.RoundChanged(m_currentRound, m_round);
                HandleRoundStart();

            }
        }
    }

    private void HandleRoundStart(){
        m_fighterSlot1.ResetVariables();
        m_fighterSlot2.ResetVariables();
        m_fighterSlot1.transform.position = m_spawnPoint1.position;
        m_fighterSlot2.transform.position = m_spawnPoint2.position;

        m_currentTime = m_time;
        EventManager.Instance.TimeChanged?.Invoke(Mathf.FloorToInt(Mathf.Clamp(m_currentTime, 0f, m_time)));

        m_state = MatchStates.InRound;
    }

    private void HandlePause(){
        TimeController.Instance.TogglePause();
    }
}

public enum MatchStates
{
    Standby,
    InRound
}