using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class SkillInfoPanel : MonoBehaviour
{
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

    [Header("Level Modifier UI")]
    [SerializeField] private Image skillLevelBackground;
    [SerializeField] private Sprite yellowLevelBackground;
    [SerializeField] private Sprite blueLevelBackground;
    [SerializeField] private Sprite derivedSkillLevelBackground;
    [SerializeField] private Button increaseSkillnfoLevel;
    [SerializeField] private Button decreaseSkillnfoLevel;

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
    [SerializeField] private TextMeshProUGUI skillType;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI skillPanelLevelText;
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
    [SerializeField] private GameObject observedSkillInfo = null;
    [SerializeField] private SpecialSkillInfoPanel observedSkillBoxPrefab = null;
    [SerializeField] private RectTransform observedSkillListContent = null;

    private List<SpecialSkillInfoPanel> observedSkillBoxList = new List<SpecialSkillInfoPanel>();
    private SkillInfoUIPanel skillInfoUIPanel = SkillInfoUIPanel.none;
    private SkillSelectionBoxV2 skillSelectionBox = null;

    public enum SkillInfoUIPanel
    {
        none,
        active,
        backend
    }

    public void Show(SkillSelectionBoxV2 skillSelectionBox) // CharacterSkill characterSkill
    {
        this.skillSelectionBox = skillSelectionBox;

        CharacterSkill _characterSkill = skillSelectionBox.GetCharacterSkill();
        if (_characterSkill == null)
        {
            Hide();
            return;
        }

        this.skillInfoPanel.gameObject.SetActive(true);
        SetupSkillInfomation(_characterSkill);
    }

    public void Hide()
    {
        this.skillInfoPanel.gameObject.SetActive(false);
    }

    private void SetupSkillInfomation(CharacterSkill characterSkill)
    {
        CharacterSubskill _characterSubskill = characterSkill.GetCharacterSubskillData();

        if (_characterSubskill.GetSubskillData().IsObservingSkill)
        {
            //SetupObservedSkillList(characterSkill);
        }
        else
        {
            //this.observedSkillListBox.SetActive(false);
            //this.observedSkillInfo.SetActive(false);
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
        
        this.displayName.SetText(_subskillData.DisplayName); // Display Name

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
            string integratedText = rangeTagText + "" + tagEffectTypeText;
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

    private void SetupObservedSkillList(CharacterSkill characterSkill)
    {
        this.observedSkillListBox.SetActive(true);
        this.observedSkillInfo.SetActive(false); // Change back to true in future if needed

        ClearObservedSkillList();

        if (characterSkill.GetObservedSkillDataList().Count == 0)
        {
            this.observedSkillInfo.gameObject.SetActive(false);
            this.skillInfomation.SetActive(true);
        }
        else
        {
            this.observedSkillInfo.gameObject.SetActive(true);
            this.skillInfomation.SetActive(false);
            this.displayName.SetText("看破");

            // Initialize the ObservedSkillBox so that the skill can be display on it respectively. 
            for (int i = 0; i < characterSkill.GetObservedSkillDataList().Count; i++)
            {
                ObservedSkillData _observedSkillData = characterSkill.GetObservedSkillDataList()[i];

                SpecialSkillInfoPanel _observedSkillBox = Instantiate(this.observedSkillBoxPrefab, this.observedSkillListContent, false);
                _observedSkillBox.Initialize(_observedSkillData);

                this.observedSkillBoxList.Add(_observedSkillBox);
            }
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
        this.skillInfoUIPanel = SkillInfoUIPanel.active;
        ChangeSkillSelectionPanel();
    }

    public void ShowBackendSkillInfoPanel()
    {
        this.skillInfoUIPanel = SkillInfoUIPanel.backend;
        ChangeSkillSelectionPanel();
    }

    private void ChangeSkillSelectionPanel()
    {
        if (this.skillInfoUIPanel == SkillInfoUIPanel.active)
        {
            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(true);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(true);
            this.counterSkillSelectionTabButton.gameObject.SetActive(false);
        }
        else if (this.skillInfoUIPanel == SkillInfoUIPanel.backend)
        {
            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(false);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(false);
            this.counterSkillSelectionTabButton.gameObject.SetActive(true);
        }
    }

    public void ShowActiveSkillPanelUI()
    {
        SetSkillPanelUI(activeSkillInfoBackground, yellowLevelBackground, yellowSkillNameBackground);
    }

    public void ShowRepulseSkillPanelUI()
    {
        SetSkillPanelUI(repulseSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void ShowDerivedSkillPanelUI()
    {
        SetSkillPanelUI(derivedSkillInfoBackground, derivedSkillLevelBackground, derivedSkillNameBackground);
    }

    public void ShowBackendSkillPanelUI()
    {
        SetSkillPanelUI(repulseSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void ShowEvasionSkillPanelUI()
    {
        SetSkillPanelUI(repulseSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void ShowCounterSkillPanelUI()
    {
        SetSkillPanelUI(counterSkillInfoBackground, yellowLevelBackground, yellowSkillNameBackground);
    }

    public void ShowObserveSkillPanelUI()
    {
        SetSkillPanelUI(observeSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
    }

    public void IncreaseSkillInfoLevel()
    {
        //increase the skill level
        this.skillSelectionBox.IncreaseSkillLevel();
        this.SetupSkillInfomation(this.skillSelectionBox.GetCharacterSkill());
    }

    public void DecreaseSkillInfoLevel()
    {
        //decrease the skill level
        this.skillSelectionBox.DecreaseSkillLevel();
        this.SetupSkillInfomation(this.skillSelectionBox.GetCharacterSkill());
    }

    private void SetSkillPanelUI(Sprite skillPanelBackground, Sprite skillLevelBackground, Sprite skillNameBackground)
    {
        this.skillInfoPanelBackground.sprite = skillPanelBackground;
        this.skillLevelBackground.sprite = skillLevelBackground;
        this.skillNameBackground.sprite = skillNameBackground;
    }
}
