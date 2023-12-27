using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private struct PlayerInputState : INetworkSerializable
    {
        public int tick;
        public bool hasPerformedAttack;
        public FixedString64Bytes performedAttackName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref hasPerformedAttack);
            serializer.SerializeValue(ref performedAttackName);
        }
    }

    private NetworkVariable<PlayerInputState> playerInputStateNV = new NetworkVariable<PlayerInputState>(
        new PlayerInputState
        {
            tick = 60,
            hasPerformedAttack = false,
            performedAttackName = "None"
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        playerInputStateNV.OnValueChanged += (PlayerInputState previousValue, PlayerInputState newValue) =>
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
            Debug.Log("Locally listened.");
            playerInputStateNV.Value = new PlayerInputState{
                tick = 50,
                hasPerformedAttack = true,
                performedAttackName = "Aduket"
            };
        }
    }

}
