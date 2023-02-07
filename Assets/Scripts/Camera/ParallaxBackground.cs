using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float _parallax;
    [SerializeField] private float _lenght;
    private Transform _camera;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;    

    private void Awake() 
    {
        _camera = Camera.main.transform;
    }

    void Start()
    {
        _startPosition = (Vector2)transform.position;
    }

    void LateUpdate()
    {
        float temp = (_camera.position.x * (1 - _parallax));
        float distance = (_camera.position.x * _parallax);

        transform.position = new Vector3(_startPosition.x + distance, transform.position.y, transform.position.z);

        if (temp > _startPosition.x + _lenght) _startPosition.x += _lenght;
        else if (temp < _startPosition.x - _lenght)  _startPosition.x -= _lenght;
    }
}
