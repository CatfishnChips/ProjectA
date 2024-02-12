using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    #region Singleton

    public static InputManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #endregion

    public int InputDelay = 2; // Amount of time before registering an input.
    public int InputBuffer = 10; // in frames
    protected List<ITouchInput> _inputsList;

    // private TouchInput<bool> _jumpInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Jump);
    // private TouchInput<bool> _dashInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Dash);
    // private TouchInput<bool> _dodgeInput = new TouchInput<bool>(false, InputTypes.Slide, SubInputTypes.Dodge);
    // private TouchContinuousInput<float> _movementInput = new TouchContinuousInput<float>(0, InputTypes.Drag, SubInputTypes.None);
    // private TouchContinuousInput<bool> _holdAInput = new TouchContinuousInput<bool>(false, InputTypes.Hold, SubInputTypes.None);
    // private TouchContinuousInput<bool> _holdBInput = new TouchContinuousInput<bool>(false, InputTypes.Hold, SubInputTypes.None);
    // private TouchQueueInput<bool, string> _attackInput = new TouchQueueInput<bool, string>(false, InputTypes.Gesture, SubInputTypes.None);

    // public TouchInput<bool> JumpInput { get => _jumpInput; }
    // public TouchInput<bool> DashInput { get => _dashInput; }
    // public TouchInput<bool> DodgeInput { get => _dodgeInput; }
    // public TouchContinuousInput<float> MovementInput { get => _movementInput; }
    // public TouchContinuousInput<bool> HoldAInput { get => _holdAInput; }
    // public TouchContinuousInput<bool> HoldBInput { get => _holdBInput; }
    // public TouchQueueInput<bool, string> AttackInput { get => _attackInput; }

    // void Start()
    // {
    //     EventManager.Instance.Move += OnMoveA;
    //     EventManager.Instance.Swipe += OnSwipe;
    //     EventManager.Instance.AttackMove += OnGestureB;
    //     EventManager.Instance.OnTap += OnTapA;
    //     EventManager.Instance.OnHoldA += OnHoldA;
    //     EventManager.Instance.OnHoldB += OnHoldB;

    //     _inputsList = new List<ITouchInput>
    //     {
    //         _jumpInput,
    //         _dashInput,
    //         _dodgeInput,
    //         _movementInput,
    //         _holdAInput,
    //         _holdBInput,
    //         _attackInput
    //     };
    // }

    // void FixedUpdate()
    // {
    //     OnFixedUpdate();
    // }

    // public void OnFixedUpdate()
    // {
    //     foreach(ITouchInput input in _inputsList)
    //     {
    //         input.OnTick();
    //     }
    //     StartCoroutine(LateFixedUpdate());
    // }

    // // For the inputs to get closed at the end of the frame if they were read.
    // // In the future if all the update logic was put together in one function, this coroutine will no longer needed.
    // private IEnumerator LateFixedUpdate() 
    // {
    //     yield return new WaitForFixedUpdate();
        
    //     foreach(ITouchInput input in _inputsList)
    //     {
    //         input.OnLateTick();
    //     }
    // }

    // void OnDisable()
    // {
    //     EventManager.Instance.Move -= OnMoveA;
    //     EventManager.Instance.Swipe -= OnSwipe;
    //     EventManager.Instance.AttackMove -= OnGestureB;
    //     EventManager.Instance.OnTap -= OnTapA;
    //     EventManager.Instance.OnHoldA -= OnHoldA;
    //     EventManager.Instance.OnHoldB -= OnHoldB;
    // }

    // #region Input Functions

    // protected virtual void OnSwipe(Vector2 direction){

    //     if (direction.y <= -0.5f) {
    //         if (direction.y <= -0.95f) direction.x = 0f;
    //         _jumpInput.Write(true);
    //     }
    //     else if (direction.y < 0.5f && direction.y > -0.5f){
    //         _dashInput.Write(true);
    //     }
    //     else if (direction.y >= 0.5f){
    //         _dodgeInput.Write(true);
    //     }
    // }

    // protected virtual void OnTapA(){
    // }

    // protected virtual void OnHoldA(bool value){
    //     _holdAInput.Write(value);
    // }

    // protected virtual void OnHoldB(bool value){
    //     _holdBInput.Write(value);
    // }

    // protected virtual void OnMoveA(float value){
    //     value = value == 0 ? 0 : Mathf.Sign(value);

    //     _movementInput.Write(value);
    // }

    // protected virtual void OnGestureB(string attackName){
    //     if (attackName == "L") return; // Temporary Bugfix

    //     Debug.Log("Attack with the name " + attackName + " has been written to the attack input on Frame: " + GameSimulator.Instance.TickCount);
    //     _attackInput.Write(true, attackName);

    //     // ActionAttack action = _attackMoveDict[attackName];
        
    //     // if (IsSameOrSubclass(typeof(ActionFighterAttack), action.GetType())){
    //     //     // if (_isGrounded) action = _groundedAttackMoveDict[attackName];
    //     //     // else action = _aerialAttackMoveDict[attackName];
    //     //     StartCoroutine(InputDelay(_isAttackPerformed, action as ActionFighterAttack));
    //     // }
    //     // else if (IsSameOrSubclass(typeof(ActionSpiritAttack), action.GetType())){
    //     //     _spiritManager?.SpawnSpirit(action as ActionSpiritAttack);
    //     // }
    // }

    // #endregion
}
