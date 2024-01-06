using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int _frame = 0;

    private Vector2 _positionA;
    private Vector2 _positionB;

    public bool Static { get => (_collider.Property & ColliderProperty.Static) == ColliderProperty.Static; }
    public bool Priority { get => (_collider.Property & ColliderProperty.Priority) == ColliderProperty.Priority; }

    public Vector2 Position { get => _rigidbody.position; }
    public ScreenSide Side { get => _rigidbody.position.x < 0 ? ScreenSide.Left : ScreenSide.Right; }


    public enum ScreenSide
    {
        Left,
        Right
    }

    public void Simulate(){
        _grounded = false;
        _velocity = targetVelocity;
        //Debug.Log(transform.name + " Position: " + _rigidbody.position);

        _positionA = new Vector2(_rigidbody.position.x, _rigidbody.position.y) + new Vector2(_offset.x * Mathf.Sign(transform.forward.x), _offset.y);
        _positionB = new Vector2(_collider.transform.position.x, _collider.transform.position.y) + new Vector2(_collider.Offset.x * Mathf.Sign(_collider.Transform.right.x), _collider.Offset.y);
        
        Vector2 deltaPosition = _velocity * Time.fixedDeltaTime;
        Vector2 surface;
        Vector2 leftover;

        _targetDeltaPosition = deltaPosition;

        // Horizontal Pass
        _targetDeltaPosition = Collision(_targetDeltaPosition * Vector2.right, _positionA, false, out surface, out leftover);
        //Debug.Log(transform.name + " Collision Delta: " + collision);
        _targetDeltaPosition = Interaction(_targetDeltaPosition, _positionB, false, out surface, out leftover);

        // Vector2 collision = Collision(deltaPosition * Vector2.right, _positionA, false, out surface, out leftover);
        // //Debug.Log(transform.name + " Collision Delta: " + collision);
        // _targetDeltaPosition = Interaction(collision, _positionB, false, out surface, out leftover);

        _targetDeltaPosition += Overlap(deltaPosition * Vector2.up, surface, _positionB + surface + deltaPosition * Vector2.up);
        //Debug.Log("Overlap - " + transform.name + " Overlap Final Target Delta: " + _targetDeltaPosition + " Frame: " + _frame);

        // Vertical Pass
        _targetDeltaPosition += Collision(deltaPosition * Vector2.up, _positionA + _targetDeltaPosition, true, out _, out leftover);

        _rigidbody.MovePosition(_rigidbody.position + _targetDeltaPosition);
        _frame ++;
    }

    #region Collision Box Methods

    // Used with the Collision Box.
    public Vector2 Collision(Vector2 deltaPosition, Vector2 position, bool verticalPass, out Vector2 surface, out Vector2 leftover){
        float distance = deltaPosition.magnitude + skinWidth;
        Vector2 direction = deltaPosition.normalized;
        Vector2 targetDelta = deltaPosition;
        surface = deltaPosition;
        leftover = Vector2.zero;

        if (distance > minMoveDistance + skinWidth){
            int count = Physics2D.BoxCastNonAlloc(position, _size, 0f, direction, _hitBuffer, distance, _mask);

            if (!verticalPass) _isTouchingWall = false;

            _hitBufferList.Clear();
            for (int i = 0; i < count; i++){
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i <_hitBufferList.Count; i++){
                
                // Is the collider on the ground?
                // if (verticalPass){
                // Vector2 currentNormal = _hitBuffer[i].normal;

                // if (currentNormal.y > _minGroundNormalY){
                //     _grounded = true;
                //     if (verticalPass){
                //         _groundNormal = currentNormal;
                //         currentNormal.x = 0;
                //     }
                // }
                // }

                float modifiedDistance = _hitBufferList[i].distance - skinWidth;
                distance = modifiedDistance < distance ? modifiedDistance : distance;

                surface = direction * distance; // Vector to the surface of the other object.
                leftover = deltaPosition - surface; // Leftover vector when colliding with the other object.
                targetDelta = surface;

                // Is the collider against the wall?
                if (!verticalPass) _isTouchingWall = true;
            }
        }

        return targetDelta;
    }

    #endregion

    #region Push Box Methods

    public Vector2 Interaction(Vector2 deltaPosition, Vector2 position, bool verticalPass, out Vector2 surface, out Vector2 leftover){
        float distance = deltaPosition.magnitude + skinWidth;
        Vector2 direction = deltaPosition.normalized;
        Vector2 targetDelta = deltaPosition;
        surface = deltaPosition;
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

                surface = direction * distance; // Vector to the surface of the other object.
                leftover = deltaPosition - surface; // Leftover vector when colliding with the other object.
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
        Vector2 collision = Collision(leftover, _positionA, false, out _, out _);
        Vector2 deltaPosition = Interaction(collision, _positionB, false, out _, out _);

        // Consecutive calls of Rigidbody2D.MovePosition() will override the previous ones.
        float distance = (_targetDeltaPosition * Vector2.right).magnitude;
        Vector2 direction = deltaPosition.normalized;

        float modifiedDistance = deltaPosition.magnitude - skinWidth;
        deltaPosition = direction * modifiedDistance;

        if (distance < modifiedDistance)
            _targetDeltaPosition.x = deltaPosition.x;

        _rigidbody.MovePosition(_rigidbody.position + _targetDeltaPosition);
        //Debug.Log("Push - " + transform.name + " TargetDeltaPosition: " + _targetDeltaPosition + " Push: " + deltaPosition + " Return: " + (surface + deltaPosition));
        return surface + deltaPosition;
    }

    private Vector2 Overlap(Vector2 deltaPosition, Vector2 surface, Vector2 position)
    {
        float distance = deltaPosition.magnitude;
        Vector2 targetDelta = Vector2.zero;

        if (distance > minMoveDistance){
            int count = Physics2D.OverlapBoxNonAlloc(position, _collider.Size, 0f, _colliderBuffer, _layerMask);

            Bounds boundsA = new Bounds(position, _collider.Size);

            _colliderBufferList.Clear();
            for (int i = 0; i < count; i++){
                _colliderBufferList.Add(_colliderBuffer[i]);
            }

            for (int i = 0; i <_colliderBufferList.Count; i++){
                if (_colliderBufferList[i].attachedRigidbody == _collider.Collider.attachedRigidbody) continue;

                Bounds boundsB = _colliderBufferList[i].bounds;

                if (!boundsA.Intersects(boundsB)) continue;

                Vector2 minA = boundsA.min; // bottom-left corner point
                Vector2 maxA = boundsA.max; // top-right corner point
                Vector2 minB = boundsB.min;
                Vector2 maxB = boundsB.max;

                Vector2 lowerMax = Vector2.Min(maxA, maxB);
                Vector2 highterMin = Vector2.Max(minA, minB);

                Vector2 overlap = lowerMax - highterMin;

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
                            direction = (new Vector2(boundsA.center.x, boundsA.center.y) - _positionB) * Vector2.right;
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
                            direction = (_positionB - new Vector2(boundsA.center.x, boundsA.center.y)) * Vector2.right;
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
                            direction = (new Vector2(boundsA.center.x, boundsA.center.y) - _positionB) * Vector2.right;
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