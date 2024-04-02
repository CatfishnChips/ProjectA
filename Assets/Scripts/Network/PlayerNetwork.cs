using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] private FighterStateMachine _ctx;

    private void Start()
    {
        if(IsOwner)
        {
        }
        else
        {
        }
        
    }

    private struct NetworkAttackInput : INetworkSerializable
    {
        public int tick;
        public bool hasPerformedAttack;
        public string performedAttackName;

        public NetworkAttackInput(int tick, bool hasPerformedAttack, string performedAttackName)
        {   
            this.tick = tick;
            this.hasPerformedAttack = hasPerformedAttack;
            this.performedAttackName = performedAttackName;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref hasPerformedAttack);
            serializer.SerializeValue(ref performedAttackName);
        }
    }

    private NetworkVariable<NetworkAttackInput> networkAttackInput = new NetworkVariable<NetworkAttackInput>(
        new NetworkAttackInput
        {
            tick = 60,
            hasPerformedAttack = false,
            performedAttackName = "None"
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        networkAttackInput.OnValueChanged += (NetworkAttackInput previousValue, NetworkAttackInput newValue) =>
        {
            Debug.Log(OwnerClientId + "; Tick: " + newValue.tick + "; Has Performed Attack: " + newValue.hasPerformedAttack + "; Performed Attack: " + newValue.performedAttackName);    
        };
    }

    private void Update()
    {
        if(!IsOwner) return;

        bool c = true;
        int i = c ? 50 : 23;
        if(Input.GetKeyDown(KeyCode.T))
        {
            JumpServerRpc(new ServerRpcParams());

            // Debug.Log("Locally listened.");
            // networkAttackInput.Value = new NetworkAttackInput{
            //     tick = 50,
            //     hasPerformedAttack = true,
            //     performedAttackName = "Aduket"
            // };
        }
    }

    [ServerRpc]
    private void JumpServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log(" ClientId: " + serverRpcParams.Receive.SenderClientId.ToString());
    }


}
