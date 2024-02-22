using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DatabaseManager;

public class SkillSelectionPanelV2 : MonoBehaviour
{
    [Header("UI Images")]
    [SerializeField] private Sprite activeSkillSlotUnselectBackgroundImage = null;
    [SerializeField] private Sprite activeSkillSlotSelectBackgroundImage = null;
    [SerializeField] private Sprite activeSkillSlotSelectedBackgroundImage = null;
    [SerializeField] private Sprite[] skillSelectionSequenceImages = null;
    [SerializeField] private Sprite backendSkillSlotUnselectBackgroundImage = null;
    [SerializeField] private Sprite backendSkillSlotSelectedBackgroundImage = null;
    [SerializeField] private Sprite backendDefenceSlotSelectedImage = null;
    [SerializeField] private Sprite backendEvasionSlotSelectedImage = null;
    [SerializeField] private Sprite backendGenericSlotSelectedImage = null;
    [SerializeField] private Sprite backendDefenceSlotImage = null;
    [SerializeField] private Sprite backendEvasionSlotImage = null;
    [SerializeField] private Sprite backendGenericSlotImage = null;

    [Header("Active Skill")]
    [SerializeField] private GameObject activeSkillSelectionListGO = null;
    [SerializeField] private GameObject[] activeSkillSlotPositions = null;
    [SerializeField] private SkillSelectionBoxV2 activeSkillSlotPrefab = null;
    [SerializeField] private Button activeSkillListBoxButton = null;
    [SerializeField] private Image[] activeSkillListSlotBackgrounds = null;
    [SerializeField] private TextMeshProUGUI[] activeSkillListSlotTexts = null;
    private List<SkillSelectionBoxV2> activeSkillSlotList = new List<SkillSelectionBoxV2>();
    private List<SkillSelectionBoxV2> selectedActiveSkilSlotlList = new List<SkillSelectionBoxV2>();
    private List<CharacterSkill> characterActiveSkillList = new List<CharacterSkill>();
    private SkillSelectionBoxV2 lastSelectedActiveSkillSelectionBox = null;

    [Header("Backend Skill")]
    [SerializeField] private GameObject backendSkillSelectionListGO = null;
    [SerializeField] private GameObject[] backendSkillSlotPositions = null;
    [SerializeField] private SkillSelectionBoxV2 backendSkillSlotPrefab = null;
    [SerializeField] private Button backendSkillListBoxButton = null;
    [SerializeField] private Image backendDefenceSlotIcon = null;
    [SerializeField] private Image backendEvasionSlotIcon = null;
    [SerializeField] private Image backendGenericSlotIcon = null;
    private List<SkillSelectionBoxV2> backendSkillSlotList = new List<SkillSelectionBoxV2>();
    private List<SkillSelectionBoxV2> selectedBackendSkilSlotlList = new List<SkillSelectionBoxV2>();
    private List<CharacterSkill> characterBackendSkillList = new List<CharacterSkill>();
    private SkillSelectionBoxV2 lastSelectedBackendSkillSelectionBox = null;

    [Header("")]
    [SerializeField] private SkillInfoPanel skillInfoPanel = null;
    private Action<SkillSelectionBoxV2> onSkillSelectedCallback = null;
    private Action<SkillSelectionBoxV2> onSkillDeselectedCallback = null;

    public void Initialize(Action<SkillSelectionBoxV2> onSkillSelectedCallback, Action<SkillSelectionBoxV2> onSkillDeselectedCallback)
    {
        this.onSkillSelectedCallback = onSkillSelectedCallback;
        this.onSkillDeselectedCallback = onSkillDeselectedCallback;

        this.activeSkillSelectionListGO.SetActive(false);
        this.backendSkillSelectionListGO.SetActive(false);
        this.skillInfoPanel.Hide();

        this.activeSkillListBoxButton.onClick.AddListener(OnActiveSkillListBoxButtonClick);
        this.backendSkillListBoxButton.onClick.AddListener(OnBackendSkillListBoxButtonClick);
    }

    public void Show(GameCharacter gameCharacter)
    {
        CharacterSkill[] _characterSkills = gameCharacter.GetSkills();

        for (int i = 0; i < _characterSkills.Length; i++)
        {
            CharacterSkill _characterSkill = _characterSkills[i];

            if (_characterSkill.GetSkillData().skillType == Skill.SkillType.active)
            {
                this.characterActiveSkillList.Add(_characterSkill);
            }
            else if (_characterSkill.GetSkillData().skillType == Skill.SkillType.backend)
            {
                this.characterBackendSkillList.Add(_characterSkill);

                Debug.Log("Backend skill: " + _characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);
            }
        }

        if (this.activeSkillSlotList.Count == 0)
        {
            InitializeActiveSkillList();
        }

        if (this.backendSkillSlotList.Count == 0)
        {
            InitializeBackendSkillList();
        }
    }

    private void OnActiveSkillListBoxButtonClick()
    {
        if (this.activeSkillSelectionListGO.activeSelf)
        {
            ShowActiveSkillSelectionList(false);
            HideSkillInfoPanel();
        }
        else
        {
            ShowActiveSkillSelectionList(true);
        }

        ShowBackendSkillSelectionList(false);

        Debug.Log("ActiveSkillListBoxButton clicked");
    }

    private void OnBackendSkillListBoxButtonClick()
    {
        if (this.backendSkillSelectionListGO.activeSelf)
        {
            ShowBackendSkillSelectionList(false);
            HideSkillInfoPanel();
        }
        else
        {
            ShowBackendSkillSelectionList(true);
        }

        ShowActiveSkillSelectionList(false);

        Debug.Log("BackendSkillListBoxButton clicked");
    }

    private void InitializeActiveSkillList()
    {
        for (int i = 0; i < this.activeSkillSlotPositions.Length; i++)
        {
            Transform _slotPosition = this.activeSkillSlotPositions[i].transform;
            SkillSelectionBoxV2 _activeSkillSlot = Instantiate(this.activeSkillSlotPrefab, _slotPosition);
            this.activeSkillSlotList.Add(_activeSkillSlot);
        }

        for (int i = 0; i < this.characterActiveSkillList.Count; i++)
        {
            CharacterSkill _characterSkill = this.characterActiveSkillList[i];

            this.activeSkillSlotList[i].Initialize(this, _characterSkill);

            if (i == 0)
            {
                this.activeSkillSlotList[i].ShowSelectionHighlight();
                ShowSkillInfoPanel(this.activeSkillSlotList[i]);
                this.lastSelectedActiveSkillSelectionBox = this.activeSkillSlotList[i];
            }
        }
    }

    private void InitializeBackendSkillList()
    {
        for (int i = 0; i < this.backendSkillSlotPositions.Length; i++)
        {
            Transform _slotPosition = this.backendSkillSlotPositions[i].transform;
            SkillSelectionBoxV2 _backendSkillSlot = Instantiate(this.backendSkillSlotPrefab, _slotPosition);
            this.backendSkillSlotList.Add(_backendSkillSlot);
        }

        for (int i = 0; i < this.characterBackendSkillList.Count; i++)
        {
            CharacterSkill _characterSkill = this.characterBackendSkillList[i];
            Subskill _subskillData = _characterSkill.GetCharacterSubskillData().GetSubskillData();

            for (int j = 0; j < this.backendSkillSlotList.Count; j++)
            {
                SkillSelectionBoxV2 _backendSkillSelectionBox = this.backendSkillSlotList[j];

                if (_backendSkillSelectionBox.GetCharacterSkill() == null)
                {
                    if (_subskillData.IsDefendingSkill)
                    {
                        if (j == 1)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                        else if (j == 4 && this.backendSkillSlotList[j - 3].GetCharacterSkill() != _characterSkill)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                    }
                    else if (_subskillData.IsEvadingSkill)
                    {
                        if (j == 0)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                        else if (j == 3 && this.backendSkillSlotList[j - 3].GetCharacterSkill() != _characterSkill)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                    }
                    else
                    {
                        if (j == 2)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                        else if (j == 5 && this.backendSkillSlotList[j - 3].GetCharacterSkill() != _characterSkill)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                    }
                }
            }

            this.backendSkillSlotList[i].SetSkillSlotFrame(this.backendSkillSlotUnselectBackgroundImage);

            if (i == 0)
            {
                this.backendSkillSlotList[i].ShowSelectionHighlight();
                ShowSkillInfoPanel(this.backendSkillSlotList[i]);
                this.lastSelectedBackendSkillSelectionBox = this.backendSkillSlotList[i];
            }
        }
    }

    // Show the ActiveSkillSelectionList
    private void ShowActiveSkillSelectionList(bool show)
    {
        this.activeSkillSelectionListGO.SetActive(show);

        for (int i = 0; i < this.activeSkillListSlotBackgrounds.Length; i++)
        {
            Image _activeSkillListSlotBackground = this.activeSkillListSlotBackgrounds[i];
            _activeSkillListSlotBackground.gameObject.SetActive(!show);
        }

        if (show)
        {
            for (int i = 0; i < this.activeSkillSlotList.Count; i++)
            {
                SkillSelectionBoxV2 _skillSelectionBox = this.activeSkillSlotList[i];

                if (_skillSelectionBox.IsHighlighted())
                {
                    ShowSkillInfoPanel(_skillSelectionBox);
                }
            }
        }
    }

    // Show the BackendSkillSelectionList
    private void ShowBackendSkillSelectionList(bool show)
    {
        this.backendSkillSelectionListGO.SetActive(show);

        if (show)
        {
            for (int i = 0; i < this.backendSkillSlotList.Count; i++)
            {
                SkillSelectionBoxV2 _skillSelectionBox = this.backendSkillSlotList[i];

                if (_skillSelectionBox.IsHighlighted())
                {
                    ShowSkillInfoPanel(_skillSelectionBox);
                }
            }
        }
    }

    // Update the small slot text and big slot number image
    private void UpdateActiveSkillListSlot()
    {
        for (int i = 0; i < this.activeSkillListSlotTexts.Length; i++)
        {
            TextMeshProUGUI _activeSkillSlotText = this.activeSkillListSlotTexts[i];
            _activeSkillSlotText.gameObject.SetActive(false);
        }

        for (int i = 0; i < this.activeSkillSlotList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.activeSkillSlotList[i];
            _skillSelectionBox.SetCurrentSkillSelectionSequence(null);
            _skillSelectionBox.SetSkillSlotFrame(this.activeSkillSlotUnselectBackgroundImage);
        }

        // Show the small slot text
        for (int i = 0; i < this.selectedActiveSkilSlotlList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.selectedActiveSkilSlotlList[i];

            if (i >= this.activeSkillListSlotTexts.Length || i >= this.skillSelectionSequenceImages.Length)
            {
                return;
            }

            TextMeshProUGUI _activeSkillSlotText = this.activeSkillListSlotTexts[i];
            _activeSkillSlotText.gameObject.SetActive(true);

            Sprite _skillSelectionSequenceImage = this.skillSelectionSequenceImages[i];
            _skillSelectionBox.SetCurrentSkillSelectionSequence(_skillSelectionSequenceImage);
            _skillSelectionBox.SetSkillSlotFrame(this.activeSkillSlotSelectedBackgroundImage);

            Debug.Log($"Selected skill box[{i}]: {_skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().DisplayName}");
        }
    }

    private void UpdateBackendSkillListSlot()
    {
        this.backendDefenceSlotIcon.gameObject.SetActive(false);
        this.backendEvasionSlotIcon.gameObject.SetActive(false);
        this.backendGenericSlotIcon.gameObject.SetActive(false);

        for (int i = 0; i < this.backendSkillSlotList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.backendSkillSlotList[i];

            if (_skillSelectionBox.GetCharacterSkill() != null)
            {
                _skillSelectionBox.SetSkillSelectedImage(null);
                _skillSelectionBox.SetSkillSlotFrame(this.backendSkillSlotUnselectBackgroundImage);
            }
        }

        for (int i = 0; i < this.selectedBackendSkilSlotlList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.selectedBackendSkilSlotlList[i];
            Subskill _subskillData = _skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData();

            _skillSelectionBox.SetSkillSlotFrame(this.backendSkillSlotSelectedBackgroundImage);

            if (_subskillData.IsDefendingSkill)
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendDefenceSlotSelectedImage);

                this.backendDefenceSlotIcon.gameObject.SetActive(true);
            }
            else if (_subskillData.IsEvadingSkill)
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendEvasionSlotSelectedImage);

                this.backendEvasionSlotIcon.gameObject.SetActive(true);
            }
            else
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendGenericSlotSelectedImage);

                this.backendGenericSlotIcon.gameObject.SetActive(true);
            }
        }
    }

    public void AddSelectedSkilSlot(SkillSelectionBoxV2 skillSelectionBox)
    {
        if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.active)
        {
            if (this.selectedActiveSkilSlotlList.Count >= GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills())
            {
                return;
            }

            this.selectedActiveSkilSlotlList.Add(skillSelectionBox);

            UpdateActiveSkillListSlot();
        }
        else if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.backend)
        {
            if (this.selectedBackendSkilSlotlList.Count >= GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedBackendSkills())
            {
                return;
            }

            this.selectedBackendSkilSlotlList.Add(skillSelectionBox);

            //TODO: Update ???
            UpdateBackendSkillListSlot();
        }
    }

    public void RemoveSelectedSkilSlot(SkillSelectionBoxV2 skillSelectionBox)
    {
        if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.active)
        {
            if (this.selectedActiveSkilSlotlList.Count == 0 || !this.selectedActiveSkilSlotlList.Contains(skillSelectionBox))
            {
                return;
            }

            this.selectedActiveSkilSlotlList.Remove(skillSelectionBox);

            UpdateActiveSkillListSlot();
        }
        else if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.backend)
        {
            if (this.selectedBackendSkilSlotlList.Count == 0 || !this.selectedBackendSkilSlotlList.Contains(skillSelectionBox))
            {
                return;
            }

            this.selectedBackendSkilSlotlList.Remove(skillSelectionBox);

            UpdateBackendSkillListSlot();
        }
    }

    public void SwapSelectedActiveSkill(SkillSelectionBoxV2 targetToSwap)
    {
        if (GetSelectedActiveSkillList().Count == 0)
        {
            return;
        }

        for (int i = 0; i < GetSelectedActiveSkillList().Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = GetSelectedActiveSkillList()[i];

            if (_skillSelectionBox == targetToSwap)
            {
                this.selectedActiveSkilSlotlList[i] = this.lastSelectedActiveSkillSelectionBox;
            }
            else if (_skillSelectionBox == GetLastSelectedActiveSkillSelectionBox())
            {
                this.selectedActiveSkilSlotlList[i] = targetToSwap;
            }
        }

        UpdateActiveSkillListSlot();
    }

    public void ReplaceSelectedActiveSkill(SkillSelectionBoxV2 targetToSwap)
    {
        if (GetSelectedActiveSkillList().Count == 0)
        {
            return;
        }

        for (int i = 0; i < GetSelectedActiveSkillList().Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = GetSelectedActiveSkillList()[i];

            if (_skillSelectionBox == GetLastSelectedActiveSkillSelectionBox())
            {
                GetLastSelectedActiveSkillSelectionBox().SetIsSelected(false);
                targetToSwap.SetIsSelected(true);

                this.selectedActiveSkilSlotlList[i] = targetToSwap;
            }
        }

        UpdateActiveSkillListSlot();
    }

    public void MoveSelectedSkillToFirst(SkillSelectionBoxV2 targetToMove, List<SkillSelectionBoxV2> selectedSkillSelectionList)
    {
        if (!selectedSkillSelectionList.Contains(targetToMove))
        {
            return;
        }

        for (int i = selectedSkillSelectionList.IndexOf(targetToMove); i > 0; i--)
        {
            selectedSkillSelectionList[i] = selectedSkillSelectionList[i - 1];
        }

        selectedSkillSelectionList[0] = targetToMove;

        UpdateActiveSkillListSlot();
    }

    // Final check to make sure default skill assigned if player did not select.
    public void CheckForNecessarySkill()
    {
        for (int i = 0; i < this.selectedActiveSkilSlotlList.Count; i++)
        {
            SkillSelectionBoxV2 _activeSkillSelectionBox = this.selectedActiveSkilSlotlList[i];
            CharacterSkill _characterActiveSkill = _activeSkillSelectionBox.GetCharacterSkill();

            CharacterSkill _selectedRepulseSkill = _characterActiveSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
            CharacterSkill _selectedDerivedSkill = _characterActiveSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();

            for (int j = 0; j < _characterActiveSkill.GetCharacterSubskillList().Count; j++)
            {
                CharacterSubskill _currentLevelActiveSkill = _characterActiveSkill.GetCharacterSubskillList()[j];

                if (!_currentLevelActiveSkill.GetSubskillData().IsAvailable)
                {
                    continue;
                }

                CharacterSkill _currentLevelRepulseSkill = _currentLevelActiveSkill.GetSelectedRepulseSkill();
                CharacterSkill _currentLevelDerivedSkill = _currentLevelActiveSkill.GetSelectedDerivedSkill();

                List<CharacterSkill> _currentLevelRepulseSkillList = _currentLevelActiveSkill.GetRepulseSkillList();
                List<CharacterSkill> _currentLevelDerivedSkillList = _currentLevelActiveSkill.GetDerivedSkillList();

                if (_currentLevelActiveSkill.GetSubskillData().Level == _activeSkillSelectionBox.GetCurrentSkillLevel())
                {
                    if (_selectedRepulseSkill != null)
                    {
                        _currentLevelActiveSkill.SetSelectedRepulseSkill(_selectedRepulseSkill);
                        _characterActiveSkill.GetCharacterSubskillData().SetSelectedRepulseSkill(_selectedRepulseSkill);
                    }
                    else if (_selectedRepulseSkill == null && _currentLevelRepulseSkillList.Count > 0)
                    {
                        CharacterSkill _repulseSkill = _currentLevelRepulseSkillList[0];

                        if (_selectedDerivedSkill != null)
                        {
                            _repulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_selectedDerivedSkill);
                        }
                        else
                        {
                            if (_currentLevelDerivedSkillList.Count == 0)
                            {
                                _repulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(null);
                            }
                            else
                            {
                                _repulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_currentLevelDerivedSkillList[0]);
                            }

                        }

                        _characterActiveSkill.GetCharacterSubskillData().SetSelectedRepulseSkill(_repulseSkill);
                        _currentLevelActiveSkill.SetSelectedRepulseSkill(_repulseSkill);
                        _selectedRepulseSkill = _characterActiveSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
                    }
                    else
                    {
                        _currentLevelActiveSkill.SetSelectedRepulseSkill(null);
                    }

                    if (_selectedDerivedSkill == null && _currentLevelDerivedSkillList.Count > 0)
                    {
                        CharacterSkill _derivedSkill = _currentLevelDerivedSkillList[0];

                        _characterActiveSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_derivedSkill);
                        _currentLevelActiveSkill.SetSelectedDerivedSkill(_derivedSkill);

                        _selectedDerivedSkill = _derivedSkill;
                    }

                    if (_selectedRepulseSkill != null && _selectedRepulseSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() == null)
                    {
                        _selectedRepulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_selectedDerivedSkill);
                    }
                }
                else
                {
                    if (_currentLevelRepulseSkill == null && _currentLevelRepulseSkillList.Count > 0)
                    {
                        CharacterSkill _repulseSkill = _currentLevelRepulseSkillList[0];

                        if (_currentLevelDerivedSkill != null)
                        {
                            _repulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_currentLevelDerivedSkill);
                        }
                        else
                        {
                            if (_currentLevelDerivedSkillList.Count == 0)
                            {
                                _repulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(null);
                            }
                            else
                            {
                                _repulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_currentLevelDerivedSkillList[0]);
                            }

                        }
                        _currentLevelActiveSkill.SetSelectedRepulseSkill(_repulseSkill);
                        _currentLevelRepulseSkill = _currentLevelActiveSkill.GetSelectedRepulseSkill();
                    }

                    if (_currentLevelDerivedSkill == null && _currentLevelDerivedSkillList.Count > 0)
                    {
                        CharacterSkill _derivedSkill = _currentLevelDerivedSkillList[0];

                        _currentLevelActiveSkill.SetSelectedDerivedSkill(_derivedSkill);
                        _currentLevelDerivedSkill = _derivedSkill;
                    }

                    if (_currentLevelRepulseSkill != null && _currentLevelRepulseSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() == null)
                    {
                        _currentLevelRepulseSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(_currentLevelDerivedSkill);
                    }
                }
            }
        }

        /*for (int i = 0; i < this.selectedBackendSkillList.Count; i++)
        {
            SkillSelectionBoxV2 _backendSkillSelectionBox = this.selectedBackendSkillList[i];
            CharacterSkill _characterBackendSkill = _backendSkillSelectionBox.GetCharacterSkill();

            CharacterSkill _selectedCounterSkill = _characterBackendSkill.GetCharacterSubskillData().GetSelectedCounterSkill();

            for (int j = 0; j < _characterBackendSkill.GetCharacterSubskillList().Count; j++)
            {
                CharacterSubskill _currentLevelBackendSkill = _characterBackendSkill.GetCharacterSubskillList()[j];

                if (!_currentLevelBackendSkill.GetSubskillData().IsAvailable)
                {
                    continue;
                }

                CharacterSkill _currentLevelCounterSkill = _currentLevelBackendSkill.GetSelectedCounterSkill();
                List<CharacterSkill> _currentLevelCounterSkillList = _characterBackendSkill.GetCharacterSubskillData().GetCounterSkillList();

                if (_currentLevelBackendSkill.GetSubskillData().Level == _backendSkillSelectionBox.GetCurrentSkillLevel())
                {
                    if (_selectedCounterSkill != null)
                    {
                        _currentLevelBackendSkill.SetSelectedRepulseSkill(_selectedCounterSkill);
                        _characterBackendSkill.GetCharacterSubskillData().SetSelectedCounterSkill(_selectedCounterSkill);
                    }
                    else if (_selectedCounterSkill == null && _currentLevelCounterSkillList.Count > 0)
                    {
                        CharacterSkill _counterSkill = _currentLevelCounterSkillList[0];

                        _characterBackendSkill.GetCharacterSubskillData().SetSelectedCounterSkill(_counterSkill);
                        _currentLevelBackendSkill.SetSelectedCounterSkill(_counterSkill);
                        _selectedCounterSkill = _counterSkill;
                    }
                    else
                    {
                        _currentLevelBackendSkill.SetSelectedRepulseSkill(null);
                    }
                }
                else
                {
                    CharacterSkill _counterSkill = _currentLevelCounterSkillList[0];
                    _currentLevelBackendSkill.SetSelectedCounterSkill(_counterSkill);
                }
            }
        }*/
    }

    public void ShowSkillSelectionPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void HideSkillSelectionPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void OnSkillSelected(SkillSelectionBoxV2 skillSelectionBox)
    {
        this.onSkillSelectedCallback(skillSelectionBox);
    }

    public void OnSkillDeselected(SkillSelectionBoxV2 skillSelectionBox)
    {
        this.onSkillDeselectedCallback(skillSelectionBox);
    }

    public void ShowSkillInfoPanel(SkillSelectionBoxV2 skillSelectionBox)
    {
        this.skillInfoPanel.Show(skillSelectionBox);
    }

    public void HideSkillInfoPanel()
    {
        this.skillInfoPanel.Hide();
    }

    public SkillSelectionBoxV2 GetLastSelectedActiveSkillSelectionBox()
    {
        return this.lastSelectedActiveSkillSelectionBox;
    }

    public void SetLastSelectedActiveSkillSelectionBox(SkillSelectionBoxV2 skillSelectionBox)
    {
        this.lastSelectedActiveSkillSelectionBox = skillSelectionBox;
    }

    public SkillSelectionBoxV2 GetLastSelectedBackendSkillSelectionBox()
    {
        return this.lastSelectedBackendSkillSelectionBox;
    }

    public void SetLastSelectedBackendSkillSelectionBox(SkillSelectionBoxV2 skillSelectionBox)
    {
        this.lastSelectedBackendSkillSelectionBox = skillSelectionBox;
    }

    public List<SkillSelectionBoxV2> GetSelectedActiveSkillList()
    {
        return this.selectedActiveSkilSlotlList;
    }

    public List<SkillSelectionBoxV2> GetSelectedBackendSkillList()
    {
        return this.selectedBackendSkilSlotlList;
    }

    public Sprite GetSkillSlotSelectBackgroundImage()
    {
        return this.activeSkillSlotSelectBackgroundImage;
    }

    public Sprite GetSkillSlotUnselectBackgroundImage()
    {
        return this.activeSkillSlotUnselectBackgroundImage;
    }
}
