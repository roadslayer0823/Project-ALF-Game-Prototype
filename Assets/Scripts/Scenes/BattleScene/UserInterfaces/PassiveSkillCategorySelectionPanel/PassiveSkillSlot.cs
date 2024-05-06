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

    public enum PassiveSkillType
    {
        None,
        HealthPoint,
        StatePoint,
        StressValue
    }

    public void UpdateSelectedPassiveSkillUI()
    {
        AssignCurrentPassiveSkillUI(this.selectedHealthPointSkill, this.selectedStatePointSkill, this.selectedStressValueSkill);
    }

    public void UpdateDefaultPassiveSkillUI()
    {
        AssignCurrentPassiveSkillUI(this.defaultHealthPointSkill, this.defaultStatePointSkill, this.defaultStressValueSkill);
    }

    public void AssignCurrentPassiveSkillUI(Sprite healthPointSkill, Sprite statePointSkill, Sprite stressPointSkill)
    {
        this.passiveSkillSlot.sprite = passiveSkillType switch
        {
            PassiveSkillType.HealthPoint => healthPointSkill,
            PassiveSkillType.StatePoint => statePointSkill,
            PassiveSkillType.StressValue => stressPointSkill,
            _ => throw new NotImplementedException()
        };
    }
}
