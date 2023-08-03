using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        this.skillType.SetText("[" + characterSkill.GetSkillData().GetSkillType().ToString() + "]");
        this.displayName.SetText(characterSkill.GetSkillData().GetDisplayName());
        this.attackDamage.SetText(characterSkill.GetSubskillByLevel(1).GetAttackDamage().ToString());
        this.statePointCost.SetText(characterSkill.GetSubskillByLevel(1).GetStatePointCost().ToString());
    }
}
