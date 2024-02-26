using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialSkillInfoPanel : MonoBehaviour
{
    [Header("ObservedSkillInfo")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI observationPercentageText;
    [SerializeField] private TextMeshProUGUI observationTitle;
    [SerializeField] private GameObject observeSkillSlot;
    [SerializeField] private GameObject observeSkillType;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image observationPercentageBar;
    [SerializeField] private Image observationPercentageFrame;
    [SerializeField] private Sprite percentageWhite;
    [SerializeField] private Sprite percentageYellow;
    [SerializeField] private Color yellowTextColor;
    [SerializeField] private Color whiteTextColor;

    private ObservedSkillData observedSkillData = null;
    private CharacterSkill observerdSkillListData = null;

    public void Initialize(ObservedSkillData observedSkillData)
    {
        this.observedSkillData = observedSkillData;

        SetupObservedSkillData();
    }

    private void SetupObservedSkillData()
    {
        if(observerdSkillListData.GetObservedSkillDataList().Count == 0)
        {
            this.observationPercentageBar.gameObject.SetActive(false);
            this.observeSkillSlot.SetActive(false);
            this.observationPercentageText.SetText("----");
            this.observationPercentageText.color = whiteTextColor;
            this.observationPercentageFrame.sprite = percentageWhite;
            this.observationPercentageFrame.SetNativeSize();
            this.observationTitle.color = whiteTextColor;
        }
        else
        {
            UpdateObserveSkillIcon();
            this.observationPercentageText.color = yellowTextColor;
            this.observationPercentageText.color = yellowTextColor;
            this.observationPercentageBar.gameObject.SetActive(true);
            this.observationPercentageFrame.sprite = percentageYellow;
            this.observationPercentageFrame.SetNativeSize();
            this.observeSkillSlot.SetActive(true);

            this.skillNameText.SetText(this.observedSkillData.GetSkillName());
            //this.skillTypeText.SetText(this.observedSkillData.GetSkillType());
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
