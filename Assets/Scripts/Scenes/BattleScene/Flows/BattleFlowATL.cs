using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowATL
{
    private GameCharacter selectedCharacter = null;
    private CharacterSkill selectedSkill = null;
    private GameCharacter attackTarget = null;

    private int partNumber = 0;
    private float animationDuration = 0.0f;
    private float animationStartTime = 0.0f;

    public BattleFlowATL( GameCharacter selectedCharacter )
    {
        this.selectedCharacter = selectedCharacter;
    }

    public GameCharacter GetSelectedCharacter()
    {
        return this.selectedCharacter;
    }

    public void SetSelectedSkill( CharacterSkill selectedSkill )
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
}
