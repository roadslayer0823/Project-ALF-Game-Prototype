using UnityEngine;
using TMPro;

public class GameCharacterInfoBox : MonoBehaviour
{
    [SerializeField] private GameCharacter selectedCharacter = null;
    [SerializeField] private SpriteRenderer healthPointFiller = null;
    [SerializeField] private TextMeshPro healthPointLabel = null;
    [SerializeField] private SpriteRenderer statePointFiller = null;
    [SerializeField] private TextMeshPro statePointLabel = null;
    [SerializeField] private SpriteRenderer stressValueFiller = null;
    [SerializeField] private TextMeshPro stressValueLabel = null;
    [SerializeField] private TextMeshPro currentSkillInfoLabel = null;

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

        UpdateBar( this.selectedCharacter.GetCurrentHealthPoint(), this.selectedCharacter.GetMaximumHealthPoint(), this.healthPointFiller, this.healthPointLabel );
        UpdateBar( this.selectedCharacter.GetCurrentStatePoint(), this.selectedCharacter.GetMaximumStatePoint(), this.statePointFiller, this.statePointLabel );
        UpdateBar( this.selectedCharacter.GetCurrentStressValue(), this.selectedCharacter.GetMaximumStressValue(), this.stressValueFiller, this.stressValueLabel );

        string _skillInfoString = "";
        CharacterSkill _characterSkill = this.selectedCharacter.GetCurrentSkill();
        if (_characterSkill != null)
        {
            CharacterSubskill _characterSubskillData = _characterSkill.GetCharacterSubskillData();
            if (_characterSubskillData != null)
            {
                DatabaseManager.Subskill _subskillData = _characterSubskillData.GetSubskillData();
                _skillInfoString = _subskillData.DisplayName;
                _skillInfoString += ( _subskillData.Strength > 1 ) ? "\n強度 +" + ( _subskillData.Strength - 1 ) : "";
                _skillInfoString += ( _subskillData.Accuracy > 1 ) ? "\n命中 +" + ( _subskillData.Accuracy - 1 ) : "";
                _skillInfoString += ( _subskillData.Evasion > 1 ) ? "\n迴避 +" + ( _subskillData.Evasion - 1 ) : "";
                _skillInfoString += ( _subskillData.EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide ) ? "\n廣角" : "";
            }
        }

        currentSkillInfoLabel.SetText( _skillInfoString );
    }

    private void UpdateBar( float currentValue, float maximumValue, SpriteRenderer filler, TextMeshPro label )
    {
        float _currentValue = currentValue;

        if (_currentValue < 0)
        {
            _currentValue = 0;
        }

        label.SetText( Mathf.CeilToInt( _currentValue ) + " / " + Mathf.CeilToInt( maximumValue ) );

        float _percentage = currentValue / maximumValue;
        filler.size = new Vector2( ( _percentage > 0 ) ? _percentage * 10.0f : 0.0f, filler.size.y );
    }
}
