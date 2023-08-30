using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Skill = DatabaseManager.Skill;

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
    }

    public void SetSelectedGameCharacter( GameCharacter selectedGameCharacter )
    {
        this.selectedGameCharacter = selectedGameCharacter;
    }

    public void ClickOnExecuteButton()
    {
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
        DisableQTEAndSkillActionButtons();

        Skill _skillData = this.qteSkill.GetSkillData();

        switch ( _skillData.skillType )
        {
            case Skill.SkillType.repulse:

                this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Repulse );

                break;

            case Skill.SkillType.derived:

                this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Derive );

                break;

            case Skill.SkillType.counter:

                this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Counter );

                break;
        }
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

    public void ShowQTEActionButton(CharacterSkill qteSkill)
    {   
        HideQTEActionButton();

        this.qteSkill = qteSkill;
        this.qteButton.SetupQTESkillActionButton(qteSkill);
    }

    public void HideQTEActionButton()
    {
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
    }

    public void HideSkillActionButtons()
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            this.skillActionButtons[ i ].gameObject.SetActive( false );
        }
    }

    public void UpdateSkillActionButtons( bool canDefend, bool canEvade, float countdownTime )
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            CharacterSkill _skill = _skillActionButton.GetSelectedSkill();
            if (_skill != null)
            {
                if (_skill.IsSkillAvailable( canDefend, canEvade ))
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

    private void OnSkillActionButtonClicked( CharacterSkill skill )
    {
        DisableQTEAndSkillActionButtons();

        this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Backend );
        this.selectedGameCharacter.SetCurrentSkill( skill );
    }

    public void DisableQTEAndSkillActionButtons()
    {
        HideQTEActionButton();

        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            if (_skillActionButton.GetSelectedSkill() != null)
            {
                _skillActionButton.DisableActionButton();
            }
        }
    }

    private void OnShowSkillSelectionPanelButtonClicked()
    {
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
    private void ShowActiveSkillSelectionPanel()
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
        this.skillSelectionPanelButtonText.SetText("Backend Skill \n後台技能");
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
        this.skillSelectionPanelButtonText.SetText("Active Skill \n主動技能");
    }
}
