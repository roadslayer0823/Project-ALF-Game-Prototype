using UnityEngine;
using TMPro;

public class SkillSlotV2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillSlotText;

    private CharacterSkill selectedSkill = null;
    private SkillSlotListPanelV2 skillSlotListPanelV2 = null;

    public void Initialize(SkillSlotListPanelV2 skillSlotListPanelV2)
    {
        this.skillSlotListPanelV2 = skillSlotListPanelV2;
    }

    public void SetSelectedSkill(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;

        SetSkillSlotText(selectedSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public void ClickToSelectSkill()
    {
        this.skillSlotListPanelV2.GetSelectedGameCharacter().SetCurrentSkill(this.selectedSkill);
    }

    public void SetSkillSlotText(string slotText)
    {
        this.skillSlotText.SetText(slotText);
    }

    public void Clear()
    {
        this.selectedSkill = null;
        this.skillSlotText.SetText("NODATA");
    }
}
