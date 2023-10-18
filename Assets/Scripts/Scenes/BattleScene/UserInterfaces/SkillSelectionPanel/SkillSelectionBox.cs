using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;

public class SkillSelectionBox : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float boxHeight = 150f;

    [Header( "" )]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform container;
    [SerializeField] private Image selectionHightlight;
    [SerializeField] private RectTransform skillSelectionBoxRect;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private Button selectionButton;
    [SerializeField] private Button minusLevelButton;
    [SerializeField] private Button plusLevelButton;

    private SkillSelectionListBox skillSelectionListBox = null;
    private CharacterSkill characterSkill = null;
    private bool isSelected = false;
    private bool isSkillLevelChanged = false;
    private int skillLevel = 1;

    private Vector3 containerOriginalPosition = Vector2.zero;

    private const string AUDIO_ID_HIGHLIGHT = "highlight";
    private const string AUDIO_ID_BOOST_LEVEL_UP= "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";

    public void Initialize(SkillSelectionListBox skillSelectionListBox, CharacterSkill characterSkill)
    {
        this.skillSelectionListBox = skillSelectionListBox;
        this.characterSkill = characterSkill;
        this.isSelected = false;

        this.skillSelectionBoxRect.sizeDelta = new Vector2( this.skillSelectionBoxRect.sizeDelta.x, this.boxHeight );
        UpdateSkillSelectionBoxData();

        this.selectionButton.onClick.AddListener(OnSelectionButtonClick);
        this.minusLevelButton.onClick.AddListener(OnMinusLevelButtonClick);
        this.plusLevelButton.onClick.AddListener(OnPlusLevelButtonClick);
    }

    public void ClickToToggle()
    {
        if (this.isSelected == true)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    // Select the skill to use in the battle
    public void Select()
    {
        this.isSelected = true;
        this.skillSelectionListBox.OnSkillSelected(this);
    }

    public void Deselect()
    {
        this.isSkillLevelChanged = false;
        this.isSelected = false;
        this.skillSelectionListBox.OnSkillDeselected(this);
    }

    public CharacterSkill GetCharacterSkill()
    {
        return this.characterSkill;
    }

    // Show the skill info
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_HIGHLIGHT);

        this.skillSelectionListBox.ShowSelectedSkillInfo(this);
    }

    public void SetSkillSelectionSequenceNumber(int sequenceText)
    {
        this.selectionText.SetText(sequenceText.ToString());

        if (sequenceText > 0)
        {
            this.selectionText.gameObject.SetActive(true);
        }
        else
        {
            this.selectionText.gameObject.SetActive(false);
        }
    }

    public void SetSkillSelectionText(string selectionText)
    {
        this.selectionText.SetText(selectionText);

        if (selectionText != "")
        {
            this.selectionText.gameObject.SetActive(true);
        }
        else
        {
            this.selectionText.gameObject.SetActive(false);
        }
    }

    public bool CheckIsSkillLevelChanged()
    {
        return this.isSkillLevelChanged;
    }

    public void MarkSelected()
    {
        this.isSelected = true;
        SetSkillSelectionText("ON");
    }

    // To mark the selected skill back to deselected if it failed to add into the SelectedSkillList.
    public void MarkDeselected()
    {
        this.isSelected = false;
        SetSkillSelectionText("");
    }

    // Set the skill data that needed to display into SkillSelectionBox.
    private void UpdateSkillSelectionBoxData()
    {
        if (this.characterSkill == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        Subskill _subskillData = this.characterSkill.GetCharacterSubskillData().GetSubskillData();

        this.skillNameText.SetText(_subskillData.DisplayName);

        if (_subskillData.Prefix.ToString() == "-")
        {
            this.skillTypeText.SetText("");
        }
        else
        {
            this.skillTypeText.SetText("[" + _subskillData.Prefix.ToString() + "]");
        }

        SetupLevelUIDisplayVisibility();
    }

    // Callback function for selection button
    private void OnSelectionButtonClick()
    {
        ClickToToggle();
    }

    private void OnMinusLevelButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_DOWN);

        this.skillLevel = Math.Clamp(this.skillLevel - 1, 1, characterSkill.GetCharacterSubskillList().Count);

        UpdateCharacterSkillLevel(this.skillLevel);
    }

    private void OnPlusLevelButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_UP);

        this.skillLevel = Math.Clamp(this.skillLevel + 1, 1, characterSkill.GetCharacterSubskillList().Count);

        UpdateCharacterSkillLevel(this.skillLevel);
    }

    private void UpdateCharacterSkillLevel(int skillLevel)
    {
        this.characterSkill.SetSelectedSkillLevel(this.skillLevel);

        // If the skill is currently selected
        if (this.isSelected)
        {
            this.isSkillLevelChanged = true;
            Select();
        }

        this.skillLevelText.SetText($"Lv. {skillLevel}");

        this.skillSelectionListBox.ShowSelectedSkillInfo(this);
        UpdateSkillSelectionBoxData();
    }

    private void SetupLevelUIDisplayVisibility()
    {
        if (this.skillLevel == characterSkill.GetCharacterSubskillList().Count)
        {
            this.plusLevelButton.gameObject.SetActive(false);
        }
        else
        {
            this.plusLevelButton.gameObject.SetActive(true);
        }

        if (this.skillLevel == 1)
        {
            this.minusLevelButton.gameObject.SetActive(false);
        }
        else
        {
            this.minusLevelButton.gameObject.SetActive(true);
        }
    }

    public void ShowSelectionHighlight()
    {
        this.selectionHightlight.gameObject.SetActive(true);
    }

    public void HideSelectionHighlight()
    {
        this.selectionHightlight.gameObject.SetActive(false);
    }

    public void MoveContainerToOrigin( float animationDuration )
    {
        this.containerOriginalPosition = this.container.position;
        StartCoroutine( RunMovingContainerToOrigin( animationDuration ) );
    }

    private IEnumerator RunMovingContainerToOrigin( float animationDuration )
    {
        yield return new WaitForEndOfFrame();
        this.container.position = this.containerOriginalPosition;

        LeanTween.move( this.container, Vector3.zero, animationDuration ).setOnComplete( () =>
        {
            this.canvas.overrideSorting = false;
            this.canvas.sortingOrder = 0;
        } );
    }

    public void BringToFront()
    {
        this.canvas.overrideSorting = true;
        this.canvas.sortingOrder = 1;
    }

    public bool IsSkillBoxSelected()
    {
        return this.isSelected;
    }
}
