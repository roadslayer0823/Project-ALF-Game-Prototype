using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SkillSlotListPanelV2 : MonoBehaviour
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

    public void Initialize()
    {
        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            this.skillSlots[i].Initialize(this);
        }
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

    private void SetActiveRecursively(Transform parentTransform, bool active)
    {
        parentTransform.gameObject.SetActive(active);
        foreach (Transform child in parentTransform)
        {
            SetActiveRecursively(child, active);
        }
    }

    public void UpdateSkillSlots(GameCharacter gameCharacter)
    {
        if (gameCharacter != null)
        {
            this.selectedGameCharacter = gameCharacter;

            this.selectedSkills = new List<CharacterSkill>(gameCharacter.GetSelectedActiveSkillList());

            if (this.selectedSkills.Count > skillSlots.Length)
            {
                return;
            }
            InsertIntoSkillSlot(this.selectedSkills);
        }
    }

    public void ChangeToRepulseMode(GameCharacter gameCharacter)
    {
        this.selectedSkills.Clear();
        List<CharacterSkill> _activeSkillList = gameCharacter.GetSelectedActiveSkillList();
        for (int i = 0; i < _activeSkillList.Count; i++)
        {
            this.selectedSkills.Add(_activeSkillList[i].GetCharacterSubskillData().GetSelectedRepulseSkill());
        }
        InsertIntoSkillSlot(this.selectedSkills);
    }

    public void ChangeToDefaultMode( GameCharacter gameCharacter )
    {
        this.selectedSkills = new List<CharacterSkill>( gameCharacter.GetSelectedActiveSkillList() );
        InsertIntoSkillSlot( this.selectedSkills );
    }

    public void ChangeToDerivedMode( GameCharacter gameCharacter )
    {
        CharacterSkill _currentSkill = gameCharacter.GetCurrentSkill();
        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            CharacterSkill _selectedSkill = this.selectedSkills[ i ];
            if (_selectedSkill == _currentSkill)
            {
                this.selectedSkills[ i ] = _currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();
                skillSlots[i].ShowSkillFrame(this.selectedSkills[i]);
            }
        }
        InsertIntoSkillSlot( this.selectedSkills );
    }

    private void InsertIntoSkillSlot(List<CharacterSkill> selectedSkills)
    {
        for (int i = 0; i < selectedSkills.Count; i++)
        {
            if (i >= skillSlots.Length)
            {
                return;
            }
            skillSlots[i].SetSelectedSkill(selectedSkills[i]);
        }

        if (this.selectedSkills.Count == 2 && this.skillSlots.Length > 2)
        {
            this.skillSlots[2].SetSelectedSkill(this.selectedSkills[1]);
        }

        if (this.selectedSkills.Count == 1 && this.skillSlots.Length > 2)
        {
            clickAreaTop.SetActive(false);
            clickAreaBottom.SetActive(false);
        }
        else
        {
            clickAreaBottom.SetActive(true);
            clickAreaTop.SetActive(true);
        }
    }

    public void ClearSkillSlots()
    {
        foreach (SkillSlotV2 slot in skillSlots)
        {
            slot.Clear();
        }
    }

    public SkillSlotV2[] GetSkillSlots()
    {
        return this.skillSlots;
    }

    public void ResetLastRoundSelectedActiveSkill()
    {
        this.selectedGameCharacter.SetSelectedActiveSkillList(this.selectedSkills);
        InsertIntoSkillSlot(this.selectedSkills);
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
                middleSkillSlot = skillSlotList[i].GetComponent<SkillSlotV2>();
            }
            else
            {
                LeanTween.scale(slotToMove, initialScale * 0.5f, 0.1f)
                .setEase(LeanTweenType.easeInOutQuad);
                skillSlotsButton[i].interactable = false;
                //SetActiveRecursively(skillInformation[i].transform, false);
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
