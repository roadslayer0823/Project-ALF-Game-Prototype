using UnityEngine;
using UnityEngine.UI;

public class PassiveSkillSlot : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PassiveSkillType passiveSkillType = PassiveSkillType.None;

    [Header("Passive Skill")]
    [SerializeField] private Image passiveSkillSlot;

    [Header("Selected Passive Skill UI")]
    [SerializeField] private Sprite selectedStatePointSkill;
    [SerializeField] private Sprite selectedStressValueSkill;
    [SerializeField] private Sprite selectedHealthPointSkill;

    [Header("Default Passive Skill UI")]
    [SerializeField] private Sprite defaultStatePointSkill;
    [SerializeField] private Sprite defaultStressValueSkill;
    [SerializeField] private Sprite defaultHealthPointSkill;

    private PassiveSkillCategorySelectionPanel passiveSkillCategorySelectionPanel = null;
    private bool isSelected = false;

    private const string AUDIO_ID_CLICK = "click";

    public enum PassiveSkillType
    {
        None,
        HealthPoint,
        StatePoint,
        StressValue
    }

    public void Initialize( PassiveSkillCategorySelectionPanel passiveSkillCategorySelectionPanel )
    {
        this.passiveSkillCategorySelectionPanel = passiveSkillCategorySelectionPanel;
        UpdateDefaultPassiveSkillUI();
    }

    public void SetIsSelected( bool isSelected )
    {
        this.isSelected = isSelected;

        if (this.isSelected)
        {
            UpdateSelectedPassiveSkillUI();
        }
        else
        {
            UpdateDefaultPassiveSkillUI();
        }
    }

    private void UpdateDefaultPassiveSkillUI()
    {
        this.passiveSkillSlot.sprite = this.passiveSkillType switch
        {
            PassiveSkillType.HealthPoint => this.defaultHealthPointSkill,
            PassiveSkillType.StatePoint => this.defaultStatePointSkill,
            PassiveSkillType.StressValue => this.defaultStressValueSkill,
            _ => null
        };
    }

    private void UpdateSelectedPassiveSkillUI()
    {
        this.passiveSkillSlot.sprite = this.passiveSkillType switch
        {
            PassiveSkillType.HealthPoint => this.selectedHealthPointSkill,
            PassiveSkillType.StatePoint => this.selectedStatePointSkill,
            PassiveSkillType.StressValue => this.selectedStressValueSkill,
            _ => null
        };
    }

    public void ClickOption()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        this.passiveSkillCategorySelectionPanel.OnPassiveSkillSlotSelected( this );
    }

    public void OptionEnter()
    {
        UpdateSelectedPassiveSkillUI();
    }

    public void OptionExit()
    {
        if (!this.isSelected)
        {
            UpdateDefaultPassiveSkillUI();
        }
    }

    public PassiveSkillType GetPassiveSkillType()
    {
        return this.passiveSkillType;
    }
}
