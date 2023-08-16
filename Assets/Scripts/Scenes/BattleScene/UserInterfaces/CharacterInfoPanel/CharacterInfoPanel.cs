using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText = null;
    [SerializeField] private TextMeshProUGUI stressRepresentationText = null;
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
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _currentHealthPoint = this.selectedCharacter.GetCurrentHealthPoint();

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }
        this.healthPointValueText.SetText( _currentHealthPoint.ToString() + " / " + _maximumHealthPoint);
        this.healthPointBar.fillAmount = _currentHealthPoint / _maximumHealthPoint;

        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        this.statePointValueText.SetText(_currentStatePoint + " / " + _maximumStatePoint);
        this.statePointBar.fillAmount = _currentStatePoint / _maximumStatePoint;
    }
}
