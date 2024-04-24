using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialSkillInfoBox : MonoBehaviour
{
    [Header("ObservedSkillInfo")]
    [SerializeField] private TextMeshProUGUI observationPercentageText;
    [SerializeField] private TextMeshProUGUI observationTitle;
    [SerializeField] private TextMeshProUGUI bottomRowText;
    [SerializeField] private TextMeshProUGUI topRowText;
    [SerializeField] private GameObject topRowSkillName;
    [SerializeField] private GameObject observeSkillUI;
    [SerializeField] private GameObject observeSkillSlot;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image observationPercentageBar;
    [SerializeField] private Image observationPercentageFrame;
    [SerializeField] private Sprite percentageWhite;
    [SerializeField] private Sprite percentageYellow;
 
    private ObservedSkillData observedSkillData = null;

    public void Initialize(ObservedSkillData observedSkillData)
    {
        this.observedSkillData = observedSkillData;
        SetupObservedSkillData();
    }

    private void SetupObservedSkillData()
    {
        if(observedSkillData == null)
        {
            this.observationPercentageText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, -1);
            this.observationTitle.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, -1);
            this.observationPercentageBar.gameObject.SetActive(false);
            this.observeSkillSlot.SetActive(true);
            this.observeSkillUI.SetActive(false);
            this.observationPercentageText.SetText("---");
            this.observationPercentageFrame.sprite = percentageWhite;
            this.observationPercentageFrame.SetNativeSize();
        }
        else
        {
            UpdateObserveSkillIcon();
            this.observationPercentageText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, 0.2f);
            this.observationTitle.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, 0.2f);
            this.observationPercentageBar.gameObject.SetActive(true);
            this.observationPercentageFrame.sprite = percentageYellow;
            this.observationPercentageFrame.SetNativeSize();
            this.observeSkillSlot.SetActive(false);
            this.observeSkillUI.SetActive(true);

            //checking the current skill name
            if(this.observedSkillData.GetNamePartB() == "-")
            {
                this.topRowSkillName.SetActive(false);
                this.bottomRowText.SetText(this.observedSkillData.GetNamePartA());
            }
            else
            {
                this.topRowSkillName.SetActive(true);
                this.topRowText.SetText(this.observedSkillData.GetNamePartA());
                this.bottomRowText.SetText(this.observedSkillData.GetNamePartB());
            }

            float _observationRate = this.observedSkillData.GetCurrentObservedRate();
            this.observationPercentageText.SetText(_observationRate * 100 + "%");
            this.observationPercentageBar.fillAmount = _observationRate;
        }
    }

    private void UpdateObserveSkillIcon()
    {
        this.skillIcon.sprite = Resources.Load<Sprite>(observedSkillData.GetSkillIconFilePath());
        this.skillIcon.SetNativeSize();
    }
}
