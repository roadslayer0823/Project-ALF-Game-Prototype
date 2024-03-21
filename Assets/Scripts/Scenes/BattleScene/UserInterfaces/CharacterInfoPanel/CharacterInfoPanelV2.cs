using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelV2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;
    [SerializeField] private TextMeshProUGUI healthPointValueText = null;
    [SerializeField] private TextMeshProUGUI statePointValueText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueText = null;
    [SerializeField] private GameObject statePointBreakText = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private Sprite stressPointBreak = null;
    [SerializeField] private Sprite stressPointNoBreak = null;
    [SerializeField] private Image stressPointStatus = null;
    [SerializeField] private Image statePointStatus = null;
    [SerializeField] private Image stressPercentageBar = null;
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image statePointBar = null;

    private GameCharacter selectedCharacter = null;

    public void SetSelectedCharacter( GameCharacter selectedCharacter )
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        // Health Point
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _currentHealthPoint = this.selectedCharacter.GetCurrentHealthPoint();
        float _virtualHealthPoint = this.selectedCharacter.GetVirtualHealthPoint();

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }
        this.healthPointValueText.SetText( _currentHealthPoint.ToString() + " / " + _maximumHealthPoint);
        this.healthPointBar.fillAmount = _currentHealthPoint / _maximumHealthPoint;

        this.virtualHPBar.fillAmount = _virtualHealthPoint / _maximumHealthPoint;

        // State Point
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        this.statePointValueText.SetText(_currentStatePoint.ToString());
        this.maxStatePointValueText.SetText(_maximumStatePoint.ToString());

        if (this.selectedCharacter.GetIsBreakStatusCausedByStatePoint())
        {
            this.statePointBreakText.SetActive(true);
            this.healthPointValueText.gameObject.SetActive(false);
            this.statePointStatus.sprite = this.statePointBreak;
        }
        else
        {
            this.statePointBreakText.SetActive(false);
            this.healthPointValueText.gameObject.SetActive(true);
            this.statePointStatus.sprite = this.statePointNoBreak;
            this.statePointBar.fillAmount = _currentStatePoint / _maximumStatePoint;
        }
        

        // Stress Value
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPercentageText.SetText("BREAK");
            this.stressPointStatus.sprite = this.stressPointBreak;
        }
        else
        {
            this.stressPercentageText.SetText( _currentStressValue.ToString() );
            this.stressPointStatus.sprite = this.stressPointNoBreak;
            this.stressPercentageBar.fillAmount = _currentStressValue;
        }
    }
}
