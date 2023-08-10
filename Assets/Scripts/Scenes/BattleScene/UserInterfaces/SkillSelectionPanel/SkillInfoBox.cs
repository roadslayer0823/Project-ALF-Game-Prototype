using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class SkillInfoBox : MonoBehaviour
{
    [SerializeField] private RectTransform skillInformation;

    [Header("SkillInfoDetails")]
    [SerializeField] private Image skillPortrait;
    [SerializeField] private TextMeshProUGUI skillType;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI attackDamage;
    [SerializeField] private TextMeshProUGUI statePointCost;
    [SerializeField] private TextMeshProUGUI skillDescription;

    public void Show(CharacterSkill characterSkill)
    {
        this.skillInformation.gameObject.SetActive(true);
        SetupSkillInfomation(characterSkill);
    }

    public void Hide()
    {
        this.skillInformation.gameObject.SetActive(false);
    }

    private void SetupSkillInfomation(CharacterSkill characterSkill)
    {
        Skill _skillData = characterSkill.GetSkillData();
        Subskill _subskillData = characterSkill.GetSubskillData();

        if (_subskillData.GetPrefix().ToString() == "-")
        {
            this.skillType.SetText("");
        }
        else
        {
            this.skillType.SetText("[" + _subskillData.GetPrefix().ToString() + "]");
        }
        this.displayName.SetText(_skillData.GetGroupName());
        this.attackDamage.SetText(_subskillData.GetAttackDamage().ToString());
        this.statePointCost.SetText(_subskillData.GetStatePointCost().ToString());
    }
}
