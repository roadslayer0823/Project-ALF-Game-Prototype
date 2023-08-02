using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillInfoBox : MonoBehaviour
{
    [SerializeField] private RectTransform skillInformation;

    [Header("SkillInfoDetails")]
    [SerializeField] private Image skillPortrait;
    [SerializeField] private TextMeshProUGUI skillType;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillStatusBoostValue;
    [SerializeField] private TextMeshProUGUI skillLoadValue;

    public void Show(DatabaseManager.Skill characterSkill)
    {
        this.skillInformation.gameObject.SetActive(true);
        SetupSkillInfomation(characterSkill);
    }

    public void Hide()
    {
        this.skillInformation.gameObject.SetActive(false);
    }

    private void SetupSkillInfomation(DatabaseManager.Skill characterSkill)
    {
        this.skillType.SetText("[" + characterSkill.GetSkillType().ToString() + "]");
        this.skillName.SetText(characterSkill.GetDisplayName());
    }
}
