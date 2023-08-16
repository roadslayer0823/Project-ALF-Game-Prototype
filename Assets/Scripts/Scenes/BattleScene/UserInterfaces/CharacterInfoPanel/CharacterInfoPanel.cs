using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image statePointBar = null;

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

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }
        this.healthPointValueText.SetText( _currentHealthPoint.ToString() + " / " + _maximumHealthPoint);
        this.healthPointBar.fillAmount = _currentHealthPoint / _maximumHealthPoint;

        // State Point
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        this.statePointValueText.SetText(_currentStatePoint + " / " + _maximumStatePoint);
        this.statePointBar.fillAmount = _currentStatePoint / _maximumStatePoint;

        // Stress Value
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();

        this.stressPercentageText.SetText(_currentStressValue + "%");
        this.stressPercentageBar.fillAmount = _currentStressValue / 100;

        if (_currentStressValue >= 100)
        {
            this.breakRepresentationText.gameObject.SetActive(true);
        }
        else
        {
            this.breakRepresentationText.gameObject.SetActive(false);
        }
    }
}
