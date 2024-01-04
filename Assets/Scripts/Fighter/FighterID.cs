using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter ID", menuName = "ScriptableObject/Communication ID/Fighter ID")]
public class FighterID : ScriptableObject
{
    [SerializeField] private ActionAttribution[] _actionAttribution;

    public ActionAttribution[] ActionAttribution { get => _actionAttribution; }
}
