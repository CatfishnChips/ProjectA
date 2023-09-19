using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System;

public class TaskMovement : Node
{
    private Vector2 _distanceToOpponent;
    private float _optimalDistance;
    FighterBT fighterBT;

    public TaskMovement(FighterBT treeContext) : base(treeContext)
    {
        fighterBT = tree as FighterBT;
    }

    public override NodeState Evaluate()
    {
        _distanceToOpponent.x = Math.Abs(fighterBT.TfEnemy.position.x - fighterBT.ContextSelf.transform.position.x);
        _distanceToOpponent.y = Math.Abs(fighterBT.TfEnemy.position.y - fighterBT.ContextSelf.transform.position.y);
        Debug.Log("X distance is: " + _distanceToOpponent.x);

        

        NonAttackingStateMovement();

        state = NodeState.RUNNING;
        return state;
    }

    private void NonAttackingStateMovement()
    {
        Debug.Log(_distanceToOpponent.x);
        if (_distanceToOpponent.x < _optimalDistance - fighterBT.DistanceMargin)
        {
            Debug.Log("So close");
            EventManager.Instance.P2Move(-1.0f * fighterBT.ContextSelf.FaceDirection);
        }
        else if (_distanceToOpponent.x > _optimalDistance + fighterBT.DistanceMargin)
        {
            Debug.Log("So far");
            EventManager.Instance.P2Move(fighterBT.ContextSelf.FaceDirection);
        }
        else
        {
            Debug.Log("At Optimal Range");
            EventManager.Instance.P2Move(0.0f);
        }

    }
}
