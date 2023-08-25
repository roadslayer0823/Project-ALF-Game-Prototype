using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] private SkillSelectionTab activeSkillSelectionPanel = null;
    [SerializeField] private SkillSelectionTab backendSkillSelectionPanel = null;

    [Header("SkillTabButtons")]
    [SerializeField] private Button attackSkillSelectionTabButton = null;
    [SerializeField] private Button repulseSkillSelectionTabButton = null;
    [SerializeField] private Button derivedSkillSelectionTabButton = null;
    [SerializeField] private Button counterSkillSelectionTabButton = null;
    [SerializeField] private Image attackSkillSelectionTabBackgroundImage = null;
    [SerializeField] private Image repulseSkillSelectionTabBackgroundImage = null;
    [SerializeField] private Image derivedSkillSelectionTabBackgroundImage = null;
    [SerializeField] private Image counterSkillSelectionTabBackgroundImage = null;

    [Header("SkillTabInteractionColor")]
    [SerializeField] private Color selectedTabColor = new Color();
    [SerializeField] private Color unselectedTabColor = new Color();

    private Action<SkillSelectionBox> onSkillSelectedCallback = null;
    private Action<SkillSelectionBox> onSkillDeselectedCallback = null;

    private GameCharacter selectedGameCharacter = null;
    private List<SkillSelectionBox> selectedActiveSkillList = new List<SkillSelectionBox>();
    private List<SkillSelectionBox> selectedBackendSkillList = new List<SkillSelectionBox>();

    private SkillInfoTab skillInfoTab = SkillInfoTab.none;
    private SkillSelectionUIPanel skillSelectionPanel = SkillSelectionUIPanel.none;
    private List<CharacterSkill> activeSkillList = new List<CharacterSkill>();
    private List<CharacterSkill> backendSkillList = new List<CharacterSkill>();

    public enum SkillInfoTab
    {
        none,
        attack,
        repulse,
        derived,
        counter
    }

    public enum SkillSelectionUIPanel
    {
        none,
        active,
        backend
    }

    public void Initialize( Action<SkillSelectionBox> onSkillSelectedCallback, Action<SkillSelectionBox> onSkillDeselectedCallback )
    {
        this.onSkillSelectedCallback = onSkillSelectedCallback;
        this.onSkillDeselectedCallback = onSkillDeselectedCallback;

        this.activeSkillSelectionPanel.Initialize( this );
        this.backendSkillSelectionPanel.Initialize( this );

        this.backendSkillSelectionPanel.gameObject.SetActive(false);

        this.attackSkillSelectionTabButton.onClick.AddListener(ShowAttackSkillTab);
        this.repulseSkillSelectionTabButton.onClick.AddListener(ShowRepulseSkillTab);
        this.derivedSkillSelectionTabButton.onClick.AddListener(ShowDerivedSkillTab);
        this.counterSkillSelectionTabButton.onClick.AddListener(ShowCounterSkillTab);
    }

    // Categorize and display all the skill that the character have based on skill category
    public void Show(GameCharacter selectedGameCharacter)
    {
        this.selectedGameCharacter = selectedGameCharacter;

        CharacterSkill[] _characterSkills = this.selectedGameCharacter.GetSkills();

        for (int i = 0; i < _characterSkills.Length; i++)
        {
            CharacterSkill _characterSkill = _characterSkills[i];

            if (_characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
            {
                activeSkillList.Add(_characterSkill);
            }
            else if (_characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend)
            {
                backendSkillList.Add(_characterSkill);
            }
        }

        this.activeSkillSelectionPanel.ShowCharacterSkillList(activeSkillList.ToArray());
        this.backendSkillSelectionPanel.ShowCharacterSkillList(backendSkillList.ToArray());

        base.gameObject.SetActive(true);

        ShowActiveSkillSelectionPanel();
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        if (skillSelectionBox.GetCharacterSkill().GetSkillData().skillType == DatabaseManager.Skill.SkillType.active && !skillSelectionBox.CheckIsSkillLevelChanged())
        {
            if (this.selectedActiveSkillList.Count < 3)
            {
                this.selectedActiveSkillList.Add(skillSelectionBox);

                UpdateSelectedSkillSequence();
            }
            else
            {
                skillSelectionBox.MarkDeselected();
            }
        }
        else if (skillSelectionBox.GetCharacterSkill().GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend && !skillSelectionBox.CheckIsSkillLevelChanged())
        {
            if (this.selectedBackendSkillList.Count < 3)
            {
                this.selectedBackendSkillList.Add(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionText("ON");
            }
            else
            {
                skillSelectionBox.MarkDeselected();
            }
        }

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

            if (this.selectedActiveSkillList.Contains(skillSelectionBox))
            {
                this.selectedActiveSkillList.Remove(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionSequenceNumber(0);

                UpdateSelectedSkillSequence();
            }
            else if (this.selectedBackendSkillList.Contains(skillSelectionBox))
            {
                this.selectedBackendSkillList.Remove(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionText("");
            }
        }
        else
        {
            Debug.Log( "The value for 'onSkillDeselectedCallback' is not assigned." );
        }
    }

    // Callback function for the ActiveSkillTabButton
    public void ShowActiveSkillSelectionPanel()
    {
        this.skillSelectionPanel = SkillSelectionUIPanel.active;

        this.activeSkillSelectionPanel.gameObject.SetActive(true);
        this.backendSkillSelectionPanel.gameObject.SetActive(false);

        this.backendSkillSelectionPanel.HideSkillInfoPanel();

        ChangeSkillSelectionPanel();
    }

    // Callback funtion for the PassiveSkillTabButton
    public void ShowBackendSkillSelectionPanel()
    {
        this.skillSelectionPanel = SkillSelectionUIPanel.backend;

        this.activeSkillSelectionPanel.gameObject.SetActive(false);
        this.backendSkillSelectionPanel.gameObject.SetActive(true);

        this.activeSkillSelectionPanel.HideSkillInfoPanel();

        ChangeSkillSelectionPanel();
    }

    //Update the sequence number of the selected skill list
    private void UpdateSelectedSkillSequence()
    {
        int skillSelectionCounter = 0;

        foreach (SkillSelectionBox skill in selectedActiveSkillList)
        {
            skillSelectionCounter++;
            skill.SetSkillSelectionSequenceNumber(skillSelectionCounter);
        }
    }

    // Callback function for the "attackSkillSelectionTabButton"
    private void ShowAttackSkillTab()
    {
        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.attack;
        this.attackSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Callback function for the "repulseSkillSelectionTabButton"
    private void ShowRepulseSkillTab()
    {
        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.repulse;
        this.repulseSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Callback function for the "derivedSkillSelectionTabButton"
    private void ShowDerivedSkillTab()
    {
        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.derived;
        this.derivedSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Callback function for the "counterSkillSelectionTabButton"
    private void ShowCounterSkillTab()
    {
        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.counter;
        this.counterSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Change all the tabs background to unselected.
    private void UnselectAllTab()
    {
        this.attackSkillSelectionTabBackgroundImage.color = this.unselectedTabColor;
        this.repulseSkillSelectionTabBackgroundImage.color = this.unselectedTabColor;
        this.derivedSkillSelectionTabBackgroundImage.color = this.unselectedTabColor;
        this.counterSkillSelectionTabBackgroundImage.color = this.unselectedTabColor;
    }

    // Switch between active skill panel and backend skill panel
    private void ChangeSkillSelectionPanel()
    {
        if (this.skillSelectionPanel == SkillSelectionUIPanel.active)
        {
            this.activeSkillSelectionPanel.ShowSelectedSkillInfo(null);

            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(true);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(true);
            this.counterSkillSelectionTabButton.gameObject.SetActive(false);
        }
        else if (this.skillSelectionPanel == SkillSelectionUIPanel.backend)
        {
            this.backendSkillSelectionPanel.ShowSelectedSkillInfo(null);

            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(false);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(false);
            this.counterSkillSelectionTabButton.gameObject.SetActive(true);
        }

        ShowAttackSkillTab();
    }

    // Switch between attack tab, repulse tab and derived tab
    private void ChangeSkillSelectionTab()
    {
        SkillSelectionTab _skillSelectionPanel = null;

        // Check whether current selected panel is active or backend
        if (this.skillSelectionPanel == SkillSelectionUIPanel.active)
        {
            _skillSelectionPanel = this.activeSkillSelectionPanel;
        }
        else if (this.skillSelectionPanel == SkillSelectionUIPanel.backend)
        {
            _skillSelectionPanel = this.backendSkillSelectionPanel;
        }

        if (this.skillInfoTab == SkillInfoTab.attack)
        {
            if (this.skillSelectionPanel == SkillSelectionUIPanel.active)
            {
                _skillSelectionPanel.ShowCharacterSkillList(activeSkillList.ToArray());
            }
            else if (this.skillSelectionPanel == SkillSelectionUIPanel.backend)
            {
                _skillSelectionPanel.ShowCharacterSkillList(backendSkillList.ToArray());
            }
            
            // Show back the last selected skill info from attack tab when switch back to the attack tab.
            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedCharacterSkill());
        }
        else if (this.skillInfoTab == SkillInfoTab.repulse)
        {
            _skillSelectionPanel.ShowRepulseSkillList();

            // Show back the last selected skill info from attack tab when switch back to the repulse tab.
            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedRepulseSkill());
        }
        else if (this.skillInfoTab == SkillInfoTab.derived)
        {
            _skillSelectionPanel.ShowDerivedSkillList();

            // Show back the last selected skill info from attack tab when switch back to the derived tab.
            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedDerivedSkill());
        }
        else if (this.skillInfoTab == SkillInfoTab.counter)
        {
            _skillSelectionPanel.ShowCounterSkillList();

            // Show back the last selected skill info from attack tab when switch back to the counter tab.
            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedCounterSkill());
        }
    }

    public SkillInfoTab GetSkillInfoTab()
    {
        return this.skillInfoTab;
    }
}
