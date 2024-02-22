using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBMInput : MonoBehaviour, IInputInvoker
{
    InputEvents _inputEvents;
    public string attackToPerform;

    public InputEvents GetInputEvents()
    {
        return _inputEvents;
    }

    public bool IsActiveAndEnabled()
    {
        return isActiveAndEnabled;
    }

    public void SetInputEvents(InputEvents inputEvents)
    {
        _inputEvents = inputEvents;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.K))
        {
            Debug.Log("Attack Input was registered at update: " + GameSimulator.Instance.UpdateCount + " and Tick: " + GameSimulator.Instance.TickCount);
            _inputEvents.OnTap?.Invoke(ScreenSide.Right);
        }       
    }
}
