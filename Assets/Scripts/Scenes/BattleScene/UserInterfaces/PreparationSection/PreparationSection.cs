using System;
using UnityEngine;
using UnityEngine.UI;

public class PreparationSection : MonoBehaviour
{
    [SerializeField] private GameObject skillMenuContainer = null;

    [Header("SkillSelectionPanel button")]
    [SerializeField] private Button showActiveSkillSelectionPanelButton = null;
    [SerializeField] private Button showBackendSkillSelectionPanelButton = null;
    [SerializeField] private SkillSelectionPanelV2 skillSelectionPanelV2 = null;
    [SerializeField] private Animator preparationSectionAnimation = null;
    [SerializeField] private Button switchingButton = null;
    [SerializeField] private Button executeButton = null;

    private BattleUiManager battleUiManager = null;
    private Action onActiveSkillButtonClickedCallback = null;
    private Action onBackendSkillButtonClickedCallback = null;
    private Action onExecuteButtonClickedCallback = null;

    private const string AUDIO_ID_EXECUTE = "execute";
    private const string AUDIO_ID_POPUP = "popup";
    private const string ANIMATION_ID_SHOW_MENU_PANEL = "ShowMenuPanel";
    private const string ANIMATION_ID_HIDE_MENU_PANEL = "HideMenuPanel";

    public void Initialize( BattleUiManager battleUiManager, Action onExecuteButtonClickedCallback, Action onShowActiveSkillSelectionPanelCallback, Action onShowBackendSkillSelectionPanelCallback )
    {
        this.battleUiManager = battleUiManager;
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
        this.onActiveSkillButtonClickedCallback = onShowActiveSkillSelectionPanelCallback;
        this.onBackendSkillButtonClickedCallback = onShowBackendSkillSelectionPanelCallback;
        this.showActiveSkillSelectionPanelButton.onClick.AddListener( ShowActiveSkillSelectionPanel );
        this.showBackendSkillSelectionPanelButton.onClick.AddListener( ShowBackendSkillSelectionPanel );
    }

    public void ClickOnExecuteButton()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_EXECUTE);

        DisableExecuteButton();
        this.battleUiManager.HideDarkLayer();

        if (this.onExecuteButtonClickedCallback != null)
        {
            this.onExecuteButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onExecuteButtonClickedCallback' is not assigned.");
        }
    }

    public void Show()
    {
        this.gameObject.SetActive( true );
        this.preparationSectionAnimation.Play(ANIMATION_ID_SHOW_MENU_PANEL);
        ShowSkillMenu();
    }

    public void HideAnimation()
    {
        this.preparationSectionAnimation.Play(ANIMATION_ID_HIDE_MENU_PANEL);
    }

    public void Hide()
    {
        this.gameObject.SetActive( false );
        HideSkillMenu();
    }

    public void ShowSkillMenu()
    {
        this.skillMenuContainer.SetActive( true );
    }

    public void HideSkillMenu()
    {
        this.skillMenuContainer.SetActive( false );
    }

    public void EnableExecuteButton()
    {
        this.executeButton.interactable = true;
    }

    public void DisableExecuteButton()
    {
        this.executeButton.interactable = false;
    }

    public void HideExecuteButton()
    {
        this.executeButton.gameObject.SetActive( false );
    }

    public void EnableSkillSelectionButtons()
    {
        this.showActiveSkillSelectionPanelButton.interactable = true;
        this.showBackendSkillSelectionPanelButton.interactable = true;
        this.switchingButton.interactable = true;
    }

    public void DisableSkillSelectionButtons()
    {
        this.showActiveSkillSelectionPanelButton.interactable = false;
        this.showBackendSkillSelectionPanelButton.interactable = false;
        this.switchingButton.interactable = false;
    }

    public void ShowActiveSkillSelectionPanel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_POPUP);

        if (this.onActiveSkillButtonClickedCallback != null)
        {
            this.onActiveSkillButtonClickedCallback();
            this.skillSelectionPanelV2.PlayAttackSkillSelectionPanelAnimation();
            this.skillSelectionPanelV2.isActiveOpened = true;
        }
        else
        {
            Debug.Log("The value for 'onActiveSkillButtonClicked' is not assigned.");
        }

        this.battleUiManager.ShowDarkLayer();
    }

    private void ShowBackendSkillSelectionPanel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_POPUP);

        if (this.onBackendSkillButtonClickedCallback != null)
        {
            this.onBackendSkillButtonClickedCallback();
            this.skillSelectionPanelV2.PlayBackendSkillSelectionPanelAnimation();
            this.skillSelectionPanelV2.isBackendOpened = true;
        }
        else
        {
            Debug.Log("The value for 'onBackendSkillButtonClicked' is not assigned.");
        }

        this.battleUiManager.ShowDarkLayer();
    }
}
