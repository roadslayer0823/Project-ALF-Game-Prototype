using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionListBox : MonoBehaviour
{
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
