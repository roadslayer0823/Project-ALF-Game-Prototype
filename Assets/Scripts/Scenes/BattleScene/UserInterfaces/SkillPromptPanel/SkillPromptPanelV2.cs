using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillPromptPanelV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float skillNameShowingDuration = 1.0f;
    [SerializeField] private float skillInfoPopSpeed = 0.25f;

    [Header("")]
    [SerializeField] private Animator speedEffectAnimator = null;
    [SerializeField] private Animator strengthEffectAnimator = null;

    [SerializeField] private GameObject skillNameGO = null;
    [SerializeField] private TextMeshProUGUI skillNameText = null;
    [SerializeField] private GameObject enemyPassiveSkillName = null;
    [SerializeField] private TextMeshProUGUI enemyPassiveSkillNameText = null;
    [SerializeField] private GameObject playerPassiveSkillName = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillNameText = null;
    [SerializeField] private GameObject speedEffectGO = null;
    [SerializeField] private GameObject strengthEffectGO = null;
    [SerializeField] private Image skillNameBackgroundImage = null;

    [Header("SkillNameBackground")]
    [SerializeField] private Sprite activeSkillBackgroundImage = null;
    [SerializeField] private Sprite backendSkillBackgroundImage = null;

    [Header("SkillBattleInfo || Enemy")]
    [SerializeField] private GameObject enemySkillInfoGO = null;
    [SerializeField] private GameObject enemySkillTagGO = null;
    [SerializeField] private GameObject enemyPassiveSkillGO = null;
    [SerializeField] private TextMeshProUGUI enemySkillTagText = null;
    [SerializeField] private Image enemyAttackSkillEffect = null;
    [SerializeField] private Image enemyRepulseSkillEffect = null;
    [SerializeField] private Image enemyAttackSkillIcon = null;
    [SerializeField] private Image enemyRepulseSkillIcon = null;
    [SerializeField] private TextMeshProUGUI enemySkillTypeText = null;
    [SerializeField] private TextMeshProUGUI enemyPassiveSkillText= null;

    [Header("SkillBattleInfo || Player")]
    [SerializeField] private GameObject playerSkillInfoGO = null;
    [SerializeField] private GameObject playerSkillTagGO = null;
    [SerializeField] private GameObject playerPassiveSkillGO = null;
    [SerializeField] private TextMeshProUGUI playerSkillTagText = null;
    [SerializeField] private Image playerAttackSkillEffect = null;
    [SerializeField] private Image playerRepulseSkillEffect = null;
    [SerializeField] private Image playerAttackSkillIcon = null;
    [SerializeField] private Image playerRepulseSkillIcon = null;
    [SerializeField] private TextMeshProUGUI playerSkillTypeText = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillText = null;

    public void Show(GameCharacter caster)
    {
        CharacterSkill _characterSkill = caster.GetCurrentSkill();
        int _skillStatIncrement = caster.GetCurrentSkillStatIncrement();

        if (_characterSkill == null || this.skillNameGO.activeInHierarchy)
        {
            return;
        }

        this.speedEffectGO.gameObject.SetActive(true);
        this.strengthEffectGO.gameObject.SetActive(true);

        if (_characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active
            || _characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse
            || _characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.derived)
        {
            this.skillNameBackgroundImage.sprite = this.activeSkillBackgroundImage;
        }
        else if (_characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend
            || _characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.counter)
        {
            this.skillNameBackgroundImage.sprite = this.backendSkillBackgroundImage;
        }

        this.skillNameText.SetText(_characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);

        int _speed = _characterSkill.GetCharacterSubskillData().GetSubskillData().Speed;
        int _strength = _characterSkill.GetCharacterSubskillData().GetSubskillData().Strength;

        if (_speed + _skillStatIncrement == 3)
        {
            this.speedEffectAnimator.Play("SpeedV2");
        }
        else if (_speed + _skillStatIncrement >= 4)
        {
            this.speedEffectAnimator.Play("GodSpeedV2");
        }

        if (_strength + _skillStatIncrement == 2)
        {
            this.strengthEffectAnimator.Play("Strength_1_V2");
        }
        else if (_strength + _skillStatIncrement >= 3)
        {
            this.strengthEffectAnimator.Play("Strength_2_V2");
        }

        this.skillNameGO.SetActive(true);
        LeanTween.delayedCall( skillNameShowingDuration, Hide );
    }

    private void Hide()
    {
        this.skillNameGO.SetActive(false);
        this.enemyPassiveSkillName.SetActive(false);
        this.playerPassiveSkillName.SetActive(false);
    }

    public void ShowPlayerSkillInfo(CharacterSkill characterSkill, string skillTypeText)
    {
        if (characterSkill != null && !LeanTween.isTweening(this.playerSkillInfoGO))
        {
            CharacterSubskill _playerCharacterSubskill = characterSkill.GetCharacterSubskillData();

            this.playerSkillInfoGO.SetActive(true);

            if (_playerCharacterSubskill != null)
            {
                if (characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.derived)
                {
                    this.playerAttackSkillEffect.gameObject.SetActive(true);
                    this.playerRepulseSkillEffect.gameObject.SetActive(false);
                    this.playerAttackSkillIcon.gameObject.SetActive(true);
                    this.playerRepulseSkillIcon.gameObject.SetActive(false);

                    if (_playerCharacterSubskill.GetSubskillData().EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide)
                    {
                        this.playerSkillTagGO.SetActive(true);
                        this.playerSkillTagText.SetText("廣角");
                    }
                    else
                    {
                        this.playerSkillTagGO.SetActive(false);
                    }
                }
                else if (characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.counter)
                {
                    this.playerAttackSkillEffect.gameObject.SetActive(false);
                    this.playerRepulseSkillEffect.gameObject.SetActive(true);
                    this.playerAttackSkillIcon.gameObject.SetActive(false);
                    this.playerRepulseSkillIcon.gameObject.SetActive(true);

                    if (_playerCharacterSubskill.GetSubskillData().EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide)
                    {
                        this.playerSkillTagGO.SetActive(true);
                        this.playerSkillTagText.SetText("对廣角");
                    }
                    else
                    {
                        this.playerSkillTagGO.SetActive(false);
                    }
                }

                this.playerSkillTypeText.SetText(skillTypeText);
            }

            LeanTween.moveLocalX(this.playerSkillInfoGO, 0.0f, this.skillInfoPopSpeed);
        }
    }

    public void ShowEnemySkillInfo(CharacterSkill characterSkill, string skillTypeText)
    {
        if (characterSkill != null && !LeanTween.isTweening(this.enemySkillInfoGO))
        {
            CharacterSubskill _enemyCharacterSubskill = characterSkill.GetCharacterSubskillData();

            this.enemySkillInfoGO.SetActive(true);

            if (_enemyCharacterSubskill != null)
            {
                if (characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.derived)
                {
                    this.enemyAttackSkillEffect.gameObject.SetActive(true);
                    this.enemyRepulseSkillEffect.gameObject.SetActive(false);
                    this.enemyAttackSkillIcon.gameObject.SetActive(true);
                    this.enemyRepulseSkillIcon.gameObject.SetActive(false);

                    if (_enemyCharacterSubskill.GetSubskillData().EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide)
                    {
                        this.enemySkillTagGO.SetActive(true);
                        this.enemySkillTagText.SetText("廣角");
                    }
                    else
                    {
                        this.enemySkillTagGO.SetActive(false);
                    }
                }
                else if (characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.counter)
                {
                    this.enemyAttackSkillEffect.gameObject.SetActive(false);
                    this.enemyRepulseSkillEffect.gameObject.SetActive(true);
                    this.enemyAttackSkillIcon.gameObject.SetActive(false);
                    this.enemyRepulseSkillIcon.gameObject.SetActive(true);

                    if (_enemyCharacterSubskill.GetSubskillData().EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide)
                    {
                        this.enemySkillTagGO.SetActive(true);
                        this.enemySkillTagText.SetText("对廣角");
                    }
                    else
                    {
                        this.enemySkillTagGO.SetActive(false);
                    }
                }

                this.enemySkillTypeText.SetText(skillTypeText);
            }

            LeanTween.moveLocalX(this.enemySkillInfoGO, 0.0f, this.skillInfoPopSpeed);
        }
    }

    public void HidePlayerSkillInfo()
    {
        LeanTween.moveLocalX(this.playerSkillInfoGO, 600.0f, this.skillInfoPopSpeed).setOnComplete(HidePlayerSkillInfoGameObject);
    }

    public void HideEnemySkillInfo()
    {
        LeanTween.moveLocalX(this.enemySkillInfoGO, -600.0f, this.skillInfoPopSpeed).setOnComplete(HideEnemySkillInfoGameObject);
    }

    private void HidePlayerSkillInfoGameObject()
    {
        this.playerSkillInfoGO.SetActive(false);
    }

    private void HideEnemySkillInfoGameObject()
    {
        this.enemySkillInfoGO.SetActive(false);
    }

    public void SetPlayerSkillInfoProgressBar(float fillAmount)
    {
        this.playerAttackSkillEffect.fillAmount = fillAmount;
        this.playerRepulseSkillEffect.fillAmount = fillAmount;
    }

    public void SetEnemySkillInfoProgressBar(float fillAmount)
    {
        this.enemyAttackSkillEffect.fillAmount = fillAmount;
        this.enemyRepulseSkillEffect.fillAmount = fillAmount;
    }
}
