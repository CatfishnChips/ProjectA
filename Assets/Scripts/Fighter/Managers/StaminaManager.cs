using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [SerializeField] private int m_stamina;
    [SerializeField] private int m_recoveryTime; // in frames
    private int _stamina;
    private HurtResponder _hurtResponder;
    private int _currentFrame = 0;

    public int Stamina {get => _stamina;}
    public bool CanBlock {get{ return _stamina > 0;}}

    private void Start(){
        _stamina = m_stamina;
    }

    public void UpdateStamina(){
        _stamina--;
        _stamina = Mathf.Clamp(_stamina, 0, m_stamina);
        EventManager.Instance.StaminaChanged?.Invoke(gameObject, _stamina);
    }

    private void FixedUpdate(){
        _currentFrame++;
        if (_currentFrame >= m_recoveryTime){
            _currentFrame = 0;
            _stamina++;
            _stamina = Mathf.Clamp(_stamina, 0, m_stamina);
            EventManager.Instance.StaminaChanged?.Invoke(gameObject, _stamina);
        }
    }
}
