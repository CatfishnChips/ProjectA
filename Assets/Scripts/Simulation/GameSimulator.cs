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

    private int _tick; // Frame of the game by logic updates.
    FighterRecord P1fighterRecData;
    FighterRecord P2fighterRecData;

    public int Tick { get => _tick; }

    // Update is called once per frame
    void FixedUpdate()
    {
        _tick++;
    }
}
