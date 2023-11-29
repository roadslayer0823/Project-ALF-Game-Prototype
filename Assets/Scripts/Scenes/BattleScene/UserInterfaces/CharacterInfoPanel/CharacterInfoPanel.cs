using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText = null;
    [SerializeField] private TextMeshProUGUI breakRepresentationText = null;
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;
    [SerializeField] private TextMeshProUGUI healthPointValueText = null;
    [SerializeField] private TextMeshProUGUI statePointValueText = null;
    [SerializeField] private Image stressPercentageBar = null;
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image statePointBar = null;
    [SerializeField] private TextMeshProUGUI roundInfoText = null;

    private GameCharacter selectedCharacter = null;

    public void SetSelectedCharacter( GameCharacter selectedCharacter )
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );

        this.playerNameText.SetText(this.selectedCharacter.GetCharacterName());
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

        this.statePointValueText.SetText(_currentStatePoint + " / " + _maximumStatePoint);
        this.statePointBar.fillAmount = _currentStatePoint / _maximumStatePoint;

        // Stress Value
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();

        this.stressPercentageText.SetText(_currentStressValue + "%");
        this.stressPercentageBar.fillAmount = 0.214f+(_currentStressValue*0.00737f);
        this.breakRepresentationText.gameObject.SetActive( this.selectedCharacter.GetIsInBreakStatus() );
    }

    public void ShowRoundInfoText( bool isPlayerFirst )
    {
        this.roundInfoText.SetText( ( isPlayerFirst ) ? "先手回合" : "後手回合" );
        this.roundInfoText.gameObject.SetActive( true );
    }

    public void HideRoundInfoText()
    {
        this.roundInfoText.gameObject.SetActive( false );
    }
}
