using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Subskill = DatabaseManager.Subskill;

public class SkillSelectionBox : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float boxHeight = 150f;

    [Header("")]
    [SerializeField] private RectTransform rectTransform;
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
    private int skillLevel = 1;

    public void Initialize(SkillSelectionListBox skillSelectionListBox, CharacterSkill characterSkill)
    {
        this.skillSelectionListBox = skillSelectionListBox;
        this.characterSkill = characterSkill;
        this.isSelected = false;
    }

    private void Start()
    {
        this.rectTransform.sizeDelta = new Vector2(0, this.boxHeight);

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

    public void Select()
    {
        this.isSelected = true;
        this.skillSelectionListBox.OnSkillSelected( this );
    }

    public void Deselect()
    {
        this.isSelected = false;
        this.skillSelectionListBox.OnSkillDeselected( this );
    }

    public CharacterSkill GetCharacterSkill()
    {
        return this.characterSkill;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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

    // To mark the selected skill back to deselected if it failed to add into the SelectedSkillList.
    public void MarkDeselected()
    {
        this.isSelected = false;
    }

    // Set the skill data that needed to display into TMP.
    private void UpdateSkillSelectionBoxData()
    {
        Subskill _subskillData = characterSkill.GetSubskillData();

        this.skillNameText.SetText(_subskillData.GetDisplayName());

        if (_subskillData.GetPrefix().ToString() == "-")
        {
            this.skillTypeText.SetText("");
        }
        else
        {
            this.skillTypeText.SetText("[" + _subskillData.GetPrefix().ToString() + "]");
        }
    }

    // Callback function for selection button
    private void OnSelectionButtonClick()
    {
        ClickToToggle();
    }

    private void OnMinusLevelButtonClick()
    {
        this.skillLevel = Math.Clamp(this.skillLevel - 1, 1, characterSkill.GetSubskillList().Count);

        this.characterSkill.SetSelectedSkillLevel(this.skillLevel);
        UpdateCharacterSkillLevel(this.skillLevel);
    }

    private void OnPlusLevelButtonClick()
    {
        this.skillLevel = Math.Clamp(this.skillLevel + 1, 1, characterSkill.GetSubskillList().Count);

        this.characterSkill.SetSelectedSkillLevel(this.skillLevel);
        UpdateCharacterSkillLevel(this.skillLevel);
    }

    private void UpdateCharacterSkillLevel(int skillLevel)
    {
        if (this.isSelected)
        {
            Deselect();
        }

        this.skillLevelText.SetText(skillLevel.ToString());

        this.skillSelectionListBox.ShowSelectedSkillInfo(this);
        UpdateSkillSelectionBoxData();
    }
}
