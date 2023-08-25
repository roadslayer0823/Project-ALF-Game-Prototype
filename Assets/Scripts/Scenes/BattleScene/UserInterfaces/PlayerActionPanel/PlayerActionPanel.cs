using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject preparationSection = null;
    [SerializeField] private GameObject battleSection = null;
    [SerializeField] private Button executeButton = null;
    [SerializeField] private SkillActionButton[] skillActionButtons = new SkillActionButton[ 0 ];

    [Header("QTE Button")]
    [SerializeField] private Button qteButton = null;
    [SerializeField] private TextMeshProUGUI qteType = null;
    [SerializeField] private TextMeshProUGUI skillName = null;
    [SerializeField] private TextMeshProUGUI strengthValue = null;
    [SerializeField] private TextMeshProUGUI accuracyValue = null;
    [SerializeField] private TextMeshProUGUI evasionValue = null;
    [SerializeField] private GameObject strength = null;
    [SerializeField] private GameObject accuracy = null;
    [SerializeField] private GameObject evasion = null;

    private GameCharacter selectedGameCharacter = null;
    private Action onExecuteButtonClickedCallback = null;
    private QTEActionType qteActionType = QTEActionType.None;

    [Header("SkillSelectionPanel button")]
    [SerializeField] private Button activeSkillButton = null;
    [SerializeField] private Button backendSkillButton = null;
    private Action onActiveSkillButtonClickedCallback = null;
    private Action onBackendSkillButtonClickedCallback = null;

    public enum QTEActionType
    {
        None,
        Repulse,
        Derive,
        Counter
    }

    public void Initialize( Action onExecuteButtonClickedCallback, Action onActiveSkillButtonClicked, Action onBackendSkillButtonClicked)
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;

        this.qteButton.onClick.AddListener( OnQTEButtonClicked );

        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            skillActionButtons[ i ].Initialize( OnSkillActionButtonClicked );
        }

        this.activeSkillButton.onClick.AddListener(OnActiveSkillButtonClicked);
        this.backendSkillButton.onClick.AddListener(OnBackendSkillButtonClicked);
        this.onActiveSkillButtonClickedCallback = onActiveSkillButtonClicked;
        this.onBackendSkillButtonClickedCallback = onBackendSkillButtonClicked;
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

        switch ( this.qteActionType )
        {
            case QTEActionType.Repulse:

                this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Repulse );

                break;

            case QTEActionType.Derive:

                this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Derive );

                break;

            case QTEActionType.Counter:

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

    public void ShowQTEActionButton(QTEActionType qteActionType, CharacterSkill qteSkill)
    {
        HideQTEActionButton();

        this.qteActionType = qteActionType;
        
        switch (qteActionType)
        {
            case QTEActionType.Repulse:

                this.qteType.SetText("迎擊");
                SetupQTEActionButton(qteSkill);
                break;

            case QTEActionType.Derive:

                this.qteType.SetText("派生");
                SetupQTEActionButton(qteSkill);
                break;

            case QTEActionType.Counter:

                this.qteType.SetText("反擊");
                SetupQTEActionButton(qteSkill);
                break;
        }
    }

    public void HideQTEActionButton()
    {
        this.qteButton.gameObject.SetActive(false);
    }

    private void SetupQTEActionButton(CharacterSkill qteSkill)
    {
        this.qteButton.gameObject.SetActive(true);

        CharacterSubskill _characterSubskill = qteSkill.GetCharacterSubskillData();
        int _strengthValue = _characterSubskill.GetSubskillData().Strength;
        int _accuracyValue = _characterSubskill.GetSubskillData().Accuracy;
        int _evasionValue =_characterSubskill.GetSubskillData().Evasion;

        this.skillName.SetText(_characterSubskill.GetSubskillData().DisplayName);

        if (_strengthValue > 1)
        {
            this.strength.gameObject.SetActive(true);
            this.strengthValue.SetText("+" + (_strengthValue - 1).ToString());
        }
        else
        {
            this.strength.gameObject.SetActive(false);
        }

        if (_accuracyValue > 1)
        {
            this.accuracy.gameObject.SetActive(true);
            this.accuracyValue.SetText("+" + (_accuracyValue - 1).ToString());
        }
        else
        {
            this.accuracy.gameObject.SetActive(false);
        }

        if (_evasionValue > 1)
        {
            this.evasion.gameObject.SetActive(true);
            this.evasionValue.SetText("+" + (_evasionValue - 1).ToString());
        }
        else
        {
            this.evasion.gameObject.SetActive(false);
        }
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

    public void UpdateSkillActionButtons( bool canDefend, bool canEvade )
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            CharacterSkill _skill = _skillActionButton.GetSelectedSkill();
            if (_skill != null && _skill.IsSkillAvailable( canDefend, canEvade ))
            {
                _skillActionButton.EnableActionButton();
            }
            else
            {
                _skillActionButton.DisableActionButton();
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
            this.skillActionButtons[ i ].DisableActionButton();
        }
    }

    // Show active skill selection panel
    private void OnActiveSkillButtonClicked()
    {
        if (this.onActiveSkillButtonClickedCallback != null)
        {
            this.activeSkillButton.interactable = false;
            this.backendSkillButton.interactable = true;

            this.onActiveSkillButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onActiveSkillButtonClicked' is not assigned.");
        }
    }

    // Show backend skill selection panel
    private void OnBackendSkillButtonClicked()
    {
        if (this.onBackendSkillButtonClickedCallback != null)
        {
            this.activeSkillButton.interactable = true;
            this.backendSkillButton.interactable = false;

            this.onBackendSkillButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onBackendSkillButtonClicked' is not assigned.");
        }
    }

    public void EnableActiveSkillButton()
    {
        this.activeSkillButton.interactable = true;
    }

    public void DisableActiveSkillButton()
    {
        this.activeSkillButton.interactable = false;
    }

    public void EnableBackendSkillButton()
    {
        this.backendSkillButton.interactable = true;
    }

    public void DisableBackendSkillButton()
    {
        this.backendSkillButton.interactable = false;
    }
}
