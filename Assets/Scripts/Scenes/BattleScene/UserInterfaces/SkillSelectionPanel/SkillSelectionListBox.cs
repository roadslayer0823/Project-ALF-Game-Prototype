using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillSelectionListBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI skillListTitle;
    [SerializeField] private RectTransform skillSelectionBoxContainer = null;
    [SerializeField] private SkillSelectionBox skillSelectionBoxPrefab = null;
    [SerializeField] private RectTransform containerContent = null;
    [SerializeField] private TextMeshProUGUI emptyListText = null;

    private SkillSelectionTab skillSelectionTab = null;
    private List<SkillSelectionBox> skillSelectionBoxList = null;

    private List<SkillSelectionBox> repulseSkillSelectionBoxList = null;
    private List<SkillSelectionBox> derivedSkillSelectionBoxList = null;
    private List<SkillSelectionBox> counterSkillSelectionBoxList = null;
    private CharacterSkill lastSelectedCharacterSkill = null;
    private CharacterSkill lastSelectedRepulseSkill = null;
    private CharacterSkill lastSelectedDerivedSkill = null;
    private CharacterSkill lastSelectedCounterSkill = null;
    private bool isLastSelectedCharacterSkillChanged = false;

    private SkillSelectionBox lastSelectedCharacterSkillBox = null;
    private SkillSelectionBox lastSelectedRepulseSkillBox = null;
    private SkillSelectionBox lastSelectedDerivedSkillBox = null;
    private SkillSelectionBox lastSelectedCounterSkillBox = null;

    public void Initialize( SkillSelectionTab skillSelectionTab )
    {
        this.skillSelectionTab = skillSelectionTab;
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

        this.emptyListText.gameObject.SetActive(false);
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

        EmptyListHandler(this.repulseSkillSelectionBoxList);

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

        EmptyListHandler(this.derivedSkillSelectionBoxList);

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

        EmptyListHandler(this.counterSkillSelectionBoxList);

        for (int i = 0; i < this.counterSkillSelectionBoxList.Count; i++)
        {
            SkillSelectionBox _skillSelectionBox = this.counterSkillSelectionBoxList[i];
            _skillSelectionBox.gameObject.SetActive(true);
        }

        HideCharacterSkillList();
        HideRepulseSkillList();
        HideDerivedSkillList();
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
        HandleSelectedSkillInfoHighlight(skillSelectionBox);

        this.skillSelectionTab.ShowSelectedSkillInfo(skillSelectionBox.GetCharacterSkill());

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

    // Decide whether hide or show the selected highlight
    public void HandleSelectedSkillInfoHighlight(SkillSelectionBox skillSelectionBox)
    {
        // To make sure the new selected character skill is only in attack tab.
        if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.attack)
        {
            if (this.lastSelectedCharacterSkillBox == null)
            {
                this.lastSelectedCharacterSkillBox = skillSelectionBox;
            }

            if (this.lastSelectedCharacterSkillBox != skillSelectionBox)
            {
                this.lastSelectedCharacterSkillBox.HideSelectionHighlight();
                this.lastSelectedCharacterSkillBox = skillSelectionBox;
            }
        }
        else if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.repulse)
        {
            if (this.lastSelectedRepulseSkillBox == null)
            {
                this.lastSelectedRepulseSkillBox = skillSelectionBox;
            }

            if (this.lastSelectedRepulseSkillBox != skillSelectionBox)
            {
                this.lastSelectedRepulseSkillBox.HideSelectionHighlight();
                this.lastSelectedRepulseSkillBox = skillSelectionBox;
            }
        }
        else if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.derived)
        {
            if (this.lastSelectedDerivedSkillBox == null)
            {
                this.lastSelectedDerivedSkillBox = skillSelectionBox;
            }

            if (this.lastSelectedDerivedSkillBox != skillSelectionBox)
            {
                this.lastSelectedDerivedSkillBox.HideSelectionHighlight();
                this.lastSelectedDerivedSkillBox = skillSelectionBox;
            }
        }
        else if (this.skillSelectionTab.GetSkillInfoTab() == SkillSelectionPanel.SkillInfoTab.counter)
        {
            if (this.lastSelectedCounterSkillBox == null)
            {
                this.lastSelectedCounterSkillBox = skillSelectionBox;
            }

            if (this.lastSelectedCounterSkillBox != skillSelectionBox)
            {
                this.lastSelectedCounterSkillBox.HideSelectionHighlight();
                this.lastSelectedCounterSkillBox = skillSelectionBox;
            }
        }

        skillSelectionBox.ShowSelectionHighlight();
    }

    private void SetupCharacterSkillSelectionBoxList(CharacterSkill[] characterSkills)
    {
        if (this.skillSelectionBoxList == null)
        {
            this.skillSelectionBoxList = new List<SkillSelectionBox>();

            // Initialize the SkillSelectionBox so that the skill can be display on it respectively. 
            for (int i = 0; i < characterSkills.Length; i++)
            {
                CharacterSkill _characterSkill = characterSkills[i];

                if (_characterSkill.GetMinimumSkillLevel() != 0)
                {
                    SkillSelectionBox skillSelectionBox = Instantiate(this.skillSelectionBoxPrefab, this.containerContent, false);

                    skillSelectionBox.Initialize(this, _characterSkill);

                    this.skillSelectionBoxList.Add(skillSelectionBox);
                }
            }

            this.isLastSelectedCharacterSkillChanged = false;
        }
    }

    private void SetupRepulseSkillSelectionBoxList()
    {
        // Destroy the previous instantiate repulseSkillSelectionBox game object
        DestroyInstantiatedSkillSelectionBox(ref this.repulseSkillSelectionBoxList);

        if (this.repulseSkillSelectionBoxList == null || this.isLastSelectedCharacterSkillChanged)
        {
            this.repulseSkillSelectionBoxList = new List<SkillSelectionBox>();

            List<CharacterSkill> _repulseSkillList = this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetRepulseSkillList();

            if (_repulseSkillList != null)
            {
                for (int i = 0; i < _repulseSkillList.Count; i++)
                {
                    CharacterSkill _repulseSkill = _repulseSkillList[i];
                    SkillSelectionBox _skillSelectionBox = null;

                    if (_repulseSkill.GetMinimumSkillLevel() != 0)
                    {
                        _skillSelectionBox = Instantiate(this.skillSelectionBoxPrefab, this.containerContent, false);

                        _skillSelectionBox.Initialize(this, _repulseSkill);

                        this.repulseSkillSelectionBoxList.Add(_skillSelectionBox);
                    }

                    if (_skillSelectionBox == null)
                    {
                        return;
                    }
                    
                    if (this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetSelectedRepulseSkill() == null)
                    {
                        this.lastSelectedCharacterSkill.GetCharacterSubskillData().SetSelectedRepulseSkill(_repulseSkill);
                        _skillSelectionBox.SetSkillSelectionText("ON");
                        _skillSelectionBox.MarkSelected();
                        _skillSelectionBox.transform.SetAsFirstSibling();
                    }
                    else if (this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetSelectedRepulseSkill().GetCharacterSubskillData().GetSubskillData().Id == _repulseSkill.GetCharacterSubskillData().GetSubskillData().Id)
                    {
                        _skillSelectionBox.SetSkillSelectionText("ON");
                        _skillSelectionBox.MarkSelected();
                        _skillSelectionBox.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        _skillSelectionBox.SetSkillSelectionText("");
                    }
                }
            }
        }
    }

    private void SetupDerivedSkillSelectionBoxList()
    {
        // Destroy the previous instantiate derivedSkillSelectionBox game object
        DestroyInstantiatedSkillSelectionBox(ref this.derivedSkillSelectionBoxList);

        if (this.derivedSkillSelectionBoxList == null || this.isLastSelectedCharacterSkillChanged)
        {
            this.derivedSkillSelectionBoxList = new List<SkillSelectionBox>();

            List<CharacterSkill> _derivedSkillList = this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetDerivedSkillList();

            if (_derivedSkillList != null)
            {
                for (int i = 0; i < _derivedSkillList.Count; i++)
                {
                    CharacterSkill _derivedSkill = _derivedSkillList[i];
                    SkillSelectionBox _skillSelectionBox = null;

                    if (_derivedSkill.GetMinimumSkillLevel() != 0)
                    {
                        _skillSelectionBox = Instantiate(this.skillSelectionBoxPrefab, this.containerContent, false);

                        _skillSelectionBox.Initialize(this, _derivedSkill);

                        this.derivedSkillSelectionBoxList.Add(_skillSelectionBox);
                    }

                    if (_skillSelectionBox == null)
                    {
                        return;
                    }

                    if (this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() == null)
                    {
                        this.lastSelectedCharacterSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_derivedSkill);
                        _skillSelectionBox.SetSkillSelectionText("ON");
                        _skillSelectionBox.MarkSelected();
                        _skillSelectionBox.transform.SetAsFirstSibling();
                    }
                    else if (this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetSelectedDerivedSkill()?.GetCharacterSubskillData().GetSubskillData().Id == _derivedSkill.GetCharacterSubskillData().GetSubskillData().Id)
                    {
                        _skillSelectionBox.SetSkillSelectionText("ON");
                        _skillSelectionBox.MarkSelected();
                        _skillSelectionBox.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        _skillSelectionBox.SetSkillSelectionText("");
                    }
                }
            }
        }
    }

    private void SetupCounterSkillSelectionBoxList()
    {
        // Destroy the previous instantiate counterSkillSelectionBox game object
        DestroyInstantiatedSkillSelectionBox(ref this.counterSkillSelectionBoxList);

        if (this.counterSkillSelectionBoxList == null || this.isLastSelectedCharacterSkillChanged)
        {
            this.counterSkillSelectionBoxList = new List<SkillSelectionBox>();

            List<CharacterSkill> _counterSkillList = this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetCounterSkillList();

            if (_counterSkillList != null)
            {
                for (int i = 0; i < _counterSkillList.Count; i++)
                {
                    CharacterSkill _counterSkill = _counterSkillList[i];
                    SkillSelectionBox _skillSelectionBox = null;

                    if (_counterSkill.GetMinimumSkillLevel() != 0)
                    {
                        _skillSelectionBox = Instantiate(this.skillSelectionBoxPrefab, this.containerContent, false);

                        _skillSelectionBox.Initialize(this, _counterSkill);

                        this.counterSkillSelectionBoxList.Add(_skillSelectionBox);
                    }

                    if (_skillSelectionBox == null)
                    {
                        return;
                    }

                    if (this.lastSelectedCharacterSkill.GetCharacterSubskillData().GetSelectedCounterSkill()?.GetCharacterSubskillData().GetSubskillData().Id == _counterSkill.GetCharacterSubskillData().GetSubskillData().Id)
                    {
                        _skillSelectionBox.SetSkillSelectionText("ON");
                        _skillSelectionBox.MarkSelected();
                        _skillSelectionBox.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        _skillSelectionBox.SetSkillSelectionText("");
                    }
                }
            }
        }
    }

    private void DestroyInstantiatedSkillSelectionBox(ref List<SkillSelectionBox> skillSelectionBoxList)
    {
        if (this.isLastSelectedCharacterSkillChanged && skillSelectionBoxList != null)
        {
            for (int i = 0; i < skillSelectionBoxList.Count; i++)
            {
                SkillSelectionBox _skillBox = skillSelectionBoxList[i];
                Destroy(_skillBox.gameObject);
            }

            skillSelectionBoxList = null;
        }
    }

    private void EmptyListHandler(List<SkillSelectionBox> boxListToCheck)
    {
        if (boxListToCheck.Count == 0)
        {
            this.emptyListText.SetText("暫無技能");
            this.emptyListText.gameObject.SetActive(true);
        }
        else
        {
            this.emptyListText.gameObject.SetActive(false);
        }
    }

    public void SetSkillListTitle(string skillListTitle)
    {
        this.skillListTitle.SetText(skillListTitle);
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

    public List<SkillSelectionBox> GetRepulseSkillSelectionBoxList()
    {
        return this.repulseSkillSelectionBoxList;
    }

    public List<SkillSelectionBox> GetDerivedSkillSelectionBoxList()
    {
        return this.derivedSkillSelectionBoxList;
    }

    public List<SkillSelectionBox> GetCounterSkillSelectionBoxList()
    {
        return this.counterSkillSelectionBoxList;
    }

    public List<SkillSelectionBox> GetSkillSelectionBoxList()
    {
        return this.skillSelectionBoxList;
    }
}
