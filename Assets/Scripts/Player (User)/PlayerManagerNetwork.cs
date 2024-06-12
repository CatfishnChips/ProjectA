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

    private bool _invokerBlockInput = false;
    private bool _prevInvokerBlockInput = false;
    private bool _serverBlockInput = false;

    private int _invokerDragInput = 0;
    private int _prevInvokerDragInput = 0;
    private int _serverDragInput = 0;

    public override void OnNetworkSpawn(){
        if(!IsOwner) return;
        Debug.Log("HELOOO");

        FighterManager[] managers = FindObjectsOfType<FighterManager>();
        foreach(FighterManager manager in managers){
            NetworkSynchedData.Instance.synchedVariables.Add(manager.fighterID, new NetworkSynchedData.SyncedVariables{fighterManager = manager, serverMovementInput = 0});
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
        inputEvents.OnDrag += OnDrag;
        inputEvents.OnHold += OnBlock;
        #endregion
    }

    void ConnectInputDevice(){
        if(_inputInvoker == null) return;
        Debug.Log("Connecting input device");
        _inputInvoker.SetInputEvents(inputEvents);
    }

    public void Update(){
        if(!NetworkSynchedData.Instance.clientFighterOwnership.ContainsKey((int)OwnerClientId)) return;

        if(IsOwner){

            if(_prevInvokerDragInput != _invokerDragInput){
                MoveInputServerRpc((int)OwnerClientId, _invokerDragInput);
            }

            if(_prevInvokerBlockInput != _invokerBlockInput){
                Debug.Log("SERVER RPC");
                BlockInputServerRpc((int)OwnerClientId, _invokerBlockInput);
            }

            _prevInvokerDragInput = _invokerDragInput;
            _invokerDragInput = 0;

            _prevInvokerBlockInput = _invokerBlockInput;
            _invokerBlockInput = false;
        
        }
            
        SyncehUpdate();

    }

    public void SyncehUpdate(){
        FighterManager manager = GetSynchedVariablesById((int)OwnerClientId).fighterManager;
        int serverDragInput = GetSynchedVariablesById((int)OwnerClientId).serverMovementInput;
        if(serverDragInput != 0) manager.fighterEvents.OnMove(serverDragInput);

        bool serverBlockInput = GetSynchedVariablesById((int)OwnerClientId).serverBlockInput;
        if(serverBlockInput) {
            Debug.Log(manager.fighterID);
            Debug.Log("Triggering On Block.");
            manager.fighterEvents.OnBlock?.Invoke();
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
        if(clientID == (int)OwnerClientId){
            VirtualButtonsMultiplayer.fighterManager = GetSynchedVariablesById((int)OwnerClientId).fighterManager;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void GestureInputServerRpc(int clientID, InputGestures gesture){
        GestureInputClientRpc(clientID, gesture);
    }

    [ClientRpc]
    public void GestureInputClientRpc(int clientID, InputGestures gesture){
        GetSynchedVariablesById(clientID).fighterManager.fighterEvents.OnDirectInputGesture(gesture);
    } 

    [ServerRpc(RequireOwnership = false)]
    public void MoveInputServerRpc(int clientID, int invokerInput){
        MoveInputClientRpc(clientID, invokerInput);
    }

    [ClientRpc]
    public void MoveInputClientRpc(int clientID, int invokerInput){
        _serverDragInput = invokerInput;
        GetSynchedVariablesById(clientID).serverMovementInput = invokerInput;
    }

    [ServerRpc(RequireOwnership = false)]
    public void BlockInputServerRpc(int clientID, bool invokerBlockInput){
        BlockInputClientRpc(clientID, invokerBlockInput);
    }

    [ClientRpc]
    public void BlockInputClientRpc(int clientID, bool invokerBlockInput){
        _serverBlockInput = invokerBlockInput;
        GetSynchedVariablesById(clientID).serverBlockInput = invokerBlockInput;
        Debug.Log("Server block input is set to: " + GetSynchedVariablesById(clientID).serverBlockInput);
    }

    #region input listener functions

    public void OnDirectInput(InputGestures gesture){
        GestureInputServerRpc((int)OwnerClientId, gesture);
    }

    public void OnDrag(ScreenSide side, GestureDirections direction){
        if(side == ScreenSide.Left) {
            if(direction == GestureDirections.Left) _invokerDragInput = -1; //fighterEvents.OnMove?.Invoke(-1);
            else if(direction == GestureDirections.Right) _invokerDragInput = 1; //fighterEvents.OnMove?.Invoke(1);
        } 
    }

    public void OnBlock(ScreenSide side){
        if(side == ScreenSide.LeftNRight) _invokerBlockInput = true;
    }

    #endregion


    private NetworkSynchedData.SyncedVariables GetSynchedVariablesById(int id){
        return NetworkSynchedData.Instance.synchedVariables[NetworkSynchedData.Instance.clientFighterOwnership[id]];
    }

}
