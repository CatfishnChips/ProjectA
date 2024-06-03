using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class NetworkMessenger : MonoBehaviour
{
    #region singleton

    public static NetworkMessenger Instance;

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


    public Dictionary<Player, FighterManager> fighterManagerAttribution = new Dictionary<Player, FighterManager>();
    public Dictionary<int, Player> clientFighterOwnership = new Dictionary<int, Player>();

}
