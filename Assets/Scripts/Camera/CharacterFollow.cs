using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFollow : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector3 _followOffset;

    void Start()
    {
        transform.position = _followTarget.position + _followOffset;
    }

    void LateUpdate()
    {
        transform.position = _followTarget.position + _followOffset;
    }
}
