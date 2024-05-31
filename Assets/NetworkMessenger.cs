using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class NetworkMessenger : NetworkBehaviour
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



    public void RunMessenger(){
        Debug.Log("I ran the messenger.");



    }



}
