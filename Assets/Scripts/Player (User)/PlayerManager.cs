using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerID _playerID;
    [SerializeField] private FighterManager _fighterManager;
    private IInputInvoker _inputInvoker;

    public InputEvents inputEvents = new InputEvents(); 

    void Start()
    {
        IInputInvoker[] inputInvokers = transform.GetComponents<IInputInvoker>();
        foreach(IInputInvoker invoker in inputInvokers)
        {
            if(invoker.IsActiveAndEnabled()){
                _inputInvoker = invoker;
                break;
            }
        }
        ConnectInputs();
        PossessFighter(_fighterManager);
    }

    void PossessFighter(FighterManager fighterManager){
        inputEvents.OnDrag += fighterManager.OnDrag;
        inputEvents.OnHold += fighterManager.OnHold;
        inputEvents.OnSwipe += fighterManager.OnSwipe;
        inputEvents.OnTap += fighterManager.OnTap;
        inputEvents.DirectAttackInputByAction += fighterManager.OnDirectAttackInputByAction;
        inputEvents.DirectAttackInputByString += fighterManager.OnDirectAttackInputByString;
    }

    void ConnectInputs(){
        if(_inputInvoker == null) return;
        _inputInvoker.SetInputEvents(inputEvents);
    }

    void OnDisable(){

    }
}
