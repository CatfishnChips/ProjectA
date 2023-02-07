using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    private Vector2 _velocity;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private CharacterController _characterController;

    void Start()
    {
        EventManager.Instance.Walk += OnWalk;
        EventManager.Instance.Dash += OnDash;
    }

    private void OnDisable() 
    {
        EventManager.Instance.Walk -= OnWalk;
        EventManager.Instance.Dash -= OnDash;

    }

    private void FixedUpdate() 
    {
        MoveCharacter();
    }

    private void MoveCharacter() 
    {
        _characterController.SimpleMove(_velocity * _movementSpeed);
    }

    private void OnWalk(float delta) 
    {
        _velocity.x = delta;
    }

    private void OnDash(Vector2 direction) 
    {
        _velocity.x = direction.x * _dashDistance;
    }
}
