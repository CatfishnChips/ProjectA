using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerManagerNetwork : NetworkBehaviour
{
    [ReadOnly] protected PlayerID _playerID;
    private FighterManager _fighterManager;
    protected IInputInvoker _inputInvoker;

    public InputEvents inputEvents = new InputEvents(); 
    protected InputEvents _networkInputEvents = new InputEvents();

    public override void OnNetworkSpawn(){
        if(!IsOwner) return;

        FighterManager[] managers = FindObjectsOfType<FighterManager>();
        foreach(FighterManager manager in managers){
            NetworkSynchedData.Instance.fighterManagerAttribution.Add(manager.fighterID, manager);    
        }

        JoinNetworkServerRpc((int)OwnerClientId);

        GameObject inputManager = GameObject.Find("InputManager");
        IInputInvoker[] inputInvokers = inputManager.GetComponents<IInputInvoker>();
        foreach(IInputInvoker invoker in inputInvokers)
        {
            Debug.Log("Looking for input invokers");
            if(invoker.IsActiveAndEnabled()){
                _inputInvoker = invoker;
                Debug.Log("Input Invoker is detected by owner: " + OwnerClientId);
                break;
            }
        }
        ConnectInputDevice();

        #region Event Subscription
        inputEvents.OnDirectInputGesture += OnDirectInput;
        #endregion
    }

    void ConnectInputDevice(){
        if(_inputInvoker == null) return;
        Debug.Log("Connecting input device");
        _inputInvoker.SetInputEvents(inputEvents);
    }

    public void Update(){
        if(!IsOwner) return;
        if(Input.GetKeyDown(KeyCode.T)){
            foreach(KeyValuePair<int, Player> clientFighter in NetworkSynchedData.Instance.clientFighterOwnership){
                Debug.Log(clientFighter.Key + " " + clientFighter.Value);
            }
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void JoinNetworkServerRpc(int clientID){

        Player fighterID = Player.None;
        if(NetworkSynchedData.Instance.clientFighterOwnership.Count != 0){
            Debug.Log("Added client to the server.");
            foreach(int id in NetworkSynchedData.Instance.clientFighterOwnership.Keys){
                if((int)OwnerClientId > id) fighterID = Player.P2;
            }
        }
        else{
            fighterID = Player.P1;
            Debug.Log("Server Assigned : " + fighterID + " To client: " + clientID);
        }


        Debug.Log("Adding value to dictionary with fighter ID: " + fighterID);
        if(!NetworkSynchedData.Instance.clientFighterOwnership.ContainsKey(clientID)) NetworkSynchedData.Instance.clientFighterOwnership.Add(clientID, fighterID);
        Debug.Log("Server Received new client. ");
        foreach(KeyValuePair<int, Player> clientFighter in NetworkSynchedData.Instance.clientFighterOwnership){
            Debug.Log("Server sending message to add clientID: " + clientFighter.Key + " fighterID: " + clientFighter.Value);
            JoinNetworkClientRpc(clientFighter.Key, clientFighter.Value);
        }
    }

    [ClientRpc]
    public void JoinNetworkClientRpc(int clientID, Player fighterID){
        Debug.Log("Adding to the network.");
        if(!NetworkSynchedData.Instance.clientFighterOwnership.ContainsKey(clientID)){
            NetworkSynchedData.Instance.clientFighterOwnership.Add(clientID, fighterID);
            Debug.Log("Added Client to network, clientID is: " + clientID + " fighterID is: " + NetworkSynchedData.Instance.clientFighterOwnership[clientID]);
        } 
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void GestureInputServerRpc(int clientID, InputGestures gesture){
        GestureInputClientRpc(clientID, gesture);
    }

    [ClientRpc]
    public void GestureInputClientRpc(int clientID, InputGestures gesture){
        NetworkSynchedData.Instance.fighterManagerAttribution[NetworkSynchedData.Instance.clientFighterOwnership[clientID]].fighterEvents.OnDirectInputGesture(gesture);
    } 

    #region input listener functions

    public void OnDirectInput(InputGestures gesture){
        GestureInputServerRpc((int)OwnerClientId, gesture);
    }

    #endregion

}
