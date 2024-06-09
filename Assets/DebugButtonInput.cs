using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugButtonInput : MonoBehaviour
{
    [SerializeField] private FighterManager _fighterManager;
    [SerializeField] private Button toggleButton;

    private bool _isHolding = false;
    private bool _isToggleOn = false;

    void Start(){
        toggleButton.onClick.AddListener(TuggleBlock);
    }

    void Update()
    {
        if (_isHolding)
        {
            // Call your method continuously while the button is held
            MethodToCall();
        }
        else if(_isToggleOn){
            MethodToCall();  
        }

    }

    private void MethodToCall()
    {
        _fighterManager.OnHold(ScreenSide.LeftNRight);
    }

    public void StartHolding()
    {
        Debug.Log("Holding  the button");
        _isHolding = true;
    }

    public void StopHolding()
    {
        _isHolding = false;
    }

    public void TuggleBlock(){
        if(_isToggleOn) _isToggleOn = false;
        else _isToggleOn = true;
    }

}
