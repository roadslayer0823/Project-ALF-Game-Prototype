using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillActionButton : MonoBehaviour
{
    [SerializeField] private Button actionButton = null;
    [SerializeField] private TextMeshProUGUI actionButtonLabel = null; 

    private CharacterSkill selectedSkill = null;

    public void SetSelectedSkill( CharacterSkill selectedSkill )
    {
        this.selectedSkill = selectedSkill;
        this.actionButtonLabel.SetText( this.selectedSkill.GetCharacterSubskillData().GetSubskillData().DisplayName );
        DisableActionButton();
    }

    public void EnableActionButton()
    {
        this.actionButton.interactable = true;
    }

    public void DisableActionButton()
    {
        this.actionButton.interactable = false;
    }

    public void ClickOnActionButton()
    {
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }
}
