using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSelectionBox : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float boxHeight = 150f;

    [Header("")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private Button selectionButton;

    private SkillSelectionListBox skillSelectionListBox = null;
    private CharacterSkill characterSkill = null;
    private bool isSelected = false;

    public void Initialize( SkillSelectionListBox skillSelectionListBox, CharacterSkill characterSkill )
    {
        this.skillSelectionListBox = skillSelectionListBox;
        this.characterSkill = characterSkill;
        this.isSelected = false;
    }

    private void Start()
    {
        SetupSkillSelectionBox();

        this.selectionButton.onClick.AddListener(OnSelectionButtonClick);
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
    private void SetupSkillSelectionBox()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, this.boxHeight);

        this.skillNameText.SetText(characterSkill.GetSkillData().GetSkillName());
        this.skillTypeText.SetText("[" + characterSkill.GetSkillData().GetSkillType().ToString() + "]");
    }

    // Callback function for selection button
    private void OnSelectionButtonClick()
    {
        ClickToToggle();
    }
}
