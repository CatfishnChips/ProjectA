using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputInvoker
{
    public Action<Vector2> Swipe{ get; }
    public Action<float> Move{ get; }
    public Action OnTap{ get; }
    public Action<bool> OnHoldA{ get; }
    public Action<bool> OnHoldB{ get; }
    public Action<string> AttackMove{ get; }
}
