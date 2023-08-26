using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterStateMachine_Class1 : FighterStateMachine
{
    protected ProjectileManager m_projectileManager;
    public ProjectileManager ProjectileManager {get => m_projectileManager;}
    
    protected override void AwakeFunction(){
       base.AwakeFunction();
       if (TryGetComponent(out ProjectileManager projectileManager)) m_projectileManager = projectileManager;
    }
}
