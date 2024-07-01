using System;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSkillSlot : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PassiveSkillType passiveSkillType = PassiveSkillType.None;

    [Header("Passive Skill")]
    [SerializeField] private Image passiveSkillSlot;

    [Header("Selected Passive Skill UI")]
    [SerializeField] private Sprite selectedStatePointSkill;
    [SerializeField] private Sprite selectedStressValueSkill;
    [SerializeField] private Sprite selectedHealthPointSkill;

    [Header("Default Passive Skill UI")]
    [SerializeField] private Sprite defaultStatePointSkill;
    [SerializeField] private Sprite defaultStressValueSkill;
    [SerializeField] private Sprite defaultHealthPointSkill;

    private PassiveSkillCategorySelectionPanel passiveSkillCategorySelectionPanel;

    public enum PassiveSkillType
    {
        None,
        HealthPoint,
        StatePoint,
        StressValue
    }

    public void UpdateSelectedPassiveSkillUI()
    {
        if(passiveSkillType == PassiveSkillType.HealthPoint)
        {
            this.passiveSkillSlot.sprite = this.selectedHealthPointSkill;
        }
        else if(passiveSkillType == PassiveSkillType.StatePoint)
        {
            this.passiveSkillSlot.sprite = this.selectedStatePointSkill;
        }
        else if(passiveSkillType == PassiveSkillType.StressValue)
        {
            this.passiveSkillSlot.sprite = this.selectedStressValueSkill;
        }
    }

    public void UpdateDefaultPassiveSkillUI()
    {
        if (passiveSkillType == PassiveSkillType.HealthPoint)
        {
            this.passiveSkillSlot.sprite = this.defaultHealthPointSkill;
        }
        else if (passiveSkillType == PassiveSkillType.StatePoint)
        {
            this.passiveSkillSlot.sprite = this.defaultStatePointSkill;
        }
        else if (passiveSkillType == PassiveSkillType.StressValue)
        {
            this.passiveSkillSlot.sprite = this.defaultStressValueSkill;
        }
    }

    public void ClickOption()
    {
        UpdateSelectedPassiveSkillUI();
    }

    public void OptionEnter()
    {
        UpdateSelectedPassiveSkillUI();
    }

    public void OptionExit()
    {
        UpdateDefaultPassiveSkillUI();
    }
}
