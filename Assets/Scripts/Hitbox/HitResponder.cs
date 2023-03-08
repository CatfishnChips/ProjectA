using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitResponder : MonoBehaviour, IHitResponder
{
    public UnityAction<CollisionData> HitResponse;
    private ActionAttack _action;

    [Header("Hitbox Properties")]
    [SerializeField] private Hitbox _hitbox;
    [SerializeField] private int m_priority;
    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]

    private int _part; 

    ActionAttack IHitResponder.Action { get => _action; }


    public void UpdateData(ActionAttack action){
        _action = action;
        _part = _action.Part;
    }

    private void Start(){
        _hitbox.HitResponder = this;
        //StartCoroutine(UpdateHitbox());
        _part = 1; // Temporary
    }

    private void Update(){
        if (!_hitbox.Active) return;
        if (_part <= 0) return;

        _hitbox.CheckHit();
    }

    // private IEnumerator UpdateHitbox(){
    //     yield return new WaitForFixedUpdate();
    //     if (_hitbox.Active)
    //         _hitbox.CheckHit();
    // }

    bool IHitResponder.CheckHit(CollisionData data){
        return true;
    }

    void IHitResponder.Response(CollisionData data){
        _part--;
        HitResponse?.Invoke(data);
    }
}
