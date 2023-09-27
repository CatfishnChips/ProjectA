using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIBehaviourController : MonoBehaviour
{
    [SerializeField] private AIDifficultySettings _settings;
    [SerializeField] private Slider _slider0, _slider1, _slider2, _slider3, _slider4, _slider5, _slider6;
    [SerializeField] private TMP_InputField _field0, _field1, _field2, _field3, _field4, _field5, _field6;

    void Start()
    {
        _slider0.value = _settings.Aggression;
        _field0.text = _settings.Aggression.ToString();

        _slider1.value = _settings.AggressionConsistency;
        _field1.text = _settings.AggressionConsistency.ToString();

        _slider2.value = _settings.CharacterProficiency;
        _field2.text = _settings.CharacterProficiency.ToString();

        _slider3.value = _settings.AttackAccuracy;
        _field3.text = _settings.AttackAccuracy.ToString();

        _slider4.value = _settings.DefensiveAccuracy;
        _field4.text = _settings.DefensiveAccuracy.ToString();

        _slider5.value = _settings.CounterAttackORDodge;
        _field5.text = _settings.CounterAttackORDodge.ToString();

        _slider6.value = _settings.OpportunityAttackConsistency;
        _field6.text = _settings.OpportunityAttackConsistency.ToString();
    }
    
    public void UpdateSettings(int i){
        switch(i){
            case 0:
            _settings.Aggression = _slider0.value;
            _field0.text = _settings.Aggression.ToString();
            break;

            case 1:
            _settings.AggressionConsistency = _slider1.value;
            _field1.text = _settings.AggressionConsistency.ToString();
            break;

            case 2:
            _settings.CharacterProficiency = _slider2.value;
            _field2.text = _settings.CharacterProficiency.ToString();
            break;

            case 3:
            _settings.AttackAccuracy = _slider3.value;
            _field3.text = _settings.AttackAccuracy.ToString();
            break;

            case 4:
            _settings.DefensiveAccuracy = _slider4.value;
            _field4.text = _settings.DefensiveAccuracy.ToString();
            break;

            case 5:
            _settings.CounterAttackORDodge = _slider5.value;
            _field5.text = _settings.CounterAttackORDodge.ToString();
            break;

            case 6:
            _settings.OpportunityAttackConsistency = _slider6.value;
            _field6.text = _settings.OpportunityAttackConsistency.ToString();
            break;
        }
    }
}
