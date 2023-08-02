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
    private ATLSlot atlSlot = null;
    private bool isATLSlotExecuted = false;

    private DatabaseManager.Skill selectedSkill = null;

    public BattleFlowATL( GameCharacter selectedCharacter )
    {
        this.selectedCharacter = selectedCharacter;
    }

    public void UpdateATLSlotInfo()
    {
        ClearATLSlotInfo();

        this.atlSlot.SetOwnerName(CheckIsPlayer() ? "Player" : "Enemy");

        if (this.selectedSkill != null)
        {
            this.atlSlot.SetSkillName(this.selectedSkill.GetDisplayName());
        }
    }

    public bool CheckIsPlayer()
    {
        return this.selectedCharacter is PlayerCharacter;
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

    public void SetSelectedSkill(DatabaseManager.Skill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
    }

    public DatabaseManager.Skill GetSelectedSkill()
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

    public ATLSlot GetATLSlot()
    {
        return atlSlot;
    }

    public void SetATLSlot(ATLSlot atlSlot)
    {
        this.atlSlot = atlSlot;
    }

    public bool GetIsATLSlotExecuted()
    {
        return this.isATLSlotExecuted;
    }

    public void SetIsATLSlotExecuted(bool isATLSlotExecuted)
    {
        this.isATLSlotExecuted = isATLSlotExecuted;
    }
}
