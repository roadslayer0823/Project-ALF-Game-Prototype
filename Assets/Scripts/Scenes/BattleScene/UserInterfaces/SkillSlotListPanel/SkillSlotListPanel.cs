using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[ 0 ];
    [SerializeField] private RectTransform scrollableArea = null;

    private GameCharacter selectedGameCharacter = null;

    private bool isSkillSlotListScrollable = false;
    private Vector2 mousePressPosition = new Vector2();
    private Vector2 mouseReleasePosition = new Vector2();
    private Vector2 currentSwipe = new Vector2();

    private List<CharacterSkill> selectedSkills = null;

    private Action onSkillSlotSwipedCallback = null;

    private const string AUDIO_ID_SCROLL = "scroll";

    public void Initialize(Action onSkillSlotSwipedCallback)
    {
        this.onSkillSlotSwipedCallback = onSkillSlotSwipedCallback;
    }

    private void Update()
    {
        if (this.isSkillSlotListScrollable)
        {
            Swipe();
        }
    }

    public void Show( GameCharacter gameCharacter = null )
    {
        if (gameCharacter != null)
        {
            this.selectedGameCharacter = gameCharacter;

            this.selectedSkills = new List<CharacterSkill>(gameCharacter.GetSelectedActiveSkillList());

            if (this.selectedSkills.Count > skillSlots.Length)
            {
                return;
            }

            InsertIntoSkillSlot( this.selectedSkills );
        }

        base.gameObject.SetActive( true );
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }

    private void InsertIntoSkillSlot(List<CharacterSkill> selectedSkills)
    {
        ClearSkillSlots();

        for (int i = 0; i < selectedSkills.Count; i++)
        {
            skillSlots[i].SetSelectedSkill(selectedSkills[i]);
        }
    }

    private void ClearSkillSlots()
    {
        foreach (SkillSlot slot in skillSlots)
        {
            slot.SetSkillSlotText("");
        }
    }

    private void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            this.mousePressPosition = Input.mousePosition;
        }

        // if the click is outside the define range
        if (!RectTransformUtility.RectangleContainsScreenPoint(this.scrollableArea, this.mousePressPosition) || 
            this.selectedSkills.Count <= 1)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            this.mouseReleasePosition = Input.mousePosition;

            if (!RectTransformUtility.RectangleContainsScreenPoint(this.scrollableArea, this.mouseReleasePosition))
            {
                return;
            }

            //create vector from the two points
            this.currentSwipe = new Vector2(this.mouseReleasePosition.x - this.mousePressPosition.x, this.mouseReleasePosition.y - this.mousePressPosition.y);

            //normalize the 2d vector
            this.currentSwipe.Normalize();

            //swipe left
            if (this.currentSwipe.x < 0 && this.currentSwipe.y > -0.5f && this.currentSwipe.y < 0.5f)
            {
                SwipeLeft();
            }
            //swipe right
            if (this.currentSwipe.x > 0 && this.currentSwipe.y > -0.5f && this.currentSwipe.y < 0.5f)
            {
                SwipeRight();
            }

            this.onSkillSlotSwipedCallback();

            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SCROLL);
        }
    }

    public void SwipeLeft()
    {
        CharacterSkill tempSlot;

        List<CharacterSkill> skillList = this.selectedGameCharacter.GetSelectedActiveSkillList();

        int i = 0;
        tempSlot = skillList[i];

        while (i < skillList.Count - 1)
        {
            skillList[i] = skillList[i + 1];
            i++;
        }

        skillList[skillList.Count - 1] = tempSlot;

        InsertIntoSkillSlot(skillList);
    }

    private void SwipeRight()
    {
        CharacterSkill tempSlot;

        List<CharacterSkill> skillList = this.selectedGameCharacter.GetSelectedActiveSkillList();

        int i = skillList.Count - 1;
        tempSlot = skillList[i];

        while (i > 0)
        {
            skillList[i] = skillList[i - 1];
            i--;
        }

        skillList[0] = tempSlot;

        InsertIntoSkillSlot(skillList);
    }

    public void SetIsSkillSlotListScrollable( bool isSkillSlotListScrollable )
    {
        this.isSkillSlotListScrollable = isSkillSlotListScrollable;
    }

    // To reset back the selected skill sequence based on last round selection. 
    public void ResetLastRoundSelectedActiveSkill()
    {
        this.selectedGameCharacter.SetSelectedActiveSkillList(this.selectedSkills);

        InsertIntoSkillSlot(this.selectedSkills);
    }
}
