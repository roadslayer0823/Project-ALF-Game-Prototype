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
    [SerializeField] private GameObject[] backendSkillBoxPositions = null;
    [SerializeField] private SkillSelectionBoxV2 backendSkillBoxPrefab = null;
    [SerializeField] private Button backendSkillListBoxButton = null;
    [SerializeField] private Image backendDefenceBoxIcon = null;
    [SerializeField] private Image backendEvasionBoxIcon = null;
    [SerializeField] private Image backendGenericBoxIcon = null;
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

    public void Show(GameCharacter gameCharacter)
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

                        if (_selectedCharacterSkill.GetSkillData().GroupName == _activeSkillSelectionBox.GetCharacterSkill().GetSkillData().GroupName)
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

                        if (_selectedCharacterSkill.GetSkillData().GroupName == _backendSkillSelectionBox.GetCharacterSkill().GetSkillData().GroupName)
                        {
                            this.selectedBackendSkillBoxList.Add(_backendSkillSelectionBox);
                            break;
                        }
                    }
                }

                UpdateBackendSkillListBox();
            }
        }

        ShowSkillSelectionPanel();
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

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
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

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
    }

    private void OnReturnButtonClick()
    {
        this.onReturnedCallback?.Invoke();
    }

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
            _activeSkillListBoxBackground.gameObject.SetActive(!show);
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

        for (int i = 0; i < this.selectedBackendSkillBoxList.Count; i++)
        {
            SkillSelectionBoxV2 _skillSelectionBox = this.selectedBackendSkillBoxList[i];
            Subskill _subskillData = _skillSelectionBox.GetCharacterSkill().GetCharacterSubskillData().GetSubskillData();

            _skillSelectionBox.SetSkillBoxFrame(this.backendSkillBoxSelectedBackgroundImage);

            if (_subskillData.IsDefendingSkill)
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendDefenceBoxSelectedImage);

                this.backendDefenceBoxIcon.gameObject.SetActive(true);
            }
            else if (_subskillData.IsEvadingSkill)
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendEvasionBoxSelectedImage);

                this.backendEvasionBoxIcon.gameObject.SetActive(true);
            }
            else
            {
                _skillSelectionBox.SetSkillSelectedImage(this.backendGenericBoxSelectedImage);

                this.backendGenericBoxIcon.gameObject.SetActive(true);
            }
        }
    }

    public void AddSelectedSkilBox(SkillSelectionBoxV2 skillSelectionBox)
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

            this.selectedBackendSkillBoxList.Add(skillSelectionBox);
            this.onSkillSelectedCallback(skillSelectionBox);

            UpdateBackendSkillListBox();
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_SKILL_ON);
    }

    public void RemoveSelectedSkilBox(SkillSelectionBoxV2 skillSelectionBox)
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

    public void ShowSkillSelectionPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void HideSkillSelectionPanel()
    {
        this.gameObject.SetActive(false);
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
        return this.selectedActiveSkillBoxList;
    }

    public List<SkillSelectionBoxV2> GetSelectedBackendSkillList()
    {
        return this.selectedBackendSkillBoxList;
    }

    public Sprite GetSkillBoxSelectBackgroundImage()
    {
        return this.activeSkillBoxSelectBackgroundImage;
    }

    public Sprite GetSkillBoxSelectedBackgroundImage()
    {
        return this.activeSkillBoxSelectedBackgroundImage;
    }

    public Sprite GetSkillBoxUnselectBackgroundImage()
    {
        return this.activeSkillBoxUnselectBackgroundImage;
    }

    public Sprite GetRepulseSkillBoxFrameImage()
    {
        return this.repulseSkillBoxFrameImage;
    }

    public Sprite GetDerivedSkillBoxFrameImage()
    {
        return this.derivedSkillBoxFrameImage;
    }

    public GameCharacter GetGameCharacter()
    {
        return this.gameCharacter;
    }
}
