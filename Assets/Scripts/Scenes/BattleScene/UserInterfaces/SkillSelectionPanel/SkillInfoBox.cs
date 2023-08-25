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
    [SerializeField] private TextMeshProUGUI strengthValue;
    [SerializeField] private TextMeshProUGUI accuracyValue;
    [SerializeField] private TextMeshProUGUI evasionValue;
    [SerializeField] private TextMeshProUGUI skillDescription;

    [Header("SkillInfoLabel")]
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI evasion;

    public void Show(CharacterSubskill characterSubskill)
    {
        if (characterSubskill == null)
        {
            Hide();
            return;
        }

        this.skillInformation.gameObject.SetActive(true);
        SetupSkillInfomation(characterSubskill);
    }

    public void Hide()
    {
        this.skillInformation.gameObject.SetActive(false);
    }

    private void SetupSkillInfomation(CharacterSubskill characterSubskill)
    {
        Subskill _subskillData = characterSubskill.GetSubskillData();

        if (_subskillData.Prefix.ToString() == "-")
        {
            this.skillType.SetText("");
        }
        else
        {
            this.skillType.SetText("[" + _subskillData.Prefix.ToString() + "]");
        }
        this.displayName.SetText(_subskillData.DisplayName);
        this.attackDamage.SetText(_subskillData.AttackDamage.ToString());
        this.statePointCost.SetText(_subskillData.StatePointCost.ToString());

        if (_subskillData.Strength > 1)
        {
            this.strength.gameObject.SetActive(true);
            this.strengthValue.SetText("+" + (_subskillData.Strength - 1).ToString());
        }
        else
        {
            this.strength.gameObject.SetActive(false);
        }

        if (_subskillData.Accuracy > 1)
        {
            this.accuracy.gameObject.SetActive(true);
            this.accuracyValue.SetText("+" + (_subskillData.Accuracy - 1).ToString());
        }
        else
        {
            this.accuracy.gameObject.SetActive(false);
        }

        if (_subskillData.Evasion > 1)
        {
            this.evasion.gameObject.SetActive(true);
            this.evasionValue.SetText("+" + (_subskillData.Evasion - 1).ToString());
        }
        else
        {
            this.evasion.gameObject.SetActive(false);
        }
    }
}
