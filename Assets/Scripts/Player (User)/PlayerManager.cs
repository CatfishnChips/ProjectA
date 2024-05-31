using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    protected PlayerID _playerID;
    [SerializeField] private FighterManager _fighterManager;
    protected IInputInvoker _inputInvoker;

    public InputEvents inputEvents = new InputEvents(); 

    void Start()
    {
        OnStart();
    }

    public virtual void OnStart(){
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

    protected virtual void PossessFighter(FighterManager fighterManager){
        inputEvents.OnDrag += fighterManager.OnDrag;
        inputEvents.OnHold += fighterManager.OnHold;
        inputEvents.OnSwipe += fighterManager.OnSwipe;
        inputEvents.OnTap += fighterManager.OnTap;
        inputEvents.OnDirectInputGesture += fighterManager.OnDirectInput;
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
