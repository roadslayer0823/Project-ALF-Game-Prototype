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

    private List<SkillSelectionBox> repulseSkillSelectionBoxList = null;
    private List<SkillSelectionBox> derivedSkillSelectionBoxList = null;
    private CharacterSkill lastSelectedCharacterSkill = null;
    private bool isLastSelectedSkillChanged = false;

    public void Initialize( SkillSelectionTab skillSelectionTab )
    {
        this.skillSelectionTab = skillSelectionTab;
        this.skillSelectionBoxPrefabObject = this.skillSelectionBoxPrefab.gameObject;
    }

    // Show main list for the active and backend character skill 
    public void ShowCharacterSkillList(CharacterSkill[] characterSkills)
    {
        this.containerContent.gameObject.SetActive(true);

        SetupCharacterSkillSelectionBoxList(characterSkills);

        for (int i = 0; i < this.skillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.skillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(true);
        }

        HideRepulseSkillList();
        HideDerivedSkillList();
    }

    // Hide main list for the active and backend character skill 
    private void HideCharacterSkillList()
    {
        if (this.skillSelectionBoxList == null)
        {
            return;
        }

        for (int i = 0; i < this.skillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.skillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(false);
        }
    }

    // Show the repulse skill list based on the last selected skill
    public void ShowRepulseSkillList()
    {
        if (this.lastSelectedCharacterSkill == null)
        {
            this.containerContent.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.containerContent.gameObject.SetActive(true);
        }

        SetupRepulseSkillSelectionBoxList();

        for (int i = 0; i < this.repulseSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.repulseSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(true);
        }

        HideCharacterSkillList();
        HideDerivedSkillList();
    }

    // Hide the repulse skill list
    private void HideRepulseSkillList()
    {
        if (this.repulseSkillSelectionBoxList == null)
        {
            return;
        }

        for (int i = 0; i < this.repulseSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.repulseSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(false);
        }
    }

    // Show the derived skill list based on the last selected skill
    public void ShowDerivedSkillList()
    {
        if (this.lastSelectedCharacterSkill == null)
        {
            this.containerContent.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.containerContent.gameObject.SetActive(true);
        }

        SetupDerivedSkillSelectionBoxList();

        for (int i = 0; i < this.derivedSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.derivedSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(true);
        }

        HideCharacterSkillList();
        HideRepulseSkillList();
    }

    // Hide the derived skill list
    private void HideDerivedSkillList()
    {
        if (this.derivedSkillSelectionBoxList == null)
        {
            return;
        }

        for (int i = 0; i < this.derivedSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.derivedSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(false);
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
        skillSelectionTab.ShowSelectedSkillInfo(skillSelectionBox.GetCharacterSkill());

        if (this.lastSelectedCharacterSkill == null && this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.attack)
        {
            this.lastSelectedCharacterSkill = skillSelectionBox.GetCharacterSkill();
        }
        else
        {
            // To make sure the new selected character skill is only in attack tab.
            if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.attack)
            {
                this.isLastSelectedSkillChanged = true;
                this.lastSelectedCharacterSkill = skillSelectionBox.GetCharacterSkill();
            }
        }
    }

    private void SetupCharacterSkillSelectionBoxList(CharacterSkill[] characterSkills)
    {
        if (this.skillSelectionBoxList == null)
        {
            this.skillSelectionBoxList = new List<SkillSelectionBox>();

            // Initialize the SkillSelectionBox so that the skill can be display on it respectively. 
            for (int i = 0; i < characterSkills.Length; i++)
            {
                GameObject skillSelectionBoxObj = Instantiate(this.skillSelectionBoxPrefabObject, this.containerContent, false);

                SkillSelectionBox skillSelectionBox = skillSelectionBoxObj.GetComponent<SkillSelectionBox>();
                skillSelectionBox.Initialize(this, characterSkills[i]);

                this.skillSelectionBoxList.Add(skillSelectionBox);
            }
        }
    }

    private void SetupRepulseSkillSelectionBoxList()
    {
        if (this.repulseSkillSelectionBoxList == null || this.isLastSelectedSkillChanged)
        {
            this.repulseSkillSelectionBoxList = new List<SkillSelectionBox>();

            // TODO: Replace with array of repulse skill, use for loop
            CharacterSkill _repulseSkill = this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetRepulseSkill();

            if (_repulseSkill != null)
            {
                GameObject skillSelectionBoxObj = Instantiate(this.skillSelectionBoxPrefabObject, this.containerContent, false);

                SkillSelectionBox skillSelectionBox = skillSelectionBoxObj.GetComponent<SkillSelectionBox>();
                skillSelectionBox.Initialize(this, _repulseSkill);

                this.repulseSkillSelectionBoxList.Add(skillSelectionBox);

                this.isLastSelectedSkillChanged = false;
            }
        }
    }

    private void SetupDerivedSkillSelectionBoxList()
    {
        if (this.derivedSkillSelectionBoxList == null || this.isLastSelectedSkillChanged)
        {
            this.derivedSkillSelectionBoxList = new List<SkillSelectionBox>();

            // TODO: Replace with array of derived skill, use for loop
            CharacterSkill _derivedSkill = this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetDerivedSkill();

            if (_derivedSkill != null)
            {
                GameObject skillSelectionBoxObj = Instantiate(this.skillSelectionBoxPrefabObject, this.containerContent, false);

                SkillSelectionBox skillSelectionBox = skillSelectionBoxObj.GetComponent<SkillSelectionBox>();
                skillSelectionBox.Initialize(this, _derivedSkill);

                this.derivedSkillSelectionBoxList.Add(skillSelectionBox);

                this.isLastSelectedSkillChanged = false;
            }
        }
    }

    public CharacterSkill GetLastSelectedCharacterSkill()
    {
        return this.lastSelectedCharacterSkill;
    }
}
