using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Data.Common;

public class FighterController : MonoBehaviour
{   
    [Header("Collision Box")]
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField] private Vector2 _size = Vector2.one;
    [SerializeField] private LayerMask _mask;
    [Space]
    [Header("Settings")]
    public Vector2 targetVelocity;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Pushbox _collider;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minGroundNormalY = 0.65f;
    [ReadOnly] public Vector2 _velocity;

    [ReadOnly] private bool _grounded;
    public bool IsGrounded { get => _grounded; }
    
    private Vector2 _groundNormal = new Vector2(0, 1);
    private const float minMoveDistance = 0.001f;
    private const float skinWidth = 0.01f;

    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);
    private Collider2D[] _colliderBuffer = new Collider2D[16];
    private List<Collider2D> _colliderBufferList = new List<Collider2D>(16);

    private bool _isTouchingWall = false;
    public bool IsTouchingWall { get => _isTouchingWall; }

    private Vector2 _targetDeltaPosition;
    private Vector2 _targetPosition;

    private int _frame = 0;
    
    private Vector2 _initialPosition;
    private Vector2 _positionA { get => new Vector2(_rigidbody.position.x, _rigidbody.position.y) + new Vector2(_offset.x * Mathf.Sign(transform.forward.x), _offset.y); }
    private Vector2 _positionB { get =>  new Vector2(_collider.transform.position.x, _collider.transform.position.y) 
        + new Vector2(_collider.Offset.x * Mathf.Sign(_collider.Transform.right.x), _collider.Offset.y); }
 
    public bool Static { get => (_collider.Property & ColliderProperty.Static) == ColliderProperty.Static; }
    public bool Priority { get => (_collider.Property & ColliderProperty.Priority) == ColliderProperty.Priority; }

    public Vector2 Position { get => _rigidbody.position; }
    public ScreenSide Side { get => _rigidbody.position.x < 0 ? ScreenSide.Left : ScreenSide.Right; }

    public enum ScreenSide
    {
        Left,
        Right
    }

    private void Start(){
        // Set the Physics2D simulation mode to manual.
        //Physics2D.simulationMode = SimulationMode2D.Script; 

        KinematicPhysics2D.Instance.Subscribe(this);
    }

    public void LateFixedUpdate(){
        // Set the last position as _targetPosition.
        //_targetPosition = _rigidbody.position;

        // Reset the rigidbody position to start.
        _rigidbody.position = _initialPosition;

        // Call Rigidbody2D.MovePosition() to interpolate between the start and end position.
        //_rigidbody.MovePosition(_rigidbody.position + _targetDeltaPosition);
        _rigidbody.MovePosition(_targetPosition);

        // Simulate Physics in LateFixedUpdate().
        //Physics2D.Simulate(0);
    }

    public void Simulate(){
        _grounded = false;
        _velocity = targetVelocity;
        //Debug.Log(transform.name + " Position: " + _rigidbody.position);

        _initialPosition = _rigidbody.position;
        
        Vector2 deltaPosition = _velocity * Time.fixedDeltaTime;
        Vector2 surface;
        Vector2 leftover;

        _targetDeltaPosition = deltaPosition;
        _targetPosition = _initialPosition;

        // Horizontal Pass
        _targetDeltaPosition = Interaction(deltaPosition * Vector2.right, false, out surface, out leftover);
        _targetDeltaPosition = Collision(_targetDeltaPosition, false, out surface, out leftover);

        // Move the Rigidbody for QueryInteractions.
        if(float.IsNaN(_targetDeltaPosition.x)){
            _targetDeltaPosition.x = 0f;
        }
        if(float.IsNaN(_targetDeltaPosition.y)){
            _targetDeltaPosition.y = 0f;
        }

        _rigidbody.position = _initialPosition + _targetDeltaPosition;

        //Debug.Log(transform.name + " Collision Delta: " + collision);

        // Vector2 collision = Collision(deltaPosition * Vector2.right, _positionA, false, out surface, out leftover);
        // //Debug.Log(transform.name + " Collision Delta: " + collision);
        // _targetDeltaPosition = Interaction(collision, _positionB, false, out surface, out leftover);

        _targetDeltaPosition += Overlap(deltaPosition * Vector2.up, surface);
        //Debug.Log("Overlap - " + transform.name + " Overlap Final Target Delta: " + _targetDeltaPosition + " Frame: " + _frame);

        // Move the Rigidbody for QueryInteractions.
        if(float.IsNaN(_targetDeltaPosition.x)){
            _targetDeltaPosition.x = 0f;
        }
        if(float.IsNaN(_targetDeltaPosition.y)){
            _targetDeltaPosition.y = 0f;
        }
        _rigidbody.position = _initialPosition + _targetDeltaPosition;

        // Vertical Pass
        _targetDeltaPosition += Collision(deltaPosition * Vector2.up, true, out _, out leftover);

        // Move the Rigidbody for QueryInteractions.
        if(float.IsNaN(_targetDeltaPosition.x)){
            _targetDeltaPosition.x = 0f;
        }
        if(float.IsNaN(_targetDeltaPosition.y)){
            _targetDeltaPosition.y = 0f;
        }
        _rigidbody.position = _initialPosition + _targetDeltaPosition;

        // Set the last position as _targetPosition.
        _targetPosition = _rigidbody.position;

        // Reset the rigidbody position to start.
        //_rigidbody.position = _initialPosition;


        // Call Rigidbody2D.MovePosition() to interpolate between the start and end position.
        //_rigidbody.MovePosition(_rigidbody.position + _targetDeltaPosition);
        //_rigidbody.MovePosition(_targetPosition);

        // Simulate Physics in LateFixedUpdate().
        //Physics2D.Simulate(0);

        // Ground check based on ground height.
        //_grounded = transform.position.y <= 0.55f ? true : false;

        _frame ++;
    }

    #region Collision Box Methods

    // Used with the Collision Box.
    public Vector2 Collision(Vector2 deltaPosition, bool verticalPass, out Vector2 surface, out Vector2 leftover){
        Vector2 position = _positionA;
        float distance = deltaPosition.magnitude + skinWidth;
        Vector2 direction = deltaPosition.normalized;
        Vector2 targetDelta = deltaPosition;
        surface = Vector2.zero;
        leftover = Vector2.zero;

        if (distance > minMoveDistance + skinWidth){
            int count = Physics2D.BoxCastNonAlloc(position, _size, 0f, direction, _hitBuffer, distance, _mask);
            
            // Reset boolean.
            if (!verticalPass) _isTouchingWall = false;

            _hitBufferList.Clear();
            for (int i = 0; i < count; i++){
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i <_hitBufferList.Count; i++){
                float modifiedDistance = _hitBufferList[i].distance - skinWidth;
                distance = modifiedDistance < distance ? modifiedDistance : distance;

                surface = direction * distance; // Vector to the surface of the object.
                leftover = deltaPosition - surface; // Leftover vector when colliding with the object.
                targetDelta = surface;

                // Is the collider against the wall?
                if (!verticalPass) _isTouchingWall = true;
            }
        }
        
        return targetDelta;
    }

    #endregion

    #region Push Box Methods

    public Vector2 Interaction(Vector2 deltaPosition, bool verticalPass, out Vector2 surface, out Vector2 leftover){
        Vector2 position = _positionB;
        float distance = deltaPosition.magnitude + skinWidth;
        Vector2 direction = deltaPosition.normalized;
        Vector2 targetDelta = deltaPosition;
        surface = Vector2.zero;
        leftover = Vector2.zero;

        if (distance > minMoveDistance + skinWidth){
            int count = Physics2D.BoxCastNonAlloc(position, _collider.Size, 0f, direction, _hitBuffer, distance, _layerMask);

            //Debug.Log("Interaction (Start) - " + transform.name + " DeltaPosition: " + deltaPosition + " Vertical Pass: " + verticalPass + " Frame: " + _frame); 

            _hitBufferList.Clear();
            for (int i = 0; i < count; i++){
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i <_hitBufferList.Count; i++){
                if (_hitBufferList[i].collider.attachedRigidbody == _collider.Collider.attachedRigidbody) continue;

                //Debug.Log(transform.name + " Hit: " + _hitBufferList[i].collider.name);
                
                float modifiedDistance = _hitBufferList[i].distance - skinWidth;
                distance = modifiedDistance < distance ? modifiedDistance : distance;

                surface = direction * distance; // Vector to the surface of the object.
                leftover = deltaPosition - surface; // Leftover vector when colliding with the object.
                targetDelta = surface; 

                // TRY TO ADD OVERLAP FUNCTION HERE IF POSSIBLE
                // if (verticalPass){
                //     Overlap()
                // }

                if (!verticalPass){       
                    FighterController controller = _hitBufferList[i].collider.GetComponentInParent<FighterController>();

                    // If the object in a state which prevents it from being pushed, return surface.
                    if (controller.Static){
                        targetDelta = surface;
                    }
                    else{
                        targetDelta = controller.Push(surface, leftover);
                        //Debug.Log("Interaction (Push) - " + transform.name + " Collider: " + _hitBufferList[i].transform.name + " Result: " + targetDelta + " Frame: " + _frame); 
                    }
                }
            }
        }
        //Debug.Log("Interaction - " + transform.name + " Surface: " + surface + " Leftover: " + leftover + " TargetDelta: " + deltaPosition + " Vertical Pass: " + verticalPass +" Frame: " + _frame);

        return targetDelta;
    }

    public Vector2 Push(Vector2 surface, Vector2 leftover){ 
        Vector2 collision = Collision(leftover, false, out _, out _);
        Vector2 targetDelta = Interaction(collision, false, out _, out _);

        float distance = (_targetDeltaPosition * Vector2.right).magnitude;
        Vector2 direction = targetDelta.normalized;

        float modifiedDistance = targetDelta.magnitude - skinWidth;
        targetDelta = direction * modifiedDistance;

        if (distance < modifiedDistance)
            _targetDeltaPosition.x = targetDelta.x;

        // Move the Rigidbody for QueryInteractions.
        _rigidbody.position = _initialPosition + _targetDeltaPosition;
        _targetPosition = _rigidbody.position;

        // Consecutive calls of Rigidbody2D.MovePosition() will override the previous ones.
        //_rigidbody.MovePosition(_rigidbody.position + _targetDeltaPosition);
        _rigidbody.MovePosition(_targetPosition);

        //Debug.Log("Push - " + transform.name + " TargetDeltaPosition: " + _targetDeltaPosition + " Push: " + deltaPosition + " Return: " + (surface + deltaPosition));
        return targetDelta;
    }

    private Vector2 Overlap(Vector2 deltaPosition, Vector2 surface)
    {
        // Calculate _positionA's position as if it would move the full vertical distance. 
        Vector2 position = _positionA;
        position += deltaPosition * Vector2.up;

        float distance = deltaPosition.magnitude;
        Vector2 targetDelta = Vector2.zero;

        if (distance > minMoveDistance){
            int count = Physics2D.OverlapBoxNonAlloc(position, _collider.Size, 0f, _colliderBuffer, _layerMask);

            // Create bounds for the collider.
            Bounds boundsA = new Bounds(position, _collider.Size);

            _colliderBufferList.Clear();
            for (int i = 0; i < count; i++){
                _colliderBufferList.Add(_colliderBuffer[i]);
            }

            for (int i = 0; i <_colliderBufferList.Count; i++){
                if (_colliderBufferList[i].attachedRigidbody == _collider.Collider.attachedRigidbody) continue;

                // Get bounds of the overlapped collider.
                Bounds boundsB = _colliderBufferList[i].bounds;

                if (!boundsA.Intersects(boundsB)) continue;

                Vector2 minA = boundsA.min; // bottom-left corner point
                Vector2 maxA = boundsA.max; // top-right corner point
                Vector2 minB = boundsB.min;
                Vector2 maxB = boundsB.max;

                Vector2 lowerMax = Vector2.Min(maxA, maxB);
                Vector2 highterMin = Vector2.Max(minA, minB);

                Vector2 overlap = lowerMax - highterMin;

                // Horizontal penetration distance between boundsA and boundsB.
                distance = overlap.x + skinWidth;

                Vector2 direction;
                Vector2 push;

                //Debug.Log(transform.name + " Overlap: " + overlap + " Distance: " + distance + " Collider: " + _colliderBufferList[i].transform.name + " Frame: " + _frame);  
                
                FighterController controller = _colliderBufferList[i].GetComponentInParent<FighterController>();
                if (controller.IsTouchingWall){
                    Debug.Log("Wall: " + controller.IsTouchingWall + " Priority: " + controller.Priority);

                    if (Priority){
                        distance = controller._collider.Size.x - overlap.x;
                        distance += _collider.Size.x + skinWidth; 
                        direction = Vector2.zero;

                        switch (Side){
                            case ScreenSide.Left:
                                Debug.Log("Moving to right side.");
                                direction = Vector2.right;
                            break;
                            case ScreenSide.Right: 
                                Debug.Log("Moving to left side.");
                                direction = Vector2.left;
                            break;
                        }

                        // Try pushing the other object.
                        push = controller.Push(Vector2.zero, direction * distance);
                        Debug.Log("Has Priority - Pushed: " + push);

                        // Move self the remaining distance.
                        distance = distance - push.magnitude;
                        
                        // Inverse direction.
                        direction = -direction;
                    }
                    else{
                        distance = controller._collider.Size.x - overlap.x;
                        distance += _collider.Size.x + skinWidth; 
                        direction = Vector2.zero;

                        switch (Side){
                            case ScreenSide.Left:
                                Debug.Log("Moving to right side.");
                                direction = Vector2.right;
                            break;
                            case ScreenSide.Right: 
                                Debug.Log("Moving to left side.");
                                direction = Vector2.left;
                            break;
                        }
                    }
                }
                else{
                    if (Priority){
                        direction = (boundsB.center - boundsA.center) * Vector2.right;
                        direction = direction.normalized;

                        // If the colliders are directly on top of each other.
                        if (direction.magnitude == 0){
                            Debug.Log("Colliders are directly on top of each other!");
                            direction = (new Vector2(boundsA.center.x, boundsA.center.y) - _initialPosition) * Vector2.right;
                            direction = direction.normalized;
                            
                            // If the deltaPosition only has vertical movement.
                            if (direction.magnitude == 0) {
                                switch (Side){
                                    case ScreenSide.Left:
                                        Debug.Log("Moving to left side.");
                                        direction = Vector2.left;
                                    break;
                                    case ScreenSide.Right: 
                                        Debug.Log("Moving to right side.");
                                        direction = Vector2.right;
                                    break;
                                }
                            }
                        } 

                        // Try pushing the other object.
                        push = controller.Push(Vector2.zero, direction * distance);
                        Debug.Log("Has Priority - Pushed: " + push);

                        // Move self the remaining distance.
                        distance = distance - push.magnitude;
                        
                        // Inverse direction.
                        direction = -direction;
                    }
                    else if (controller.Priority || controller.Static){
                        direction = (boundsA.center - boundsB.center) * Vector2.right;
                        direction = direction.normalized;

                        // If the colliders are directly on top of each other.
                        if (direction.magnitude == 0){
                            Debug.Log("Colliders are directly on top of each other!");
                            direction = (_initialPosition - new Vector2(boundsA.center.x, boundsA.center.y)) * Vector2.right;
                            direction = direction.normalized;
                            
                            // If the deltaPosition only has vertical movement.
                            if (direction.magnitude == 0) {
                                switch (Side){
                                    case ScreenSide.Left:
                                        Debug.Log("Moving to right side.");
                                        direction = Vector2.right;
                                    break;
                                    case ScreenSide.Right: 
                                        Debug.Log("Moving to left side.");
                                        direction = Vector2.left;
                                    break;
                                }
                            }
                        } 

                        Debug.Log("Other collider has priority or static!");
                    }
                    else{
                        direction = (boundsB.center - boundsA.center) * Vector2.right;
                        direction = direction.normalized;

                        // If the colliders are directly on top of each other.
                        if (direction.magnitude == 0){
                            Debug.Log("Colliders are directly on top of each other!");
                            direction = (new Vector2(boundsA.center.x, boundsA.center.y) - _initialPosition) * Vector2.right;
                            direction = direction.normalized;
                            
                            // If the deltaPosition only has vertical movement.
                            if (direction.magnitude == 0) {
                                switch (Side){
                                    case ScreenSide.Left:
                                        Debug.Log("Moving to left side.");
                                        direction = Vector2.left;
                                    break;
                                    case ScreenSide.Right: 
                                        Debug.Log("Moving to right side.");
                                        direction = Vector2.right;
                                    break;
                                }
                            }
                        } 

                        // Try pushing the other object.
                        push = controller.Push(Vector2.zero, direction * distance);
                        Debug.Log("Pushed: " + push);

                        // Move self the remaining distance.
                        distance = distance - push.magnitude;
                        
                        // Inverse direction.
                        direction = -direction;
                    }
                }
                targetDelta = direction * distance;
            } 
        }

        return targetDelta;
    }

    #endregion

    // Gizmo showing the Collision Box. Collision Box is used to interact with the stage.
    private void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(_rigidbody.position.x, _rigidbody.position.y, 0) + new Vector3(_offset.x * Mathf.Sign(transform.forward.x), _offset.y, 0), new Vector3(_size.x, _size.y, 0));
    }
}