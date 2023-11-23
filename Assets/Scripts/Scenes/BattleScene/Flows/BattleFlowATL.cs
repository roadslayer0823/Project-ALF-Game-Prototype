using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowATL
{
    private GameCharacter selectedCharacter = null;
    private GameCharacter attackTarget = null;

    private int partNumber = 0;
    private float animationDuration = 0.0f;
    private float animationStartTime = 0.0f;
    private int atlNumber = 0;
    private ATLSlot atlSlot = null;
    private ATLSlotV2 atlSlotV2 = null;
    private bool isATLSlotExecuted = false;
    private bool isSkillSlotSwipedManually = false;

    private CharacterSkill selectedSkill = null;

    public BattleFlowATL( int atlNumber, GameCharacter selectedCharacter )
    {
        this.atlNumber = atlNumber;
        this.selectedCharacter = selectedCharacter;
    }

    public void UpdateATLSlotInfo()
    {
        ClearATLSlotInfo();

        this.atlSlot.SetOwnerName(this.selectedCharacter.GetCharacterName());

        if (this.selectedSkill != null)
        {
            this.atlSlot.SetSkillName(this.selectedSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);
        }
    }

    public bool CheckIsPlayer()
    {
        return this.selectedCharacter is PlayerCharacter;
    }

    public void Finish()
    {
        this.atlSlot.MarkATLSlotInactive();
        this.atlSlot.HideSelectionHighlight();

        if (CheckIsPlayer() && !isSkillSlotSwipedManually)
        {
            // Automatically swipe the Skill Slot leftwards.
            this.atlSlot.onATLSlotExecutedCallback();
            this.atlSlot.onSkillSlotSwipedCallback();
        }
    }

    private void ClearATLSlotInfo()
    {
        this.atlSlot.SetOwnerName("NODATA");
        this.atlSlot.SetSkillName("NODATA");
    }

    public GameCharacter GetSelectedCharacter()
    {
        return this.selectedCharacter;
    }

    public void SetSelectedSkill(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public void SetAttackTarget( GameCharacter attackTarget )
    {
        this.attackTarget = attackTarget;
    }

    public GameCharacter GetAttackTarget()
    {
        return this.attackTarget;
    }

    public int GetATLNumber()
    {
        return this.atlNumber;
    }

    public void SetATLSlot( ATLSlot atlSlot )
    {
        this.atlSlot = atlSlot;
    }

    public void SetATLSlotV2(ATLSlotV2 atlSlotV2)
    {
        this.atlSlotV2 = atlSlotV2;
    }

    public ATLSlot GetATLSlot()
    {
        return this.atlSlot;
    }

    public ATLSlotV2 GetATLSlotV2()
    {
        return this.atlSlotV2;
    }

    public bool GetIsATLSlotExecuted()
    {
        return this.isATLSlotExecuted;
    }

    public void SetIsATLSlotExecuted(bool isATLSlotExecuted)
    {
        this.isATLSlotExecuted = isATLSlotExecuted;
    }

    public void SetSkillSlotSwipedManually(bool swipedManually)
    {
        this.isSkillSlotSwipedManually = swipedManually;
    }
}
