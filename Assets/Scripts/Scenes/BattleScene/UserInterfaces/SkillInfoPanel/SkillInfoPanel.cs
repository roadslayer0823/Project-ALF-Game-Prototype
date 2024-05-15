using TMPro;
using System;
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
    [SerializeField] private SpecialSkillInfoBox observedSkillBoxPrefab = null;
    [SerializeField] private RectTransform observedSkillListContent = null;

    private List<SpecialSkillInfoBox> observedSkillBoxList = new List<SpecialSkillInfoBox>();
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
            if(this.skillType == SubSkillType.repulse)
            {
                ShowRepulseSkillPanelUI();
            }
            else if(this.skillType == SubSkillType.derived)
            {
                if(this.skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSelectedDerivedSkill() == null)
                {
                    ShowActiveSkillPanelUI();
                }
                else
                {
                    ShowDerivedSkillPanelUI();
                }
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
            }
            else if (subskill.IsEvadingSkill)
            {
                ShowEvasionSkillPanelUI();
            }
            else if (subskill.IsObservingSkill)
            {
                ShowObserveSkillPanelUI();
                this.counterSkillSelectionTabButton.SetActive(false);
            }
            else if (this.skillType == SubSkillType.counter)
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
        Subskill _subskillData = _characterSubskill.GetSubskillData();

        if (this.skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
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
        this.integratedText = skillTypeText + " " + skillNameText;
        this.displayName.SetText(integratedText);

        this.levelText.SetText(activeSkill.GetCharacterSubskillData().GetSubskillData().Level.ToString()); // Level Text


        if (BattleCalculationManager.GetAttackDamage(_subskillData) > 0) // Attack Damage
        {
            this.attackDamage.SetActive(true);
            this.attackDamageValue.SetText( BattleCalculationManager.GetAttackDamage( _subskillData ).ToString() );
        }
        else
        {
            this.attackDamage.SetActive(false);
        }

        if (BattleCalculationManager.GetStatePointCost(_subskillData) > 0) // State Point Cost
        {
            this.statePointCost.SetActive(true);
            this.statePointCostValue.SetText( BattleCalculationManager.GetStatePointCost( _subskillData ).ToString() );
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
            this.speedIcon.sprite = _subskillData.Speed switch
            {
                1 => null,
                2 => this.speedIconLevelTwo,
                3 => this.speedIconLevelThree,
                4 => this.speedIconLevelFour,
                _ => throw new NotImplementedException()
            };
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

        if (BattleCalculationManager.GetStressValueDamage(_subskillData) > 1) // Stress Value Damage
        {
            this.stressDamage.SetActive(true);
            this.stressDamageValue.SetText( BattleCalculationManager.GetStressValueDamage( _subskillData ).ToString() + "%" );
        }
        else
        {
            this.stressDamage.SetActive(false);
        }

        if (_subskillData.StressResistance > 0) // Stress Resistance
        {
            this.stressResistance.SetActive( true );
            this.stressResistanceValue.SetText( string.Format( "{0:0%}", _subskillData.StressResistance ) );
        }
        else
        {
            this.stressResistance.SetActive( false );
        }

        if (BattleCalculationManager.GetMaxStatePointUp(_subskillData) > 1) // Max State Point Up
        {
            this.maxStatePointUp.SetActive(true);
            this.maxStatePointUpValue.SetText( BattleCalculationManager.GetMaxStatePointUp( _subskillData ).ToString() );
        }
        else
        {
            this.maxStatePointUp.SetActive(false);
        }

        if (BattleCalculationManager.GetStatePointDamage(_subskillData) > 1) // State Point Damage
        {
            this.statePointDamage.SetActive(true);
            this.statePointDamageValue.SetText( BattleCalculationManager.GetStatePointDamage( _subskillData ).ToString() );
        }
        else
        {
            this.statePointDamage.SetActive(false);
        }

        string rangeTagText = "";
        string tagEffectTypeText = "";

        if (_subskillData.Range != Subskill.RangeType.none)
        {
            rangeTagText = _subskillData.Range switch
            {
                Subskill.RangeType.melee => "【近戰】",
                 Subskill.RangeType.ranged => "【遠程】",
                Subskill.RangeType.melee_or_ranged => "【近/遠】",
                Subskill.RangeType.none => "",
                _ => throw new NotImplementedException()
            };
        }

        if (_subskillData.EffectType == Subskill.EffectTypeEnum.wide)
        {
            tagEffectTypeText = $"【{ TerminologyManager.GetWideEffectTypeText(characterSkill.GetSkillData()) }】";
        }
        this.integratedText = rangeTagText + "" + tagEffectTypeText;
        this.tagArea.text = this.integratedText;

        if (string.IsNullOrEmpty(this.tagArea.text))
        {
            this.tagArea.gameObject.SetActive(false);
        }
        else
        {
            this.tagArea.gameObject.SetActive(true);
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
        this.observedSkillBoxPrefab.Initialize( characterSkill.GetObservedSkillRecord() );
    }

    public void ShowActiveSkillInfoPanel()
    {
        this.skillInfoTabButton = SkillInfoTabButton.active;
        //ChangeSkillSelectionPanel();
        ChangeSkillSelectionPanelV2();
        ShowBackendSkillTabButton(false, false);
    }

    public void ShowBackendSkillInfoPanel()
    {
        this.skillInfoTabButton = SkillInfoTabButton.backend;
        //ChangeSkillSelectionPanel();
        ChangeSkillSelectionPanelV2();
    }

    private void ChangeSkillSelectionPanelV2()
    {
        (bool attackSkillInfo, bool repulseSkillInfo, bool derivedSkillInfo, bool counterSkillInfo) = skillInfoTabButton switch
        {
            SkillInfoTabButton.active => (true, true, true, false),
            SkillInfoTabButton.backend => (false, false, false, true),
            _ => (false, false, false, false)
        };
        this.attackSkillSelectionTabButton.gameObject.SetActive(attackSkillInfo);
        this.repulseSkillSelectionTabButton.gameObject.SetActive(repulseSkillInfo);
        this.derivedSkillSelectionTabButton.gameObject.SetActive(derivedSkillInfo);
        this.counterSkillSelectionTabButton.gameObject.SetActive(counterSkillInfo);
    }

    public void ShowActiveSkillPanelUI()
    {
        skillType = SubSkillType.none;
        UpdateSkillInfoPanel(this.selectedSkill);
        SetSkillPanelUI(this.activeSkillInfoBackground, this.yellowLevelBackground, this.yellowSkillNameBackground);
        this.derivedSkillSelectionTabButton.gameObject.SetActive(true);

        if (this.selectedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
        {
            if (this.skillSelectionBox.GetIsSelected())
            {
                this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetActiveSkillBoxSelectedBackgroundImage());
            }
        }
    }

    public void ShowRepulseSkillPanelUI()
    {
        ShowActiveSubSkillInfoPanelUI(this.selectedSkill.GetCharacterSubskillData().GetSelectedRepulseSkill(), SubSkillType.repulse, true, repulseSkillInfoBackground, blueLevelBackground, blueSkillNameBackground);
        if (this.skillSelectionBox.GetIsSelected())
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetRepulseSkillBoxFrameImage());
        }
    }

    public void ShowDerivedSkillPanelUI()
    {
        ShowActiveSubSkillInfoPanelUI(this.selectedSkill.GetCharacterSubskillData().GetSelectedDerivedSkill(), SubSkillType.derived, false, derivedSkillInfoBackground, derivedSkillLevelBackground, derivedSkillNameBackground);
        if (this.skillSelectionBox.GetIsSelected())
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetDerivedSkillBoxFrameImage());
        }
    }

    public void ShowActiveSubSkillInfoPanelUI(CharacterSkill selectedSkill, SubSkillType skillType, bool isDerivedActive, Sprite skillInfoBackground, Sprite levelBackground, Sprite skillNameBackground)
    {
        if(selectedSkill == null)
        {
            return;
        }
        this.skillType = skillType;
        this.derivedSkillSelectionTabButton.gameObject.SetActive(isDerivedActive);
        UpdateSkillInfoPanel(selectedSkill);
        SetSkillPanelUI(skillInfoBackground, levelBackground, skillNameBackground);
    }

    public void ShowDefenceSkillPanelUI()
    {
        ShowBackendSkillPanelUI(this.backendSkillInfoBackground);
        if (this.skillSelectionBox.GetIsSelected())
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetBackendSkillBoxSelectedBackgroundImage());
        }
    }

    public void ShowEvasionSkillPanelUI()
    {
        ShowBackendSkillPanelUI(this.evasionSkillInfoBackground);
        if (this.skillSelectionBox.GetIsSelected())
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetBackendSkillBoxSelectedBackgroundImage());
        }
    }

    public void ShowBackendSkillPanelUI(Sprite skillInfoBackground)
    {
        UpdateSkillInfoPanel(this.selectedSkill);
        ShowBackendSkillTabButton(false, false);
        SetSkillPanelUI(skillInfoBackground, this.blueLevelBackground, this.blueSkillNameBackground);
    }

    public void ShowCounterSkillPanelUI()
    {
        if (this.selectedSkill.GetCharacterSubskillData().GetSelectedCounterSkill() == null)
        {
            return;
        }

        this.skillType = SubSkillType.counter;
        Subskill selectedBackendSkill = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();
        UpdateSkillInfoPanel(this.selectedSkill.GetCharacterSubskillData().GetSelectedCounterSkill());
        SetSkillPanelUI(this.counterSkillInfoBackground, this.yellowLevelBackground, this.yellowSkillNameBackground);

        (bool isDefending, bool isEvading) = selectedBackendSkill switch
        {
            {IsDefendingSkill : true} => (true, false),
            {IsEvadingSkill: true } => (false, true),
            {IsObservingSkill: true } => (false, false),
            _ => throw new NotImplementedException()
        };
        ShowBackendSkillTabButton(isDefending, isEvading);

        if (this.skillSelectionBox.GetIsSelected())
        {
            this.skillSelectionBox.SetSkillBoxFrame(this.skillSelectionBox.GetSkillSelectionPanel().GetCounterSkillBoxFrameImage());
        }
    }

    public void ShowObserveSkillPanelUI()
    {
        ShowBackendSkillPanelUI(this.observeSkillInfoBackground);
    }

    public void IncreaseSkillInfoLevel()
    {
        //increase the skill level
        this.skillSelectionBox.IncreaseSkillLevel();
    }

    public void DecreaseSkillInfoLevel()
    {
        //decrease the skill level
        this.skillSelectionBox.DecreaseSkillLevel();
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

    private void SetSkillPanelUI(Sprite skillPanelBackground, Sprite skillLevelBackground, Sprite skillNameBackground)
    {
        this.skillInfoPanelBackground.sprite = skillPanelBackground;
        this.skillLevelBackground.sprite = skillLevelBackground;
        this.skillNameBackground.sprite = skillNameBackground;
    }
}
