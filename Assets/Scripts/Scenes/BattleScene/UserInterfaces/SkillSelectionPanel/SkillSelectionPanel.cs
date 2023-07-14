using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionPanel : MonoBehaviour
{
    [SerializeField] private SkillSelectionTab activeSkillSelectionTab = null;
    [SerializeField] private SkillSelectionTab backendSkillSelectionTab = null;

    [Header("SkillTabButtons")]
    [SerializeField] private RectTransform activeSkillSelectionTabButton = null;
    [SerializeField] private RectTransform backendSkillSelectionTabButton = null;

    [Header("SkillTabInteractionColor")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    private Action<SkillSelectionBox> onSkillSelectedCallback = null;
    private Action<SkillSelectionBox> onSkillDeselectedCallback = null;

    private List<SkillSelectionBox> selectedActiveSkillList = new List<SkillSelectionBox>();
    private List<SkillSelectionBox> selectedBackendSkillList = new List<SkillSelectionBox>();

    public void Initialize( Action<SkillSelectionBox> onSkillSelectedCallback, Action<SkillSelectionBox> onSkillDeselectedCallback )
    {
        this.onSkillSelectedCallback = onSkillSelectedCallback;
        this.onSkillDeselectedCallback = onSkillDeselectedCallback;

        this.activeSkillSelectionTab.Initialize( this );
        this.backendSkillSelectionTab.Initialize( this );
    }

    private void Start()
    {
        this.backendSkillSelectionTab.gameObject.SetActive(false);

        this.activeSkillSelectionTabButton.GetComponent<Button>().onClick.AddListener(OnActiveSkillTabClick);
        this.backendSkillSelectionTabButton.GetComponent<Button>().onClick.AddListener(OnPassiveSkillClick);
    }

    // Categorize and display all the skill that the character have based on skill category
    public void Show( CharacterSkill[] characterSkills )
    {
        List<CharacterSkill> activeSkillList = new List<CharacterSkill>();
        List<CharacterSkill> backendSkillList = new List<CharacterSkill>();

        for (int i = 0; i < characterSkills.Length; i++)
        {
            CharacterSkill characterSkill = characterSkills[ i ];
            SkillDatabase.SkillData characterSkillData = characterSkill.GetSkillData();

            if (characterSkillData.GetSkillType() == SkillDatabase.SkillData.SkillType.Active)
            {
                activeSkillList.Add( characterSkill );
            }
            else if (characterSkillData.GetSkillType() == SkillDatabase.SkillData.SkillType.Backend)
            {
                backendSkillList.Add( characterSkill );
            }
        }

        this.activeSkillSelectionTab.Show( activeSkillList.ToArray() );
        this.backendSkillSelectionTab.Show( backendSkillList.ToArray() );
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        if (this.onSkillSelectedCallback != null)
        {
            this.onSkillSelectedCallback( skillSelectionBox );

            if (skillSelectionBox.GetCharacterSkill().GetSkillData().GetSkillType() == SkillDatabase.SkillData.SkillType.Active)
            {
                if (this.selectedActiveSkillList.Count < 3)
                {
                    this.selectedActiveSkillList.Add(skillSelectionBox);

                    UpdateSelectedSkillSequence();
                }
                else
                {
                    skillSelectionBox.MarkDeselected();
                }
            }
            else if (skillSelectionBox.GetCharacterSkill().GetSkillData().GetSkillType() == SkillDatabase.SkillData.SkillType.Backend)
            {
                if (this.selectedBackendSkillList.Count < 3)
                {
                    this.selectedBackendSkillList.Add(skillSelectionBox);

                    skillSelectionBox.SetSkillSelectionText("ON");
                }
                else
                {
                    skillSelectionBox.MarkDeselected();
                }
            }
        }
        else
        {
            Debug.Log( "The value for 'onSkillSelectedCallback' is not assigned." );
        }
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        if (this.onSkillDeselectedCallback != null)
        {
            this.onSkillDeselectedCallback( skillSelectionBox );

            if (this.selectedActiveSkillList.Contains(skillSelectionBox))
            {
                this.selectedActiveSkillList.Remove(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionSequenceNumber(0);

                UpdateSelectedSkillSequence();
            }
            else if (this.selectedBackendSkillList.Contains(skillSelectionBox))
            {
                this.selectedBackendSkillList.Remove(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionText("");
            }
        }
        else
        {
            Debug.Log( "The value for 'onSkillDeselectedCallback' is not assigned." );
        }
    }

    // Callback function for the ActiveSkillTabButton
    private void OnActiveSkillTabClick()
    {
        this.activeSkillSelectionTab.gameObject.SetActive(true);
        this.backendSkillSelectionTab.gameObject.SetActive(false);

        this.activeSkillSelectionTabButton.GetComponent<Image>().color = this.selectedColor;
        this.backendSkillSelectionTabButton.GetComponent<Image>().color = this.normalColor;

        this.backendSkillSelectionTab.HideSkillInfoPanel();
    }

    // Callback funtion for the PassiveSkillTabButton
    private void OnPassiveSkillClick()
    {
        this.activeSkillSelectionTab.gameObject.SetActive(false);
        this.backendSkillSelectionTab.gameObject.SetActive(true);

        this.activeSkillSelectionTabButton.GetComponent<Image>().color = this.normalColor;
        this.backendSkillSelectionTabButton.GetComponent<Image>().color = this.selectedColor;

        this.activeSkillSelectionTab.HideSkillInfoPanel();
    }

    //Update the sequence number of the selected skill list
    private void UpdateSelectedSkillSequence()
    {
        int skillSelectionCounter = 0;

        foreach (SkillSelectionBox skill in selectedActiveSkillList)
        {
            skillSelectionCounter++;
            skill.SetSkillSelectionSequenceNumber(skillSelectionCounter);
        }
    }
}
