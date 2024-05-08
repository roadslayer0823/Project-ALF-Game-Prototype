using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;

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
 
    private ObservedSkillRecord observedSkillRecord = null;

    public void Initialize(ObservedSkillRecord observedSkillRecord)
    {
        this.observedSkillRecord = observedSkillRecord;
        SetupObservedSkillData();
    }

    private void SetupObservedSkillData()
    {
        if(observedSkillRecord == null)
        {
            this.observationPercentageText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, -1);
            this.observationTitle.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, -1);
            this.observationPercentageBar.gameObject.SetActive(false);
            this.observeSkillSlot.SetActive(true);
            this.observeSkillUI.SetActive(false);
            this.observationPercentageText.SetText("---");
            this.observationPercentageFrame.sprite = this.percentageWhite;
            this.observationPercentageFrame.SetNativeSize();
        }
        else
        {
            Subskill _observedSkillRecordSubskillData = this.observedSkillRecord.GetSubskillData();

            this.skillIcon.sprite = Resources.Load<Sprite>( _observedSkillRecordSubskillData.IconFilePathOn );
            this.skillIcon.SetNativeSize();

            this.observationPercentageText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, 0.2f);
            this.observationTitle.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, 0.2f);
            this.observationPercentageBar.gameObject.SetActive(true);
            this.observationPercentageFrame.sprite = this.percentageYellow;
            this.observationPercentageFrame.SetNativeSize();
            this.observeSkillSlot.SetActive(false);
            this.observeSkillUI.SetActive(true);

            //checking the current skill name
            if (_observedSkillRecordSubskillData.NamePartB == "-")
            {
                this.topRowSkillName.SetActive( false );
                this.bottomRowText.SetText( _observedSkillRecordSubskillData.NamePartA );
            }
            else
            {
                this.topRowSkillName.SetActive( true );
                this.topRowText.SetText( _observedSkillRecordSubskillData.NamePartA );
                this.bottomRowText.SetText( _observedSkillRecordSubskillData.NamePartB );
            }

            float _observationRate = this.observedSkillRecord.GetCurrentObservedRate();
            this.observationPercentageText.SetText( $"{ _observationRate.ConvertToIntegerInPercentage() }%" );
            this.observationPercentageBar.fillAmount = _observationRate;
        }
    }
}
