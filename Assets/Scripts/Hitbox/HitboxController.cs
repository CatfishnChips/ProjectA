using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private string _clipName;
    private float _currentClipLenght;
    private AnimatorClipInfo[] _currentClipInfo;


    private AnimatorStateInfo _currentStateInfo;
    private float _stateNormalizedTime;
    private float _stateLenght;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateHitbox(SC_Hitbox hitbox){

    }

    public void RemoveHitbox(){

    }
    
    public void RemoveAllHitboxes(){

    }

    public void ChangeHitboxDamage(){

    }

    public void ChangeHitboxSize(){

    }

    private void Test() 
    {
        _currentClipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        _currentClipLenght = _currentClipInfo[0].clip.length;
        _clipName = _currentClipInfo[0].clip.name;
        
        _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _stateNormalizedTime = _currentStateInfo.normalizedTime;
        _stateLenght = _currentStateInfo.length;
    }
}
