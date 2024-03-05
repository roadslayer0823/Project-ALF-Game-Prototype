using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;
using SkillType = BattleSkillManager.SkillType;
using BackendSkillType = SkillSlotV2.BackendSkillType;
using StateType = SkillSlotV2.StateType;

public class BackendSkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlotV2[] backendSkillSlot = new SkillSlotV2[ 0 ];
    [SerializeField] private SkillSlotV2 qteSkillSlot = null;
    [SerializeField] private GameObject qteButton = null;

    private CharacterSkill qteSkill = null;
    private GameCharacter selectedGameCharacter = null;
    private List<CharacterSkill> selectedSkills = new List<CharacterSkill>();
    private SkillSlotV2 selectedSkillSlot = null;

    public void Initialize()
    {
        for (int i = 0; i < this.backendSkillSlot.Length; i++)
        {
            this.backendSkillSlot[i].InitializeBackendSkillSlot(this);
        }
    }

    public void Show()
    {
        base.gameObject.SetActive(true);
    }

    public void Hide()
    {
        base.gameObject.SetActive(false);
    }

    public void HideQTEButton()
    {
        this.qteButton.SetActive(false);
    }

    public void ShowQTEButton()
    {
        this.qteButton.SetActive(true);
    }

    public void OnSkillSlotSelected( SkillSlotV2 skillSlot )
    {
        if (this.selectedSkillSlot != null)
        {
            this.selectedSkillSlot.SetCurrentStateType( StateType.Enabled );
        }

        this.selectedSkillSlot = skillSlot;
    }

    public void UpdateBackendSkillSlots( GameCharacter gameCharacter, List<SkillType> skillTypeList )
    {
        for (int i = 0; i < this.backendSkillSlot.Length; i++)
        {
            SkillSlotV2 _backendSkillSlot = this.backendSkillSlot[i];

            _backendSkillSlot.SetSelectedSkill(null);
        }

        this.selectedGameCharacter = gameCharacter;
        this.selectedSkills = new List<CharacterSkill>( gameCharacter.GetSelectedBackendSkillList() );
        this.selectedSkillSlot = null;

        CharacterSkill _defenceSlotCharacterSkill = null;
        CharacterSkill _evasionSlotCharacterSkill = null;
        CharacterSkill _observationSlotCharacterSkill = null;

        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            CharacterSkill _selectedSkill = this.selectedSkills[ i ];
            Subskill _subskillData = _selectedSkill.GetCharacterSubskillData().GetSubskillData();

            for (int j = 0; j < this.backendSkillSlot.Length; j++)
            {
                SkillSlotV2 _backendSkillSlot = this.backendSkillSlot[j];

                if (_backendSkillSlot.GetSelectedSkill() == _selectedSkill)
                {
                    continue;
                }

                switch (_backendSkillSlot.backendSkillType)
                {
                    case BackendSkillType.Defense:

                        if (_subskillData.IsDefendingSkill && _defenceSlotCharacterSkill == null)
                        {
                            _backendSkillSlot.SetSelectedSkill(_selectedSkill);
                            _defenceSlotCharacterSkill = _selectedSkill;
                        }

                        break;

                    case BackendSkillType.Evasion:

                        if (_subskillData.IsEvadingSkill && _evasionSlotCharacterSkill == null)
                        {
                            _backendSkillSlot.SetSelectedSkill(_selectedSkill);
                            _evasionSlotCharacterSkill = _selectedSkill;
                        }

                        break;

                    case BackendSkillType.Generic:

                        if (_subskillData.IsObservingSkill && _observationSlotCharacterSkill == null)
                        {
                            _backendSkillSlot.SetSelectedSkill(_selectedSkill);
                            _observationSlotCharacterSkill = _selectedSkill;
                        }
                        else if (_subskillData.IsDefendingSkill && _defenceSlotCharacterSkill != null && _defenceSlotCharacterSkill != _selectedSkill)
                        {
                            _backendSkillSlot.SetSelectedSkill(_selectedSkill);
                        }
                        else if (_subskillData.IsEvadingSkill && _evasionSlotCharacterSkill != null && _evasionSlotCharacterSkill != _selectedSkill)
                        {
                            _backendSkillSlot.SetSelectedSkill(_selectedSkill);
                        }

                        break;
                }
            }
        }

        bool _isAbleToDefend = skillTypeList.Contains( SkillType.Defend );
        bool _isAbleToEvade = skillTypeList.Contains( SkillType.Evade );
        bool _isAbleToObserve = skillTypeList.Contains( SkillType.Observe );

        for (int i = 0; i < this.backendSkillSlot.Length; i++)
        {
            SkillSlotV2 _backendSkillSlot = backendSkillSlot[ i ];
            CharacterSkill _backendSkill = _backendSkillSlot.GetSelectedSkill();

            if (_backendSkill != null)
            {
                Subskill _subskillData = _backendSkill.GetCharacterSubskillData().GetSubskillData();
                backendSkillSlot[i].UpdateCurrentSkillDisplayTextUI(true);

                if (( _isAbleToDefend && _subskillData.IsDefendingSkill )
                   || ( _isAbleToEvade && _subskillData.IsEvadingSkill )
                   || ( _isAbleToObserve && _subskillData.IsObservingSkill )
                   )
                {
                    _backendSkillSlot.SetCurrentStateType( ( _backendSkill == this.selectedGameCharacter.GetCurrentSkill() ) ? StateType.Selected : StateType.Enabled );
                }
                else
                {
                    _backendSkillSlot.SetCurrentStateType( StateType.Disabled );
                }
            }
            else
            {
                backendSkillSlot[i].UpdateCurrentSkillDisplayTextUI(false);
            }
        }
    }

    public void ShowQteSkillSlot(GameCharacter gameCharacter)
    {
        this.selectedGameCharacter = gameCharacter;

        CharacterSkill _currentSkill = this.selectedGameCharacter.GetCurrentSkill();
        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            CharacterSkill _selectedSkill = this.selectedSkills[i];
            if (_selectedSkill == _currentSkill)
            {
                this.selectedSkills[i] = _currentSkill.GetCharacterSubskillData().GetSelectedCounterSkill();
            }
            SetQTEActionButton(this.selectedSkills[i]);
        }
    }

    public void SetQTEActionButton(CharacterSkill qteSkill)
    {
        this.qteSkill = qteSkill;

        if (qteSkill != null)
        {
            qteSkillSlot.SetSelectedSkill(qteSkill);
        }
    }

    public GameCharacter GetSelectedGameCharacter()
    {
        return this.selectedGameCharacter;
    }

    public SkillSlotV2 GetSkillSlot( CharacterSkill skill )
    {
        for (int i = 0; i < this.backendSkillSlot.Length; i++)
        {
            SkillSlotV2 _skillSlot = this.backendSkillSlot[ i ];
            if (_skillSlot.GetSelectedSkill() == skill)
            {
                return _skillSlot;
            }
        }

        return null;
    }
}
