using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputInvoker : MonoBehaviour
{
    private PlayerManager playerManager;
    protected InputEvents _inputEvents;

    public InputEvents InputEvents { get => _inputEvents; set => _inputEvents = value; }

    void Awake()
    {
        _inputEvents = new InputEvents();
    }

}
