using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Skill = DatabaseManager.Skill;

public class SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] private SkillSelectionTab activeSkillSelectionPanelTab = null;
    [SerializeField] private SkillSelectionTab backendSkillSelectionPanelTab = null;

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
    private CharacterSkill selectedRepulseSkill = null;
    private CharacterSkill selectedDerivedSkill = null;
    private CharacterSkill selectedCounterSkill = null;

    private SkillInfoTab skillInfoTab = SkillInfoTab.none;
    private SkillSelectionUIPanel skillSelectionPanel = SkillSelectionUIPanel.none;
    private List<CharacterSkill> activeSkillList = new List<CharacterSkill>();
    private List<CharacterSkill> backendSkillList = new List<CharacterSkill>();

    private const float skillSelectionBoxAnimationDuration = 0.2f;
    private bool isPlayingSkillSelectionBoxAnimation = false;

    private const string AUDIO_ID_CLICK = "click";
    private const string AUDIO_ID_SKILL_OFF = "skill_off";
    private const string AUDIO_ID_SKILL_ON = "skill_on";

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

        this.activeSkillSelectionPanelTab.Initialize( this );
        this.backendSkillSelectionPanelTab.Initialize( this );

        this.backendSkillSelectionPanelTab.gameObject.SetActive(false);

        this.attackSkillSelectionTabButton.onClick.AddListener(OnShowAttackSkillTabClicked);
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

            if (_characterSkill.GetSkillData().skillType == Skill.SkillType.active)
            {
                activeSkillList.Add(_characterSkill);
            }
            else if (_characterSkill.GetSkillData().skillType == Skill.SkillType.backend)
            {
                backendSkillList.Add(_characterSkill);
            }
        }

        this.activeSkillSelectionPanelTab.ShowCharacterSkillList(activeSkillList.ToArray());
        this.backendSkillSelectionPanelTab.ShowCharacterSkillList(backendSkillList.ToArray());

        base.gameObject.SetActive(true);

        ShowActiveSkillSelectionPanel();
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        if (this.isPlayingSkillSelectionBoxAnimation)
        {
            return;
        }

        if (this.onSkillSelectedCallback != null)
        {
            this.onSkillSelectedCallback(skillSelectionBox);
        }
        else
        {
            Debug.Log("The value for 'onSkillSelectedCallback' is not assigned.");
        }

        if (skillSelectionBox.CheckIsSkillLevelChanged())
        {
            return;
        }

        CharacterSubskill _lastSelectedCharacterActiveSkill = this.activeSkillSelectionPanelTab.GetLastSelectedCharacterSkill()?.GetCharacterSubskillData();
        CharacterSubskill _lastSelectedCharacterBackendSkill = this.backendSkillSelectionPanelTab.GetLastSelectedCharacterSkill()?.GetCharacterSubskillData();
        CharacterSkill _currentSelectedSkill = skillSelectionBox.GetCharacterSkill();
        Skill.SkillType _skillType = skillSelectionBox.GetCharacterSkill().GetSkillData().skillType;

        if (_skillType == Skill.SkillType.active)
        {
            if (this.selectedActiveSkillList.Count < GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills())
            {
                this.selectedActiveSkillList.Add(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionText("ON");
                skillSelectionBox.BringToFront();

                UpdateSkillSelectionListOrder(this.selectedActiveSkillList);
            }
            else
            {
                skillSelectionBox.MarkDeselected();
            }
        }
        else if (_skillType == Skill.SkillType.repulse) // repulse skill
        {
            if (_lastSelectedCharacterActiveSkill.GetSelectedRepulseSkill() == null)
            {
                this.selectedRepulseSkill = skillSelectionBox.GetCharacterSkill();

                _lastSelectedCharacterActiveSkill.SetSelectedRepulseSkill(_currentSelectedSkill);

                skillSelectionBox.SetSkillSelectionText("ON");

                skillSelectionBox.transform.SetAsFirstSibling();
            }
            else
            {
                Debug.Log("Max repulse skill selected");
                skillSelectionBox.MarkDeselected();
            }
        }
        else if (_skillType == Skill.SkillType.derived) // derived skill
        {
            if (_lastSelectedCharacterActiveSkill.GetSelectedDerivedSkill() == null)
            {
                this.selectedDerivedSkill = skillSelectionBox.GetCharacterSkill();

                _lastSelectedCharacterActiveSkill.SetSelectedDerivedSkill(_currentSelectedSkill);

                skillSelectionBox.SetSkillSelectionText("ON");

                skillSelectionBox.transform.SetAsFirstSibling();
            }
            else
            {
                Debug.Log("Max derived skill selected");
                skillSelectionBox.MarkDeselected();
            }
        }
        else if (_skillType == Skill.SkillType.backend)
        {
            if (this.selectedBackendSkillList.Count < GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedBackendSkills())
            {
                this.selectedBackendSkillList.Add(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionText("ON");
                skillSelectionBox.BringToFront();

                UpdateSkillSelectionListOrder(this.selectedBackendSkillList);
            }
            else
            {
                skillSelectionBox.MarkDeselected();
            }
        }
        else if (_skillType == Skill.SkillType.counter) // counter skill
        {
            if (_lastSelectedCharacterBackendSkill.GetSelectedCounterSkill() == null)
            {
                this.selectedCounterSkill = skillSelectionBox.GetCharacterSkill();

                _lastSelectedCharacterBackendSkill.SetSelectedCounterSkill(_currentSelectedSkill);

                skillSelectionBox.SetSkillSelectionText("ON");

                skillSelectionBox.transform.SetAsFirstSibling();
            }
            else
            {
                Debug.Log("Max counter skill selected");
                skillSelectionBox.MarkDeselected();
            }
        }

        if (skillSelectionBox.IsSkillBoxSelected())
        {
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SKILL_ON);
        }
        else if (!skillSelectionBox.IsSkillBoxSelected())
        {
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        }
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        CharacterSkill _lastSelectedCharacterActiveSkill = this.activeSkillSelectionPanelTab.GetLastSelectedCharacterSkill();
        CharacterSubskill _activeSubskillData = _lastSelectedCharacterActiveSkill?.GetCharacterSubskillData();

        CharacterSkill _lastSelectedCharacterBackendSkill = this.backendSkillSelectionPanelTab.GetLastSelectedCharacterSkill();
        CharacterSubskill _backendSubskillData = _lastSelectedCharacterBackendSkill?.GetCharacterSubskillData();

        if (this.selectedActiveSkillList.Contains(skillSelectionBox))
        {
            this.selectedActiveSkillList.Remove(skillSelectionBox);

            skillSelectionBox.SetSkillSelectionText("");

            UpdateSkillSelectionListOrder(this.selectedActiveSkillList);
        }
        else if (skillInfoTab == SkillInfoTab.repulse && _activeSubskillData?.GetSelectedRepulseSkill() != null) // repulse skill
        {
            this.selectedRepulseSkill = null;
            _activeSubskillData.SetSelectedRepulseSkill(null);

            skillSelectionBox.SetSkillSelectionText("");
        }
        else if (skillInfoTab == SkillInfoTab.derived && _activeSubskillData?.GetSelectedDerivedSkill() != null)// derived skill
        {
            this.selectedDerivedSkill = null;
            _activeSubskillData.SetSelectedDerivedSkill(null);

            skillSelectionBox.SetSkillSelectionText("");
        }
        else if (this.selectedBackendSkillList.Contains(skillSelectionBox))
        {
            this.selectedBackendSkillList.Remove(skillSelectionBox);

            skillSelectionBox.SetSkillSelectionText("");

            if (skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
            {
                skillSelectionBox.GetCharacterSkill().ClearObservedSkillDataList();
            }

            UpdateSkillSelectionListOrder(this.selectedBackendSkillList);
        }
        else if (skillInfoTab == SkillInfoTab.counter && _backendSubskillData?.GetSelectedCounterSkill() != null) // counter skill
        {
            this.selectedCounterSkill = null;
            _backendSubskillData.SetSelectedCounterSkill(null);

            skillSelectionBox.SetSkillSelectionText("");
        }

        skillSelectionBox.MarkDeselected();

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SKILL_OFF);

        if (this.onSkillDeselectedCallback != null)
        {
            this.onSkillDeselectedCallback( skillSelectionBox );
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

        this.activeSkillSelectionPanelTab.gameObject.SetActive(true);
        this.backendSkillSelectionPanelTab.gameObject.SetActive(false);

        this.backendSkillSelectionPanelTab.HideSkillInfoPanel();

        ChangeSkillSelectionPanel();
    }

    // Callback funtion for the PassiveSkillTabButton
    public void ShowBackendSkillSelectionPanel()
    {
        this.skillSelectionPanel = SkillSelectionUIPanel.backend;

        this.activeSkillSelectionPanelTab.gameObject.SetActive(false);
        this.backendSkillSelectionPanelTab.gameObject.SetActive(true);

        this.activeSkillSelectionPanelTab.HideSkillInfoPanel();

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

    private void ShowAttackSkillTab()
    {
        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.attack;
        this.attackSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Callback function for the "attackSkillSelectionTabButton"
    private void OnShowAttackSkillTabClicked()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);

        ShowAttackSkillTab();
    }

    // Callback function for the "repulseSkillSelectionTabButton"
    private void ShowRepulseSkillTab()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);

        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.repulse;
        this.repulseSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Callback function for the "derivedSkillSelectionTabButton"
    private void ShowDerivedSkillTab()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);

        UnselectAllTab();

        this.skillInfoTab = SkillInfoTab.derived;
        this.derivedSkillSelectionTabBackgroundImage.color = this.selectedTabColor;

        ChangeSkillSelectionTab();
    }

    // Callback function for the "counterSkillSelectionTabButton"
    private void ShowCounterSkillTab()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);

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
            this.activeSkillSelectionPanelTab.ShowSelectedSkillInfo(null);
            this.activeSkillSelectionPanelTab.SetSkillListTitle("Active Skill \n主動技能");

            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(true);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(true);
            this.counterSkillSelectionTabButton.gameObject.SetActive(false);
        }
        else if (this.skillSelectionPanel == SkillSelectionUIPanel.backend)
        {
            this.backendSkillSelectionPanelTab.ShowSelectedSkillInfo(null);
            this.backendSkillSelectionPanelTab.SetSkillListTitle("Backend Skill \n後台技能");

            this.attackSkillSelectionTabButton.gameObject.SetActive(true);
            this.repulseSkillSelectionTabButton.gameObject.SetActive(false);
            this.derivedSkillSelectionTabButton.gameObject.SetActive(false);
            this.counterSkillSelectionTabButton.gameObject.SetActive(true);
        }

        ShowAttackSkillTab();
    }

    // Check whether current showing panel is actiev skill or backend skill panel tab
    private SkillSelectionTab CurrentShowingSkillSelectionPanel()
    {
        SkillSelectionTab _skillSelectionPanel = null;

        // Check whether current selected panel is active or backend
        if (this.skillSelectionPanel == SkillSelectionUIPanel.active)
        {
            _skillSelectionPanel = this.activeSkillSelectionPanelTab;
        }
        else if (this.skillSelectionPanel == SkillSelectionUIPanel.backend)
        {
            _skillSelectionPanel = this.backendSkillSelectionPanelTab;
        }

        return _skillSelectionPanel;
    }

    // Switch between attack tab, repulse tab and derived tab
    private void ChangeSkillSelectionTab()
    {
        SkillSelectionTab _skillSelectionPanel = CurrentShowingSkillSelectionPanel();

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
            
            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedCharacterSkill());
        }
        else if (this.skillInfoTab == SkillInfoTab.repulse)
        {
            _skillSelectionPanel.ShowRepulseSkillList();

            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedRepulseSkill());

            SelectDefaultSkillBox(_skillSelectionPanel.GetRepulseSkillSelectionBoxList(), ref this.selectedRepulseSkill);
        }
        else if (this.skillInfoTab == SkillInfoTab.derived)
        {
            _skillSelectionPanel.ShowDerivedSkillList();

            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedDerivedSkill());

            SelectDefaultSkillBox(_skillSelectionPanel.GetDerivedSkillSelectionBoxList(), ref this.selectedDerivedSkill);
        }
        else if (this.skillInfoTab == SkillInfoTab.counter)
        {
            _skillSelectionPanel.ShowCounterSkillList();

            _skillSelectionPanel.ShowSelectedSkillInfo(_skillSelectionPanel.GetLastSelectedCounterSkill());

            SelectDefaultSkillBox(_skillSelectionPanel.GetCounterSkillSelectionBoxList(), ref this.selectedCounterSkill);
        }
    }

    // Select default repulse, derived and counter skill if the user didn't select any
    private void SelectDefaultSkillBox(List<SkillSelectionBox> instantiatedSkillSelectionBoxList, ref CharacterSkill selectedSkill)
    {
        if (instantiatedSkillSelectionBoxList == null || instantiatedSkillSelectionBoxList.Count == 0)
        {
            return;
        }

        if (selectedSkill == null && instantiatedSkillSelectionBoxList.Count > 0)
        {
            SkillSelectionBox _skillSelectionBox = instantiatedSkillSelectionBoxList[0];
            _skillSelectionBox.MarkSelected();

            selectedSkill = _skillSelectionBox.GetCharacterSkill();
        }
    }

    // Final check to make sure default skill assigned if player did not select.
    public void CheckForNecessarySkill()
    {
        for (int i = 0; i < this.selectedActiveSkillList.Count; i++)
        {
            SkillSelectionBox _activeSkillSelectionBox = this.selectedActiveSkillList[i];
            CharacterSkill _characterActiveSkill = _activeSkillSelectionBox.GetCharacterSkill();

            CharacterSkill _selectedRepulseSkill = _characterActiveSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
            CharacterSkill _selectedDerivedSkill = _characterActiveSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();

            List<CharacterSkill> _repulseSkillList = _characterActiveSkill.GetCharacterSubskillData().GetRepulseSkillList();
            List<CharacterSkill> _derivedSkillList = _characterActiveSkill.GetCharacterSubskillData().GetDerivedSkillList();

            if (_selectedRepulseSkill == null && _repulseSkillList.Count > 0)
            {
                CharacterSkill _repulseSkill = _repulseSkillList[0];

                _characterActiveSkill.GetCharacterSubskillData().SetSelectedRepulseSkill(_repulseSkill);
            }

            if (_selectedDerivedSkill == null && _derivedSkillList.Count > 0)
            {
                CharacterSkill _derivedSkill = _derivedSkillList[0];

                _characterActiveSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_derivedSkill);
            }
        }

        for (int i = 0; i < this.selectedBackendSkillList.Count; i++)
        {
            SkillSelectionBox _backendSkillSelectionBox = this.selectedBackendSkillList[i];
            CharacterSkill _characterActiveSkill = _backendSkillSelectionBox.GetCharacterSkill();

            CharacterSkill _selectedCounterSkill = _characterActiveSkill.GetCharacterSubskillData().GetSelectedCounterSkill();
            List<CharacterSkill> _counterSkillList = _characterActiveSkill.GetCharacterSubskillData().GetCounterSkillList();

            if (_selectedCounterSkill == null && _counterSkillList.Count > 0)
            {
                CharacterSkill _counterSkill = _counterSkillList[0];

                _characterActiveSkill.GetCharacterSubskillData().SetSelectedCounterSkill(_counterSkill);
            }
        }
    }

    private void UpdateSkillSelectionListOrder( List<SkillSelectionBox> selectedSkillList )
    {
        this.isPlayingSkillSelectionBoxAnimation = true;

        List<SkillSelectionBox> _skillSelectionBoxList = CurrentShowingSkillSelectionPanel().GetSkillSelectionListBox().GetSkillSelectionBoxList();
        for (int i = 0; i < _skillSelectionBoxList.Count; i++)
        {
            _skillSelectionBoxList[ i ].MoveContainerToOrigin( SkillSelectionPanel.skillSelectionBoxAnimationDuration );
        }

        for (int i = 0; i < selectedSkillList.Count; i++)
        {
            selectedSkillList[ i ].transform.SetSiblingIndex( i );
        }

        LeanTween.delayedCall( SkillSelectionPanel.skillSelectionBoxAnimationDuration, () => { this.isPlayingSkillSelectionBoxAnimation = false; } );
    }

    public SkillInfoTab GetSkillInfoTab()
    {
        return this.skillInfoTab;
    }
}
