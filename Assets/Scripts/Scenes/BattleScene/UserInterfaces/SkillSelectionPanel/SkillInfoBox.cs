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
    [SerializeField] private TextMeshProUGUI attackDamageValue;
    [SerializeField] private TextMeshProUGUI statePointCostValue;
    [SerializeField] private TextMeshProUGUI strengthValue;
    [SerializeField] private TextMeshProUGUI accuracyValue;
    [SerializeField] private TextMeshProUGUI evasionValue;
    [SerializeField] private TextMeshProUGUI stressDamageValue;
    [SerializeField] private TextMeshProUGUI skillDescription;

    [Header("SkillInfoLabel")]
    [SerializeField] private TextMeshProUGUI attackDamage;
    [SerializeField] private TextMeshProUGUI statePointCost;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI evasion;
    [SerializeField] private TextMeshProUGUI stressDamage;

    [Header("SkillInfoBox")]
    [SerializeField] private GameObject skillDataBox = null;
    [SerializeField] private GameObject observedSkillListBox = null;

    [Header("ObservedSkillInfo")]
    [SerializeField] private GameObject observedSkillInfo = null;

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
        if (characterSubskill.GetSubskillData().IsObservingSkill)
        {
            SetupObservedSkillList(characterSubskill);
        }
        else
        {
            this.skillDataBox.SetActive(true);
            this.observedSkillListBox.SetActive(false);
            this.observedSkillInfo.SetActive(false);
        }

        Subskill _subskillData = characterSubskill.GetSubskillData();

        if (_subskillData.Prefix.ToString() == "-") // Prefix
        {
            this.skillType.SetText("");
        }
        else
        {
            this.skillType.SetText("[" + _subskillData.Prefix.ToString() + "]");
        }

        // Display Name
        this.displayName.SetText(_subskillData.DisplayName);

        if (_subskillData.AttackDamage > 0) // Attack Damage
        {
            this.attackDamage.gameObject.SetActive(true);
            this.attackDamageValue.SetText(_subskillData.AttackDamage.ToString());
        }
        else
        {
            this.attackDamage.gameObject.SetActive(false);
        }

        if (_subskillData.StatePointCost > 0) // State Point Cost
        {
            this.statePointCost.gameObject.SetActive(true);
            this.statePointCostValue.SetText(_subskillData.StatePointCost.ToString());
        }
        else
        {
            this.statePointCost.gameObject.SetActive(false);
        }

        if (_subskillData.Strength > 1) // Strength
        {
            this.strength.gameObject.SetActive(true);
            this.strengthValue.SetText("+" + (_subskillData.Strength - 1).ToString());
        }
        else
        {
            this.strength.gameObject.SetActive(false);
        }

        if (_subskillData.Accuracy > 1) // Accuracy
        {
            this.accuracy.gameObject.SetActive(true);
            this.accuracyValue.SetText("+" + (_subskillData.Accuracy - 1).ToString());
        }
        else
        {
            this.accuracy.gameObject.SetActive(false);
        }

        if (_subskillData.Evasion > 1) // Evasion
        {
            this.evasion.gameObject.SetActive(true);
            this.evasionValue.SetText("+" + (_subskillData.Evasion - 1).ToString());
        }
        else
        {
            this.evasion.gameObject.SetActive(false);
        }

        if (_subskillData.StressDamage > 1) // Stress Damage
        {
            this.stressDamage.gameObject.SetActive(true);
            this.stressDamageValue.SetText("+" + (_subskillData.StressDamage - 1).ToString());
        }
        else
        {
            this.stressDamage.gameObject.SetActive(false);
        }

        if (_subskillData.Description == "-") // Description
        {
            this.skillDescription.SetText("");
        }
        else
        {
            this.skillDescription.SetText(_subskillData.Description);
        }
    }

    private void SetupObservedSkillList(CharacterSubskill characterSubskill)
    {
        this.skillDataBox.SetActive(false);
        this.observedSkillListBox.SetActive(true);
        this.observedSkillInfo.SetActive(false); // Change back to true in future if needed

        //TODO: List all observed skills. 
    }
}
