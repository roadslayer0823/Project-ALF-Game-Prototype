using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SkillType = BattleSkillManager.SkillType;
using System.Collections;

public class BattleUiManager : MonoBehaviour
{
    [System.Obsolete][SerializeField] private PlayerActionPanel playerActionPanel = null;
    [System.Obsolete][SerializeField] private SkillSelectionPanel skillSelectionPanel = null;
    [System.Obsolete][SerializeField] private SkillSlotListPanel skillSlotListPanel = null;
    [System.Obsolete][SerializeField] private ATLSlotListPanel atlSlotListPanel = null;
    [SerializeField] private CharacterInfoPanel characterInfoPanel = null;
    [SerializeField] private BattleResultPanel battleResultPanel = null;
    //[SerializeField] private TMP_Text instructionLabel = null;
    [SerializeField] private TMP_Text messageLabel = null;

    [Header( "Version 2" )]
    [SerializeField] private ScreenSizeController screenSizeController = null;
    [SerializeField] private PreparationSection preparationSection = null;
    [SerializeField] private SkillSelectionPanelV2 skillSelectionPanelV2 = null;
    [SerializeField] private SkillInfoPanel skillInfoPanel = null;
    [SerializeField] private ActiveSkillSlotListPanelV2 activeSkillSlotListPanelV2 = null;
    [SerializeField] private BackendSkillSlotListPanel backendSkillSlotListPanel = null;
    [SerializeField] private BattleLog battleLog = null;
    [System.Obsolete][SerializeField] private ATLSlotListPanelV2 atlSlotListPanelV2 = null;
    [SerializeField] private PlayerDashboard playerDashboard = null;
    [SerializeField] private EnemyCharacterInfoBox_UI_V2 enemyCharacterInfoBox = null;
    [SerializeField] private SkillPromptPanelV2 skillPromptPanel = null;
    [SerializeField] private GameObject enemyCharacterInfoBoxUI = null;
    [SerializeField] private GameObject enemyCharacterInfoBoxHUD = null;
    [SerializeField] private GameObject darkLayer = null;
    [SerializeField] private Animator battleStartUiAnimator = null;
    [SerializeField] private TMP_Text turnText = null;

    [Header( "Debug" )]
    [SerializeField] private EnemyDebugMenuPanel enemyDebugMenuPanel = null;

    private BattleGameManager battleGameManager = null;
    private GameCharacter selectedGameCharacter = null;
    private CharacterSkill selectedSkill = null;

    // Version 2
    private ATLSlotListPanelV3 atlSlotListPanelV3 = null;
    private CharacterInfoPanelV2 characterInfoPanelV2 = null;
    private const string AUDIO_ID_BACK = "back";

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;

        this.screenSizeController.Initialize();
        Canvas.ForceUpdateCanvases();

        if (this.preparationSection == null)
        {
            this.playerActionPanel.Initialize( OnExecuteButtonClicked, ShowActiveSkillSelectionPanel, ShowBackendSkillSelectionPanel );
        }
        else
        {
            this.preparationSection.Initialize( this, OnExecuteButtonClicked, ShowActiveSkillSelectionPanel, ShowBackendSkillSelectionPanel );

            if (this.playerDashboard != null)
            {
                this.preparationSection.HideExecuteButton();
            }
        }

        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.Initialize( OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
        }
        else
        {
            this.skillSelectionPanelV2.Initialize( this, OnSkillSelectedFromSkillSelectionPanelV2, OnSkillDeselectedFromSkillSelectionPanelV2, ReturnToSkillMenu );
        }

        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Initialize( OnSkillSlotSwiped );
        }
        else
        {
            this.skillSlotListPanel.SetIsSkillSlotListScrollable( false );
            this.activeSkillSlotListPanelV2.Initialize( OnSkillSlotSelected );
            this.backendSkillSlotListPanel.Initialize( OnSkillSlotSelected );
        }

        if (this.playerDashboard == null)
        {
            this.atlSlotListPanelV3.Initialize();
        }
        else
        {
            this.playerDashboard.Initialize( OnExecuteButtonClicked );
            this.atlSlotListPanelV3 = this.playerDashboard.GetAtlSlotListPanelV3();
            this.characterInfoPanelV2 = this.playerDashboard.GetCharacterInfoPanelV2();
            this.characterInfoPanelV2.Initialize();
        }
    }

    public void SetSelectedGameCharacter( GameCharacter gameCharacter )
    {
        this.selectedGameCharacter = gameCharacter;
    }

    public void CheckWhetherToEnableExecuteButton()
    {
        if (this.selectedGameCharacter.GetSelectedActiveSkillList().Count > 0)
        {
            if (this.preparationSection == null)
            {
                this.playerActionPanel.EnableExecuteButton();
            }
            else
            {
                if (this.playerDashboard == null)
                {
                    this.preparationSection.EnableExecuteButton();
                }
                else
                {
                    this.playerDashboard.ShowExecuteButtonContainer();
                }
            }
        }
        else
        {
            if (this.preparationSection == null)
            {
                this.playerActionPanel.DisableExecuteButton();
            }
            else
            {
                if (this.playerDashboard == null)
                {
                    this.preparationSection.DisableExecuteButton();
                }
                else
                {
                    this.playerDashboard.HideExecuteButtonContainer();
                }
            }
        }
    }

    public void SetAllActive( bool value )
    {
        if (this.preparationSection == null)
        {
            this.playerActionPanel.gameObject.SetActive( value );
        }
        else
        {
            this.preparationSection.gameObject.SetActive( value );
        }

        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.gameObject.SetActive( value );
        }
        else
        {
            this.skillSelectionPanelV2.gameObject.SetActive( value );
        }

        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.gameObject.SetActive( value );
        }
        else
        {
            this.activeSkillSlotListPanelV2.gameObject.SetActive( value );
            this.backendSkillSlotListPanel.gameObject.SetActive( value );
        }

        if (this.playerDashboard == null)
        {
            if (this.atlSlotListPanelV2 == null)
            {
                this.atlSlotListPanel.gameObject.SetActive( value );
            }
            else
            {
                this.atlSlotListPanelV2.gameObject.SetActive( value );
            }

            this.characterInfoPanel.gameObject.SetActive( value );
        }
        else
        {
            this.playerDashboard.gameObject.SetActive( value );
        }

        this.battleResultPanel.gameObject.SetActive( value );
        this.battleLog.gameObject.SetActive (value);
        this.enemyCharacterInfoBox.gameObject.SetActive(value);
    }

    public void PlayInstructionAnimation()
    {
        //this.instructionLabel.transform.localScale = Vector3.zero;
        //LeanTween.scale( this.instructionLabel.gameObject, Vector3.one, 0.9f ).setEaseOutCubic().setLoopPingPong( 1 ).setOnComplete( () =>
        //{
        //    this.instructionLabel.gameObject.SetActive( false );
        //} );
        this.battleStartUiAnimator.Play("CrossingActionMode");
    }

    public void ShowMessage( string message )
    {
        this.messageLabel.SetText( message );
        this.messageLabel.transform.localScale = Vector3.zero;

        this.messageLabel.gameObject.SetActive( true );
        LeanTween.scale( this.messageLabel.gameObject, Vector3.one, 0.6f ).setEaseOutExpo().setLoopPingPong( 1 ).setOnComplete( () =>
        {
            this.messageLabel.gameObject.SetActive( false );
        } );
    }

    public void ShowBattleSection()
    {
        this.skillInfoPanel.Hide();
        this.preparationSection.Hide();
    }

#region Player Action Panel

    public void ShowPreparationSection()
    {
        this.playerActionPanel.gameObject.SetActive( true );
        this.playerActionPanel.ShowPreparationSection();
        this.playerActionPanel.ShowActiveSkillSelectionPanel();
        this.playerActionPanel.HideBattleSection();
    }

    public void ShowBattleSection( GameCharacter gameCharacter )
    {
        DisablePlayerActionPanelButtons();
        this.playerActionPanel.ShowSkillActionButtons( gameCharacter.GetSelectedBackendSkillList().ToArray() );
        this.playerActionPanel.ShowBattleSection();
        this.playerActionPanel.HidePreparationSection();
    }

    public void UpdatePlayerActionPanelCharacter( GameCharacter gameCharacter )
    {
        this.playerActionPanel.SetSelectedGameCharacter( gameCharacter );
    }

    public void UpdatePlayerActionPanelButtons( CharacterSkill qteSkill, bool canDefend, bool canEvade, bool canObserve, float countdownTime )
    {
        this.playerActionPanel.ShowQTEActionButton( qteSkill, countdownTime );
        this.playerActionPanel.UpdateSkillActionButtons( canDefend, canEvade, canObserve, countdownTime );
    }

    public void DisablePlayerActionPanelButtons()
    {
        UpdatePlayerActionPanelButtons( null, false, false, false, 0.0f );
    }

    public void OnExecuteButtonClicked()
    {
        ReturnToSkillMenu();
        if (this.skillSelectionPanelV2.GetIsAnimationPlayable())
        {
            this.skillSelectionPanelV2.gameObject.SetActive(true);
            this.skillSelectionPanelV2.StartHideSkillSelectionPanel();
        }
        else
        {
            this.preparationSection.HideAnimation();
        }
        StartCoroutine(WaitForTurnStartAnimation());
    }

    public IEnumerator WaitForTurnStartAnimation()
    {
        this.turnText.text = "Turn " + this.battleGameManager.GetBattleFlowManager_V2().GetCurrentRound().GetRoundNumber();
        this.battleStartUiAnimator.Play("TurnStart");
        HideDarkLayer();
        this.skillSelectionPanel.CheckForNecessarySkill();

        yield return new WaitForSeconds(2f);
        this.battleGameManager.StartExecution();
    }

    private void ShowActiveSkillSelectionPanel()
    {
        if (this.preparationSection == null)
        {
            this.skillSelectionPanel.ShowActiveSkillSelectionPanel();
        }
        else
        {
            this.preparationSection.HideSkillMenu();
            ShowSkillSelectionPanel( SkillSelectionPanelV2.SkillType.Active );
        }
    }

    private void ShowBackendSkillSelectionPanel()
    {
        if (this.preparationSection == null)
        {
            this.skillSelectionPanel.ShowBackendSkillSelectionPanel();
        }
        else
        {
            this.preparationSection.HideSkillMenu();
            ShowSkillSelectionPanel( SkillSelectionPanelV2.SkillType.Backend );
        }
    }

    public PlayerActionPanel GetPlayerActionPanel()
    {
        return this.playerActionPanel;
    }

#endregion

#region Preparation Section

    public void ReturnToSkillMenu()
    {
        this.skillInfoPanel.Hide();
        HideSkillSelectionPanel();
        HideDarkLayer();
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BACK);

        this.preparationSection.ShowSkillMenu();
    }

    public PreparationSection GetPreparationSection()
    {
        return this.preparationSection;
    }

#endregion

#region Skill Selection Panel

    public void ShowSkillSelectionPanel()
    {
        this.skillSelectionPanel.Show( this.selectedGameCharacter );
    }

    public void ShowSkillSelectionPanel( SkillSelectionPanelV2.SkillType skillType )
    {
        this.skillSelectionPanelV2.Show( this.selectedGameCharacter, skillType );
    }

    public void HideSkillSelectionPanel()
    {
        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.Hide();
        }
        else
        {
            this.skillSelectionPanelV2.HideSkillSelectionPanel();
        }
    }

    public void OnSkillSelectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        if (!skillSelectionBox.CheckIsSkillLevelChanged())
        {
            this.selectedGameCharacter.AddSelectedSkill(skillSelectionBox.GetCharacterSkill());
        }
        
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
        CheckWhetherToEnableExecuteButton();
    }

    public void OnSkillSelectedFromSkillSelectionPanelV2( SkillSelectionBoxV2 skillSelectionBox )
    {
        //this.selectedGameCharacter.AddSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel(this.selectedGameCharacter);
        CheckWhetherToEnableExecuteButton();
    }

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        this.selectedGameCharacter.RemoveSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
        CheckWhetherToEnableExecuteButton();
    }

    public void OnSkillDeselectedFromSkillSelectionPanelV2( SkillSelectionBoxV2 skillSelectionBox )
    {
        this.selectedGameCharacter.RemoveSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel(this.selectedGameCharacter);
        CheckWhetherToEnableExecuteButton();
    }

    public SkillSelectionPanel GetSkillSelectionPanel()
    {
        return this.skillSelectionPanel;
    }

#endregion

#region Skill Slot List Panel

    public void ShowSkillSlotListPanel()
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Show();
        }
        else
        {
            this.activeSkillSlotListPanelV2.Show();
            this.backendSkillSlotListPanel.Show();
        }
    }

    public void HideSkillSlotListPanel()
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Hide();
        }
        else
        {
            this.activeSkillSlotListPanelV2.Hide();
            this.backendSkillSlotListPanel.Hide();
        }
    }

    public void PlayHideSkillSlotListPanelAnimation()
    {
        this.activeSkillSlotListPanelV2.HideAnimation();
        this.backendSkillSlotListPanel.HideAnimation();
    }

    public void UpdateSkillSlotListPanel( GameCharacter gameCharacter )
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.UpdateSkillSlots( gameCharacter );
        }
        else
        {
            //this.skillSlotListPanelV2.UpdateSkillSlots( gameCharacter );
        }
    }

    public SkillSlotListPanel GetSkillSlotListPanel()
    {
        return this.skillSlotListPanel;
    }

    public ActiveSkillSlotListPanelV2 GetActiveSkillSlotListPanelV2()
    {
        return this.activeSkillSlotListPanelV2;
    }

    public BackendSkillSlotListPanel GetBackendSkillSlotListPanel()
    {
        return this.backendSkillSlotListPanel;
    }

    public void OnSkillSlotSwiped()
    {
        this.atlSlotListPanel.OnSkillSlotUpdated();
    }

    public void OnSkillSlotSelected( SkillSlotV2 skillSlot, bool isSelectingSkill )
    {
        List<SkillSlotV2> _skillSlotList = null;

        if (this.selectedSkill != null)
        {
            _skillSlotList = GetMatchedSkillSlotList( this.selectedSkill );
            for (int i = 0; i < _skillSlotList.Count; i++)
            {
                SkillSlotV2 _skillSlot = _skillSlotList[ i ];
                _skillSlot.SetCurrentStateType( SkillSlotV2.StateType.Enabled );
                _skillSlot.SetIsSelected( false );
            }

            this.selectedSkill = null;
        }

        if (skillSlot != null)
        {
            this.selectedSkill = skillSlot.GetSelectedSkill();

            _skillSlotList = GetMatchedSkillSlotList( this.selectedSkill );
            for (int i = 0; i < _skillSlotList.Count; i++)
            {
                SkillSlotV2 _skillSlot = _skillSlotList[ i ];
                _skillSlot.SetIsSelected( true );

                if (isSelectingSkill)
                {
                    _skillSlot.PlaySkillOutlineAnimation();
                }
            }
        }
    }

    public bool IsUsingSkillSlotListPanelV2()
    {
        return ( this.activeSkillSlotListPanelV2 != null );
    }

#endregion

#region ATL Slot List Panel

    public void ShowATLSlotListPanel( BattleFlowATL[] flowATLs )
    {
        if (this.atlSlotListPanelV2 != null)
        {
            this.atlSlotListPanel.gameObject.SetActive( false );
            this.atlSlotListPanelV2.SetUp( flowATLs );
            this.atlSlotListPanelV2.gameObject.SetActive( true );
        }
        else
        {
            this.atlSlotListPanel.Show( flowATLs, OnSkillSlotSwiped, OnATLSlotExecuted );
        }
    }

    public void ShowATLSlotListPanel( BattleFlowATL_V2[] flowATLs )
    {
        if (this.atlSlotListPanelV3 != null)
        {
            this.atlSlotListPanelV3.SetUp( flowATLs );
            this.atlSlotListPanelV3.gameObject.SetActive( true );
        }
        else
        {
            this.atlSlotListPanelV2.SetUp( flowATLs );
            this.atlSlotListPanelV2.gameObject.SetActive( true );
        }
    }

    public void HideATLSlotListPanel()
    {
        this.atlSlotListPanel.Hide();

        if (this.atlSlotListPanelV2 != null)
        {
            this.atlSlotListPanelV2.gameObject.SetActive( false );
        }
    }

    public void OnATLSlotExecuted()
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.SwipeLeft();
        }
        else
        {
            this.activeSkillSlotListPanelV2.ClickBottom();
        }
    }

    public ATLSlotListPanelV2 GetATLSlotListPanelV2()
    {
        return this.atlSlotListPanelV2;
    }

    public ATLSlotListPanelV3 GetATLSlotListPanelV3()
    {
        return this.atlSlotListPanelV3;
    }

#endregion

#region Character Info Panel

    public CharacterInfoPanel GetCharacterInfoPanel()
    {
        return this.characterInfoPanel;
    }

    public CharacterInfoPanelV2 GetCharacterInfoPanelV2()
    {
        return this.characterInfoPanelV2;
    }

#endregion

#region Battle Result Panel

    public void ShowVictoryResult()
    {
        this.battleResultPanel.ShowVictory();
    }

    public void ShowDefeatResult()
    {
        this.battleResultPanel.ShowDefeat();
    }

#endregion

#region Battle Skill Manager

    public void UpdateSkillButtons( List<SkillType> skillTypeList = null )
    {
        List<SkillType> _skillTypeList = skillTypeList ?? new List<SkillType>();
        bool _hasActiveSkillType = _skillTypeList.Contains( SkillType.Active );
        bool _isAbleToRepulse = false;

        Debug.Log( "UpdateSkillButtons" );
        if (_skillTypeList.Contains( SkillType.Repulse ))
        {
            _isAbleToRepulse = true;

            Debug.Log( "SkillType.Repulse" );
            this.activeSkillSlotListPanelV2.ChangeToRepulseMode( this.selectedGameCharacter );
        }
        else if (_skillTypeList.Contains( SkillType.Derive ))
        {
            Debug.Log( "SkillType.Derive" );
            this.activeSkillSlotListPanelV2.ChangeToDerivedMode( this.selectedGameCharacter, _hasActiveSkillType );

            if (_hasActiveSkillType)
            {
                this.activeSkillSlotListPanelV2.EnableInteraction();
            }
            else
            {
                this.activeSkillSlotListPanelV2.DisableInteraction();
            }
        }
        else
        {
            Debug.Log( "Default" );
            this.activeSkillSlotListPanelV2.ChangeToDefaultMode( this.selectedGameCharacter, ( _hasActiveSkillType ) ? SkillSlotV2.StateType.Enabled : SkillSlotV2.StateType.Disabled );
        }

        if (this.backendSkillSlotListPanel == null)
        {
            this.playerActionPanel.ShowSkillActionButtons( this.selectedGameCharacter.GetSelectedBackendSkillList().ToArray() );
            this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, _skillTypeList.Contains( SkillType.Defend ) );
            this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, _skillTypeList.Contains( SkillType.Evade ) );
            this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, _skillTypeList.Contains( SkillType.Observe ) );
        }
        else
        {
            if (this.preparationSection == null)
            {
                this.playerActionPanel.gameObject.SetActive( false );
            }

            this.backendSkillSlotListPanel.UpdateBackendSkillSlots( this.selectedGameCharacter, _skillTypeList );
        }

        if (_isAbleToRepulse)
        {
            CharacterSkill _assignedSkill = this.selectedGameCharacter.GetAssignedSkill();
            if (_assignedSkill != null)
            {
                if (_assignedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
                {
                    CharacterSkill _repulseSkill = _assignedSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
                    this.activeSkillSlotListPanelV2.GetSkillSlots( _repulseSkill )[ 0 ].SelectSkill();
                }
            }
        }
    }

    public void OnSkillBeingUsed( CharacterSkill skill )
    {
        List<SkillSlotV2> _skillSlotList = GetMatchedSkillSlotList( skill );
        for (int i = 0; i < _skillSlotList.Count; i++)
        {
            _skillSlotList[ i ].SetCurrentStateType( SkillSlotV2.StateType.Activated );
        }
    }

    public List<SkillSlotV2> GetMatchedSkillSlotList( CharacterSkill skill )
    {
        List<SkillSlotV2> _matchedSkillSlotList = this.activeSkillSlotListPanelV2.GetSkillSlots( skill );

        if (_matchedSkillSlotList.Count <= 0)
        {
            SkillSlotV2 _backendSkillSlot = this.backendSkillSlotListPanel.GetSkillSlot( skill );
            if (_backendSkillSlot != null)
            {
                _matchedSkillSlotList.Add( _backendSkillSlot );
            }
        }

        return _matchedSkillSlotList;
    }

#endregion

    public PlayerDashboard GetPlayerDashboard()
    {
        return this.playerDashboard;
    }

    public SkillPromptPanelV2 GetSkillPromptPanel()
    {
        return this.skillPromptPanel;
    }

    public void ShowEnemyCharacterInfoBoxUI()
    {
        this.enemyCharacterInfoBoxUI.SetActive( true );
        this.enemyCharacterInfoBoxHUD.SetActive( false );
    }

    public void ShowEnemyCharacterInfoBoxHUD()
    {
        this.enemyCharacterInfoBoxUI.SetActive( false );
        this.enemyCharacterInfoBoxHUD.SetActive( true );
    }

    public void HideEnemyCharacterInfoBox()
    {
        this.enemyCharacterInfoBoxUI.SetActive( false );
        this.enemyCharacterInfoBoxHUD.SetActive( false );
    }

    public EnemyDebugMenuPanel GetEnemyDebugMenuPanel()
    {
        return this.enemyDebugMenuPanel;
    }

    public void ShowDarkLayer()
    {
        this.darkLayer.SetActive( true );
    }

    public void HideDarkLayer()
    {
        this.darkLayer.SetActive( false );
    }
}
