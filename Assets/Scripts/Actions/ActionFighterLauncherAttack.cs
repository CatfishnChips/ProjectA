using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fighter Launcher Attack Action", menuName = "ScriptableObject/Action/Attack/FighterLauncherAttack")]
public class ActionFighterLauncherAttack : ActionFighterAttack
{
    public override bool CheckSwithStateFunction(FighterStateFactory factory){
        if (HadHit){
            if (_ctx.HitCollisionData.hurtbox.Owner.CurrentRootState == FighterStates.Stunned){
                if (_ctx.HitCollisionData.hurtbox.Owner.CurrentRootState == FighterStates.Knockup){
                    if (_ctx.ActionInput.Read()){
                        if(_ctx.GestureActionDict.ContainsKey(_ctx.ActionManager.GetAction(_ctx.ChainActionGesture).inputGesture)){
                            FighterStates state = _ctx.ActionManager.GetAction(_ctx.ChainActionGesture).stateName;
                            if (state == FighterStates.Airborne){
                                //_isChainSuccessful = true;
                                Debug.Log("Launcher Attack State Transition");
                                _ctx.SwitchState(factory.GetRootState(FighterRootStates.Airborne));
                                return true;
                            }
                        }
                        else{
                            _ctx.ActionInput.Remove();
                        }
                    }
                }
            }
        }
        return false;
    }
}
