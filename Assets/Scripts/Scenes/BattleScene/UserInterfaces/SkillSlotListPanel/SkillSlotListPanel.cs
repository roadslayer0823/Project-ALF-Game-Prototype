using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[ 0 ];

    public void Show( GameCharacter gameCharacter )
    {
        List<CharacterSkill> _selectedSkills = gameCharacter.GetSelectedActiveSkillList();

        if (_selectedSkills.Count > skillSlots.Length)
        {
            return;
        }

        for (int i = 0; i < _selectedSkills.Count; i++)
        {
            skillSlots[i].SetSelectedSkill(_selectedSkills[i]);
        }
    }
}
