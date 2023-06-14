using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    #region Singleton

    public static EventManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    #region Game Events

    //public UnityAction<> OnCombatBegin;
    //public UnityAction<> OnCombatEnd;
    public UnityAction<GameObject, int> HealthChanged;
    public UnityAction<GameObject, int> StaminaChanged;

    #endregion
    
    #region Movement Events
    // These actions are called by the Touch A, which is used primarily for movement.
    public UnityAction<Vector2> Swipe;
    public UnityAction<float> Move;
    public UnityAction OnTap;
    public UnityAction<bool> OnHoldA;
    public UnityAction<bool> OnHoldB;

    #endregion

    #region Attack Events

    public UnityAction<string> AttackMove;
    public UnityAction<string> FighterAttackStarted;
    public UnityAction FighterAttackEnded;
    public UnityAction FighterAttackInterrupted;

    #endregion


    #region Player2

    #region Game Events

    //public UnityAction<> OnCombatBegin;
    //public UnityAction<> OnCombatEnd;
    public UnityAction<GameObject, int> P2HealthChanged;
    public UnityAction<GameObject, int> P2StaminaChanged;

    #endregion

    #region Movement Events
    // These actions are called by the Touch A, which is used primarily for movement.
    public UnityAction<Vector2> P2Swipe;
    public UnityAction<float> P2Move;
    public UnityAction P2OnTap;
    public UnityAction<bool> P2OnHoldA;
    public UnityAction<bool> P2OnHoldB;

    #endregion

    #region Attack Events

    public UnityAction<string> P2AttackMove;
    public UnityAction<string> P2FighterAttackStarted;
    public UnityAction P2FighterAttackEnded;
    public UnityAction P2FighterAttackInterrupted;

    #endregion

    #endregion
}
