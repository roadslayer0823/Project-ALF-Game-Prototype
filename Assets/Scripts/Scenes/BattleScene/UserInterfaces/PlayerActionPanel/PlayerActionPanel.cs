using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;

public class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject preparationSection = null;
    [SerializeField] private GameObject battleSection = null;
    [SerializeField] private Button executeButton = null;
    [SerializeField] private SkillActionButton[] skillActionButtons = new SkillActionButton[ 0 ];

    [Header("QTE Button")]
    [SerializeField] private SkillActionButton qteButton = null;

    private GameCharacter selectedGameCharacter = null;
    private Action onExecuteButtonClickedCallback = null;
    private CharacterSkill qteSkill = null;

    [Header("SkillSelectionPanel button")]
    [SerializeField] private Button showSkillSelectionPanelButton = null;
    [SerializeField] private TextMeshProUGUI skillSelectionPanelButtonText = null;
    private Action onActiveSkillButtonClickedCallback = null;
    private Action onBackendSkillButtonClickedCallback = null;
    private bool isShowingActiveSkillSelectionPanelNext = false;

    private const string AUDIO_ID_CLICK = "click";
    private const string AUDIO_ID_EXECUTE = "execute";
    private const string AUDIO_ID_POPUP = "popup";
    private const string AUDIO_ID_PASSIVE_FLASH = "passive_flash";

    public enum SkillActionButtonType
    {
        None,
        Defense,
        Evasion,
        Observation
    }

    public void Initialize( Action onExecuteButtonClickedCallback, Action onShowActiveSkillSelectionPanelCallback, Action onShowBackendSkillSelectionPanelCallback)
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;

        this.qteButton.SetOnClick( OnQTEButtonClicked );

        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            skillActionButtons[ i ].Initialize( OnSkillActionButtonClicked );
        }

        this.showSkillSelectionPanelButton.onClick.AddListener(OnShowSkillSelectionPanelButtonClicked);
        this.onActiveSkillButtonClickedCallback = onShowActiveSkillSelectionPanelCallback;
        this.onBackendSkillButtonClickedCallback = onShowBackendSkillSelectionPanelCallback;

        this.skillSelectionPanelButtonText?.SetText("Backend Skill \n後台技能");
    }

    public void SetSelectedGameCharacter( GameCharacter selectedGameCharacter )
    {
        this.selectedGameCharacter = selectedGameCharacter;
    }

    public void ClickOnExecuteButton()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_EXECUTE);

        DisableExecuteButton();

        if (this.onExecuteButtonClickedCallback != null)
        {
            this.onExecuteButtonClickedCallback();
        }
        else
        {
            Debug.Log( "The value for 'onExecuteButtonClickedCallback' is not assigned." );
        }
    }

    private void OnQTEButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        DisableQTEAndSkillActionButtons( true );
        this.selectedGameCharacter.SetCurrentSkill( this.qteSkill );
    }

    public void ShowPreparationSection()
    {
        this.preparationSection.SetActive( true );
    }

    public void HidePreparationSection()
    {
        this.preparationSection.SetActive( false );
    }

    public void ShowBattleSection()
    {
        this.battleSection.SetActive( true );
    }

    public void HideBattleSection()
    {
        this.battleSection.SetActive( false );
    }

    public void EnableExecuteButton()
    {
        this.executeButton.interactable = true;
    }

    public void DisableExecuteButton()
    {
        this.executeButton.interactable = false;
    }

    public void ShowQTEActionButton( CharacterSkill qteSkill, float countdownTime, bool hasSpecialIcon = false )
    {
        HideQTEActionButton();

        this.qteSkill = qteSkill;

        if (qteSkill != null)
        {
            this.qteButton.SetupQTESkillActionButton( qteSkill );
            this.qteButton.GetCountdownTimer().StartCountdownTimer( countdownTime );

            if (hasSpecialIcon)
            {
                this.qteButton.ShowSpecialIcon();
            }
            else
            {
                this.qteButton.HideSpecialIcon();
            }

            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_PASSIVE_FLASH );
        }
    }

    public void HideQTEActionButton()
    {
        this.qteButton.GetCountdownTimer().StopCountdownTimer();
        this.qteButton.gameObject.SetActive(false);
    }

    public void ShowSkillActionButtons( CharacterSkill[] skills )
    {
        HideSkillActionButtons();

        for (int i = 0; i < skills.Length; i++)
        {
            if (i < this.skillActionButtons.Length)
            {
                SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
                _skillActionButton.SetSelectedSkill( skills[ i ] );
                _skillActionButton.gameObject.SetActive( true );
            }
        }

        if (skills.Length > 0)
        {
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_PASSIVE_FLASH);
        }
    }

    public void HideSkillActionButtons()
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            this.skillActionButtons[ i ].gameObject.SetActive( false );
        }
    }

    public void UpdateSkillActionButtons( bool canDefend, bool canEvade, bool canObserve, float countdownTime )
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            CharacterSkill _skill = _skillActionButton.GetSelectedSkill();
            if (_skill != null)
            {
                if (_skill.IsSkillAvailable( canDefend, canEvade, canObserve ))
                {
                    _skillActionButton.EnableActionButton( countdownTime );
                }
                else
                {
                    _skillActionButton.DisableActionButton();
                }
            }
        }
    }

    public void UpdateSkillActionButtons( SkillActionButtonType skillActionButtonType, bool isActionButtonEnabled, float countdownTime = 0.0f )
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            CharacterSkill _skill = _skillActionButton.GetSelectedSkill();
            if (_skill != null)
            {
                Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();

                if (( skillActionButtonType == SkillActionButtonType.Defense && _subskillData.IsDefendingSkill )
                    || ( skillActionButtonType == SkillActionButtonType.Evasion && _subskillData.IsEvadingSkill )
                    || ( skillActionButtonType == SkillActionButtonType.Observation && _subskillData.IsObservingSkill ))
                {
                    if (isActionButtonEnabled && selectedGameCharacter.IsAbleToUseBackendSkill( _skill ))
                    {
                        _skillActionButton.EnableActionButton( countdownTime );
                    }
                    else
                    {
                        _skillActionButton.DisableActionButton();
                    }
                }
            }
        }
    }

    private void OnSkillActionButtonClicked( SkillActionButton skillActionButton )
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        CharacterSkill _skill = skillActionButton.GetSelectedSkill();
        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        if (_subskillData.IsDefendingSkill || _subskillData.IsEvadingSkill)
        {
            this.selectedGameCharacter.SetCurrentSkill( _skill );
            DisableQTEAndSkillActionButtons( true );
        }
        else if (_subskillData.IsObservingSkill)
        {
            this.selectedGameCharacter.SetCurrentObservingSkill( _skill, true );
            skillActionButton.DisableActionButton();
        }
    }

    public void DisableQTEAndSkillActionButtons( bool ignoreObservedSkillButton = false )
    {
        HideQTEActionButton();

        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            CharacterSkill _skill = _skillActionButton.GetSelectedSkill();
            if (_skill != null)
            {
                bool _needToDisableActionButton = true;

                if (ignoreObservedSkillButton && _skill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                {
                    _needToDisableActionButton = false;
                }

                if (_needToDisableActionButton)
                {
                    _skillActionButton.DisableActionButton();
                }
            }
        }
    }

    private void OnShowSkillSelectionPanelButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_POPUP);

        if (this.isShowingActiveSkillSelectionPanelNext)
        {
            ShowActiveSkillSelectionPanel();
        }
        else
        {
            ShowBackendSkillSelectionPanel();
        }
    }

    // Show active skill selection panel
    public void ShowActiveSkillSelectionPanel()
    {
        if (this.onActiveSkillButtonClickedCallback != null)
        {
            this.onActiveSkillButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onActiveSkillButtonClicked' is not assigned.");
        }

        this.isShowingActiveSkillSelectionPanelNext = false;
        this.skillSelectionPanelButtonText?.SetText("Backend Skill \n後台技能");
    }

    // Show backend skill selection panel
    private void ShowBackendSkillSelectionPanel()
    {
        if (this.onBackendSkillButtonClickedCallback != null)
        {
            this.onBackendSkillButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onBackendSkillButtonClicked' is not assigned.");
        }

        this.isShowingActiveSkillSelectionPanelNext = true;
        this.skillSelectionPanelButtonText?.SetText("Active Skill \n主動技能");
    }
}
