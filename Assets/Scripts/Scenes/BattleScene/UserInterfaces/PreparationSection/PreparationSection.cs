using System;
using UnityEngine;
using UnityEngine.UI;

public class PreparationSection : MonoBehaviour
{
    [SerializeField] private GameObject skillMenuContainer = null;

    [Header("SkillSelectionPanel button")]
    [SerializeField] private Button showActiveSkillSelectionPanelButton = null;
    [SerializeField] private Button showBackendSkillSelectionPanelButton = null;
    [SerializeField] private Button executeButton = null;

    private GameCharacter selectedGameCharacter = null;
    private Action onActiveSkillButtonClickedCallback = null;
    private Action onBackendSkillButtonClickedCallback = null;
    private Action onExecuteButtonClickedCallback = null;
    private bool isShowingActiveSkillSelectionPanelNext = false;

    private const string AUDIO_ID_EXECUTE = "execute";
    private const string AUDIO_ID_POPUP = "popup";

    public void Initialize(Action onExecuteButtonClickedCallback, Action onShowActiveSkillSelectionPanelCallback, Action onShowBackendSkillSelectionPanelCallback)
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
        this.showActiveSkillSelectionPanelButton.onClick.AddListener(ShowActiveSkillSelectionPanel);
        this.showBackendSkillSelectionPanelButton.onClick.AddListener(ShowBackendSkillSelectionPanel);
        this.onActiveSkillButtonClickedCallback = onShowActiveSkillSelectionPanelCallback;
        this.onBackendSkillButtonClickedCallback = onShowBackendSkillSelectionPanelCallback;
    }

    public void SetSelectedGameCharacter(GameCharacter selectedGameCharacter)
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
            Debug.Log("The value for 'onExecuteButtonClickedCallback' is not assigned.");
        }
    }

    public void Show()
    {
        this.gameObject.SetActive( true );
        ShowSkillMenu();
    }

    public void Hide()
    {
        this.gameObject.SetActive( false );
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

    public void ShowActiveSkillSelectionPanel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_POPUP);

        if (this.onActiveSkillButtonClickedCallback != null)
        {
            this.onActiveSkillButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onActiveSkillButtonClicked' is not assigned.");
        }

        this.isShowingActiveSkillSelectionPanelNext = false;
    }

    private void ShowBackendSkillSelectionPanel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_POPUP);

        if (this.onBackendSkillButtonClickedCallback != null)
        {
            this.onBackendSkillButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onBackendSkillButtonClicked' is not assigned.");
        }

        this.isShowingActiveSkillSelectionPanelNext = true;
    }

}
