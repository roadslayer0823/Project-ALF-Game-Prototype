using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using System.Collections;

public class SkillSelectionPanelV2 : MonoBehaviour
{
    [SerializeField] private bool onActiveSkillSlotFollow = false;
    [SerializeField] private GameObject skillSelectionPanelObject = null;

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
    private SkillSelectionBoxV2 defaultDefendSkillSelectionBox = null;
    private SkillSelectionBoxV2 defaultEvadeSkillSelectionBox = null;
    private SkillSelectionBoxV2 genericSkillSelectionBox = null;

    [Header("")]
    [SerializeField] private Button returnButton = null;
    [SerializeField] private SkillInfoPanel skillInfoPanel = null;
    [SerializeField] private PreparationSection preparationSection = null;
    [SerializeField] private ActiveSkillSlotListPanelV2 activeSkillSlotListPanelV2 = null;
    [SerializeField] private BackendSkillSlotListPanel backendSkillSlotListPanel = null;
    [SerializeField] private Animator skillSelectionPanelAnimation = null;
    [SerializeField] private Animator preparationSectionAnimation = null;
    [SerializeField] private Animator attackSkillSlotListPanelAnimation = null;
    [SerializeField] private Animator backendSkillSlotListPanelAnimation = null;

    private BattleUiManager battleUiManager = null;
    private Action<SkillSelectionBoxV2> onSkillSelectedCallback = null;
    private Action<SkillSelectionBoxV2> onSkillDeselectedCallback = null;
    private Action onReturnedCallback = null;
    private bool isAnimationPlayable = false;
    public bool isActiveOpened = false;
    public bool isBackendOpened = false;

    private GameCharacter gameCharacter = null;

    private const string ANIMATION_ID_SHOW_ATTACK_SKILL_SELECTION_PANEL = "ShowAttackSkillSelectionPanel";
    private const string ANIMATION_ID_SHOW_BACKEND_SKILL_SELECTION_PANEL = "ShowBackendSkillSelectionPanel";
    private const string ANIMATION_ID_HIDE_ATTACK_SKILL_SELECTION_PANEL = "HideAttackSkillSelectionPanel";
    private const string ANIMATION_ID_HIDE_BACKEND_SKILL_SELECTION_PANEL = "HideBackendSkillSelectionPanel";
    private const string ANIMATION_ID_SHOW_ATTACK_INFO_PANEL = "ShowAttackInfoPanel";
    private const string ANIMATION_ID_SHOW_BACKEND_INFO_PANEL = "ShowBackendPanel";
    private const string ANIMATION_ID_HIDE_ATTACK_INFO_PANEL = "HideAttackInfoPanel";
    private const string ANIMATION_ID_HIDE_BACKEND_INFO_PANEL = "HideBackendInfoPanel";
    private const string ANIMATION_ID_HIDE_MENU_PANEL = "HideMenuPanel";
    private const string AUDIO_ID_SKILL_OFF = "skill_off";
    private const string AUDIO_ID_ACTIVESKILL_ON = "active_skill_on";
    private const string AUDIO_ID_BACKENDSKILL_ON = "backend_skill_on";
    private const string AUDIO_ID_CLICK = "click";

    public enum SkillType
    {
        None,
        Active,
        Backend
    }

    public void Initialize( BattleUiManager battleUiManager, Action<SkillSelectionBoxV2> onSkillSelectedCallback, Action<SkillSelectionBoxV2> onSkillDeselectedCallback, Action onReturnedCallback )
    {
        this.battleUiManager = battleUiManager;
        this.onSkillSelectedCallback = onSkillSelectedCallback;
        this.onSkillDeselectedCallback = onSkillDeselectedCallback;
        this.onReturnedCallback = onReturnedCallback;

        this.activeSkillSelectionListGO.SetActive( false );
        this.backendSkillSelectionListGO.SetActive( false );
        this.skillInfoPanel.Hide();

        this.activeSkillListBoxButton.onClick.AddListener( OnActiveSkillListBoxButtonClick );
        this.backendSkillListBoxButton.onClick.AddListener( OnBackendSkillListBoxButtonClick );
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
        else if(onActiveSkillSlotFollow)
        {
            SetMiddleActiveSkillFromBattleSkillSlot();
        }

        if (this.backendSkillBoxList.Count == 0)
        {
            InitializeBackendSkillList();
            UpdateBackendSkillListBoxV2();
        }

        ShowSkillSelectionPanel( skillType );
    }

    // when click the active skill list box button
    private void OnActiveSkillListBoxButtonClick()
    {
        if (this.isActiveOpened)
        {
            this.skillSelectionPanelAnimation.Play(ANIMATION_ID_HIDE_ATTACK_SKILL_SELECTION_PANEL);
            this.skillInfoPanel.PlaySkillInfoPanelAnimation(ANIMATION_ID_HIDE_ATTACK_INFO_PANEL);
            this.isActiveOpened = false;
            this.isBackendOpened = true;
        }
        else
        {
            PlayAttackSkillSelectionPanelAnimation();
            ShowActiveSkillSelectionList(true);
            ShowBackendSkillSelectionList(false);
            this.battleUiManager.ShowDarkLayer();
            this.isActiveOpened = true;
            this.isBackendOpened = false;
        }
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
    }

    // when click the backend skill list box button
    private void OnBackendSkillListBoxButtonClick()
    {
        if (this.isBackendOpened)
        {
            this.skillSelectionPanelAnimation.Play(ANIMATION_ID_HIDE_BACKEND_SKILL_SELECTION_PANEL);
            this.skillInfoPanel.PlaySkillInfoPanelAnimation(ANIMATION_ID_HIDE_BACKEND_INFO_PANEL);
            this.isBackendOpened = false;
            this.isActiveOpened = true;
        }
        else
        {
            PlayBackendSkillSelectionPanelAnimation();
            ShowBackendSkillSelectionList(true);
            ShowActiveSkillSelectionList(false);
            this.battleUiManager.ShowDarkLayer();
            this.isBackendOpened = true;
            this.isActiveOpened = false;
        }
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
    }

    public void PlayAttackSkillSelectionPanelAnimation()
    {
        this.skillSelectionPanelAnimation.Play(ANIMATION_ID_SHOW_ATTACK_SKILL_SELECTION_PANEL);
        this.skillInfoPanel.PlaySkillInfoPanelAnimation(ANIMATION_ID_SHOW_ATTACK_INFO_PANEL);
    }

    public void PlayBackendSkillSelectionPanelAnimation()
    {
        this.skillSelectionPanelAnimation.Play(ANIMATION_ID_SHOW_BACKEND_SKILL_SELECTION_PANEL);
        this.skillInfoPanel.PlaySkillInfoPanelAnimation(ANIMATION_ID_SHOW_BACKEND_INFO_PANEL);
    }

    public void HideAttackSkillSelectionPanel()
    {
        this.activeSkillSelectionListGO.SetActive(false);
        this.battleUiManager.ReturnToSkillMenu();
    }

    public void HideBackendSkillSelectionPanel()
    {
        this.backendSkillSelectionListGO.SetActive(false);
        this.battleUiManager.ReturnToSkillMenu();
    }

    public void ShowAttackSkillSelectionBox()
    {
        this.skillSelectionPanelAnimation.Play(ANIMATION_ID_SHOW_BACKEND_SKILL_SELECTION_PANEL);
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
                            SetDefaultDefendSkillSelectionBox(_backendSkillSelectionBox);
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
                            SetDefaultEvadeSkillSelectionBox(_backendSkillSelectionBox);
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
    private void UpdateActiveSkillListBoxUI()
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

    private void UpdateActiveSkillListBoxV2()
    {
        for (int i = 0; i < this.activeSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _activeSkillSelectionBoxInList = this.activeSkillBoxList[i];

            if (_activeSkillSelectionBoxInList.GetCharacterSkill() == null)
            {
                continue;
            }

            for (int j = 0; j < this.gameCharacter.GetSelectedActiveSkillList().Count; j++)
            {
                CharacterSkill _selectedCharacterSkill = this.gameCharacter.GetSelectedActiveSkillList()[j];
                int _selectedCharacterSkillLevel = _selectedCharacterSkill.GetSelectedSkillLevel();

                if (_selectedCharacterSkill.GetSkillData().Id == _activeSkillSelectionBoxInList.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData().SkillId
                    && !this.selectedActiveSkillBoxList.Contains(_activeSkillSelectionBoxInList))
                {
                    _activeSkillSelectionBoxInList.SetCurrentSkillLevel(_selectedCharacterSkillLevel);
                    _activeSkillSelectionBoxInList.GetCharacterSkill().SetSelectedSkillLevel(_selectedCharacterSkillLevel);
                    _activeSkillSelectionBoxInList.UpdateSkillSelectionBoxData();
                    this.selectedActiveSkillBoxList.Add(_activeSkillSelectionBoxInList);
                    break;
                }
            }
        }
        UpdateActiveSkillListBoxUI();
    }

    public void AddSelectedSkillBoxV2(SkillSelectionBoxV2 skillSelectionBox)
    {
        Skill.SkillType _skillType = skillSelectionBox.GetCharacterSkill().GetSkillData().skillType;
        Subskill _subskill = skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData();
        List<CharacterSkill> _backendSkillList = this.gameCharacter.GetSelectedBackendSkillList();

        int defendSkillCount = 0;
        int evadeSkillCount = 0;
        int observeSkillCount = 0;

        for (int i = 0; i < _backendSkillList.Count; i++)
        {
            Subskill _backendSubSkill = _backendSkillList[i].GetCharacterSubskillData().GetSubskillData();

            if (_backendSubSkill.IsDefendingSkill)
            {
                defendSkillCount += 1;
            }
            else if (_backendSubSkill.IsEvadingSkill)
            {
                evadeSkillCount += 1;
            }
            else if (_backendSubSkill.IsObservingSkill)
            {
                observeSkillCount += 1;
            }
        }

        if (_skillType == Skill.SkillType.active)
        {
            this.gameCharacter.AddSelectedSkill(skillSelectionBox.GetCharacterSkill());
            this.onSkillSelectedCallback(skillSelectionBox);
            UpdateActiveSkillListBoxV2();
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_ACTIVESKILL_ON);
        }

        else if (_skillType == Skill.SkillType.backend)
        {
            if (GetGenericSkillSelectionBox() == null)
            {
                SetGenericSkillSelectionBox(skillSelectionBox);
            }
            else if (_subskill.IsDefendingSkill)
            {
                if (observeSkillCount > 0)
                {
                    RemoveSelectedSkillBox(GetDefaultDefendSkillSelectionBox());
                    SetDefaultDefendSkillSelectionBox(skillSelectionBox);

                }
                else if (evadeSkillCount > 1)
                {
                    RemoveSelectedSkillBox(GetGenericSkillSelectionBox(),true);
                    SetGenericSkillSelectionBox(skillSelectionBox);
                }
                else
                {
                    SetDefaultDefendSkillSelectionBox(skillSelectionBox);
                }
            }
            else if (_subskill.IsEvadingSkill)
            {
                if (observeSkillCount > 0)
                {
                    RemoveSelectedSkillBox(GetDefaultEvadeSkillSelectionBox());
                    SetDefaultEvadeSkillSelectionBox(skillSelectionBox);
                }
                else if (defendSkillCount > 1)
                {
                    RemoveSelectedSkillBox(GetGenericSkillSelectionBox(),true);
                    SetGenericSkillSelectionBox(skillSelectionBox);
                }
                else
                {
                    SetDefaultEvadeSkillSelectionBox(skillSelectionBox);
                }

            }
            else if (_subskill.IsObservingSkill)
            {
                if (GetGenericSkillSelectionBox() != null || defendSkillCount > 1 || evadeSkillCount > 1)
                {
                    RemoveSelectedSkillBox(GetGenericSkillSelectionBox(),true);
                }
                SetGenericSkillSelectionBox(skillSelectionBox);
            }
            else
            {
                Debug.Log("Unknown subskill");
            }
            this.gameCharacter.AddSelectedSkill(skillSelectionBox.GetCharacterSkill());
            this.onSkillSelectedCallback(skillSelectionBox);
            UpdateBackendSkillListBoxV2();
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BACKENDSKILL_ON);
        }
    }

    public void UpdateBackendSkillListBoxV2()
    {
        this.selectedBackendSkillBoxList.Clear();

        for (int i = 0; i < this.backendSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _backendSkillSelectionBox = this.backendSkillBoxList[i];
            List<CharacterSkill> _selectedCharacterSkill = this.gameCharacter.GetSelectedBackendSkillList();

            for (int j = 0; j < _selectedCharacterSkill.Count; j++)
            {
                CharacterSkill _backendSkillSelectionBoxCharacterSkill = _backendSkillSelectionBox.GetCharacterSkill();

                if (_backendSkillSelectionBoxCharacterSkill != null
                    && _selectedCharacterSkill[j].GetSkillData().Id == _backendSkillSelectionBoxCharacterSkill.GetCharacterSubskillData().GetSubskillData().SkillId)
                {
                    _backendSkillSelectionBox.UpdateSkillIcon(true);
                    _backendSkillSelectionBox.SetIsSelected(true);
                    this.selectedBackendSkillBoxList.Add(_backendSkillSelectionBox);
                }
                else
                {
                    _backendSkillSelectionBox.SetSkillSelectedImage(null);
                    _backendSkillSelectionBox.SetSkillBoxFrame(this.backendSkillBoxUnselectBackgroundImage);
                }
            }
        }
        UpdateBackendSkillSmallBox();
    }

    public void UpdateBackendSkillSmallBox()
    {
        this.backendDefenceBoxIcon.gameObject.SetActive(false);
        this.backendEvasionBoxIcon.gameObject.SetActive(false);
        this.backendGenericBoxIcon.gameObject.SetActive(false);
        int defendSkillCount = 0;
        int evadeSkillCount = 0;
        List<CharacterSkill> _backendSkillList = this.gameCharacter.GetSelectedBackendSkillList();

        for (int i = 0; i < _backendSkillList.Count; i++)
        {
            Subskill _subskill = _backendSkillList[i].GetCharacterSubskillData().GetSubskillData();

            if (_subskill.IsDefendingSkill)
            {
                defendSkillCount += 1;
            }
            else if (_subskill.IsEvadingSkill)
            {
                evadeSkillCount += 1;
            }
        }

        for (int i = 0; i < this.selectedBackendSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.selectedBackendSkillBoxList[i];
            Subskill _subskillData = _skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData();

            _skillSelectionBox.SetSkillBoxFrame(this.backendSkillBoxSelectedBackgroundImage);

            if (_subskillData.IsObservingSkill)
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendGenericBoxSelectedImage);
                this.backendGenericBoxIcon.sprite = this.backendGenericBoxImage;
                this.backendGenericBoxIcon.gameObject.SetActive(true);
            }
            else if (_subskillData.IsDefendingSkill)
            {
                this.backendDefenceBoxIcon.sprite = this.backendDefenceBoxImage;

                if(defendSkillCount > 1)
                {
                    this.backendGenericBoxIcon.sprite = this.backendDefenceBoxImage;
                    this.backendGenericBoxIcon.gameObject.SetActive(true);
                }
                this.backendDefenceBoxIcon.gameObject.SetActive(true);
                _skillSelectionBox.SetSkillSelectedImage(this.backendDefenceBoxSelectedImage);
            }
            else if (_subskillData.IsEvadingSkill)
            {
                this.backendEvasionBoxIcon.sprite = this.backendEvasionBoxImage;

                if (evadeSkillCount > 1)
                {
                    this.backendGenericBoxIcon.sprite = this.backendEvasionBoxImage;
                    this.backendGenericBoxIcon.gameObject.SetActive(true);
                }
                this.backendEvasionBoxIcon.gameObject.SetActive(true);
                _skillSelectionBox.SetSkillSelectedImage(this.backendEvasionBoxSelectedImage);
            }          
        }
    }

    // remove selected skill box, check if need to replace when default skill get selected
    public void RemoveSelectedSkillBox(SkillSelectionBoxV2 skillSelectionBox, bool needToChangeSlot = false)
    {
        CharacterSkill _characterSkill = skillSelectionBox.GetCharacterSkill();
        Skill.SkillType _skillType = _characterSkill.GetSkillData().skillType;
        Subskill _subskill = skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData();

        this.gameCharacter.RemoveSelectedSkill(skillSelectionBox.GetCharacterSkill());
        this.onSkillDeselectedCallback(skillSelectionBox);
        skillSelectionBox.UpdateSkillIcon(false);
        skillSelectionBox.SetIsSelected(false);

        if (_skillType == Skill.SkillType.active)
        {
            this.selectedActiveSkillBoxList.Remove(skillSelectionBox);
            UpdateActiveSkillListBoxUI();
        }
        else if (_skillType == Skill.SkillType.backend)
        {
            if(needToChangeSlot)
            {
                if (_subskill.IsDefendingSkill && _characterSkill == GetDefaultDefendSkillSelectionBox().GetCharacterSkill())
                {
                    Debug.Log("set generic defend to default defend when default get remove");
                    SetDefaultDefendSkillSelectionBox(GetGenericSkillSelectionBox());
                }
                else if (_subskill.IsEvadingSkill && _characterSkill == GetDefaultEvadeSkillSelectionBox().GetCharacterSkill())
                {
                    Debug.Log("set generic evade to default evade when default get remove");
                    SetDefaultEvadeSkillSelectionBox(GetGenericSkillSelectionBox());
                }
                SetGenericSkillSelectionBox(null);
            }
            this.selectedBackendSkillBoxList.Remove(skillSelectionBox);
            UpdateBackendSkillListBoxV2();
        }
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SKILL_OFF);
    }

    public bool IsCharacterAllowedToAddOrRemoveSkill(CharacterSkill characterSkill, bool isAdding = true)
    {
        int _selectedActiveSkillsCount = this.gameCharacter.GetSelectedActiveSkillList().Count;
        int _selectedBackendSkillsCount = this.gameCharacter.GetSelectedBackendSkillList().Count;
        int _defendSkillCount = 0;
        int _evadeSkillCount = 0;
        int _observeSkillCount = 0;

        Skill.SkillType _skillType = characterSkill.GetSkillData().skillType;
        Subskill _subskill = characterSkill.GetCharacterSubskillData().GetSubskillData();
        List<CharacterSkill> _backendSkillList = this.gameCharacter.GetSelectedBackendSkillList();

        for (int i = 0; i < _backendSkillList.Count; i++)
        {
            Subskill _backendSubSkill = _backendSkillList[i].GetCharacterSubskillData().GetSubskillData();

            if (_backendSubSkill.IsDefendingSkill)
            {
                _defendSkillCount += 1;
            }
            if (_backendSubSkill.IsEvadingSkill)
            {
                _evadeSkillCount += 1;
            }
            if (_backendSubSkill.IsObservingSkill)
            {
                _observeSkillCount += 1;
            }
        }

        if (_skillType == Skill.SkillType.active)
        {
            if(isAdding)
            {
                return _selectedActiveSkillsCount < 3;
            }
            else
            {
                return _selectedActiveSkillsCount > 0;
            }
        }

        else if (_skillType == Skill.SkillType.backend)
        {
            if (isAdding)
            {
                if (_subskill.IsDefendingSkill) return _defendSkillCount < 2;

                else if (_subskill.IsEvadingSkill) return _evadeSkillCount < 2;

                else return _subskill.IsObservingSkill;
            }
            else if (_selectedBackendSkillsCount > 2)
            {
                if (_subskill.IsDefendingSkill) return _defendSkillCount > 1;

                else if (_subskill.IsEvadingSkill) return _evadeSkillCount > 1;

                else return _subskill.IsObservingSkill;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
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
        UpdateActiveSkillListBoxUI();
    }

    public void SetMiddleActiveSkillFromBattleSkillSlot()
    {
        SkillSelectionBoxV2 middleSkillBox = null;
        CharacterSkill middleSkill = this.battleUiManager.GetActiveSkillSlotListPanelV2().GetMiddleSkillSlot().GetSelectedSkill();
        
        for (int i = 0; i < this.GetSelectedActiveSkillList().Count; i++)
        {
            if (middleSkill.GetSkillData().Id == this.GetSelectedActiveSkillList()[i].GetCharacterSkill().GetSkillData().Id)
            {
                middleSkillBox = this.GetSelectedActiveSkillList()[i];
            }
            else
            {
                Debug.Log("Skill not available");
            }
        }

        for (int i = this.GetSelectedActiveSkillList().IndexOf(middleSkillBox); i > 0; i--)
        {
            this.GetSelectedActiveSkillList()[i] = this.GetSelectedActiveSkillList()[i - 1];
            this.gameCharacter.GetSelectedActiveSkillList()[i] = this.GetSelectedActiveSkillList()[i - 1].GetCharacterSkill();
        }
        this.GetSelectedActiveSkillList()[0] = middleSkillBox;
        this.gameCharacter.GetSelectedActiveSkillList()[0] = middleSkill;
        UpdateActiveSkillListBoxUI();
    }


    // show the skill selection panel
    public void ShowSkillSelectionPanel( SkillType skillType )
    {
        this.gameObject.SetActive( true );
        SetIsAnimationPlayable(true);
        ShowActiveSkillSelectionList( skillType == SkillType.Active );
        ShowBackendSkillSelectionList( skillType == SkillType.Backend );
    }

    // hide the skill selection panel
    public void HideSkillSelectionPanel()
    {
        this.gameObject.SetActive(false);
        SetIsAnimationPlayable(false);
    }

    public void StartHideSkillSelectionPanel()
    {
        StartCoroutine(PlayMenuPanelAnimation());
    }

    public IEnumerator PlayMenuPanelAnimation()
    {
        if (this.activeSkillSelectionListGO.activeSelf)
        {
            this.skillSelectionPanelAnimation.Play(ANIMATION_ID_HIDE_ATTACK_SKILL_SELECTION_PANEL);
            this.skillInfoPanel.PlaySkillInfoPanelAnimation(ANIMATION_ID_HIDE_ATTACK_INFO_PANEL);
            yield return new WaitForSeconds(0.25f);
            this.preparationSection.ShowSkillMenu();
            yield return new WaitForSeconds(0.1f);
            this.preparationSectionAnimation.Play(ANIMATION_ID_HIDE_MENU_PANEL);
        }
        else if (this.backendSkillListBoxGO.activeSelf)
        {
            this.skillSelectionPanelAnimation.Play(ANIMATION_ID_HIDE_BACKEND_SKILL_SELECTION_PANEL);
            this.skillInfoPanel.PlaySkillInfoPanelAnimation(ANIMATION_ID_HIDE_BACKEND_INFO_PANEL);
            yield return new WaitForSeconds(0.2f);
            this.preparationSection.ShowSkillMenu();
            yield return new WaitForSeconds(0.1f);
            this.preparationSectionAnimation.Play(ANIMATION_ID_HIDE_MENU_PANEL);
        }
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

    public void SetIsAnimationPlayable(bool isPlayable)
    {
        this.isAnimationPlayable = isPlayable;
    }

    public bool GetIsAnimationPlayable()
    {
        return this.isAnimationPlayable;
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

    public SkillSelectionBoxV2 GetDefaultDefendSkillSelectionBox()
    {
        return this.defaultDefendSkillSelectionBox;
    }

    public void SetDefaultDefendSkillSelectionBox(SkillSelectionBoxV2 skillSelection)
    {
        this.defaultDefendSkillSelectionBox = skillSelection;
    }

    public SkillSelectionBoxV2 GetDefaultEvadeSkillSelectionBox()
    {
        return this.defaultEvadeSkillSelectionBox;
    }

    public void SetDefaultEvadeSkillSelectionBox(SkillSelectionBoxV2 skillSelection)
    {
        this.defaultEvadeSkillSelectionBox = skillSelection;
    }

    public SkillSelectionBoxV2 GetGenericSkillSelectionBox()
    {
        return this.genericSkillSelectionBox;
    }

    public void SetGenericSkillSelectionBox(SkillSelectionBoxV2 skillSelection)
    {
        this.genericSkillSelectionBox = skillSelection;
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

    public void SetActiveSkillSlotSequenceFollowing(bool isOn)
    {
        onActiveSkillSlotFollow = isOn;
    }
}
