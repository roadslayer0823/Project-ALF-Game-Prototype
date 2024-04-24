using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static DatabaseManager;

public class SkillSelectionPanelV2 : MonoBehaviour
{
    [Header("UI Images")]
    [SerializeField] private Sprite activeSkillBoxUnselectBackgroundImage = null;
    [SerializeField] private Sprite activeSkillBoxSelectBackgroundImage = null;
    [SerializeField] private Sprite activeSkillBoxSelectedBackgroundImage = null;
    [SerializeField] private Sprite repulseSkillBoxFrameImage = null;
    [SerializeField] private Sprite derivedSkillBoxFrameImage = null;
    [SerializeField] private Sprite[] skillSelectionSequenceImages = null;
    [SerializeField] private Sprite backendSkillBoxUnselectBackgroundImage = null;
    [SerializeField] private Sprite backendSkillBoxSelectedBackgroundImage = null;
    [SerializeField] private Sprite backendDefenceBoxSelectedImage = null;
    [SerializeField] private Sprite backendEvasionBoxSelectedImage = null;
    [SerializeField] private Sprite backendGenericBoxSelectedImage = null;
    [SerializeField] private Sprite backendDefenceBoxImage = null;
    [SerializeField] private Sprite backendEvasionBoxImage = null;
    [SerializeField] private Sprite backendGenericBoxImage = null;
    [SerializeField] private Sprite counterSkillBoxFrameImage = null;

    [Header("Active Skill")]
    [SerializeField] private GameObject activeSkillSelectionListGO = null;
    [SerializeField] private GameObject[] activeSkillBoxPositions = null;
    [SerializeField] private SkillSelectionBoxV2 activeSkillBoxPrefab = null;
    [SerializeField] private Button activeSkillListBoxButton = null;
    [SerializeField] private Image[] activeSkillListBoxBackgrounds = null;
    [SerializeField] private TextMeshProUGUI[] activeSkillListBoxTexts = null;
    private List<SkillSelectionBoxV2> activeSkillBoxList = new List<SkillSelectionBoxV2>();
    private List<SkillSelectionBoxV2> selectedActiveSkillBoxList = new List<SkillSelectionBoxV2>();
    private List<CharacterSkill> characterActiveSkillList = new List<CharacterSkill>();
    private SkillSelectionBoxV2 lastSelectedActiveSkillSelectionBox = null;

    [Header("Backend Skill")]
    [SerializeField] private GameObject backendSkillSelectionListGO = null;
    [SerializeField] private GameObject backendSkillListBoxGO = null;
    [SerializeField] private GameObject[] backendSkillBoxPositions = null;
    [SerializeField] private SkillSelectionBoxV2 backendSkillBoxPrefab = null;
    [SerializeField] private Button backendSkillListBoxButton = null;
    [SerializeField] private Image backendDefenceBoxIcon = null;
    [SerializeField] private Image backendDefenceBoxFilledIcon = null;
    [SerializeField] private Image backendEvasionBoxIcon = null;
    [SerializeField] private Image backendEvasionBoxFilledIcon = null;
    [SerializeField] private Image backendGenericBoxIcon = null;
    [SerializeField] private Image backendGenericBoxFilledIcon = null;
    private List<SkillSelectionBoxV2> backendSkillBoxList = new List<SkillSelectionBoxV2>();
    private List<SkillSelectionBoxV2> selectedBackendSkillBoxList = new List<SkillSelectionBoxV2>();
    private List<CharacterSkill> characterBackendSkillList = new List<CharacterSkill>();
    private SkillSelectionBoxV2 lastSelectedBackendSkillSelectionBox = null;

    [Header("")]
    [SerializeField] private Button returnButton = null;
    [SerializeField] private SkillInfoPanel skillInfoPanel = null;

    private Action<SkillSelectionBoxV2> onSkillSelectedCallback = null;
    private Action<SkillSelectionBoxV2> onSkillDeselectedCallback = null;
    private Action onReturnedCallback = null;

    private GameCharacter gameCharacter = null;

    private const string AUDIO_ID_SKILL_OFF = "skill_off";
    private const string AUDIO_ID_SKILL_ON = "skill_on";
    private const string AUDIO_ID_CLICK = "click";

    public enum SkillType
    {
        None,
        Active,
        Backend
    }

    public void Initialize( Action<SkillSelectionBoxV2> onSkillSelectedCallback, Action<SkillSelectionBoxV2> onSkillDeselectedCallback, Action onReturnedCallback )
    {
        this.onSkillSelectedCallback = onSkillSelectedCallback;
        this.onSkillDeselectedCallback = onSkillDeselectedCallback;
        this.onReturnedCallback = onReturnedCallback;

        this.activeSkillSelectionListGO.SetActive(false);
        this.backendSkillSelectionListGO.SetActive(false);
        this.skillInfoPanel.Hide();

        this.activeSkillListBoxButton.onClick.AddListener(OnActiveSkillListBoxButtonClick);
        this.backendSkillListBoxButton.onClick.AddListener(OnBackendSkillListBoxButtonClick);
        this.returnButton.onClick.AddListener( OnReturnButtonClick );
    }

    // show the skill selection panel
    public void Show( GameCharacter gameCharacter, SkillType skillType )
    {
        this.gameCharacter = gameCharacter;

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
            }
        }

        if (this.activeSkillBoxList.Count == 0)
        {
            InitializeActiveSkillList();
        }
        else
        {
            if (this.selectedActiveSkillBoxList.Count != 0)
            {
                this.selectedActiveSkillBoxList.Clear();

                for (int i = 0; i < this.activeSkillBoxList.Count; i++)
                {
                    SkillSelectionBoxV2 _activeSkillSelectionBox = this.activeSkillBoxList[i];

                    if (_activeSkillSelectionBox.GetCharacterSkill() == null)
                    {
                        continue;
                    }

                    for (int j = 0; j < this.gameCharacter.GetSelectedActiveSkillList().Count; j++)
                    {
                        CharacterSkill _selectedCharacterSkill = this.gameCharacter.GetSelectedActiveSkillList()[j];
                        int _selectedCharacterSkillLevel = _selectedCharacterSkill.GetSelectedSkillLevel();

                        if (_selectedCharacterSkill.GetSkillData().Id == _activeSkillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().SkillId)
                        {
                            _activeSkillSelectionBox.SetCurrentSkillLevel(_selectedCharacterSkillLevel);
                            _activeSkillSelectionBox.GetCharacterSkill().SetSelectedSkillLevel(_selectedCharacterSkillLevel);
                            _activeSkillSelectionBox.UpdateSkillSelectionBoxData();

                            this.selectedActiveSkillBoxList.Add(_activeSkillSelectionBox);

                            break;
                        }
                    }
                }

                UpdateActiveSkillListBox();
            }
        }

        if (this.backendSkillBoxList.Count == 0)
        {
            InitializeBackendSkillList();
        }
        else
        {
            if (this.selectedBackendSkillBoxList.Count != 0)
            {
                this.selectedBackendSkillBoxList.Clear();

                for (int i = 0; i < this.backendSkillBoxList.Count; i++)
                {
                    SkillSelectionBoxV2 _backendSkillSelectionBox = this.backendSkillBoxList[i];

                    if (_backendSkillSelectionBox.GetCharacterSkill() == null)
                    {
                        continue;
                    }

                    for (int j = 0; j < this.gameCharacter.GetSelectedBackendSkillList().Count; j++)
                    {
                        CharacterSkill _selectedCharacterSkill = this.gameCharacter.GetSelectedBackendSkillList()[j];

                        if (_selectedCharacterSkill.GetSkillData().Id == _backendSkillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().SkillId)
                        {
                            this.selectedBackendSkillBoxList.Add(_backendSkillSelectionBox);
                            break;
                        }
                    }
                }

                UpdateBackendSkillListBox();
            }
        }

        ShowSkillSelectionPanel( skillType );
    }

    // when click the active skill list box button
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

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
    }

    // when click the backend skill list box button
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

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
    }

    // when click the return button
    private void OnReturnButtonClick()
    {
        this.onReturnedCallback?.Invoke();
    }

    // initialize active skill list
    private void InitializeActiveSkillList()
    {
        for (int i = 0; i < this.activeSkillBoxPositions.Length; i++)
        {
            Transform _boxPosition = this.activeSkillBoxPositions[i].transform;
            SkillSelectionBoxV2 _activeSkillBox = Instantiate(this.activeSkillBoxPrefab, _boxPosition);
            this.activeSkillBoxList.Add(_activeSkillBox);
        }

        for (int i = 0; i < this.characterActiveSkillList.Count; i++)
        {
            CharacterSkill _characterSkill = this.characterActiveSkillList[i];

            this.activeSkillBoxList[i].Initialize(this, _characterSkill);

            if (i == 0)
            {
                this.activeSkillBoxList[i].ShowSelectionHighlight();
                this.lastSelectedActiveSkillSelectionBox = this.activeSkillBoxList[i];
            }
        }
    }

    // initialize backend skill list
    private void InitializeBackendSkillList()
    {
        for (int i = 0; i < this.backendSkillBoxPositions.Length; i++)
        {
            Transform _boxPosition = this.backendSkillBoxPositions[i].transform;
            SkillSelectionBoxV2 _backendSkillBox = Instantiate(this.backendSkillBoxPrefab, _boxPosition);
            this.backendSkillBoxList.Add(_backendSkillBox);
        }

        for (int i = 0; i < this.characterBackendSkillList.Count; i++)
        {
            CharacterSkill _characterSkill = this.characterBackendSkillList[i];
            Subskill _subskillData = _characterSkill.GetCharacterSubskillData().GetSubskillData();

            for (int j = 0; j < this.backendSkillBoxList.Count; j++)
            {
                SkillSelectionBoxV2 _backendSkillSelectionBox = this.backendSkillBoxList[j];

                if (_backendSkillSelectionBox.GetCharacterSkill() == null)
                {
                    if (_subskillData.IsDefendingSkill)
                    {
                        if (j == 1)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                        else if (j == 4 && this.backendSkillBoxList[j - 3].GetCharacterSkill() != _characterSkill)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                    }
                    else if (_subskillData.IsEvadingSkill)
                    {
                        if (j == 0)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);

                            this.backendSkillBoxList[j].ShowSelectionHighlight();
                            this.lastSelectedBackendSkillSelectionBox = this.backendSkillBoxList[j];
                        }
                        else if (j == 3 && this.backendSkillBoxList[j - 3].GetCharacterSkill() != _characterSkill)
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
                        else if (j == 5 && this.backendSkillBoxList[j - 3].GetCharacterSkill() != _characterSkill)
                        {
                            _backendSkillSelectionBox.Initialize(this, _characterSkill);
                        }
                    }
                }
            }

            this.backendSkillBoxList[i].SetSkillBoxFrame(this.backendSkillBoxUnselectBackgroundImage);
        }
    }

    // Show the ActiveSkillSelectionList
    public void ShowActiveSkillSelectionList(bool show)
    {
        this.activeSkillSelectionListGO.SetActive(show);

        for (int i = 0; i < this.activeSkillListBoxBackgrounds.Length; i++)
        {
            Image _activeSkillListBoxBackground = this.activeSkillListBoxBackgrounds[i];
            _activeSkillListBoxBackground.gameObject.SetActive(false);
        }

        if (show)
        {
            for (int i = 0; i < this.activeSkillBoxList.Count; i++)
            {
                SkillSelectionBoxV2 _skillSelectionBox = this.activeSkillBoxList[i];

                if (_skillSelectionBox.IsHighlighted())
                {
                    ShowSkillInfoPanel(_skillSelectionBox);
                }
            }

            for (int i = 0; i < this.selectedActiveSkillBoxList.Count; i++)
            {
                Image _activeSkillListBoxBackground = this.activeSkillListBoxBackgrounds[i];
                _activeSkillListBoxBackground.gameObject.SetActive(false);

                TextMeshProUGUI _activeSkillBoxText = this.activeSkillListBoxTexts[i];
                _activeSkillBoxText.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < this.selectedActiveSkillBoxList.Count; i++)
            {
                Image _activeSkillListBoxBackground = this.activeSkillListBoxBackgrounds[i];
                _activeSkillListBoxBackground.gameObject.SetActive(true);

                TextMeshProUGUI _activeSkillBoxText = this.activeSkillListBoxTexts[i];
                _activeSkillBoxText.gameObject.SetActive(false);
            }
        }
    }

    // Show the BackendSkillSelectionList
    public void ShowBackendSkillSelectionList(bool show)
    {
        this.backendSkillSelectionListGO.SetActive(show);

        if (show)
        {
            for (int i = 0; i < this.backendSkillBoxList.Count; i++)
            {
                SkillSelectionBoxV2 _skillSelectionBox = this.backendSkillBoxList[i];

                if (_skillSelectionBox.IsHighlighted())
                {
                    ShowSkillInfoPanel(_skillSelectionBox);
                }
            }

            if (this.backendDefenceBoxFilledIcon.gameObject.activeInHierarchy)
            {
                this.backendDefenceBoxFilledIcon.gameObject.SetActive(false);
                this.backendDefenceBoxIcon.gameObject.SetActive(true);
            }

            if (this.backendEvasionBoxFilledIcon.gameObject.activeInHierarchy)
            {
                this.backendEvasionBoxFilledIcon.gameObject.SetActive(false);
                this.backendEvasionBoxIcon.gameObject.SetActive(true);
            }

            if (this.backendGenericBoxFilledIcon.gameObject.activeInHierarchy)
            {
                this.backendGenericBoxFilledIcon.gameObject.SetActive(false);
                this.backendGenericBoxIcon.gameObject.SetActive(true);
            }
        }
        else
        {
            if (this.backendDefenceBoxIcon.gameObject.activeInHierarchy)
            {
                this.backendDefenceBoxIcon.gameObject.SetActive(false);
                this.backendDefenceBoxFilledIcon.gameObject.SetActive(true);
            }

            if (this.backendEvasionBoxIcon.gameObject.activeInHierarchy)
            {
                this.backendEvasionBoxIcon.gameObject.SetActive(false);
                this.backendEvasionBoxFilledIcon.gameObject.SetActive(true);
            }

            if (this.backendGenericBoxIcon.gameObject.activeInHierarchy)
            {
                this.backendGenericBoxIcon.gameObject.SetActive(false);
                this.backendGenericBoxFilledIcon.gameObject.SetActive(true);
            }
        }
    }

    // Update the small box text and big box number image
    private void UpdateActiveSkillListBox()
    {
        for (int i = 0; i < this.activeSkillListBoxTexts.Length; i++)
        {
            TextMeshProUGUI _activeSkillBoxText = this.activeSkillListBoxTexts[i];
            _activeSkillBoxText.gameObject.SetActive(false);
        }

        for (int i = 0; i < this.activeSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.activeSkillBoxList[i];
            _skillSelectionBox.SetCurrentSkillSelectionSequence(null);
            _skillSelectionBox.SetSkillBoxFrame(this.activeSkillBoxUnselectBackgroundImage);
        }

        // Show the small box text
        for (int i = 0; i < this.selectedActiveSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.selectedActiveSkillBoxList[i];

            if (i >= this.activeSkillListBoxTexts.Length || i >= this.skillSelectionSequenceImages.Length)
            {
                return;
            }

            TextMeshProUGUI _activeSkillBoxText = this.activeSkillListBoxTexts[i];
            _activeSkillBoxText.gameObject.SetActive(true);

            Sprite _skillSelectionSequenceImage = this.skillSelectionSequenceImages[i];
            _skillSelectionBox.SetCurrentSkillSelectionSequence(_skillSelectionSequenceImage);
            _skillSelectionBox.SetSkillBoxFrame(this.activeSkillBoxSelectedBackgroundImage);
        }
    }

    // update backend skill list box
    private void UpdateBackendSkillListBox()
    {
        this.backendDefenceBoxIcon.gameObject.SetActive(false);
        this.backendEvasionBoxIcon.gameObject.SetActive(false);
        this.backendGenericBoxIcon.gameObject.SetActive(false);

        for (int i = 0; i < this.backendSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.backendSkillBoxList[i];

            if (_skillSelectionBox.GetCharacterSkill() != null)
            {
                _skillSelectionBox.SetSkillSelectedImage(null);
                _skillSelectionBox.SetSkillBoxFrame(this.backendSkillBoxUnselectBackgroundImage);
            }
        }

        CharacterSkill _defenceSlotBackendSkill = null;
        CharacterSkill _evasionSlotBackendSkill = null;
        CharacterSkill _genericSlotBackendSkill = null;

        for (int i = 0; i < this.selectedBackendSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.selectedBackendSkillBoxList[i];
            CharacterSkill _characterBackendSkill = _skillSelectionBox.GetCharacterSkill();
            Subskill _subskillData = _characterBackendSkill.GetCharacterSubskillData().GetSubskillData();

            _skillSelectionBox.SetSkillBoxFrame(this.backendSkillBoxSelectedBackgroundImage);

            if (_subskillData.IsDefendingSkill)
            {
                if (_defenceSlotBackendSkill == null)
                {
                    _defenceSlotBackendSkill = _characterBackendSkill;

                    this.backendDefenceBoxIcon.sprite = this.backendDefenceBoxImage;
                    this.backendDefenceBoxIcon.gameObject.SetActive(true);
                }
                else if (_defenceSlotBackendSkill != null && _defenceSlotBackendSkill != _characterBackendSkill && _genericSlotBackendSkill == null)
                {
                    _genericSlotBackendSkill = _characterBackendSkill;

                    this.backendGenericBoxIcon.sprite = this.backendDefenceBoxImage;
                    this.backendGenericBoxIcon.gameObject.SetActive(true);
                }

                _skillSelectionBox.SetSkillSelectedImage(this.backendDefenceBoxSelectedImage);
            }
            else if (_subskillData.IsEvadingSkill)
            {
                if (_evasionSlotBackendSkill == null)
                {
                    _evasionSlotBackendSkill = _characterBackendSkill;

                    this.backendEvasionBoxIcon.sprite = this.backendEvasionBoxImage;
                    this.backendEvasionBoxIcon.gameObject.SetActive(true);
                }
                else if (_evasionSlotBackendSkill != null && _evasionSlotBackendSkill != _characterBackendSkill && _genericSlotBackendSkill == null)
                {
                    _genericSlotBackendSkill = _characterBackendSkill;

                    this.backendGenericBoxIcon.sprite = this.backendEvasionBoxImage;
                    this.backendGenericBoxIcon.gameObject.SetActive(true);
                }

                _skillSelectionBox.SetSkillSelectedImage(this.backendEvasionBoxSelectedImage);
            }
            else if (!this.backendGenericBoxIcon.gameObject.activeInHierarchy && _subskillData.IsObservingSkill && _genericSlotBackendSkill == null)
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendGenericBoxSelectedImage);

                _genericSlotBackendSkill = _characterBackendSkill;

                this.backendGenericBoxIcon.sprite = this.backendGenericBoxImage;
                this.backendGenericBoxIcon.gameObject.SetActive(true);
            }
        }
    }

    // add selected skill box
    public void AddSelectedSkillBox(SkillSelectionBoxV2 skillSelectionBox)
    {
        if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.active)
        {
            if (this.selectedActiveSkillBoxList.Count >= GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills())
            {
                return;
            }

            this.selectedActiveSkillBoxList.Add(skillSelectionBox);
            this.onSkillSelectedCallback(skillSelectionBox);

            UpdateActiveSkillListBox();
        }
        else if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.backend)
        {
            if (this.selectedBackendSkillBoxList.Count >= GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedBackendSkills())
            {
                return;
            }

            CharacterSkill _characterBackendSkill = skillSelectionBox.GetCharacterSkill();
            Subskill _subskillData = _characterBackendSkill.GetCharacterSubskillData().GetSubskillData();

            if (_subskillData.IsDefendingSkill && this.backendDefenceBoxIcon.gameObject.activeInHierarchy && this.backendGenericBoxIcon.gameObject.activeInHierarchy)
            {
                return;
            }

            if (_subskillData.IsEvadingSkill && this.backendEvasionBoxIcon.gameObject.activeInHierarchy && this.backendGenericBoxIcon.gameObject.activeInHierarchy)
            {
                return;
            }

            if (_subskillData.IsObservingSkill && this.backendGenericBoxIcon.gameObject.activeInHierarchy)
            {
                return;
            }

            this.selectedBackendSkillBoxList.Add(skillSelectionBox);
            this.onSkillSelectedCallback(skillSelectionBox);
            skillSelectionBox.UpdateSkillIcon(true);
            skillSelectionBox.SetIsSelected(true);

            UpdateBackendSkillListBox();
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SKILL_ON);
    }

    // remove selected skill box
    public void RemoveSelectedSkillBox(SkillSelectionBoxV2 skillSelectionBox)
    {
        if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.active)
        {
            if (this.selectedActiveSkillBoxList.Count == 0 || !this.selectedActiveSkillBoxList.Contains(skillSelectionBox))
            {
                return;
            }

            this.selectedActiveSkillBoxList.Remove(skillSelectionBox);
            this.onSkillDeselectedCallback(skillSelectionBox);

            UpdateActiveSkillListBox();
        }
        else if (skillSelectionBox.GetCharacterSkillType() == Skill.SkillType.backend)
        {
            if (this.selectedBackendSkillBoxList.Count == 0 || !this.selectedBackendSkillBoxList.Contains(skillSelectionBox))
            {
                return;
            }

            this.selectedBackendSkillBoxList.Remove(skillSelectionBox);
            this.onSkillDeselectedCallback(skillSelectionBox);

            UpdateBackendSkillListBox();
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SKILL_OFF);
    }

    // swap the selected active skill box
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
                this.selectedActiveSkillBoxList[i] = this.lastSelectedActiveSkillSelectionBox;
            }
            else if (_skillSelectionBox == GetLastSelectedActiveSkillSelectionBox())
            {
                this.selectedActiveSkillBoxList[i] = targetToSwap;
            }
        }

        UpdateActiveSkillListBox();
    }

    // replace the selected active skill
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

                this.selectedActiveSkillBoxList[i] = targetToSwap;
            }
        }

        UpdateActiveSkillListBox();
    }

    // move the selected active skill to first
    public void MoveSelectedSkillToFirst(SkillSelectionBoxV2 targetToMove, List<SkillSelectionBoxV2> selectedSkillSelectionList)
    {
        if (!selectedSkillSelectionList.Contains(targetToMove))
        {
            return;
        }

        for (int i = selectedSkillSelectionList.IndexOf(targetToMove); i > 0; i--)
        {
            selectedSkillSelectionList[i] = selectedSkillSelectionList[i - 1];
            this.gameCharacter.GetSelectedActiveSkillList()[i] = selectedSkillSelectionList[i - 1].GetCharacterSkill();
        }

        selectedSkillSelectionList[0] = targetToMove;
        this.gameCharacter.GetSelectedActiveSkillList()[0] = targetToMove.GetCharacterSkill();

        UpdateActiveSkillListBox();
    }

    // show the skill selection panel
    public void ShowSkillSelectionPanel( SkillType skillType )
    {
        this.gameObject.SetActive( true );
        ShowActiveSkillSelectionList( skillType == SkillType.Active );
        ShowBackendSkillSelectionList( skillType == SkillType.Backend );
    }

    // hide the skill selection panel
    public void HideSkillSelectionPanel()
    {
        this.gameObject.SetActive(false);
    }

    // show the skill info panel
    public void ShowSkillInfoPanel(SkillSelectionBoxV2 skillSelectionBox)
    {
        this.skillInfoPanel.Show(skillSelectionBox);
    }

    // hide the skill info panel
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
        return this.selectedActiveSkillBoxList;
    }

    public List<SkillSelectionBoxV2> GetSelectedBackendSkillList()
    {
        return this.selectedBackendSkillBoxList;
    }

    public Sprite GetActiveSkillBoxSelectBackgroundImage()
    {
        return this.activeSkillBoxSelectBackgroundImage;
    }

    public Sprite GetActiveSkillBoxSelectedBackgroundImage()
    {
        return this.activeSkillBoxSelectedBackgroundImage;
    }

    public Sprite GetActiveSkillBoxUnselectBackgroundImage()
    {
        return this.activeSkillBoxUnselectBackgroundImage;
    }

    public Sprite GetBackendSkillBoxSelectedBackgroundImage()
    {
        return this.backendSkillBoxSelectedBackgroundImage;
    }

    public Sprite GetBackendSkillBoxUnselectBackgroundImage()
    {
        return this.backendSkillBoxUnselectBackgroundImage;
    }

    public Sprite GetRepulseSkillBoxFrameImage()
    {
        return this.repulseSkillBoxFrameImage;
    }

    public Sprite GetDerivedSkillBoxFrameImage()
    {
        return this.derivedSkillBoxFrameImage;
    }

    public Sprite GetCounterSkillBoxFrameImage()
    {
        return this.counterSkillBoxFrameImage;
    }

    public GameCharacter GetGameCharacter()
    {
        return this.gameCharacter;
    }
}
