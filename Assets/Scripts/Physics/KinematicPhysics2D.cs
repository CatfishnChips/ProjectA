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

    #region MonoBehaviour

    private void FixedUpdate(){
        KinematicPhysicsUpdate();
    }

    #endregion

    [ReadOnly] [SerializeField] private List<FighterController> _controllers = new List<FighterController>();

    public void Subscribe(FighterController controller){
        _controllers.Add(controller);
    }

    public void Unsubscribe(FighterController controller){
        _controllers.Remove(controller);
    }

    public void Initialize(){
        // Set the Physics2D simulation mode to manual.
        Physics2D.simulationMode = SimulationMode2D.Script; 
    }

    public void KinematicPhysicsUpdate(){

        foreach (FighterController controller in _controllers){
            controller.Simulate();
        }

        foreach (FighterController controller in _controllers){
            controller.LateFixedUpdate();
        }

        // Call Physics2D.Simulate().
        Physics2D.Simulate(0);
    }
}
