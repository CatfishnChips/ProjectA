using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FighterState{Idle, Stunned, Attacking}

public class Fighter
{
    protected FighterState currentState;
    protected float health;
}