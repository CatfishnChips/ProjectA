using UnityEngine;


public struct FighterRecordData
{
    public Vector3 position;
    public int faceDirection;
    public FighterStates rootState;
    public FighterStates subState;
    public int meshAnimationFrame;
    public int boxAnimationFrame;
    public string attackName;
}

public class FighterRecord : MonoBehaviour
{
    private FighterStateMachine _fsm;

    private void Awake()
    {
        _fsm = transform.GetComponent<FighterStateMachine>();
    }

    public FighterRecordData RecordFrame()
    {
        FighterRecordData frd = new FighterRecordData
        {
            position = _fsm.transform.position,
            faceDirection = _fsm.FaceDirection,
            rootState = _fsm.CurrentRootState,
            subState = _fsm.CurrentSubState,
            meshAnimationFrame = 0,
            boxAnimationFrame = 0,
            attackName = _fsm.AttackAction.name
        };

        return frd;
    }
}
