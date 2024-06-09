using UnityEngine;

public class KBMInput : MonoBehaviour, IInputInvoker
{
    InputEvents _inputEvents;
    [SerializeField] private InputType _inputType;
    public string attackToPerform;
    public string secondAttackToPerform;

    public InputEvents GetInputEvents()
    {
        return _inputEvents;
    }

    public InputType GetInputType(){
        return _inputType;
    }

    public bool IsActiveAndEnabled()
    {
        return isActiveAndEnabled;
    }

    public void SetInputEvents(InputEvents inputEvents)
    {
        _inputEvents = inputEvents;
    }

    public void SetActiveAndEnabled(bool state){
        if(state){
            this.enabled = state;
        }
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
