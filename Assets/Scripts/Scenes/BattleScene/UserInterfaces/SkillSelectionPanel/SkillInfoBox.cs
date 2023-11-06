using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;
using Skill = DatabaseManager.Skill;
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
    [SerializeField] private TextMeshProUGUI statePointDamageValue;
    [SerializeField] private TextMeshProUGUI speedValue;
    [SerializeField] private TextMeshProUGUI tagEffectType;

    [Header("SkillInfoLabel")]
    [SerializeField] private TextMeshProUGUI attackDamage;
    [SerializeField] private TextMeshProUGUI statePointCost;
    [SerializeField] private TextMeshProUGUI strength;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI evasion;
    [SerializeField] private TextMeshProUGUI stressDamage;
    [SerializeField] private TextMeshProUGUI evasionStress;
    [SerializeField] private TextMeshProUGUI statePointDamage;
    [SerializeField] private TextMeshProUGUI speed;

    [Header("SkillInfoBox")]
    [SerializeField] private GameObject skillDataBox = null;
    [SerializeField] private GameObject observedSkillListBox = null;

    [Header("ObservedSkillInfo")]
    [SerializeField] private GameObject observedSkillInfo = null;
    [SerializeField] private ObservedSkillBox observedSkillBoxPrefab = null;
    [SerializeField] private RectTransform observedSkillListContent = null;
    [SerializeField] private TextMeshProUGUI noRecordFoundText = null;

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
        this.statePointCostTitle.SetText(TerminologyManager.STATE_POINT+"消耗");

        if (BattleLogicManager.GetAttackDamage(_subskillData) > 0) // Attack Damage
        {
            this.attackDamage.gameObject.SetActive(true);
            this.attackDamageValue.SetText(BattleLogicManager.GetAttackDamage(_subskillData).ToString());
        }
        else
        {
            this.attackDamage.gameObject.SetActive(false);
        }

        if (BattleLogicManager.GetStatePointCost(_subskillData) > 0) // State Point Cost
        {
            this.statePointCost.gameObject.SetActive(true);
            this.statePointCostValue.SetText(BattleLogicManager.GetStatePointCost(_subskillData).ToString());
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

        if (_subskillData.Speed > 1) // Speed
        {
            this.speed.gameObject.SetActive(true);
            this.speedValue.SetText(TerminologyManager.GetSpeedLevelText(_subskillData.Speed));
        }
        else
        {
            this.speed.gameObject.SetActive(false);
        }


        if (_subskillData.EvasionStress > 1) // Evasion Stress
        {
            this.evasionStress.gameObject.SetActive(true);
            this.evasionStressValue.SetText((_subskillData.EvasionStress).ToString());
        }
        else
        {
            this.evasionStress.gameObject.SetActive(false);
        }

        if (BattleLogicManager.GetStressValueDamage(_subskillData) > 1) // Stress Value Damage
        {
            this.stressDamage.gameObject.SetActive(true);
            this.stressDamageValue.SetText(BattleLogicManager.GetStressValueDamage(_subskillData).ToString());
        }
        else
        {
            this.stressDamage.gameObject.SetActive(false);
        }

        if (BattleLogicManager.GetStatePointDamage(_subskillData) > 1) // State Point Damage
        {
            this.statePointDamage.gameObject.SetActive(true);
            this.statePointDamageValue.SetText(BattleLogicManager.GetStatePointDamage(_subskillData).ToString());
        }
        else
        {
            this.statePointDamage.gameObject.SetActive(false);
        }

        if (_subskillData.EffectType == Subskill.EffectTypeEnum.wide) // tag effect type
        {
            string effectType = "";
            Skill _skillData = characterSkill.GetSkillData();
            this.tagEffectType.gameObject.SetActive(true);

            if (_skillData.skillType == Skill.SkillType.repulse || _skillData.skillType == Skill.SkillType.derived)
            {
                effectType += "對";
            }
            effectType += "廣角";
            tagEffectType.text ="【" + effectType + "】";
        }
        else
        {
            this.tagEffectType.gameObject.SetActive(false);
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

        if (characterSkill.GetObservedSkillDataList().Count == 0)
        {
            this.noRecordFoundText.SetText("暫無記錄");
            this.noRecordFoundText.gameObject.SetActive(true);
        }
        else
        {
            this.noRecordFoundText.gameObject.SetActive(false);

            // Initialize the ObservedSkillBox so that the skill can be display on it respectively. 
            for (int i = 0; i < characterSkill.GetObservedSkillDataList().Count; i++)
            {
                ObservedSkillData _observedSkillData = characterSkill.GetObservedSkillDataList()[i];

                ObservedSkillBox _observedSkillBox = Instantiate(this.observedSkillBoxPrefab, this.observedSkillListContent, false);
                _observedSkillBox.Initialize(_observedSkillData);

                this.observedSkillBoxList.Add(_observedSkillBox);
            }
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
