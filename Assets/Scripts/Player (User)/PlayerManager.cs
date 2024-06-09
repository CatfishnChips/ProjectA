using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    protected PlayerID _playerID;
    [SerializeField] private FighterManager _fighterManager;
    protected List<IInputInvoker> _inputInvokers;

    [SerializeField] private InputType _inputType;

    public InputEvents inputEvents = new InputEvents(); 

    void Start()
    {
        OnStart();
    }

    public virtual void OnStart(){
        _inputInvokers = new List<IInputInvoker>();
        IInputInvoker[] inputInvokers = GameObject.FindGameObjectWithTag("InputManager").GetComponents<IInputInvoker>();
        foreach(IInputInvoker invoker in inputInvokers)
        {
            if((_inputType & invoker.GetInputType()) != 0){
                invoker.SetActiveAndEnabled(true);
                _inputInvokers.Add(invoker);
            } 
            
        }
        ConnectInputs();
        ControlFighter(_fighterManager);
    }

    protected virtual void ControlFighter(FighterManager fighterManager){
        fighterManager.SubscribeInput(inputEvents);
    }

    void ConnectInputs(){
        if(_inputInvokers.Count == 0) return;
        foreach(IInputInvoker invoker in _inputInvokers){
            invoker.SetInputEvents(inputEvents);
        }
    }

    void OnDisable(){

    }
}
