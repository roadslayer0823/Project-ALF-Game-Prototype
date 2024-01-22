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

    private List<Vector3> initialScale = new List<Vector3>();
    private GameCharacter selectedGameCharacter = null;
    private List<CharacterSkill> selectedSkills = new List<CharacterSkill>();
    private const string AUDIO_ID_WHEEL = "wheel";

    public void Initialize()
    {
        for (int i = 0; i < this.skillSlots.Length; i++)
        {
            this.skillSlots[ i ].Initialize( this );
        }
        MoveSlot(-1);
        MoveSlot(1);
    }

    private void GetLocalScale()
    {
        foreach (GameObject slot in skillSlotList)
        {
            Vector3 scale = slot.transform.localScale;
            initialScale.Add(scale);
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
        for (int i = 0; i < gameCharacter.GetSelectedActiveSkillList().Count; i++)
        {
            CharacterSkill skillList = gameCharacter.GetSelectedActiveSkillList()[i];

            for (int j = 0; j < skillList.GetCharacterSubskillData().GetRepulseSkillList().Count; j++)
            {
                this.selectedSkills.Add(skillList.GetCharacterSubskillData().GetRepulseSkillList()[j]);
            }
        }
        InsertIntoSkillSlot(this.selectedSkills);
    }

    public void ChangeToDefaultMode(GameCharacter gameCharacter)
    {
        this.selectedSkills.Clear();
        this.selectedSkills = new List<CharacterSkill>(gameCharacter.GetSelectedActiveSkillList());
        if (this.selectedSkills.Count > skillSlots.Length)
        {
            return;
        }
        InsertIntoSkillSlot(this.selectedSkills);
    }

    public void ChangeToDerivedMode(GameCharacter gameCharacter)
    {
        CharacterSkill skillList = gameCharacter.GetCurrentSkill();
        this.selectedSkills = skillList.GetCharacterSubskillData().GetDerivedSkillList();
        InsertIntoSkillSlot(this.selectedSkills);
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
    }

    private void ClearSkillSlots()
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
        Debug.Log("go down");
    }

    public void ClickTop()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_WHEEL);
        MoveSlot(1);
        Debug.Log("go up");
    }

    private void MoveSlot(int direction)
    {
        GetLocalScale();
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
                LeanTween.scale(slotToMove, initialScale[i] * 1f, 0.3f)
                .setEase(LeanTweenType.easeInOutQuad);
                skillSlotsButton[i].interactable = true;
                SetActiveRecursively(skillInformation[i].transform, true);
            }
            else
            {
                LeanTween.scale(slotToMove, initialScale[i] * 0.5f, 0.3f)
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
