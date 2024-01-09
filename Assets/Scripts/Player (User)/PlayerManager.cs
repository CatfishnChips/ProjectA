using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerID _playerID;
    [SerializeField] private FighterManager _fighterManager;
    private InputInvoker _inputInvoker;

    public InputEvents inputEvents = new InputEvents(); 

    void Start()
    {
        InputInvoker[] inputInvokers = transform.GetComponents<InputInvoker>();
        foreach(InputInvoker invoker in inputInvokers)
        {
            if(invoker.isActiveAndEnabled){
                _inputInvoker = invoker;
                break;
            }
        }
        ConnectInputs();
        PossessFighter(_fighterManager);
    }

    void PossessFighter(FighterManager fighterManager){
        inputEvents.AttackMove += fighterManager.OnGestureB;
        inputEvents.Move += fighterManager.OnMoveA;
        inputEvents.Swipe += fighterManager.OnSwipe;
        inputEvents.OnTap += fighterManager.OnTapA;
        inputEvents.OnHoldA += fighterManager.OnHoldA;
        inputEvents.OnHoldB += fighterManager.OnHoldB;
        Debug.Log(inputEvents.Swipe.ToString());
    }

    void ConnectInputs(){
        if(_inputInvoker == null) return;
        _inputInvoker.InputEvents = inputEvents;
    }

    void OnDisable(){

    }
}
