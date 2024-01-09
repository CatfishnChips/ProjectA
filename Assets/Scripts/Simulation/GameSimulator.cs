using System.Collections;
using UnityEngine;

public class GameSimulator : MonoBehaviour
{

    #region Singleton

    public static GameSimulator Instance;

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

    private int _tickCount; // Frame of the game by logic updates.
    private int _updateCount; // Frame of the game by render updates.
    FighterRecord P1fighterRecData;
    FighterRecord P2fighterRecData;

    public int TickCount { get => _tickCount; }
    public int UpdateCount { get => _updateCount; }

    // Update is called once per frame
    void FixedUpdate()
    {
        _tickCount++;
    }

    void Update()
    {
        _updateCount++;
    }

}
