using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObservedSkillBox : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float boxHeight = 100f;

    [Header("ObservedSkillInfo")]
    [SerializeField] private RectTransform observedSkillBoxRect = null;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI observationPercentageText;
    [SerializeField] private Image observationPercentageBar;
    [SerializeField] private Image selectionHighlight = null;

    private ObservedSkillData observedSkillData = null;
    private bool isSelected = false;

    public void Initialize(ObservedSkillData observedSkillData)
    {
        this.observedSkillData = observedSkillData;

        SetupObservedSkillData();
    }

    private void SetupObservedSkillData()
    {
        this.observedSkillBoxRect.sizeDelta = new Vector2(this.observedSkillBoxRect.sizeDelta.x, this.boxHeight);

        this.skillNameText.SetText(this.observedSkillData.GetSkillName());

        this.skillTypeText.SetText(this.observedSkillData.GetSkillType().ToString());

        float _observationRate = this.observedSkillData.GetCurrentObservedRate();
        this.observationPercentageText.SetText(_observationRate * 100 + "%");
        this.observationPercentageBar.fillAmount = _observationRate;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isSelected)
        {
            this.selectionHighlight.gameObject.SetActive(false);

            this.isSelected = false;
        }
        else
        {
            this.selectionHighlight.gameObject.SetActive(true);

            this.isSelected = true;
        }
    }
}
