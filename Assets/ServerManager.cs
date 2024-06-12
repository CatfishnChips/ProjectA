using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public void SetupServer(){
        FighterManager[] managers = FindObjectsOfType<FighterManager>();
        foreach(FighterManager manager in managers){
            NetworkSynchedData.Instance.synchedVariables.Add(manager.fighterID, new NetworkSynchedData.SyncedVariables{fighterManager = manager, serverMovementInput = 0});
        }
    }
}
