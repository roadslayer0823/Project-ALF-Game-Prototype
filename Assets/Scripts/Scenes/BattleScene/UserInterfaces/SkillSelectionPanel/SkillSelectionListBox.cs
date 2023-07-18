using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionListBox : MonoBehaviour
{
    [Header( "References" )]
    [SerializeField] private RectTransform skillSelectionBoxContainer = null;
    [SerializeField] private SkillSelectionBox skillSelectionBoxPrefab = null;
    [SerializeField] private RectTransform containerContent = null;

    private SkillSelectionTab skillSelectionTab = null;
    private List<SkillSelectionBox> skillSelectionBoxList = null;
    private GameObject skillSelectionBoxPrefabObject = null;

    public void Initialize( SkillSelectionTab skillSelectionTab )
    {
        this.skillSelectionTab = skillSelectionTab;
        this.skillSelectionBoxPrefabObject = this.skillSelectionBoxPrefab.gameObject;
    }

    public void Show( CharacterSkill[] characterSkills )
    {
        // Check and clear the "skillSelectionBoxList" if not empty to make sure the list is empty
        if (this.skillSelectionBoxList != null)
        {
            for (int i = 0; i < skillSelectionBoxList.Count; i++)
            {
                Destroy( skillSelectionBoxList[ i ].gameObject );
            }
        }

        this.skillSelectionBoxList = new List<SkillSelectionBox>();

        // Initialize the SkillSelectionBox so that the skill can be display on it respectively. 
        for (int i = 0; i < characterSkills.Length; i++)
        {
            GameObject skillSelectionBoxObj = Instantiate( this.skillSelectionBoxPrefabObject, this.containerContent, false );

            SkillSelectionBox skillSelectionBox = skillSelectionBoxObj.GetComponent<SkillSelectionBox>();
            skillSelectionBox.Initialize( this, characterSkills[ i ] );

            skillSelectionBoxList.Add( skillSelectionBox );
        }
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionTab.OnSkillSelected( skillSelectionBox );
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionTab.OnSkillDeselected( skillSelectionBox );
    }

    public void ShowSelectedSkillInfo(SkillSelectionBox skillSelectionBox)
    {
        skillSelectionTab.ShowSelectedSkillInfo(skillSelectionBox);
    }
}
