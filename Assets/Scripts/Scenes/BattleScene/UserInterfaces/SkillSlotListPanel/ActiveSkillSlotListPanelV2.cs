using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ActiveSkillSlotListPanelV2 : MonoBehaviour
{
    [SerializeField] private SkillSlotV2[] skillSlots = new SkillSlotV2[0];
    [SerializeField] private GameObject clickAreaTop;
    [SerializeField] private GameObject clickAreaBottom;
    [SerializeField] private List<Button> skillSlotsButton = null;
    [SerializeField] private List<GameObject> skillSlotList;
    [SerializeField] private List<Transform> fixedSlotPosition;
    [SerializeField] private List<GameObject> skillInformation;

    private Vector3 initialScale = new Vector3(1f, 1f, 1f);
    private GameCharacter selectedGameCharacter = null;
    private List<CharacterSkill> selectedSkills = new List<CharacterSkill>();
    private const string AUDIO_ID_WHEEL = "wheel";
    private SkillSlotV2 middleSkillSlot = null;

    private Action<SkillSlotV2> onSkillSlotSelectedCallback = null;

    public void Initialize( Action<SkillSlotV2> onSkillSlotSelectedCallback )
    {
        this.onSkillSlotSelectedCallback = onSkillSlotSelectedCallback;

        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            this.skillSlots[i].Initialize(this);
        }

        SetActiveRecursively(skillInformation[1].transform, false);
        SetActiveRecursively(skillInformation[2].transform, false);

        this.middleSkillSlot = this.skillSlots[ 0 ];
        ArrangeSkillSlot(1);
    }

    public void Show()
    {
        base.gameObject.SetActive(true);
    }

    public void Hide()
    {
        base.gameObject.SetActive(false);
    }

    public void EnableInteraction()
    {
        clickAreaTop.SetActive(true);
        clickAreaBottom.SetActive(true);
    }

    public void DisableInteraction()
    {
        clickAreaTop.SetActive(false);
        clickAreaBottom.SetActive(false);
    }

    public void OnSkillSlotSelected( SkillSlotV2 skillSlot )
    {
        this.onSkillSlotSelectedCallback?.Invoke( skillSlot );
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

    public void ChangeToDerivedMode( GameCharacter gameCharacter )
    {
        this.selectedGameCharacter = gameCharacter;

        CharacterSkill _currentSkill = this.selectedGameCharacter.GetCurrentSkill();
        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            CharacterSkill _selectedSkill = this.selectedSkills[ i ];
            if (_selectedSkill == _currentSkill)
            {
                this.selectedSkills[ i ] = _currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();
            }
        }

        UpdateSkillSlotsWithSelectedSkills();
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
        OnSkillSlotSelected( null );
        ClearSkillSlots();

        if (this.selectedSkills.Count == 2 && this.skillSlots.Length > 2)
        {
            int _middleSkillIndex = (middleSkillSlotSkillIndex < 0) ? 0 : middleSkillSlotSkillIndex;
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
                SkillSlotV2 _skillSlot = this.skillSlots[i];
                if (_skillSlot == this.middleSkillSlot)
                {
                    _skillSlot.SetSelectedSkill(this.selectedSkills[_middleSkillIndex]);
                }
                else
                {
                    _skillSlot.SetSelectedSkill(this.selectedSkills[_otherSkillIndex]);
                }
            }
        }
        if (this.selectedSkills.Count == 1)
        {
            LockSkillSlot();
        }
        else
        {
            UnlockSkillSlot();
        }

        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            this.skillSlots[i].SetSelectedSkill(this.selectedSkills[i]);
        }

        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            this.skillSlots[i].SetCurrentStateType(stateType);
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

    public SkillSlotV2 GetSkillSlot( CharacterSkill skill )
    {
        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            SkillSlotV2 _skillSlot = this.skillSlots[ i ];
            if (_skillSlot.GetSelectedSkill() == skill)
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
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
        MoveSlot(-1);
    }

    public void ClickTop()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
        MoveSlot(1);
    }

    public void LockSkillSlot()
    {
        this.clickAreaTop.SetActive(false);
        this.clickAreaBottom.SetActive(false);
    }

    public void UnlockSkillSlot()
    {
        this.clickAreaBottom.SetActive(true);
        this.clickAreaTop.SetActive(true);
    }

    private void MoveSlot(int direction)
    {
        if (this.selectedSkills.Count == 2)
        {
            for (int i = 0; i < this.skillSlots.Length; i++)
            {
                if (this.skillSlots[i] == middleSkillSlot)
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
                    _slot.SetSelectedSkill(middleSkillSlot.GetSelectedSkill());
                    break;
                }
            }
        }

        for (int i = 0; i < skillSlotList.Count; i++)
        {
            int newIndex = (i + direction + skillSlotList.Count) % skillSlotList.Count; // Calculate the new index

            GameObject slotToMove = skillSlotList[i];
            SkillSlotV2 currentSlot = skillSlots[i];
            Vector3 targetPosition = fixedSlotPosition[newIndex].position;

            // Use LeanTween to move the slot to the new position
            LeanTween.move(slotToMove, targetPosition, 0.5f)
                .setEase(LeanTweenType.easeInOutQuad);

            if (newIndex == 1)
            {
                LeanTween.scale(slotToMove, initialScale * 1f, 0.1f)
                .setEase(LeanTweenType.easeInOutQuad);
                skillSlotsButton[i].interactable = true;
                SetActiveRecursively(skillInformation[i].transform, true);
                currentSlot.UpdateCharacterSkillLevel(currentSlot.skillLevel);
                middleSkillSlot = skillSlotList[i].GetComponent<SkillSlotV2>();
            }
            else
            {
                LeanTween.scale(slotToMove, initialScale * 0.5f, 0.1f)
                .setEase(LeanTweenType.easeInOutQuad);
                skillSlotsButton[i].interactable = false;
                SetActiveRecursively(skillInformation[i].transform, false);
            }
        }
        ArrangeSkillSlot(direction);
    }

    //1 = moving slot to down direction, -1 = moving slot to up direction
    private void ArrangeSkillSlot(int direction)
    {
        if (direction == 1)
        {
            int i = skillSlotList.Count - 1;
            GameObject tempSlot = skillSlotList[i];
            GameObject tempSkillInformation = skillInformation[i];
            Button tempButton = skillSlotsButton[i];
            while (i > 0)
            {
                skillSlotList[i] = skillSlotList[i - 1];
                skillSlotsButton[i] = skillSlotsButton[i - 1];
                skillInformation[i] = skillInformation[i - 1];
                i--;
            }

            skillSlotList[i] = tempSlot;
            skillSlotsButton[i] = tempButton;
            skillInformation[i] = tempSkillInformation;
        }
        else if (direction == -1)
        {
            int i = 0;
            GameObject tempSlot = skillSlotList[i];
            GameObject tempSkillInformation = skillInformation[i];
            Button tempButton = skillSlotsButton[i];
            while (i < skillSlotList.Count - 1)
            {
                skillSlotList[i] = skillSlotList[i + 1];
                skillSlotsButton[i] = skillSlotsButton[i + 1];
                skillInformation[i] = skillInformation[i + 1];
                i++;
            }

            skillSlotList[skillSlotList.Count - 1] = tempSlot;
            skillSlotsButton[skillSlotsButton.Count - 1] = tempButton;
            skillInformation[skillInformation.Count - 1] = tempSkillInformation;
        }
    }

    public GameCharacter GetSelectedGameCharacter()
    {
        return this.selectedGameCharacter;
    }
}
