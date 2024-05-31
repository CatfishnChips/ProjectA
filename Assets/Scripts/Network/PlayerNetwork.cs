using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private void Start()
    {
        if(IsOwner)
        {
        }
        else
        {
        }
        
    }

    private struct NetworkSpikeInput : INetworkSerializable
    {
        public int tick;
        public bool hasPerformedAttack;
        public string performedAttackName;

        public NetworkSpikeInput(int tick, bool hasPerformedAttack, string performedAttackName)
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

    private NetworkVariable<NetworkSpikeInput> networkSpikeInput = new NetworkVariable<NetworkSpikeInput>(
        new NetworkSpikeInput
        {
            tick = 60,
            hasPerformedAttack = false,
            performedAttackName = "None"
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        networkSpikeInput.OnValueChanged += InputSpikeListener;
    }

    private void InputSpikeListener(NetworkSpikeInput previousValue, NetworkSpikeInput newValue){
        Debug.Log(OwnerClientId + "; Tick: " + newValue.tick + "; Has Performed Attack: " + newValue.hasPerformedAttack + "; Performed Attack: " + newValue.performedAttackName);
    }

    public void Update()
    {
        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.T))
        {
            // Debug.Log("Locally listened.");
            // networkSpikeInput.Value = new NetworkSpikeInput{
            //     tick = 50,
            //     hasPerformedAttack = true,
            //     performedAttackName = "Aduket"
            // };
        }
    }

    [ServerRpc]
    public void SendGestureServerRpc(InputGestures gesture, ServerRpcParams serverRpcParams)
    {
        Debug.Log(" ClientId: " + serverRpcParams.Receive.SenderClientId.ToString() + "Performed: " + gesture);
        TestClientRpc(new ClientRpcParams());
    }

    [ClientRpc]
    public void TestClientRpc(ClientRpcParams clientRpcParams){
        Debug.Log("Take action");
    }


}
