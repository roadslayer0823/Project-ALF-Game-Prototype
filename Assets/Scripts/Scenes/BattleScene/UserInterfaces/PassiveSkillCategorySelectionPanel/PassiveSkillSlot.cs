using UnityEngine;
using UnityEngine.UI;

public class PassiveSkillSlot : MonoBehaviour
{
    [Header("Passive Skill")]
    [SerializeField] private Image passiveSkillSlot;

    [Header("Highlight Passive Skill UI")]
    [SerializeField] private Image highLightPassiveSkill;

    [Header("Selected Passive Skill UI")]
    [SerializeField] private Image passiveSkillLogo;

    private PassiveSkillCategorySelectionPanel passiveSkillCategorySelectionPanel = null;


    public void Initialize( PassiveSkillCategorySelectionPanel passiveSkillCategorySelectionPanel )
    {
        this.passiveSkillCategorySelectionPanel = passiveSkillCategorySelectionPanel;
    }

    public void UpdateHighlightPassiveSkillUI()
    {
        Color colorAlpha = this.highLightPassiveSkill.color;
        colorAlpha.a = 1.0f;
        this.highLightPassiveSkill.color = colorAlpha;
        this.passiveSkillLogo.color = colorAlpha;
    }

    public void UpdateDefaultPassiveSkillUI()
    {
        Color colorAlpha = this.highLightPassiveSkill.color;
        colorAlpha.a = 0.0f;
        this.highLightPassiveSkill.color = colorAlpha;
        this.passiveSkillLogo.color = colorAlpha;
    }
}
