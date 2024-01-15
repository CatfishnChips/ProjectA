using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KinematicPhysics2D : MonoBehaviour
{
    #region Singleton

    public static KinematicPhysics2D Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Initialize();
    }

    #endregion

    private List<Action> updateList = new List<Action>();

    public void Subscribe(Action action){
        updateList.Add(action);
    }

    public void Initialize(){
        // Set the Physics2D simulation mode to manual.
        //Physics2D.simulationMode = SimulationMode2D.Script; 
    }

    public void KinematicPhysicsUpdate(){

        foreach (Action action in updateList){
            action?.Invoke();
        }

        // Call Physics2D.Simulate().
        Physics2D.Simulate(0);
    }
}
