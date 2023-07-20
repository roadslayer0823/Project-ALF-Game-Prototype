using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[ 0 ];

    private GameCharacter selectedGameCharacter;

    private Vector2 mousePressPosition;
    private Vector2 mouseReleasePosition;
    private Vector2 currentSwipe;

    private RectTransform rectTransform;
    List<CharacterSkill> selectedSkills = new List<CharacterSkill>();

    public void Show( GameCharacter gameCharacter = null )
    {
        if (gameCharacter != null)
        {
            this.selectedGameCharacter = gameCharacter;

            this.selectedSkills = gameCharacter.GetSelectedActiveSkillList();

            if (this.selectedSkills.Count > skillSlots.Length)
            {
                return;
            }

            InsertIntoSkillSlot( this.selectedSkills );
        }

        this.gameObject.SetActive( true );
    }

    public void Hide()
    {
        this.gameObject.SetActive( false );
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Swipe();
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
            mousePressPosition = Input.mousePosition;
        }

        // if the click is outside the define range
        if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePressPosition) || 
            this.selectedSkills.Count == 0)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            mouseReleasePosition = Input.mousePosition;

            //create vector from the two points
            currentSwipe = new Vector2(mouseReleasePosition.x - mousePressPosition.x, mouseReleasePosition.y - mousePressPosition.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe left
            if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                SwipeLeft();
            }
            //swipe right
            if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
            {
                SwipeRight();
            }
        }
    }

    private void SwipeLeft()
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
}
