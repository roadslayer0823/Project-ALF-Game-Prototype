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

    public void Initialize(SkillSelectionListBox skillSelectionListBox, CharacterSkill characterSkill)
    {
        this.skillSelectionListBox = skillSelectionListBox;
        this.characterSkill = characterSkill;
        this.isSelected = false;
    }

    private void Start()
    {
        this.skillSelectionBoxRect.sizeDelta = new Vector2(0, this.boxHeight);

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
        this.isSkillLevelChanged = false;
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

    public bool CheckIsSkillLevelChanged()
    {
        return this.isSkillLevelChanged;
    }

    // To mark the selected skill back to deselected if it failed to add into the SelectedSkillList.
    public void MarkDeselected()
    {
        this.isSelected = false;
    }

    // Set the skill data that needed to display into TMP.
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
    }

    // Callback function for selection button
    private void OnSelectionButtonClick()
    {
        ClickToToggle();
    }

    private void OnMinusLevelButtonClick()
    {
        this.skillLevel = Math.Clamp(this.skillLevel - 1, 1, characterSkill.GetCharacterSubskillList().Count);

        UpdateCharacterSkillLevel(this.skillLevel);
    }

    private void OnPlusLevelButtonClick()
    {
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

        this.skillLevelText.SetText(skillLevel.ToString());

        this.skillSelectionListBox.ShowSelectedSkillInfo(this);
        UpdateSkillSelectionBoxData();
    }

    public void ShowSelectionHighlight()
    {
        this.selectionHightlight.gameObject.SetActive(true);
    }

    public void HideSelectionHighlight()
    {
        this.selectionHightlight.gameObject.SetActive(false);
    }
}
