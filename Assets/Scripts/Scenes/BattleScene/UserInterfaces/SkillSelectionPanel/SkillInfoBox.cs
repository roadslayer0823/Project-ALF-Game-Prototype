using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;
using System.Collections.Generic;

public class SkillInfoBox : MonoBehaviour
{
    [SerializeField] private RectTransform skillInformation;

    [Header("SkillInfoDetails")]
    [SerializeField] private Image skillPortrait;
    [SerializeField] private TextMeshProUGUI skillType;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI attackDamageValue;
    [SerializeField] private TextMeshProUGUI statePointCostTitle;
    [SerializeField] private TextMeshProUGUI statePointCostValue;
    [SerializeField] private TextMeshProUGUI strengthValue;
    [SerializeField] private TextMeshProUGUI accuracyValue;
    [SerializeField] private TextMeshProUGUI evasionValue;
    [SerializeField] private TextMeshProUGUI stressDamageValue;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI evasionStressValue;

    [Header("SkillInfoLabel")]
    [SerializeField] private TextMeshProUGUI attackDamage;
    [SerializeField] private TextMeshProUGUI statePointCost;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI evasion;
    [SerializeField] private TextMeshProUGUI stressDamage;
    [SerializeField] private TextMeshProUGUI evasionStress;

    [Header("SkillInfoBox")]
    [SerializeField] private GameObject skillDataBox = null;
    [SerializeField] private GameObject observedSkillListBox = null;

    [Header("ObservedSkillInfo")]
    [SerializeField] private GameObject observedSkillInfo = null;
    [SerializeField] private ObservedSkillBox observedSkillBoxPrefab = null;
    [SerializeField] private RectTransform observedSkillListContent = null;

    private List<ObservedSkillBox> observedSkillBoxList = new List<ObservedSkillBox>();

    public void Show(CharacterSkill characterSkill)
    {
        if (characterSkill == null)
        {
            Hide();
            return;
        }

        this.skillInformation.gameObject.SetActive(true);

        SetupSkillInfomation(characterSkill);
    }

    public void Hide()
    {
        this.skillInformation.gameObject.SetActive(false);
    }

    private void SetupSkillInfomation(CharacterSkill characterSkill)
    {
        CharacterSubskill _characterSubskill = characterSkill.GetCharacterSubskillData();

        if (_characterSubskill.GetSubskillData().IsObservingSkill)
        {
            SetupObservedSkillList(characterSkill);
        }
        else
        {
            this.skillDataBox.SetActive(true);
            this.observedSkillListBox.SetActive(false);
            this.observedSkillInfo.SetActive(false);
        }

        Subskill _subskillData = _characterSubskill.GetSubskillData();

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

        //State Point Title
        this.statePointCostTitle.SetText(TerminologyManager.STATE_POINT+"點數");

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

        if (_subskillData.EvasionStress > 1) // Evasion Stress
        {
            this.evasionStress.gameObject.SetActive(true);
            this.evasionStressValue.SetText("+" + (_subskillData.EvasionStress).ToString());
        }
        else
        {
            this.evasionStress.gameObject.SetActive(false);
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

    private void SetupObservedSkillList(CharacterSkill characterSkill)
    {
        this.skillDataBox.SetActive(false);
        this.observedSkillListBox.SetActive(true);
        this.observedSkillInfo.SetActive(false); // Change back to true in future if needed

        ClearObservedSkillList();

        Debug.Log("No of element: " + characterSkill.GetObservedSkillDataList().Count);

        // Initialize the SkillSelectionBox so that the skill can be display on it respectively. 
        for (int i = 0; i < characterSkill.GetObservedSkillDataList().Count; i++)
        {
            ObservedSkillData _observedSkillData = characterSkill.GetObservedSkillDataList()[i];

            ObservedSkillBox _observedSkillBox = Instantiate(this.observedSkillBoxPrefab, this.observedSkillListContent, false);
            _observedSkillBox.Initialize(_observedSkillData);

            this.observedSkillBoxList.Add(_observedSkillBox);
        }
    }

    private void ClearObservedSkillList()
    {
        for (int i = 0; i < this.observedSkillBoxList.Count; i++)
        {
            ObservedSkillBox _observedSkillBox = this.observedSkillBoxList[i];
            Destroy(_observedSkillBox.gameObject);
        }

        this.observedSkillBoxList.Clear();
    }
}
