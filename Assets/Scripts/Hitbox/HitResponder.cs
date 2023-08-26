using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitResponder : MonoBehaviour, IHitResponder
{
    public UnityAction<CollisionData> HitResponse;
    private ActionAttack _action;

    [SerializeField] private List<Hitbox> _hitboxes;

    [Tooltip("Dictates how many times a move can hit. Set to 1 for single hit moves.")]
    [SerializeField] [ReadOnly] int _part; 

    ActionAttack IHitResponder.Action { get => _action; }

    public void UpdateData(ActionAttack action){
        _action = action;
        _part = _action.Part;
    }

    private void Start(){
        foreach (Hitbox hitbox in _hitboxes){
            hitbox.HitResponder = this;
        }

        //StartCoroutine(UpdateHitbox());
        _part = 1; // Temporary
    }

    private void Update(){
        foreach (Hitbox hitbox in _hitboxes){
            if (_part <= 0) return;
            if (!hitbox.Active)return;
            hitbox.CheckHit();
        }
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
