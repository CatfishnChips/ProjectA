using System.Collections.Generic;
using System.ComponentModel;
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

        Player fighterID = Player.None;

        if(OwnerClientId == 0) fighterID = Player.P1;
        else if(OwnerClientId == 1) fighterID = Player.P2;

        FighterManager[] managers = FindObjectsOfType<FighterManager>();
        foreach(FighterManager manager in managers){
            NetworkMessenger.Instance.fighterManagerAttribution.Add(manager.fighterID, manager);    
        }

        JoinNetworkServerRpc((int)OwnerClientId, fighterID);

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
            foreach(KeyValuePair<int, Player> clientFighter in NetworkMessenger.Instance.clientFighterOwnership){
                Debug.Log(clientFighter.Key + " " + clientFighter.Value);
            }
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void JoinNetworkServerRpc(int clientID, Player fighterID){
        Debug.Log("Adding value to dictionary.");
        if(!NetworkMessenger.Instance.clientFighterOwnership.ContainsKey(clientID)) NetworkMessenger.Instance.clientFighterOwnership.Add(clientID, fighterID);
        Debug.Log("Server Received new client. ");
        foreach(KeyValuePair<int, Player> clientFighter in NetworkMessenger.Instance.clientFighterOwnership){
            Debug.Log("Server sending message to add clientID: " + clientFighter.Key + " fighterID: " + clientFighter.Value);
            JoinNetworkClientRpc(clientFighter.Key, clientFighter.Value);
        }
    }

    [ClientRpc]
    public void JoinNetworkClientRpc(int clientID, Player fighterID){
        Debug.Log("Adding to the network.");
        if(!NetworkMessenger.Instance.clientFighterOwnership.ContainsKey(clientID)){
            NetworkMessenger.Instance.clientFighterOwnership.Add(clientID, fighterID);
            Debug.Log("Added Client to network, clientID is: " + clientID + " fighterID is: " + NetworkMessenger.Instance.clientFighterOwnership[clientID]);
        } 
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void GestureInputServerRpc(int clientID, InputGestures gesture){
        GestureInputClientRpc(clientID, gesture);
    }

    [ClientRpc]
    public void GestureInputClientRpc(int clientID, InputGestures gesture){
        NetworkMessenger.Instance.fighterManagerAttribution[NetworkMessenger.Instance.clientFighterOwnership[clientID]].fighterEvents.OnDirectInputGesture(gesture);
    } 

    #region input listener functions

    public void OnDirectInput(InputGestures gesture){
        GestureInputServerRpc((int)OwnerClientId, gesture);
    }

    #endregion

}
