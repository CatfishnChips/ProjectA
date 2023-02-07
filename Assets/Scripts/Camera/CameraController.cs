using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public bool _inCombat; // Alternatively enums can be used to determine the camera state.
    [SerializeField] private GameObject _virtualCamera1, _virtualCamera2;
    [SerializeField] private Transform _combatConfineBorders;
    [SerializeField] private CinemachineTargetGroup _combatTargetGroup;

    private Transform _player;

    private void Awake() 
    {
        _player = GameObject.Find("Player").transform;
    }

    void Start()
    {
        //EventManager.Instance.OnCombatBegin +=
        //EventManager.Instance.OnCombatEnd +=

        UpdateCamera();
    }

    private void OnDisable() 
    {
        //EventManager.Instance.OnCombatBegin -=
        //EventManager.Instance.OnCombatEnd -=
    }

    private void UpdateCamera() 
    {
        if (_inCombat) 
        {
            //_combatConfineBorders.position = _player.position;
            AddToTargetGroup();
            _combatConfineBorders.position = _combatTargetGroup.transform.position; // Don't forget to add the target before running this code!
            _virtualCamera1.SetActive(false);
            _virtualCamera2.SetActive(true);
        }
        else 
        {
            _virtualCamera1.SetActive(true);
            _virtualCamera2.SetActive(false);
        }
    }

    // Using events might be better in this situation.
    // Listen to OnCombatStart or sth.
    public void SwitchState() 
    {
        if (_inCombat) _inCombat = false;
        else _inCombat = true;

        UpdateCamera();
    }

    private void AddToTargetGroup() 
    {
        //_combatTargetGroup.AddMember();
    }

    private void RemoveFromTargetGroup() 
    {
        //_combatTargetGroup.RemoveMember();
    }
}
