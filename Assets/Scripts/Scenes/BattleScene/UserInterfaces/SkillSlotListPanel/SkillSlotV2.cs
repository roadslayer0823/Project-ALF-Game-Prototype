using UnityEngine;
using TMPro;

public class SkillSlotV2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillSlotText;

    private CharacterSkill selectedSkill = null;

    public void SetSelectedSkill(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;

        SetSkillSlotText(selectedSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
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
