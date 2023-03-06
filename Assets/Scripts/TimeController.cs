using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    #region Singleton

    public static TimeController Instance;

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

    private float _timeScale;
    private float _fixedDeltaTime;
    [SerializeField] private float _timeScaleTarget = 0.5f;
    [SerializeField] private float _timeScaleRate = 0.2f;
    [SerializeField] private int _timeBeforeRecovery = 50; // in frames
    [SerializeField] private int _timeOfRecovery = 100; // in frames 
    private bool _inEffect;

    [SerializeField] private float timeScale;
    [SerializeField] private float fixedDeltaTime;

    private void Update(){
        timeScale = Time.timeScale;
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void SetVariables(){
        _timeScale = Time.timeScale;
        _fixedDeltaTime = Time.fixedDeltaTime;
    }

    public bool SlowDown(){
        if (_inEffect) return false;
        StartCoroutine(AdjustTimeScale());
        return true;
    }

    private IEnumerator AdjustTimeScale(){
        _inEffect = true;
        Time.timeScale = _timeScaleTarget;
        //Time.fixedDeltaTime = _fixedDeltaTime * _timeScaleTarget;

        yield return new WaitForSecondsRealtime(_timeBeforeRecovery * _fixedDeltaTime);
        while (_inEffect){
            yield return new WaitForSecondsRealtime(_fixedDeltaTime);
            //yield return new WaitForFixedUpdate();
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, _timeScale, _timeScaleRate);
            //Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;

            if (Time.timeScale == _timeScale) _inEffect = false;
        }
        Time.timeScale = _timeScale;
        //Time.fixedDeltaTime = _fixedDeltaTime;
    }
    
}
