using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;

public class SkillActionButton : MonoBehaviour
{
    [SerializeField] private Button actionButton = null;
    [SerializeField] private TextMeshProUGUI actionType = null;
    [SerializeField] private TextMeshProUGUI actionSkillName = null;
    [SerializeField] private TextMeshProUGUI strengthValue = null;
    [SerializeField] private TextMeshProUGUI accuracyValue = null;
    [SerializeField] private TextMeshProUGUI evasionValue = null;
    [SerializeField] private GameObject strength = null;
    [SerializeField] private GameObject accuracy = null;
    [SerializeField] private GameObject evasion = null;

    private CharacterSkill selectedSkill = null;

    private void Start()
    {
        this.actionButton.onClick.AddListener(ClickOnActionButton);
    }

    public void SetSelectedSkill( CharacterSkill selectedSkill )
    {
        this.selectedSkill = selectedSkill;

        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();
        SetupSkillActionButton(_subskillData);

        DisableActionButton();
    }

    private void SetupSkillActionButton(Subskill subskillData)
    {
        //Setup action type
        if (subskillData.IsDefendingSkill)
        {
            this.actionType.SetText("防禦");
        }
        else if (subskillData.IsEvadingSkill)
        {
            this.actionType.SetText("迴避");
        }
        else if (subskillData.IsInterceptable)
        {
            this.actionType.SetText("看破");
        }
        else
        {
            this.actionType.gameObject.SetActive(false);
        }

        //Setup skill name
        this.actionSkillName.SetText(subskillData.DisplayName);

        //Setup Strength, Accuracy and Evasion value
        int _strengthValue = subskillData.Strength;
        int _accuracyValue = subskillData.Accuracy;
        int _evasionValue = subskillData.Evasion;

        if (_strengthValue > 1)
        {
            this.strength.gameObject.SetActive(true);
            this.strengthValue.SetText("+" + (_strengthValue - 1).ToString());
        }
        else
        {
            this.strength.gameObject.SetActive(false);
        }

        if (_accuracyValue > 1)
        {
            this.accuracy.gameObject.SetActive(true);
            this.accuracyValue.SetText("+" + (_accuracyValue - 1).ToString());
        }
        else
        {
            this.accuracy.gameObject.SetActive(false);
        }

        if (_evasionValue > 1)
        {
            this.evasion.gameObject.SetActive(true);
            this.evasionValue.SetText("+" + (_evasionValue - 1).ToString());
        }
        else
        {
            this.evasion.gameObject.SetActive(false);
        }
    }

    public void EnableActionButton()
    {
        this.actionButton.interactable = true;
    }

    public void DisableActionButton()
    {
        this.actionButton.interactable = false;
    }

    public void ClickOnActionButton()
    {
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }
}
