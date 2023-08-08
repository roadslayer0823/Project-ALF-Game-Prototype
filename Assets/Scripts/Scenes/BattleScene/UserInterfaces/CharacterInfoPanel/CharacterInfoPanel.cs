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
        this.healthPointValueText.SetText(this.selectedCharacter.GetRemainingHealthPoint().ToString());
        this.healthPointBar.fillAmount = this.selectedCharacter.GetRemainingHealthPoint() / this.selectedCharacter.GetMaximumHealthPoint();
        this.statePointBar.fillAmount = this.selectedCharacter.GetRemainingStatePoint() / this.selectedCharacter.GetMaximumStatePoint();
    }
}
