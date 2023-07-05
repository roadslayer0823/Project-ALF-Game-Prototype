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

    public void Initialize( SkillSelectionTab skillSelectionTab, CharacterSkill[] characterSkills )
    {
        skillSelectionBoxPrefabObject = skillSelectionBoxPrefab.gameObject;

        for (int i = 0; i < characterSkills.Length; i++)
        {
            GameObject _skillSelectionBoxObj = Instantiate( skillSelectionBoxPrefabObject, skillSelectionBoxContainer, false );
            _skillSelectionBoxObj.GetComponent<RectTransform>().anchoredPosition = new Vector2( 0.0f, i * itemHeight );
            _skillSelectionBoxObj.GetComponent<SkillSelectionBox>().Initialize( this, characterSkills[ i ] );
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
