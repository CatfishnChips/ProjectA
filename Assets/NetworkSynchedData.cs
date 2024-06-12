using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkSynchedData : MonoBehaviour
{
    #region singleton

    public static NetworkSynchedData Instance;

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


    public class SyncedVariables{
        public FighterManager fighterManager;
        public int serverMovementInput;
        public bool serverBlockInput;
    }

    public Dictionary<int, Player> clientFighterOwnership = new Dictionary<int, Player>();
    public Dictionary<Player, SyncedVariables> synchedVariables = new Dictionary<Player, SyncedVariables>();

}
