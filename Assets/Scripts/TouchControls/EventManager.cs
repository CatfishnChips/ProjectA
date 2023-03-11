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

    #endregion
    
    #region Movement Events
    // These actions are called by the Touch A, which is used primarily for movement.
    public UnityAction<Vector2> Dash;
    public UnityAction<float> Walk;

    #endregion

    #region Attack Events

    public UnityAction<string> AttackMove;
    public UnityAction<string> FighterAttackStarted;
    public UnityAction FighterAttackEnded;
    public UnityAction FighterAttackInterrupted;

    #endregion
}
