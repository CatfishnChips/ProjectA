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
        inputEvents.OnHold += fighterManager.OnHold;
        inputEvents.OnSwipe += fighterManager.OnSwipe;
        inputEvents.OnTap += fighterManager.OnTap;
    }

    void ConnectInputs(){
        if(_inputInvoker == null) return;
        _inputInvoker.InputEvents = inputEvents;
    }

    void OnDisable(){

    }
}
