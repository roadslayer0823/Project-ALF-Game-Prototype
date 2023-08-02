using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[ 0 ];

    private GameCharacter selectedGameCharacter = null;

    private bool isSkillSlotListScrollable = false;
    private Vector2 mousePressPosition = new Vector2();
    private Vector2 mouseReleasePosition = new Vector2();
    private Vector2 currentSwipe = new Vector2();

    private RectTransform rectTransform = null;
    private List<DatabaseManager.Skill> selectedSkills = null;

    private Action onSkillSlotSwipedCallback = null;

    public void Initialize(Action onSkillSlotSwipedCallback)
    {
        this.onSkillSlotSwipedCallback = onSkillSlotSwipedCallback;
    }

    private void Start()
    {
        this.rectTransform = GetComponent<RectTransform>();
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

            this.selectedSkills = new List<DatabaseManager.Skill>(gameCharacter.GetSelectedActiveSkillList());

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

    private void InsertIntoSkillSlot(List<DatabaseManager.Skill> selectedSkills)
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
        if (!RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, this.mousePressPosition) || 
            this.selectedSkills.Count == 0)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            this.mouseReleasePosition = Input.mousePosition;

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
        }
    }

    public void SwipeLeft()
    {
        DatabaseManager.Skill tempSlot;

        List<DatabaseManager.Skill> skillList = this.selectedGameCharacter.GetSelectedActiveSkillList();

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
        DatabaseManager.Skill tempSlot;

        List<DatabaseManager.Skill> skillList = this.selectedGameCharacter.GetSelectedActiveSkillList();

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
}
