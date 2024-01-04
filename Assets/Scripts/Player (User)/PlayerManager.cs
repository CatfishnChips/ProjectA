using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerID _playerID;
    [SerializeField] private FighterManager _fighterManager;
    [SerializeField] private IInputInvoker _inputInvoker;

    public InputEventsStruct inputEvents = new InputEventsStruct(); 

    void Awake()
    {
        _inputInvoker = GetComponent<IInputInvoker>();
        GetInputs();
        PossessFighter(_fighterManager);
    }

    void PossessFighter(FighterManager fighterManager){
        inputEvents.AttackMove += fighterManager.OnGestureB;
        inputEvents.Move += fighterManager.OnMoveA;
        inputEvents.Swipe += fighterManager.OnSwipe;
        inputEvents.OnTap += fighterManager.OnTapA;
        inputEvents.OnHoldA += fighterManager.OnHoldA;
        inputEvents.OnHoldB += fighterManager.OnHoldB;
    }

    void GetInputs(){
        // _inputInvoker.Swipe += inputEvents.Swipe;
        // _inputInvoker.Move += inputEvents.Move;
        // _inputInvoker.OnTap += inputEvents.OnTap;
        // _inputInvoker.OnHoldA += inputEvents.OnHoldA;
        // _inputInvoker.OnHoldB += inputEvents.OnHoldB;
        // _inputInvoker.AttackMove += inputEvents.AttackMove;
    }

    void OnDisable(){

    }
}
