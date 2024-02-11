using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter ID", menuName = "ScriptableObject/Communication ID/Fighter ID")]
public class FighterID : ScriptableObject
{
    [SerializeField] private InputAttackAttribution[] _inputAttackAttribution;
    [SerializeField] private ActionAttribution[] _actionAttribution;
    [SerializeField] private ComboMove[] combosArray;

    public InputAttackAttribution[] InputAttackAttribution { get => _inputAttackAttribution; }
    public ActionAttribution[] ActionAttribution { get => _actionAttribution; }
    public ComboMove[] CombosArray { get => combosArray; set => combosArray = value; }
}
