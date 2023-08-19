using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FighterStateMachine_Class0 : FighterStateMachine
{
    private bool m_focus = false;
    [SerializeField] private int m_focusTime; // In Frames
    [SerializeField] [ReadOnly] private int m_focusTimer;

    public bool Focus {get{return m_focus;} set{m_focus = value;}}
    public int FocusTimer {get{return m_focusTimer;}}

    protected override void AwakeFunction(){
        _animator = GetComponent<Animator>();
        _colBoxAnimator = transform.Find("Hurtboxes").GetComponent<Animator>();
        
        _states = new FighterStateFactory(this);
        _states.OverrideDictionary(new Dictionary<FighterStates, FighterBaseState>{
            [FighterStates.Dodge] = new FighterDodgeState_Class0(this, _states)
        });

        _comboListener = new ComboListener(this);
        
        _animOverrideCont = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animOverrideCont;

        _clipOverrides = new AnimationClipOverrides(_animOverrideCont.overridesCount);
        _animOverrideCont.GetOverrides(_clipOverrides);

        _colBoxOverrideCont = new AnimatorOverrideController(_colBoxAnimator.runtimeAnimatorController);
        _colBoxAnimator.runtimeAnimatorController = _colBoxOverrideCont;

        _colBoxClipOverrides = new AnimationClipOverrides(_colBoxOverrideCont.overridesCount);
        _colBoxOverrideCont.GetOverrides(_colBoxClipOverrides);

        _attackMoveDict = new Dictionary<string, ActionAttack>();
        _actionDictionary = new Dictionary<string, ActionBase>();

        foreach (ActionAttribution attribution in _actionAttribution)
        {
            if (attribution.action.GetType() == typeof(ActionAttack) || attribution.action.GetType() == typeof(ActionContinuousAttack))
            {
                ActionAttack action = Instantiate(attribution.action) as ActionAttack;
                _attackMoveDict.Add(action.name, action); // All Attack Actions
                if(action.Tags.HasFlag(Tags.Grounded)) _groundedAttackMoveDict.Add(action.name, action); // Grounded attack Actions
                else if(action.Tags.HasFlag(Tags.Aerial)) _aerialAttackMoveDict.Add(action.name, action); // Aerial Attack Actions
            }
            else 
            {
                _actionDictionary.Add(attribution.action.name, attribution.action);
            }
        }

        if (TryGetComponent(out HitResponder hitResponder)) _hitResponder = hitResponder;
        if (TryGetComponent(out HurtResponder hurtResponder)) _hurtResponder = hurtResponder;
        if (TryGetComponent(out Rigidbody2D rigidbody2D)) _rigidbody2D = rigidbody2D;
        if (TryGetComponent(out HealthManager healthManager)) _healthManager = healthManager;
        if (TryGetComponent(out StaminaManager staminaManager)) _staminaManager = staminaManager;

        ResetVariables();
    }

    public override void ResetVariables()
    {
        base.ResetVariables();
        m_focus = false;
        m_focusTimer = 0;
    }

    public void ActivateFocus(){
        m_focus = true;
        m_focusTimer = m_focusTime;
    }

    private IEnumerator FocusCoroutine(){
        while (m_focus){
            yield return new WaitForFixedUpdate();
            m_focusTimer --;

            if(m_focusTimer <= 0){
                m_focus = false;
            }
        }
    }
}
