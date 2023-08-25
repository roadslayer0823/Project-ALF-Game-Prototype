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
    private List<SkillSelectionBox> counterSkillSelectionBoxList = null;
    private CharacterSkill lastSelectedCharacterSkill = null;
    private CharacterSkill lastSelectedRepulseSkill = null;
    private CharacterSkill lastSelectedDerivedSkill = null;
    private CharacterSkill lastSelectedCounterSkill = null;
    private bool isLastSelectedCharacterSkillChanged = false;

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
        HideCounterSkillList();
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
        HideCounterSkillList();
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
        HideCounterSkillList();
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

    // Show the counter skill list based on the last selected skill
    public void ShowCounterSkillList()
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

        SetupCounterSkillSelectionBoxList();

        for (int i = 0; i < this.counterSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.counterSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(true);
        }

        HideCharacterSkillList();
        HideRepulseSkillList();
        HideDerivedSkillList();
        //XXXX
    }

    // Hide the counter skill list
    private void HideCounterSkillList()
    {
        if (this.counterSkillSelectionBoxList == null)
        {
            return;
        }

        for (int i = 0; i < this.counterSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.counterSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(false);
        }
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionTab.OnSkillSelected( skillSelectionBox );
        ShowSelectedSkillInfo(skillSelectionBox);
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionTab.OnSkillDeselected( skillSelectionBox );
        ShowSelectedSkillInfo(skillSelectionBox);
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
                this.isLastSelectedCharacterSkillChanged = true;
                this.lastSelectedCharacterSkill = skillSelectionBox.GetCharacterSkill();
                this.lastSelectedRepulseSkill = null;
                this.lastSelectedDerivedSkill = null;
            }
            else if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.repulse)
            {
                this.lastSelectedRepulseSkill = skillSelectionBox.GetCharacterSkill();
            }
            else if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.derived)
            {
                this.lastSelectedDerivedSkill = skillSelectionBox.GetCharacterSkill();
            }
            else if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.counter)
            {
                this.lastSelectedCounterSkill = skillSelectionBox.GetCharacterSkill();
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
        if (this.repulseSkillSelectionBoxList == null || this.isLastSelectedCharacterSkillChanged)
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

                this.isLastSelectedCharacterSkillChanged = false;
            }
        }
    }

    private void SetupDerivedSkillSelectionBoxList()
    {
        if (this.derivedSkillSelectionBoxList == null || this.isLastSelectedCharacterSkillChanged)
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

                this.isLastSelectedCharacterSkillChanged = false;
            }
        }
    }

    private void SetupCounterSkillSelectionBoxList()
    {
        if (this.counterSkillSelectionBoxList == null || this.isLastSelectedCharacterSkillChanged)
        {
            this.counterSkillSelectionBoxList = new List<SkillSelectionBox>();

            // TODO: Replace with array of counter skill, use for loop
            CharacterSkill _counterSkill = this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetCounterSkill();

            if (_counterSkill != null)
            {
                GameObject skillSelectionBoxObj = Instantiate(this.skillSelectionBoxPrefabObject, this.containerContent, false);

                SkillSelectionBox skillSelectionBox = skillSelectionBoxObj.GetComponent<SkillSelectionBox>();
                skillSelectionBox.Initialize(this, _counterSkill);

                this.counterSkillSelectionBoxList.Add(skillSelectionBox);

                this.isLastSelectedCharacterSkillChanged = false;
            }
        }
    }

    public CharacterSkill GetLastSelectedCharacterSkill()
    {
        return this.lastSelectedCharacterSkill;
    }

    public CharacterSkill GetLastSelectedRepulseSkill()
    {
        return this.lastSelectedRepulseSkill;
    }

    public CharacterSkill GetLastSelectedDerivedSkill()
    {
        return this.lastSelectedDerivedSkill;
    }

    public CharacterSkill GetLastSelectedCounterSkill()
    {
        return this.lastSelectedCounterSkill;
    }
}
