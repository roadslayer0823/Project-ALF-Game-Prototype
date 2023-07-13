using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    private CharacterSkill selectedSkill = null;

    public void SetSelectedSkill( CharacterSkill selectedSkill )
    {
        this.selectedSkill = selectedSkill;
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }
}
