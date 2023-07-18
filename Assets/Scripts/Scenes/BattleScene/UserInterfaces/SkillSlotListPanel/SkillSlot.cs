using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillSlotText;

    private CharacterSkill selectedSkill = null;

    public void SetSelectedSkill( CharacterSkill selectedSkill )
    {
        this.selectedSkill = selectedSkill;

        SetSkillSlotText(selectedSkill.GetSkillData().GetSkillName());
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public void SetSkillSlotText(string slotText)
    {
        if (slotText == "")
        {
            this.skillSlotText.SetText("NODATA");
        }
        else
        {
            this.skillSlotText.SetText(slotText);
        }
    }
}
