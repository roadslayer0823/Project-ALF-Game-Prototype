using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Skill = DatabaseManager.Skill;
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
    [SerializeField] private CountdownTimer countdownTimer = null;

    private CharacterSkill selectedSkill = null;
    private Action<CharacterSkill> onActionButtonClickedCallback = null;

    public void Initialize( Action<CharacterSkill> onActionButtonClickedCallback )
    {
        this.onActionButtonClickedCallback = onActionButtonClickedCallback;

        SetOnClick(ClickOnActionButton);
    }

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

    // For QTE skill
    public void SetupQTESkillActionButton(CharacterSkill characterSkill)
    {
        if (characterSkill == null)
        {
            return;
        }

        this.gameObject.SetActive(true);

        Skill skillData = characterSkill.GetSkillData();
        // Setup QTE action type
        switch (skillData.skillType)
        {
            case Skill.SkillType.repulse:

                this.actionType.SetText("迎擊");
                break;

            case Skill.SkillType.derived:

                this.actionType.SetText("派生");
                break;

            case Skill.SkillType.counter:

                this.actionType.SetText("反擊");
                break;
        }

        CharacterSubskill _characterSubskill = characterSkill.GetCharacterSubskillData();
        SetupSkillActionButtonDetail(_characterSubskill.GetSubskillData());
    }

    // For backend skill
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

        SetupSkillActionButtonDetail(subskillData);
    }

    private void SetupSkillActionButtonDetail(Subskill subskillData)
    {
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

    public void EnableActionButton( float countdownTime )
    {
        this.actionButton.interactable = true;
        this.countdownTimer.StartCountdownTimer( countdownTime );
    }

    public void DisableActionButton()
    {
        this.actionButton.interactable = false;
        this.countdownTimer.StopCountdownTimer();
    }

    public void ClickOnActionButton()
    {
        this.onActionButtonClickedCallback?.Invoke( selectedSkill );
    }

    public void SetOnClick(UnityAction callbackFunction)
    {
        this.actionButton.onClick.AddListener(callbackFunction);
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public CountdownTimer GetCountdownTimer()
    {
        return this.countdownTimer;
    }
}
