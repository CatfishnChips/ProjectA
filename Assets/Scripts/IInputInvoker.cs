using UnityEngine;

public interface IInputInvoker 
{
    public InputEvents GetInputEvents();
    public void SetInputEvents(InputEvents inputEvents);
    public bool IsActiveAndEnabled();
    public void SetActiveAndEnabled(bool active);
    public InputType GetInputType();
}
