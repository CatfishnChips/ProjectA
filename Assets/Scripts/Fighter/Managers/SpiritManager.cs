using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpiritManager : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private SpiritController _prefab; // Serialization is temporary. Should be set.
    private ActionSpiritAttack _action_Spirit;

    [Header("Spirit Settings")]
    [Tooltip("Total Spirit : How much spirit the fighter at its maximum value.")]
    [SerializeField] private float _spirit;

    [Tooltip("Passive Spirit Regeneration : How much spirit is recovered in a single frame.")]
    [SerializeField] private float _spiritRegen;

    [SerializeField] [ReadOnly] private float _currentSpirit;

    private Player _player;

    [Header("Line Renderer")]
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private AnimationCurve _lineCurve;
    private GameObject _lineObject;
    private LineRenderer _lineRenderer;

    [Space]

    private SpiritState _state = SpiritState.Idle;
    private bool _isDeployed = false;
    private ActionAttack _action; 

    public ActionSpiritAttack Action {get {return _action_Spirit;} set {_action_Spirit = value;}}
    public SpiritState State {get {return _state;} set {_state = value;}}
    public bool IsDeployed {get{return _isDeployed;} set {_isDeployed = value;}}

    private Vector2 _spiritPosition;
    private FighterStateMachine _target;
    private int _frame;
    private float _time;
    private float _direction;
    private int _targetFrame;
    private int _duration;
    private Vector2 _initialPos;
    private Vector2 _position;
    private Vector2 _velocity;

    #region Knockup (Arc) & Knockback (Line) Variables 
    private float _groundOffset;
    private float _time1, _time2;
    private float _gravity1, _gravity2;
    private float _drag;
    private float _knockup; // Calculated Knockup required to reach the player.
    private float _knockback; // Calculated Knockback required to reach the player.

    #endregion

    private void Awake(){
        _player = GetComponent<FighterStateMachine>().Player;
        _state = SpiritState.Idle;
    }

    private void Start(){
        switch(_player){
            case Player.P1:
            EventManager.Instance.RecoveredFromStun_P2 += OnRecover;
            break;

            case Player.P2:
            EventManager.Instance.RecoveredFromStun_P1 += OnRecover;
            break;
        }

        _prefab = Instantiate(_prefab);
        _prefab.Manager = this;
        _prefab.gameObject.SetActive(false);
        Reset();
    }

    private void OnDisable(){
        switch(_player){
            case Player.P1:
            EventManager.Instance.RecoveredFromStun_P2 -= OnRecover;
            break;

            case Player.P2:
            EventManager.Instance.RecoveredFromStun_P1 -= OnRecover;
            break;
        }
    }

    private void OnRecover(){
        _state = SpiritState.Idle; 
        if (_lineObject)
        _lineObject.SetActive(false);
    }

    private void FixedUpdate(){
       UpdateSpirit(_spiritRegen);
    }

    public void UpdateSpirit(float value){
        if (_currentSpirit >= _spirit) return;

        _currentSpirit += value;
        _currentSpirit = Mathf.Clamp(_currentSpirit, 0, _spirit);

        switch(_player)
        {
            case Player.P1:
                EventManager.Instance.SpiritChanged_P1?.Invoke(_currentSpirit, _spirit);   
            break;

            case Player.P2:
                EventManager.Instance.SpiritChanged_P2?.Invoke(_currentSpirit, _spirit);
            break;
        }
    }

    public void SpawnSpirit(ActionSpiritAttack action){
        if (_isDeployed) return;
        if (_state == SpiritState.Idle) return;

        if (_currentSpirit >= 1){
            UpdateSpirit(-1);

            _action_Spirit = action;
            _targetFrame = _target.CurrentFrame + _action_Spirit.StartFrames - _action.HitStop;

            CalculateSpawnLocation();
            CheckAgainstStageBorders();
            CalculateDistance();

            // Spawn Spirit and Start Animations

            _prefab.transform.position = _spiritPosition;
            _prefab.transform.rotation = Quaternion.Euler(0f, 90f * -_direction, 0f);
            _prefab.transform.localScale = new Vector3(-_direction, 1f, 1f);
            _isDeployed = true;

            _action_Spirit.CalculatedKnockback = _knockback;
            _action_Spirit.CalculatedKnockup = _knockup;
            _prefab.Action_Spirit = _action_Spirit;
            _prefab.gameObject.SetActive(true);
            _prefab.Activate();
        }
    }

    private void CalculateSpawnLocation(){
        //Debug.Log("SpiritManager - Frame: " + _frame + " TargetFrame: " + _targetFrame + " CurrentFrame: " + _target.CurrentFrame);

        if(_targetFrame <= _duration){
            _frame = _targetFrame;
        }
        else{
            _frame = _duration;
        }

        switch(_state){
            case SpiritState.Knockback:
                CalculateLinePosition();
            break;

            case SpiritState.Knockup:
                CalculateArcPosition();
            break;
        }

        _spiritPosition.x = _position.x + (_action_Spirit.HitboxOffset.x * _direction);
        _spiritPosition.y = _position.y - _action_Spirit.HitboxOffset.y / 2; // Adding the full value sometimes causes it to miss due to the current shape of the hurtboxes during the animation.
        //Debug.Log("Calculated Position: " + _position + " Hitbox Offset: " + _action_Spirit.HitboxOffset + " Final Position: " + _spiritPosition);
    }

    private void CalculateDistance(){
        _knockup = (transform.position.y - 0.5f) - _position.y; // 0.5f Model Center Offset
        _knockback = Mathf.Abs(transform.position.x - _position.x) - 1f; // 1f In-front of the Fighter
        //Debug.Log("SpiritManager - Calculated Knockback: " + _knockback + " Calculated Knockup: " + _knockup + " Direction: " + _direction);
    }

    public void Setup(FighterStateMachine ctx, CollisionData data){
        _action = data.action;
        _target = data.hurtbox.Owner;
        _groundOffset = ctx.transform.position.y - 0.5f; // y = 0.5f is the centre position of the character.
        _direction = Mathf.Sign(data.hitbox.Transform.right.x);
        _initialPos = new Vector2(_target.transform.position.x, _target.transform.position.y);

        if (_action.KnockupStun.x + _action.KnockupStun.y > 0){
            _state = SpiritState.Knockup;
            _duration = _action.KnockupStun.x + _action.KnockupStun.y;
            CalculateArcData();
        }
        else if (_action.KnockbackStun > 0){
            _state = SpiritState.Knockback;
            _duration = _action.KnockbackStun;
            CalculateLineData();
        }
        else{
            _state = SpiritState.Idle;
        }
    }

    private void CheckAgainstStageBorders(){
        Vector4 stageBorders = MatchConducter.Instance.StageBorders;

        // Off the stage borders in X position.
        if (_spiritPosition.x > stageBorders.x){
            _spiritPosition.x = stageBorders.x;
        }
        else if (_spiritPosition.x < stageBorders.y){
            _spiritPosition.x = stageBorders.y;
        }

        // Off the stage borders in Y position.
        if (_spiritPosition.y > stageBorders.w){
            _spiritPosition.y = stageBorders.w;
        }
        else if (_spiritPosition.y < stageBorders.z){
            _spiritPosition.y = stageBorders.z;
        }
    }

    public void DrawPath(){
        if (!_lineObject){
            _lineObject = new GameObject();
            _lineRenderer = _lineObject.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.widthCurve = _lineCurve;
            _lineRenderer.material = _lineMaterial;
        } 
        _lineObject.SetActive(true);
        _lineRenderer.positionCount = _duration;

        for(int currentStep=0; currentStep<_duration; currentStep++)
        {
            _frame = currentStep;  
            switch(_state){
                case SpiritState.Knockback:
                    CalculateLinePosition();
                break;

                case SpiritState.Knockup:
                    CalculateArcPosition();
                break;
            }
            Vector3 currentPosition = new Vector3(_position.x,_position.y,0f);
            
            _lineRenderer.SetPosition(currentStep,currentPosition);
        }
    }

    #region Knockup (Arc) Functions

    private void CalculateArcData(){
        if (_state != SpiritState.Knockup) return;

        // Zone 1 - Projectile Motion
        _time1 = _action.KnockupStun.x * Time.fixedDeltaTime;
        _gravity1 = - 2 * _action.Knockup / Mathf.Pow(_time1, 2); // g = 2h/t^2
        _velocity.y = 2 * _action.Knockup / _time1; // Initial vertical velocity. V0y = 2h/t

        // Zone 2 - Free Fall
        _time2 = _action.KnockupStun.y * Time.fixedDeltaTime; 
        _gravity2 = -2 * (_action.Knockup + _groundOffset) / (_time2 * _time2); // Free Fall h = (1/2)gt^2 --> g = 2h/t^2

        _drag = -2 * _action.Knockback / Mathf.Pow(_time1 + _time2, 2);
        _drag *= _direction;

        _velocity.x = 2 * _action.Knockback / (_time1 + _time2); // Initial horizontal velocity;
        _velocity.x *= _direction; 
    }

    private void CalculateArcPosition(){
        _time = _frame * Time.fixedDeltaTime;
        _position.x = (_drag * Mathf.Pow(_time, 2) / 2) + (_velocity.x * _time) + _initialPos.x; // Projectile Motion

        if (_frame < _action.KnockupStun.x)
        {
            //Zone 1
            _position.y = (_gravity1 * Mathf.Pow(_time, 2) / 2) + (_velocity.y * _time) + _initialPos.y; // Projectile Motion y = y0 + v0y * t − (1/2)gt^2
        }
        else
        {
            //Zone 2
            _frame -= _action.KnockupStun.x;
            _time = _frame * Time.fixedDeltaTime;
            
            float maxHeight = (_gravity1 * Mathf.Pow(_time1, 2) / 2) + (_velocity.y * _time1) + _initialPos.y;
            _position.y = (_gravity2 * Mathf.Pow(_time, 2) / 2) + maxHeight; // Free Fall h = (1/2)gt^2
        }
    }

    #endregion

    #region Knockback (Line) Functions

    private void CalculateLineData(){
        float time = _action.KnockbackStun * Time.fixedDeltaTime;

        _drag = -2 * _action.Knockback / Mathf.Pow(time, 2);
        _drag *= _direction;

        _velocity.x = 2 * _action.Knockback / time; // Initial horizontal velocity;
        _velocity.x *= _direction;
    }

    private void CalculateLinePosition(){
        _time = _frame * Time.fixedDeltaTime;
        _position.x = (_drag * Mathf.Pow(_time, 2) / 2) + (_velocity.x * _time) + _initialPos.x;
        _position.y = _initialPos.y;
    }

    #endregion

    #region Spirit Functions

    public void Reset(){
        _currentSpirit = 0f;

        switch(_player)
        {
            case Player.P1:
                EventManager.Instance.SpiritChanged_P1?.Invoke(_currentSpirit, _spirit);    
            break;

            case Player.P2:
                EventManager.Instance.SpiritChanged_P2?.Invoke(_currentSpirit, _spirit);
            break;
        }
    }

    #endregion
}
