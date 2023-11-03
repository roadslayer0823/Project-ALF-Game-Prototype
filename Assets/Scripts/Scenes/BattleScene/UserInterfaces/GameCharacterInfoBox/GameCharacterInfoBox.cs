using UnityEngine;
using TMPro;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class GameCharacterInfoBox : MonoBehaviour
{
    [SerializeField] private GameCharacter selectedCharacter = null;
    [SerializeField] private SpriteRenderer healthPointFiller = null;
    [SerializeField] private SpriteRenderer virtualHealthPointFiller = null;
    [SerializeField] private TextMeshPro healthPointLabel = null;
    [SerializeField] private SpriteRenderer statePointFiller = null;
    [SerializeField] private TextMeshPro statePointLabel = null;
    [SerializeField] private SpriteRenderer stressValueFiller = null;
    [SerializeField] private TextMeshPro stressValueLabel = null;
    [SerializeField] private TextMeshPro currentSkillInfoLabel = null;
    [SerializeField] private GameObject breakStatusIndicator = null;
    [SerializeField] private GameObject breakStatusForStatePoint = null;
    [SerializeField] private GameObject breakStatusForStressValue = null;
    [SerializeField] private TextMeshPro energyMarkerLabel = null;

    void Awake()
    {
        this.selectedCharacter.SetGameCharacterInfoBox( this );
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
    }

    public void ShowDisplayInfo()
    {
        this.gameObject.SetActive( true );
    }

    public void HideDisplayInfo()
    {
        this.gameObject.SetActive( false );
    }

    public void UpdateDisplayInfo()
    {
        if (BattleLogicManager.IsGameCharacterDead( selectedCharacter ))
        {
            HideDisplayInfo();
            return;
        }

        UpdateBar( this.selectedCharacter.GetCurrentHealthPoint(), this.selectedCharacter.GetMaximumHealthPoint(), this.healthPointFiller, this.healthPointLabel, false );
        UpdateBar( this.selectedCharacter.GetVirtualHealthPoint(), this.selectedCharacter.GetMaximumHealthPoint(), this.virtualHealthPointFiller, null, false );
        UpdateBar( this.selectedCharacter.GetCurrentStatePoint(), this.selectedCharacter.GetMaximumStatePoint(), this.statePointFiller, this.statePointLabel, true );
        UpdateBar( this.selectedCharacter.GetCurrentStressValue(), this.selectedCharacter.GetMaximumStressValue(), this.stressValueFiller, this.stressValueLabel, false );

        this.breakStatusIndicator.SetActive( this.selectedCharacter.GetIsInBreakStatus() );
        this.breakStatusForStatePoint.SetActive( this.selectedCharacter.GetIsBreakStatusCausedByStatePoint() );
        this.breakStatusForStressValue.SetActive( this.selectedCharacter.GetIsBreakStatusCausedByStressValue() );

        if (this.selectedCharacter.HasEnergyMarker())
        {
            this.energyMarkerLabel.SetText( $"能量殘響ATL：{ this.selectedCharacter.GetEnergyMarkerRemainingATLs() }" );
            this.energyMarkerLabel.gameObject.SetActive( true );
        }
        else
        {
            this.energyMarkerLabel.gameObject.SetActive( false );
        }

        string _skillInfoString = "";

        if (!this.selectedCharacter.GetIsInBreakStatus())
        {
            CharacterSkill _characterSkill = this.selectedCharacter.GetCurrentSkill();
            if (_characterSkill != null)
            {
                if (this.selectedCharacter.GetIsAbleToUseSkill())
                {
                    CharacterSubskill _characterSubskillData = _characterSkill.GetCharacterSubskillData();
                    if (_characterSubskillData != null)
                    {
                        Subskill _subskillData = _characterSubskillData.GetSubskillData();
                        int _skillStatIncrement = selectedCharacter.GetCurrentSkillStatIncrement();

                        _skillInfoString = "<size=120%><color=#FFFF00><b>" + _subskillData.DisplayName + "</b></color></size>";

                        if (_subskillData.EffectType == Subskill.EffectTypeEnum.wide)
                        {
                            _skillInfoString += "\n";

                            Skill _skillData = _characterSkill.GetSkillData();
                            if (_skillData.skillType == Skill.SkillType.repulse
                                || _skillData.skillType == Skill.SkillType.backend)
                            {
                                _skillInfoString += "對";
                            }

                            _skillInfoString += "廣角";
                        }

                        int _speed = _subskillData.Speed + _skillStatIncrement;
                        string _speedLevelText = TerminologyManager.GetSpeedLevelText( _speed );
                        if (_speed > 1)
                        {
                            _skillInfoString += "\n";

                            if (_skillStatIncrement > 0)
                            {
                                _skillInfoString += "<color=#FFFF00>" + _speedLevelText + "</color>";
                            }
                            else
                            {
                                _skillInfoString += _speedLevelText;
                            }
                        }

                        _skillInfoString += ( _subskillData.Strength > 1 ) ? "\n強度 +" + ( _subskillData.Strength - 1 ) : "";

                        if (_skillStatIncrement > 0 && _subskillData.Strength > 0)
                        {
                            if (_subskillData.Strength == 1)
                            {
                                _skillInfoString += "\n強度";
                            }

                            _skillInfoString += " <color=#00FF00>+" + _skillStatIncrement + "</color>";
                        }

                        _skillInfoString += ( _subskillData.Accuracy > 1 ) ? "\n命中 +" + ( _subskillData.Accuracy - 1 ) : "";

                        if (_skillStatIncrement > 0 && _subskillData.Accuracy > 0)
                        {
                            if (_subskillData.Accuracy == 1)
                            {
                                _skillInfoString += "\n命中";
                            }

                            _skillInfoString += " <color=#00FF00>+" + _skillStatIncrement + "</color>";
                        }

                        _skillInfoString += ( _subskillData.Evasion > 1 ) ? "\n迴避 +" + ( _subskillData.Evasion - 1 ) : "";

                        if (_skillStatIncrement > 0 && _subskillData.Evasion > 0)
                        {
                            if (_subskillData.Evasion == 1)
                            {
                                _skillInfoString += "\n迴避";
                            }

                            _skillInfoString += " <color=#00FF00>+" + _skillStatIncrement + "</color>";
                        }
                    }
                }
                else
                {
                    _skillInfoString += "<size=120%><color=#FF0000>無法使用技能</color></size>";
                }
            }
        }

        this.currentSkillInfoLabel.SetText( _skillInfoString );
    }

    private void UpdateBar( float currentValue, float maximumValue, SpriteRenderer filler, TextMeshPro label, bool isNegativeValueAllowed )
    {
        float _currentValue = currentValue;

        if (!isNegativeValueAllowed)
        {
            if (_currentValue < 0)
            {
                _currentValue = 0;
            }
        }

        if (label != null)
        {
            label.SetText(Mathf.CeilToInt(_currentValue) + " / " + Mathf.CeilToInt(maximumValue));
        }

        float _percentage = currentValue / maximumValue;
        filler.size = new Vector2( ( _percentage > 0 ) ? _percentage * 10.0f : 0.0f, filler.size.y );
    }
}
