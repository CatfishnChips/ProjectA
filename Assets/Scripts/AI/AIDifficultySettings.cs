using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "FighterAI")]
public class AIDifficultySettings : ScriptableObject
{
    public new string name;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("The likelihood of the AI to take aggressive action when it sees possible.")]
    private float _aggression;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("How fast the aggresion level will deplete for each consecutive aggresive action. The lower the faster")]
    private float _aggressionConsistency;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("Determines how well does the AI knows the moveset of the character. Important for performing combos.")]
    private float _characterProficiency;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("Determines the proficiency of the AI to release attack that will hit the opponent.")]
    private float _attackAccuracy;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("The probabilty of AI to take successful defensive action.")]
    private float _defensiveAccuracy;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("Which action will the AI choose upon a successful defensive action descision (Higher favors dodge).")]
    private float _counterAttackOrDodge;


    public bool AggressionResult(float generatedNumber, bool calledPerFrame) 
    {
        bool result = calledPerFrame ? generatedNumber < _aggression * Time.fixedDeltaTime : generatedNumber < _aggression;
        // calledPerFrame determines, wheter this funciton is meant to be called each frame or needed for just one call on a specific event.
        if (result && _aggression > 0) _aggression -= (100 - _aggressionConsistency) * 0.75f; // If decided to make an attack decrease aggression.
        if(_aggression < 0) _aggression = 0;
        return result;
    }

    public bool ComboResult(float generatedNumber)
    {
        return generatedNumber < (_characterProficiency * Time.fixedDeltaTime); // Calls per frame to this method
    }

    public bool AttackAccuracy(float generatedNumber)
    {
        return generatedNumber < _attackAccuracy;
    }

    public bool DefensiveActionResult(float generatedNumber)
    {
        return generatedNumber < _defensiveAccuracy; // One time call to this method.
    }

    public string DefensiveActionType(float generatedNumber)
    {
        return generatedNumber < _counterAttackOrDodge ? "Dodge" : "Counter";
    }

}
