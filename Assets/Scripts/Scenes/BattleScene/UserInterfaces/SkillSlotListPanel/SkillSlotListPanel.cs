using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[ 0 ];

    public void Show( GameCharacter gameCharacter )
    {
        List<CharacterSkill> _selectedSkills = gameCharacter.GetSelectedSkills();
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[ i ].SetSelectedSkill( _selectedSkills[ i ] );
        }
    }
}
