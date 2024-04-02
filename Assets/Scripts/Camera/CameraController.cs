using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    #region Singleton

    public static CameraController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SetVariables();
    }

    #endregion

    public bool _inCombat; // Alternatively enums can be used to determine the camera state.
    [SerializeField] private GameObject _virtualCamera1, _virtualCamera2;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private float orthographicSize;
    private float previousOrthoSize;
    private float orthoLerpPercentage;

    private Transform _player;

    private void SetVariables() 
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        //EventManager.Instance.OnCombatBegin +=
        //EventManager.Instance.OnCombatEnd +=

        
        //orthographicSize = playerFollowCam.m_Lens.OrthographicSize;
        //previousOrthoSize = orthographicSize;

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
            AddToTargetGroup();
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

    public void ScreenShake(Vector3 velocity) {
        _impulseSource.GenerateImpulseAt(transform.position, velocity);
    }

    private void Update(){
        // if(playerFollowCam.m_Lens.OrthographicSize != orthographicSize){
        //     SmoothAdjustOrthoSize(previousOrthoSize, orthographicSize, 1.5f);
        // }
        // else{
        //     orthoLerpPercentage = 0.0f;
        // }
        // orthoLerpPercentage = Mathf.Clamp(orthoLerpPercentage, 0.0f, 1.0f);
    }

    private void SmoothAdjustOrthoSize(float startPoint, float target, float speed){
        // playerFollowCam.m_Lens.OrthographicSize = Mathf.Lerp(startPoint, target, orthoLerpPercentage);
        // orthoLerpPercentage += Time.deltaTime * speed;
    }

    public void setOrthoSize(float size){
        // previousOrthoSize = playerFollowCam.m_Lens.OrthographicSize;
        // orthographicSize = size;
    }
}
