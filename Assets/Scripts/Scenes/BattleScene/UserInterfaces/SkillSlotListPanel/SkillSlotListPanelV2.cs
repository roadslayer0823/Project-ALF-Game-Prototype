using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotListPanelV2 : MonoBehaviour
{
    [SerializeField] private SkillSlotV2[] skillSlots = new SkillSlotV2[0];
    private GameCharacter selectedGameCharacter = null;
    private List<CharacterSkill> selectedSkills = null;
    private const string AUDIO_ID_WHEEL = "wheel";

    public void Show()
    {
        base.gameObject.SetActive(true);
    }

    public void Hide()
    {
        base.gameObject.SetActive(false);
    }

    public void UpdateSkillSlots(GameCharacter gameCharacter)
    {
        if (gameCharacter != null)
        {
            this.selectedGameCharacter = gameCharacter;

            this.selectedSkills = new List<CharacterSkill>(gameCharacter.GetSelectedActiveSkillList());

            if (this.selectedSkills.Count > skillSlots.Length)
            {
                return;
            }

            InsertIntoSkillSlot(this.selectedSkills);
        }
    }

    private void InsertIntoSkillSlot(List<CharacterSkill> selectedSkills)
    {
        ClearSkillSlots();

        for (int i = 0; i < selectedSkills.Count; i++)
        {
            skillSlots[i].SetSelectedSkill(selectedSkills[i]);
        }
    }

    private void ClearSkillSlots()
    {
        foreach (SkillSlotV2 slot in skillSlots)
        {
            slot.Clear();
        }
    }

    public SkillSlotV2[] GetSkillSlots()
    {
        return this.skillSlots;
    }

    public void ClickTop()
    {
        CharacterSkill tempSlot;

        List<CharacterSkill> skillList = this.selectedGameCharacter.GetSelectedActiveSkillList();

        int i = 0;
        tempSlot = skillList[i];

        while (i < skillList.Count - 1)
        {
            skillList[i] = skillList[i + 1];
            i++;
        }

        skillList[skillList.Count - 1] = tempSlot;

        InsertIntoSkillSlot(skillList);
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
        Debug.Log("go down");
    }

    public void ClickBottom()
    {
        CharacterSkill tempSlot;
     
        List<CharacterSkill> skillList = this.selectedGameCharacter.GetSelectedActiveSkillList();

        int i = skillList.Count - 1;
        tempSlot = skillList[i];

        while (i > 0)
        {
            skillList[i] = skillList[i - 1];
            i--;
        }

        skillList[0] = tempSlot;

        InsertIntoSkillSlot(skillList);
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
        Debug.Log("go up");
    }

    // To reset back the selected skill sequence based on last round selection. 
    public void ResetLastRoundSelectedActiveSkill()
    {
        this.selectedGameCharacter.SetSelectedActiveSkillList(this.selectedSkills);

        InsertIntoSkillSlot(this.selectedSkills);
    }
}
