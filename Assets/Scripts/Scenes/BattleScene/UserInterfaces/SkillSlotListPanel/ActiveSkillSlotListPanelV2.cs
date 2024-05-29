using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ActiveSkillSlotListPanelV2 : MonoBehaviour
{
    [SerializeField] private GameObject clickAreaTop;
    [SerializeField] private GameObject clickAreaBottom;
    [SerializeField] private GameObject bottomTopContainer = null;
    [SerializeField] private GameObject middleContainer = null;
    [SerializeField] private SkillSlotV2[] skillSlots = new SkillSlotV2[0];
    [SerializeField] private List<SkillSlotV2> currentSkillSlotPosition;
    [SerializeField] private List<Button> skillSlotsButton = null;
    [SerializeField] private List<GameObject> skillSlotList;
    [SerializeField] private List<Transform> fixedSlotPosition;
    [SerializeField] private List<GameObject> skillInformation;

    private Vector3 initialScale = new Vector3(1f, 1f, 1f);
    private GameCharacter selectedGameCharacter = null;
    private List<CharacterSkill> selectedSkills = new List<CharacterSkill>();
    private const string AUDIO_ID_WHEEL = "wheel";
    private SkillSlotV2 middleSkillSlot = null;
    public bool isAnimationRunning = true;

    private Action<SkillSlotV2,bool> onSkillSlotSelectedCallback = null;

    public void Initialize( Action<SkillSlotV2,bool> onSkillSlotSelectedCallback )
    {
        this.onSkillSlotSelectedCallback = onSkillSlotSelectedCallback;

        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            this.skillSlots[i].Initialize(this);
        }

        SetActiveRecursively(this.skillInformation[1].transform, false);
        SetActiveRecursively(this.skillInformation[2].transform, false);

        this.middleSkillSlot = this.skillSlots[ 0 ];
        this.middleSkillSlot.SetCurrentIsMiddleSlot(true);
        ArrangeSkillSlot(1);
    }

    public void Show()
    {
        base.gameObject.SetActive(true);
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].isActivated = true;
        }
    }

    public void Hide()
    {
        base.gameObject.SetActive(false);
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].isActivated = false;
        }
    }

    public void EnableInteraction()
    {
        this.clickAreaTop.SetActive(true);
        this.clickAreaBottom.SetActive(true);
    }

    public void DisableInteraction()
    {
        this.clickAreaTop.SetActive(false);
        this.clickAreaBottom.SetActive(false);
    }

    public void OnSkillSlotSelected( SkillSlotV2 skillSlot, bool isSelectingSkill )
    {
        this.onSkillSlotSelectedCallback?.Invoke( skillSlot, isSelectingSkill );
    }

    private void SetActiveRecursively(Transform parentTransform, bool active)
    {
        parentTransform.gameObject.SetActive(active);
        foreach (Transform child in parentTransform)
        {
            SetActiveRecursively(child, active);
        }
    }

    public void ChangeToDefaultMode( GameCharacter gameCharacter, SkillSlotV2.StateType stateType )
    {
        this.selectedGameCharacter = gameCharacter;

        int _middleSkillSlotSkillIndex = ( this.selectedSkills.Count == 2 ) ? GetMiddleSkillSlotSkillIndex() : 0;
        this.selectedSkills = new List<CharacterSkill>( this.selectedGameCharacter.GetSelectedActiveSkillList() );
        UpdateSkillSlotsWithSelectedSkills( _middleSkillSlotSkillIndex, stateType );
    }

    public void ChangeToRepulseMode( GameCharacter gameCharacter )
    {
        this.selectedGameCharacter = gameCharacter;

        int _middleSkillSlotSkillIndex = ( this.selectedSkills.Count == 2 ) ? GetMiddleSkillSlotSkillIndex() : 0;
        this.selectedSkills.Clear();

        List<CharacterSkill> _activeSkillList = this.selectedGameCharacter.GetSelectedActiveSkillList();
        for (int i = 0; i < _activeSkillList.Count; i++)
        {
            this.selectedSkills.Add( _activeSkillList[ i ].GetCharacterSubskillData().GetSelectedRepulseSkill() );
        }

        UpdateSkillSlotsWithSelectedSkills( _middleSkillSlotSkillIndex );
    }

    public void ChangeToDerivedMode( GameCharacter gameCharacter, bool hasActiveSkillType )
    {
        this.selectedGameCharacter = gameCharacter;

        CharacterSkill _currentSkill = this.selectedGameCharacter.GetCurrentSkill();
        string _currentSkillSubskillId = _currentSkill.GetCharacterSubskillData().GetSubskillData().Id;
        CharacterSkill _derivedSkill = null;

        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            if (this.selectedSkills[ i ].GetCharacterSubskillData().GetSubskillData().Id == _currentSkillSubskillId)
            {
                _derivedSkill = _currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();
                this.selectedSkills[ i ] = _derivedSkill;
                break;
            }
        }

        UpdateSkillSlotsWithSelectedSkills( stateType: ( ( hasActiveSkillType ) ? SkillSlotV2.StateType.Enabled : SkillSlotV2.StateType.Disabled ) );

        if (_derivedSkill != null && !hasActiveSkillType && BattleLogicManagerV2.IsAbleToUseAttackingAndDefendingSkills( this.selectedGameCharacter ))
        {
            List<SkillSlotV2> _skillSlots = GetSkillSlots( _derivedSkill );
            for (int i = 0; i < _skillSlots.Count; i++)
            {
                SkillSlotV2 _skillSlot = _skillSlots[ i ];
                if (_skillSlot.GetCurrentStateType() != SkillSlotV2.StateType.Enabled)
                {
                    _skillSlot.SetCurrentStateType( SkillSlotV2.StateType.Enabled );
                }
            }
        }
    }

    private int GetMiddleSkillSlotSkillIndex()
    {
        CharacterSkill _middleSkillSlotSelectedSkill = this.middleSkillSlot.GetSelectedSkill();
        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            if (_middleSkillSlotSelectedSkill == this.selectedSkills[ i ])
            {
                return i;
            }
        }

        return -1;
    }

    private void UpdateSkillSlotsWithSelectedSkills( int middleSkillSlotSkillIndex = 0, SkillSlotV2.StateType stateType = SkillSlotV2.StateType.Enabled )
    {
        OnSkillSlotSelected( null, false );
        ClearSkillSlots();

        SkillSlotV2.StateType _stateType = stateType;

        if (!BattleLogicManagerV2.IsAbleToUseAttackingAndDefendingSkills( this.selectedGameCharacter ))
        {
            _stateType = SkillSlotV2.StateType.Disabled;
        }

        if (_stateType == SkillSlotV2.StateType.Enabled)
        {
            for (int i = 0; i < this.selectedSkills.Count; i++)
            {
                this.selectedSkills[ i ].ResetSelectedSkillLevelToPreset();
            }
        }

        if (this.selectedSkills.Count == 2 && this.skillSlots.Length > 2)
        {
            int _middleSkillIndex = ( middleSkillSlotSkillIndex < 0 ) ? 0 : middleSkillSlotSkillIndex;
            int _otherSkillIndex = -1;

            for (int i = 0; i < this.selectedSkills.Count; i++)
            {
                if (i != _middleSkillIndex)
                {
                    _otherSkillIndex = i;
                    break;
                }
            }

            for (int i = 0; i < this.skillSlots.Length; i++)
            {
                SkillSlotV2 _skillSlot = this.skillSlots[ i ];
                _skillSlot.SetSelectedSkill( this.selectedSkills[ ( _skillSlot == this.middleSkillSlot ) ? _middleSkillIndex : _otherSkillIndex ] );
            }
        }
        else
        {
            for (int i = 0; i < this.selectedSkills.Count; i++)
            {
                this.skillSlots[ i ].SetSelectedSkill( this.selectedSkills[ i ] );
            }
        }

        if (this.selectedSkills.Count == 1)
        {
            DisableInteraction();
        }
        else
        {
            EnableInteraction();
        }

        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            this.skillSlots[ i ].SetCurrentStateType( _stateType );
        }
    }

    public void ClearSkillSlots()
    {
        foreach (SkillSlotV2 slot in this.skillSlots)
        {
            slot.Clear();
        }
    }

    public SkillSlotV2[] GetSkillSlots()
    {
        return this.skillSlots;
    }

    public List<SkillSlotV2> GetSkillSlots( CharacterSkill skill )
    {
        List<SkillSlotV2> _matchedSkillSlotList = new();

        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            SkillSlotV2 _skillSlot = this.skillSlots[ i ];
            if (_skillSlot.GetSelectedSkill() == skill)
            {
                _matchedSkillSlotList.Add( _skillSlot );
            }
        }

        return _matchedSkillSlotList;
    }

    public SkillSlotV2 GetSelectedSkillSlot()
    {
        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            SkillSlotV2 _skillSlot = this.skillSlots[ i ];
            if (_skillSlot.GetCurrentStateType() == SkillSlotV2.StateType.Selected)
            {
                return _skillSlot;
            }
        }

        return null;
    }

    public void ResetLastRoundSelectedActiveSkill()
    {
        this.selectedGameCharacter.SetSelectedActiveSkillList( this.selectedSkills );
        UpdateSkillSlotsWithSelectedSkills();
    }

    public void ClickBottom()
    {
        if(isAnimationRunning == true)
        {
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
            MoveSlot(-1);
            this.isAnimationRunning = false;
        }
    }

    public void ClickTop()
    {
        if(isAnimationRunning == true)
        {
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
            MoveSlot(1);
            this.isAnimationRunning = false;
        }
    }

    private void MoveSlot(int direction)
    {
        if (this.selectedSkills.Count == 2)
        {
            for (int i = 0; i < this.skillSlots.Length; i++)
            {
                if (this.skillSlots[i] == this.middleSkillSlot)
                {
                    int _index = i + direction;
                    if (_index >= this.skillSlots.Length)
                    {
                        _index = 0;
                    }
                    else if (_index < 0)
                    {
                        _index = this.skillSlots.Length - 1;
                    }
                    SkillSlotV2 _slot = this.skillSlots[_index];
                    _slot.SetSelectedSkill(this.middleSkillSlot.GetSelectedSkill());
                    break;
                }
            }

            OnSkillSlotSelected( GetSelectedSkillSlot(), false );
        }

        if(this.isAnimationRunning == true)
        {
            for (int i = 0; i < this.skillSlotList.Count; i++)
            {
                int newIndex = (i + direction + this.skillSlotList.Count) % this.skillSlotList.Count; // Calculate the new index

                GameObject slotToMove = this.skillSlotList[i];
                SkillSlotV2 currentSlot = this.skillSlots[i];
                Vector3 targetPosition = this.fixedSlotPosition[newIndex].position;

                if (newIndex == 1)
                {
                    LeanTween.scale(slotToMove, this.initialScale * 1f, 0.1f).setEase(LeanTweenType.easeInOutQuad);
                    this.skillSlotsButton[i].interactable = true;
                    this.skillSlotList[i].transform.SetParent(this.middleContainer.transform, false);
                    SetActiveRecursively(this.skillInformation[i].transform, true);
                    currentSlot.UpdateCharacterSkillLevel(currentSlot.skillLevel);
                    this.middleSkillSlot = skillSlotList[i].GetComponent<SkillSlotV2>();
                    this.currentSkillSlotPosition[i].SetCurrentIsMiddleSlot(true);
                }
                else
                {
                    LeanTween.scale(slotToMove, this.initialScale * 0.5f, 0.1f).setEase(LeanTweenType.easeInOutQuad);
                    this.skillSlotsButton[i].interactable = false;
                    this.skillSlotList[i].transform.SetParent(this.bottomTopContainer.transform, false);
                    SetActiveRecursively(this.skillInformation[i].transform, false);
                    this.currentSkillSlotPosition[i].SetCurrentIsMiddleSlot(false);
                }

                // Use LeanTween to move the slot to the new position
                LeanTween.move(slotToMove, targetPosition, 0.5f).setOnComplete(() =>
                {
                    this.isAnimationRunning = true;
                }).setEase(LeanTweenType.easeInOutQuad);
            }
        }
        ArrangeSkillSlot(direction);
    }

    //1 = moving slot to down direction, -1 = moving slot to up direction
    private void ArrangeSkillSlot(int direction)
    {
        if (direction == 1)
        {
            int i = this.skillSlotList.Count - 1;
            Button tempButton = this.skillSlotsButton[i];
            GameObject tempSlot = this.skillSlotList[i];
            GameObject tempSkillInformation = this.skillInformation[i];
            SkillSlotV2 tempPosition = this.currentSkillSlotPosition[i];
            while (i > 0)
            {
                this.skillSlotList[i] = this.skillSlotList[i - 1];
                this.skillInformation[i] = this.skillInformation[i - 1];
                this.skillSlotsButton[i] = this.skillSlotsButton[i - 1];
                this.currentSkillSlotPosition[i] = this.currentSkillSlotPosition[i - 1];
                i--;
            }

            this.skillSlotList[i] = tempSlot;
            this.skillSlotsButton[i] = tempButton;
            this.currentSkillSlotPosition[i] = tempPosition;
            this.skillInformation[i] = tempSkillInformation;
        }
        else if (direction == -1)
        {
            int i = 0;
            GameObject tempSlot = this.skillSlotList[i];
            Button tempButton = this.skillSlotsButton[i];
            GameObject tempSkillInformation = this.skillInformation[i];
            SkillSlotV2 tempPosition = this.currentSkillSlotPosition[i];

            while (i < this.skillSlotList.Count - 1)
            {
                this.skillSlotsButton[i] = this.skillSlotsButton[i + 1];
                this.skillSlotList[i] = this.skillSlotList[i + 1];
                this.skillInformation[i] = this.skillInformation[i + 1];
                this.currentSkillSlotPosition[i] = this.currentSkillSlotPosition[i + 1];
                i++;
            }
            this.skillSlotsButton[this.skillSlotsButton.Count - 1] = tempButton;
            this.skillSlotList[this.skillSlotList.Count - 1] = tempSlot;
            this.skillInformation[this.skillInformation.Count - 1] = tempSkillInformation;
            this.currentSkillSlotPosition[this.currentSkillSlotPosition.Count - 1] = tempPosition;
        }
    }

    public GameCharacter GetSelectedGameCharacter()
    {
        return this.selectedGameCharacter;
    }

    public SkillSlotV2 GetMiddleSkillSlot()
    {
        return this.middleSkillSlot;
    }
}
