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

    #region Movement Events
    // These actions are called by the Touch A, which is used primarily for movement.
    public UnityAction<Vector2> Dash;
    public UnityAction<float> Walk;

    #endregion

    #region Attack Events

    #endregion
}
