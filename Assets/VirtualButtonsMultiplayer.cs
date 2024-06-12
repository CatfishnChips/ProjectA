using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualButtonsMultiplayer : MonoBehaviour
{

    public static FighterManager fighterManager;

    private bool _isHolding1 = false;
    private bool _isHolding2 = false;

    void Start(){
    }

    void Update()
    {
        if(fighterManager == null) return;

        if (_isHolding1 && _isHolding2)
        {
            // Call your method continuously while the button is held
            MethodToCall();
        }

    }

    private void MethodToCall()
    {
        Debug.Log("Invoking");
        GestureController.Instance._inputEvents.OnHold?.Invoke(ScreenSide.LeftNRight);
    }

    public void StartHolding1()
    {
        _isHolding1 = true;
    }

    public void StopHolding1()
    {
        _isHolding1 = false;
    }


    public void StartHolding2()
    {
        _isHolding2 = true;
    }

    public void StopHolding2()
    {
        _isHolding2 = false;
    }

}
