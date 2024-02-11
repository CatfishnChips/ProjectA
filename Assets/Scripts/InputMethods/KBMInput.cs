using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBMInput : InputInvoker
{
    public string attackToPerform;

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
