using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/FighterAnimationFolder", fileName = "New FighterAnimationFolder")]
public class FighterAnimationFolder : ScriptableObject
{   
    [Header("Mesh Animations")]
    [field: SerializeField] private AnimationClip _meshAnimationStartup;
    [field: SerializeField] private AnimationClip _meshAnimationActive;
    [field: SerializeField] private AnimationClip _meshAnimationRecovery;

    [Header("Box Animations")]
    [field: SerializeField] private AnimationClip _boxAnimationStartup;
    [field: SerializeField] private AnimationClip _boxAnimationActive;
    [field: SerializeField] private AnimationClip _boxAnimationRecovery;

    public AnimationClip MeshAnimationStartup { get => _meshAnimationStartup; }
    public AnimationClip MeshAnimationActive { get => _meshAnimationActive; }
    public AnimationClip MeshAnimationRecovery { get => _meshAnimationRecovery; }

    public AnimationClip BoxAnimationStartup { get => _boxAnimationStartup; }
    public AnimationClip BoxAnimationActive { get => _boxAnimationActive; }
    public AnimationClip BoxAnimationRecovery { get => _boxAnimationRecovery; }
}
