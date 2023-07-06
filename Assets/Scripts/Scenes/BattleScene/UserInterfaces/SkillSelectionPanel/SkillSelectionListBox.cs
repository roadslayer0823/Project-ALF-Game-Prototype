using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionListBox : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float itemHeight = 100.0f;

    [Header( "References" )]
    [SerializeField] private RectTransform skillSelectionBoxContainer = null;
    [SerializeField] private SkillSelectionBox skillSelectionBoxPrefab = null;

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
        if (this.skillSelectionBoxList != null)
        {
            for (int i = 0; i < skillSelectionBoxList.Count; i++)
            {
                Destroy( skillSelectionBoxList[ i ].gameObject );
            }
        }

        this.skillSelectionBoxList = new List<SkillSelectionBox>();

        for (int i = 0; i < characterSkills.Length; i++)
        {
            GameObject _skillSelectionBoxObj = Instantiate( this.skillSelectionBoxPrefabObject, this.skillSelectionBoxContainer, false );
            _skillSelectionBoxObj.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0.0f, i * this.itemHeight );

            SkillSelectionBox skillSelectionBox = _skillSelectionBoxObj.GetComponent<SkillSelectionBox>();
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
}
