using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class SkillInfoPanel : MonoBehaviour
{
    [SerializeField] private SubSkillType skillType = SubSkillType.none;
    [SerializeField] private RectTransform skillInfoPanel;
    [SerializeField] private GameObject skillInfomation;

    [Header("SkillTabButtons")]
    [SerializeField] private Image skillInfoPanelBackground = null;
    [SerializeField] private Sprite activeSkillInfoBackground = null;
    [SerializeField] private Sprite repulseSkillInfoBackground = null;
    [SerializeField] private Sprite derivedSkillInfoBackground = null;
    [SerializeField] private Sprite counterSkillInfoBackground = null;
    [SerializeField] private Sprite backendSkillInfoBackground = null;
    [SerializeField] private Sprite evasionSkillInfoBackground = null;
    [SerializeField] private Sprite observeSkillInfoBackground = null;
    [SerializeField] private GameObject attackSkillSelectionTabButton = null;
    [SerializeField] private GameObject repulseSkillSelectionTabButton = null;
    [SerializeField] private GameObject derivedSkillSelectionTabButton = null;
    [SerializeField] private GameObject counterSkillSelectionTabButton = null;
    [SerializeField] private GameObject defenceSkillSelectionTabButton = null;
    [SerializeField] private GameObject evasionSkillSelectionTabButton = null;

    [Header("Level Modifier UI")]
    [SerializeField] private Image skillLevelBackground;
    [SerializeField] private Sprite yellowLevelBackground;
    [SerializeField] private Sprite blueLevelBackground;
    [SerializeField] private Sprite derivedSkillLevelBackground;

    [Header("Skill Name UI")]
    [SerializeField] private Image skillNameBackground;
    [SerializeField] private Sprite yellowSkillNameBackground;
    [SerializeField] private Sprite blueSkillNameBackground;
    [SerializeField] private Sprite derivedSkillNameBackground;

    [Header("SkillInfoDetails")]
    [SerializeField] private Image skillPortrait;
    [SerializeField] private Image speedIcon;
    [SerializeField] private Image strengthState;
    [SerializeField] private Sprite strengthOn;
    [SerializeField] private Sprite strengthOff;
    [SerializeField] private Sprite speedIconLevelTwo;
    [SerializeField] private Sprite speedIconLevelThree;
    [SerializeField] private Sprite speedIconLevelFour;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI attackDamageValue;
    [SerializeField] private TextMeshProUGUI statePointCostValue;
    [SerializeField] private TextMeshProUGUI maxStatePointUpValue;
    [SerializeField] private TextMeshProUGUI strengthValue;
    [SerializeField] private TextMeshProUGUI stressDamageValue;
    [SerializeField] private TextMeshProUGUI stressResistanceValue;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI evasionStressValue;
    [SerializeField] private TextMeshProUGUI statePointDamageValue;
    [SerializeField] private TextMeshProUGUI speedValue;
    [SerializeField] private TextMeshProUGUI tagArea;
    [SerializeField] private TextMeshProUGUI tagEffectType;
    [SerializeField] private RectTransform tagListRectTransform;

    [Header("SkillInfoGameObject")]
    [SerializeField] private GameObject attackDamage;
    [SerializeField] private GameObject statePointCost;
    [SerializeField] private GameObject strength;
    [SerializeField] private GameObject maxStatePointUp;
    [SerializeField] private GameObject stressResistance;
    [SerializeField] private GameObject stressDamage;
    [SerializeField] private GameObject evasionStress;
    [SerializeField] private GameObject statePointDamage;
    [SerializeField] private GameObject speed;

    [Header("SkillInfoBox")]
    [SerializeField] private GameObject observedSkillListBox = null;

    [Header("ObservedSkillInfo")]
    [SerializeField] private SpecialSkillInfoPanel observedSkillBoxPrefab = null;
    [SerializeField] private RectTransform observedSkillListContent = null;

    private List<SpecialSkillInfoPanel> observedSkillBoxList = new List<SpecialSkillInfoPanel>();
    private CharacterSkill selectedSkill;
    private SkillInfoTabButton skillInfoTabButton = SkillInfoTabButton.none;
    private SkillSelectionBoxV2 skillSelectionBox = null;
    private string integratedText = "";

    public enum SkillInfoTabButton
    {
        none,
        active,
        backend
    }

    public enum SubSkillType
    {
        none,
        repulse,
        derived,
        counter
    }

    public void Show(SkillSelectionBoxV2 skillSelectionBox) // CharacterSkill characterSkill
    {
        this.skillSelectionBox = skillSelectionBox;

        this.selectedSkill = skillSelectionBox.GetCharacterSkill();
        if (this.selectedSkill == null)
        {
            Hide();
            return;
        }

        this.skillInfoPanel.gameObject.SetActive(true);
        SetupSkillInfomation(this.selectedSkill, this.skillSelectionBox.GetCharacterSkill());
        Subskill subskill = skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData();

        if (this.skillSelectionBox.GetCharacterSkill().GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
        {
            ShowActiveSkillInfoPanel();
            if(skillType == SubSkillType.repulse)
            {
                ShowRepulseSkillPanelUI();
            }
            else if(skillType == SubSkillType.derived)
            {
                ShowDerivedSkillPanelUI();
            }
            else
            {
                ShowActiveSkillPanelUI();
            }
        }
        else
        {
            ShowBackendSkillInfoPanel();
            if (subskill.IsDefendingSkill)
            {
                ShowDefenceSkillPanelUI();
                ShowBackendSkillTabButton(true, false);
            }
            else if (subskill.IsEvadingSkill)
            {
                ShowEvasionSkillPanelUI();
                ShowBackendSkillTabButton(false, true);
            }
            else if (subskill.IsObservingSkill)
            {
                ShowObserveSkillPanelUI();
                ShowBackendSkillTabButton(false, false);
            }
            else if (skillType == SubSkillType.counter)
            {
                ShowCounterSkillPanelUI();
            }
        }
    }

    public void Hide()
    {
        this.skillInfoPanel.gameObject.SetActive(false);
    }

    private void SetupSkillInfomation(CharacterSkill characterSkill, CharacterSkill activeSkill)
    {
        CharacterSubskill _characterSubskill = characterSkill.GetCharacterSubskillData();

        if (skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
        {
            SetupObservedSkill(characterSkill);
            this.skillInfomation.SetActive(false);
            this.observedSkillListBox.SetActive(true);
        }
        else
        {
            this.skillInfomation.SetActive(true);
            this.observedSkillListBox.SetActive(false);
        }

        Subskill _subskillData = _characterSubskill.GetSubskillData();

        string skillTypeText = "";
        if (_subskillData.Prefix.ToString() == "-") // Prefix
        {
            skillTypeText = "";
        }
        else
        {
            skillTypeText = "[" + _subskillData.Prefix.ToString() + "]";
        }

        string skillNameText = _subskillData.DisplayName; // display name
        integratedText = skillTypeText + " " + skillNameText;
        displayName.SetText(integratedText);

        this.levelText.SetText(activeSkill.GetCharacterSubskillData().GetSubskillData().Level.ToString()); // Level Text


        if (BattleLogicManager.GetAttackDamage(_subskillData) > 0) // Attack Damage
        {
            this.attackDamage.SetActive(true);
            this.attackDamageValue.SetText(BattleLogicManager.GetAttackDamage(_subskillData).ToString() + "%");
        }
        else
        {
            this.attackDamage.SetActive(false);
        }

        if (BattleLogicManager.GetStatePointCost(_subskillData) > 0) // State Point Cost
        {
            this.statePointCost.SetActive(true);
            this.statePointCostValue.SetText(BattleLogicManager.GetStatePointCost(_subskillData).ToString());
        }
        else
        {
            this.statePointCost.SetActive(false);
        }

        if (_subskillData.Strength > 1) // Strength
        {
            this.strengthState.sprite = this.strengthOn;
            this.strengthValue.SetText("+" + (_subskillData.Strength - 1).ToString());
            this.strengthValue.gameObject.SetActive(true);
        }
        else
        {
            this.strengthState.sprite = this.strengthOff;
            this.strengthValue.gameObject.SetActive(false);
        }

        if (_subskillData.Speed > 1) // Speed
        {
            if(_subskillData.Speed == 2)
            {
                this.speedIcon.sprite = this.speedIconLevelTwo;
            }
            else if(_subskillData.Speed == 3)
            {
                this.speedIcon.sprite = this.speedIconLevelThree;
            }
            else if(_subskillData.Speed == 4)
            {
                this.speedIcon.sprite = this.speedIconLevelFour;
            }
            this.speedValue.SetText(TerminologyManager.GetSpeedLevelText(_subskillData.Speed));
            this.speedValue.gameObject.SetActive(true);
        }
        else
        {
            this.speedValue.gameObject.SetActive(false);
        }

        if (_subskillData.EvasionStress > 1) // Evasion Stress
        {
            this.evasionStress.SetActive(true);
            this.evasionStressValue.SetText((_subskillData.EvasionStress).ToString());
        }
        else
        {
            this.evasionStress.SetActive(false);
        }

        if (BattleLogicManager.GetStressValueDamage(_subskillData) > 1) // Stress Value Damage
        {
            this.stressDamage.SetActive(true);
            this.stressDamageValue.SetText(BattleLogicManager.GetStressValueDamage(_subskillData).ToString() + "%");
        }
        else
        {
            this.stressDamage.SetActive(false);
        }

        if (_subskillData.StressResistance > 1) // Stress Resistance
        {
            this.stressResistance.SetActive(true);
            this.stressResistanceValue.SetText((_subskillData.StressResistance).ToString() + "%");
        }
        else
        {
            this.stressResistance.SetActive(false);
        }

        if (BattleLogicManager.GetMaxStatePointUp(_subskillData) > 1) // Max State Point Up
        {
            this.maxStatePointUp.SetActive(true);
            this.maxStatePointUpValue.SetText(BattleLogicManager.GetMaxStatePointUp(_subskillData).ToString());
        }
        else
        {
            this.maxStatePointUp.SetActive(false);
        }

        if (BattleLogicManager.GetStatePointDamage(_subskillData) > 1) // State Point Damage
        {
            this.statePointDamage.SetActive(true);
            this.statePointDamageValue.SetText(BattleLogicManager.GetStatePointDamage(_subskillData).ToString());
        }
        else
        {
            this.statePointDamage.SetActive(false);
        }

        if (_subskillData.Range != Subskill.RangeType.none || _subskillData.EffectType == Subskill.EffectTypeEnum.wide)
        {
            string rangeTagText = "";
            if (_subskillData.Range == Subskill.RangeType.melee)
            {
                rangeTagText = "【近戰】";
            }
            else if (_subskillData.Range == Subskill.RangeType.ranged)
            {
                rangeTagText = "【遠程】";
            }
            string tagEffectTypeText = $"【{ TerminologyManager.GetWideEffectTypeText(characterSkill.GetSkillData()) }】";
            integratedText = rangeTagText + "" + tagEffectTypeText;
            this.tagArea.text = integratedText;
            this.tagArea.gameObject.SetActive(true);
        }

        else
        {
            this.tagArea.gameObject.SetActive(false);
        }

        if (_subskillData.Description == "-") // Description
        {
            //this.skillDescription.SetText("");
        }
        else
        {
            //this.skillDescription.SetText(_subskillData.Description);
        }
    }

    private void SetupObservedSkill(CharacterSkill characterSkill)
    {
        this.displayName.SetText("看破");

        if (characterSkill.GetObservedSkillDataList().Count != 0)
        {
            for (int i = 0; i < characterSkill.GetObservedSkillDataList().Count; i++)
            {
                ObservedSkillData _observedSkillData = characterSkill.GetObservedSkillDataList()[i];
                observedSkillBoxPrefab.Initialize(_observedSkillData);
            }
        }
        else
        {
            Debug.Log("none observe skill");
        }
    }

    private void ClearObservedSkillList()
    {
        for (int i = 0; i < this.observedSkillBoxList.Count; i++)
        {
            SpecialSkillInfoPanel _observedSkillBox = this.observedSkillBoxList[i];
            Destroy(_observedSkillBox.gameObject);
        }

        this.observedSkillBoxList.Clear();
    }

    public void ShowActiveSkillInfoPanel()
    {
        this.skillInfoTabButton = SkillInfoTabButton.active;
        ChangeSkillSelectionPanel();
    }

    public void ShowBackendSkillInfoPanel()
    {
        this.skillInfoTabButton = SkillInfoTabButton.backend;
        ChangeSkillSelectionPanel();
    }

    private void ChangeSkillSelectionPanel()
    {
        if (this.skillInfoTabButton == SkillInfoTabButton.active)
        {
            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(true);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(true);
            this.counterSkillSelectionTabButton.gameObject.SetActive(false);
        }
        else if (this.skillInfoTabButton == SkillInfoTabButton.backend)
        {
            this.attackSkillSelectionTabButton.gameObject.SetActive(false);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(false);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(false);
            this.counterSkillSelectionTabButton.gameObject.SetActive(true);
        }
    }

    public void ShowActiveSkillPanelUI()
    {
        skillType = SubSkillType.none;
        UpdateSkillInfoPanel(this.selectedSkill);
        SetSkillPanelUI(activeSkillInfoBackground, yellowLevelBackground, yellowSkillNameBackground);
        this.derivedSkillSelectionTabButton.gameObject.SetActive(true);

        if (this.skillSelectionBox.GetIsSelected())
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetSkillBoxSelectedBackgroundImage());
        }
        else
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetSkillBoxSelectBackgroundImage());
        }
    }

    public void ShowRepulseSkillPanelUI()
    {
        if (this.selectedSkill.GetCharacterSubskillData().GetSelectedRepulseSkill() == null)
        {
            return;
        }

        skillType = SubSkillType.repulse;
        this.derivedSkillSelectionTabButton.gameObject.SetActive(true);
        UpdateSkillInfoPanel(this.selectedSkill.GetCharacterSubskillData().GetSelectedRepulseSkill());
        SetSkillPanelUI(repulseSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
        this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetRepulseSkillBoxFrameImage());
    }

    public void ShowDerivedSkillPanelUI()
    {
        if (this.selectedSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() == null)
        {
            return;
        }

        skillType = SubSkillType.derived;
        this.derivedSkillSelectionTabButton.gameObject.SetActive(false);
        UpdateSkillInfoPanel(this.selectedSkill.GetCharacterSubskillData().GetSelectedDerivedSkill());
        SetSkillPanelUI(derivedSkillInfoBackground, derivedSkillLevelBackground, derivedSkillNameBackground);
        this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetDerivedSkillBoxFrameImage());
    }

    public void ShowDefenceSkillPanelUI()
    {
        UpdateSkillInfoPanel(this.selectedSkill);
        SetSkillPanelUI(backendSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void ShowEvasionSkillPanelUI()
    {
        UpdateSkillInfoPanel(this.selectedSkill);
        SetSkillPanelUI(evasionSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void ShowCounterSkillPanelUI()
    {
        skillType = SubSkillType.counter;
        UpdateSkillInfoPanel(this.selectedSkill.GetCharacterSubskillData().GetSelectedCounterSkill());
        SetSkillPanelUI(counterSkillInfoBackground, yellowLevelBackground, yellowSkillNameBackground);
    }

    public void ShowObserveSkillPanelUI()
    {
        UpdateSkillInfoPanel(this.selectedSkill);
        SetSkillPanelUI(observeSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void IncreaseSkillInfoLevel()
    {
        //increase the skill level
        this.skillSelectionBox.IncreaseSkillLevel();
        UpdateSubSkillType();
    }

    public void DecreaseSkillInfoLevel()
    {
        //decrease the skill level
        this.skillSelectionBox.DecreaseSkillLevel();
        UpdateSubSkillType();
    }

    public void UpdateSkillInfoPanel(CharacterSkill selectedSkill)
    {
        SetupSkillInfomation(selectedSkill, this.skillSelectionBox.GetCharacterSkill());
    }

    public void ShowBackendSkillTabButton(bool isDefenceSkill, bool isEvasionSkill)
    {
        this.defenceSkillSelectionTabButton.SetActive(isDefenceSkill);
        this.evasionSkillSelectionTabButton.SetActive(isEvasionSkill);
    }

    public void UpdateSubSkillType()
    {
        CharacterSkill activeSkill = this.skillSelectionBox.GetCharacterSkill();
        CharacterSubskill subSkill = activeSkill.GetCharacterSubskillData();
        this.SetupSkillInfomation(activeSkill, activeSkill);
        if (skillType == SubSkillType.repulse)
        {
            this.SetupSkillInfomation(subSkill.GetSelectedRepulseSkill(), activeSkill);
        }
        else if (skillType == SubSkillType.derived)
        {
            this.SetupSkillInfomation(subSkill.GetSelectedDerivedSkill(), activeSkill);
        }
        else if (skillType == SubSkillType.counter)
        {
            this.SetupSkillInfomation(subSkill.GetSelectedCounterSkill(), activeSkill);
        }
    }

    private void SetSkillPanelUI(Sprite skillPanelBackground, Sprite skillLevelBackground, Sprite skillNameBackground)
    {
        this.skillInfoPanelBackground.sprite = skillPanelBackground;
        this.skillLevelBackground.sprite = skillLevelBackground;
        this.skillNameBackground.sprite = skillNameBackground;
    }
}
