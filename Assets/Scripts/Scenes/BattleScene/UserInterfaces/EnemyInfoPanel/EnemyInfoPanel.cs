using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI enemyNameText = null;
    [SerializeField] private TextMeshProUGUI effectMarkerLabel = null;
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;
    [SerializeField] private GameObject stressBreakStatus = null;
    [SerializeField] private GameObject effectMarker = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private Sprite stressPointBreak = null;
    [SerializeField] private Sprite stressPointNoBreak = null;
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image statePointStatus = null;
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image statePointBar = null;

    private GameCharacter selectedCharacter = null;

    public void SetSelectedCharacter(GameCharacter selectedCharacter)
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated(UpdateDisplayInfo);

        this.enemyNameText.SetText(this.selectedCharacter.GetCharacterName());
        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        // Health Point
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _currentHealthPoint = this.selectedCharacter.GetCurrentHealthPoint();
        float _virtualHealthPoint = this.selectedCharacter.GetVirtualHealthPoint();

        if (this.selectedCharacter.HasEnergyMarker())
        {
            this.effectMarkerLabel.gameObject.SetActive(true);
        }
        else
        {
            this.effectMarkerLabel.gameObject.SetActive(false);
        }

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }
        this.healthPointBar.fillAmount = _currentHealthPoint / _maximumHealthPoint;
        this.virtualHPBar.fillAmount = _virtualHealthPoint / _maximumHealthPoint;

        // State Point
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        if (this.selectedCharacter.GetIsBreakStatusCausedByStatePoint())
        {
            this.statePointStatus.sprite = this.statePointNoBreak;
            Debug.Log("break");
        }
        else
        {
            this.statePointStatus.sprite = this.statePointBreak;
            this.statePointBar.fillAmount = _currentStatePoint / _maximumStatePoint;
        }

        // Stress Value
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPercentageText.gameObject.SetActive(false);
            this.stressBreakStatus.gameObject.SetActive(true);
        }
        else
        {
            this.stressPercentageText.gameObject.SetActive(true);
            this.stressPercentageText.SetText(_currentStressValue.ToString());
            this.stressBreakStatus.gameObject.SetActive(false);
        }
    }
}
