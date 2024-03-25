using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshPro enemyNameText = null;
    [SerializeField] private TextMeshPro effectMarkerLabel = null;
    [SerializeField] private TextMeshPro stressPercentageText = null;
    [SerializeField] private GameObject stressBreakUI = null;
    [SerializeField] private GameObject stateBreakUI = null;
    [SerializeField] private GameObject effectMarker = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private Sprite stressPointBreak = null;
    [SerializeField] private Sprite stressPointNoBreak = null;
    [SerializeField] private SpriteRenderer virtualHPBar = null;
    [SerializeField] private SpriteRenderer statePointStatus = null;
    [SerializeField] private SpriteRenderer stressPointStatus = null;
    [SerializeField] private SpriteRenderer healthPointBar = null;
    [SerializeField] private SpriteRenderer statePointBar = null;

    private GameCharacter selectedCharacter = null;

    public void SetSelectedCharacter(GameCharacter selectedCharacter)
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated(UpdateDisplayInfo);

        this.enemyNameText.text = this.selectedCharacter.GetCharacterName();
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

        float _healthPointPercentage = _currentHealthPoint / _maximumHealthPoint;
        float _virtualPointPercentage = _virtualHealthPoint / _maximumHealthPoint;

        healthPointBar.size = new Vector2((_healthPointPercentage > 0) ? _healthPointPercentage * 10.0f : 0.0f, healthPointBar.size.y);
        virtualHPBar.size = new Vector2((_virtualPointPercentage > 0) ? _virtualPointPercentage * 10.0f : 0.0f, virtualHPBar.size.y);

        // State Point
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        if (this.selectedCharacter.GetIsBreakStatusCausedByStatePoint())
        {
            this.statePointStatus.sprite = this.statePointBreak;
            this.stateBreakUI.SetActive(true);
        }
        else
        {
            this.stateBreakUI.SetActive(false);
            this.statePointStatus.sprite = this.statePointNoBreak;
            float _statePointPercentage = _currentStatePoint / _maximumStatePoint;
            statePointBar.size = new Vector2((_statePointPercentage> 0) ? _statePointPercentage * 10.0f : 0.0f, statePointBar.size.y);
        }

        // Stress Value
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPointStatus.sprite = this.stressPointBreak;
            this.stressPercentageText.gameObject.SetActive(false);
            this.stressBreakUI.gameObject.SetActive(true);
        }
        else
        {
            this.stressPointStatus.sprite = this.stressPointNoBreak;
            this.stressPercentageText.gameObject.SetActive(true);
            this.stressPercentageText.text = _currentStressValue.ToString();
            this.stressBreakUI.gameObject.SetActive(false);
        }
    }
}
