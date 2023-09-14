using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FighterStateMachine_Class0 : FighterStateMachine
{
    [Header("Focus")]
    [SerializeField] [ReadOnly] private bool m_focus = false;
    [SerializeField] private int m_focusTime; // In Frames
    [SerializeField] [ReadOnly] private int m_focusTimer;

    public bool Focus {get{return m_focus;} set{m_focus = value;}}
    public int FocusTimer {get{return m_focusTimer;}}

    protected override void FixedUpdateFunction(){
        base.FixedUpdateFunction();
        AdvanceFocusTimer();
    }

    protected override void OnHurt(CollisionData data){
        if (_isHurt) return;
        _hurtCollisionData = data;
        _isHurt = true;

        if (_isInvulnerable) return;

        // This part alternatively be in a modifed Stunned state.
        if(!CanBlock) {
            SetFocus(false);
        }
    }

    public override void ResetVariables()
    {
        base.ResetVariables();
        m_focus = false;
        m_focusTimer = 0;
    }

    public void SetFocus(bool value){
        m_focus = value;
        m_focusTimer = m_focusTime;
        switch(_player){
            case Player.P1:
                EventManager.Instance.Focus_P1?.Invoke(value);
            break;

            case Player.P2:
                EventManager.Instance.Focus_P2?.Invoke(value);
            break;
        }
        
    }

    private void AdvanceFocusTimer(){
        if (!m_focus) return;

        if (_currentSubState != FighterStates.Attack){
            m_focusTimer --;

            if(m_focusTimer <= 0){
                SetFocus(false);
            }
        }
    }
}
