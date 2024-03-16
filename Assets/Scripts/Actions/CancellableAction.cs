using UnityEngine;

public class CancellableAction : ActionBase
{
    [Tooltip("On which frame the action will be canceled if a chain input is listened.")]
    [SerializeField] private int _cancelFrame;
    [Tooltip("How many frames will the action wait before starting to listen for chain input.")]
    [SerializeField] private int _inputIgnoreFrames;

    public int CancelFrame { get => _cancelFrame; }
    public int InputIgnoreFrames { get => _inputIgnoreFrames; }
}
