using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillPromptPanelV2 : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float skillNameShowingDuration = 1.0f;

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

        if (_characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
        {
            this.skillNameBackgroundImage.sprite = this.activeSkillBackgroundImage;
        }
        else if (_characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend)
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
}
