using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] private SkillSelectionTab activeSkillSelectionTab = null;
    [SerializeField] private SkillSelectionTab backendSkillSelectionTab = null;

    private Action<SkillSelectionBox> onSkillSelectedCallback = null;
    private Action<SkillSelectionBox> onSkillDeselectedCallback = null;

    public void Initialize( CharacterSkill[] characterSkills, Action<SkillSelectionBox> onSkillSelectedCallback, Action<SkillSelectionBox> onSkillDeselectedCallback )
    {
        this.onSkillSelectedCallback = onSkillSelectedCallback;
        this.onSkillDeselectedCallback = onSkillDeselectedCallback;

        List<CharacterSkill> activeSkillList = new List<CharacterSkill>();
        List<CharacterSkill> backendSkillList = new List<CharacterSkill>();

        for (int i = 0; i < characterSkills.Length; i++)
        {
            CharacterSkill characterSkill = characterSkills[ i ];
            SkillDatabase.SkillData characterSkillData = characterSkill.GetSkillData();

            if (characterSkillData.GetSkillType() == SkillDatabase.SkillData.SkillType.Active)
            {
                activeSkillList.Add( characterSkill );
            }
            else if (characterSkillData.GetSkillType() == SkillDatabase.SkillData.SkillType.Backend)
            {
                backendSkillList.Add( characterSkill );
            }

            this.activeSkillSelectionTab.Initialize( this, activeSkillList.ToArray() );
            this.backendSkillSelectionTab.Initialize( this, backendSkillList.ToArray() );
        }
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        if (this.onSkillSelectedCallback != null)
        {
            this.onSkillSelectedCallback( skillSelectionBox );
        }
        else
        {
            Debug.Log( "The value for 'onSkillSelectedCallback' is not assigned." );
        }
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        if (this.onSkillDeselectedCallback != null)
        {
            this.onSkillDeselectedCallback( skillSelectionBox );
        }
        else
        {
            Debug.Log( "The value for 'onSkillDeselectedCallback' is not assigned." );
        }
    }
}
