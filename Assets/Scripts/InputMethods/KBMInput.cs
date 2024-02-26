using UnityEngine;

public class KBMInput : MonoBehaviour, IInputInvoker
{
    InputEvents _inputEvents;
    public string attackToPerform;
    public string secondAttackToPerform;

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
            //_inputEvents.OnTap?.Invoke(ScreenSide.Right);
            _inputEvents.DirectAttackInputByString?.Invoke(attackToPerform);
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            _inputEvents.DirectAttackInputByString?.Invoke(secondAttackToPerform);
        }
    }
}
