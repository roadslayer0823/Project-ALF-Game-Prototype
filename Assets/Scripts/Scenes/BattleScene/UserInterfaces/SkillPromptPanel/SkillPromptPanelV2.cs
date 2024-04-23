using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillPromptPanelV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float skillNameShowingDuration = 1.0f;
    [SerializeField] private float skillInfoPopSpeed = 0.1f;
    [SerializeField] private float passiveSkillSlotFadeSpeed = 0.5f;
    [SerializeField] private float passiveSkillSlotShowingDuration = 1.5f;

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

    [Header("SkillInfo || Enemy")]
    [SerializeField] private GameObject enemySkillInfoGO = null;
    [SerializeField] private GameObject enemySkillTagGO = null;
    [SerializeField] private GameObject enemyCommandPhaseGO = null;
    [SerializeField] private GameObject enemyPassiveSkillSlot1 = null;
    [SerializeField] private GameObject enemyPassiveSkillSlot2 = null;
    [SerializeField] private GameObject enemyPassiveSkillSlot3 = null;
    [SerializeField] private GameObject enemyPassiveSkillSlot4 = null;
    [SerializeField] private TextMeshProUGUI enemySkillTagText = null;
    [SerializeField] private Image enemyActiveSkillProgressBar = null;
    [SerializeField] private Image enemyBackendSkillProgressBar = null;
    [SerializeField] private Image enemyActiveSkillIcon = null;
    [SerializeField] private Image enemyBackendSkillIcon = null;
    [SerializeField] private Image enemyPassiveSkillBackgroundSlot1 = null;
    [SerializeField] private Image enemyPassiveSkillBackgroundSlot2 = null;
    [SerializeField] private Image enemyPassiveSkillBackgroundSlot3 = null;
    [SerializeField] private Image enemyPassiveSkillBackgroundSlot4 = null;
    [SerializeField] private TextMeshProUGUI enemyCommandPhaseText = null;
    [SerializeField] private TextMeshProUGUI enemyPassiveSkillTextSlot1 = null;
    [SerializeField] private TextMeshProUGUI enemyPassiveSkillTextSlot2 = null;
    [SerializeField] private TextMeshProUGUI enemyPassiveSkillTextSlot3 = null;
    [SerializeField] private TextMeshProUGUI enemyPassiveSkillTextSlot4 = null;

    [Header("SkillInfo || Player")]
    [SerializeField] private GameObject playerSkillInfoGO = null;
    [SerializeField] private GameObject playerSkillTagGO = null;
    [SerializeField] private GameObject playerCommandPhaseGO = null;
    [SerializeField] private GameObject playerPassiveSkillSlot1 = null;
    [SerializeField] private GameObject playerPassiveSkillSlot2 = null;
    [SerializeField] private GameObject playerPassiveSkillSlot3 = null;
    [SerializeField] private GameObject playerPassiveSkillSlot4 = null;
    [SerializeField] private TextMeshProUGUI playerSkillTagText = null;
    [SerializeField] private Image playerActiveSkillProgressBar = null;
    [SerializeField] private Image playerBackendSkillProgressBar = null;
    [SerializeField] private Image playerActiveSkillIcon = null;
    [SerializeField] private Image playerBackendSkillIcon = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot1 = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot2 = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot3 = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot4 = null;
    [SerializeField] private TextMeshProUGUI playerCommandPhaseText = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot1 = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot2 = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot3 = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot4 = null;

    private void Hide()
    {
        this.skillNameGO.SetActive(false);
        this.enemyPassiveSkillName.SetActive(false);
        this.playerPassiveSkillName.SetActive(false);
    }

    public void SetCommandPhaseProgressBar(float fillAmount, bool isActiveSkill, bool isPlayer)
    {
        if (isPlayer)
        {
            if (isActiveSkill)
            {
                this.playerActiveSkillProgressBar.gameObject.SetActive(true);
                this.playerBackendSkillProgressBar.gameObject.SetActive(false);
                this.playerActiveSkillIcon.gameObject.SetActive(true);
                this.playerBackendSkillIcon.gameObject.SetActive(false);

                this.playerActiveSkillProgressBar.fillAmount = fillAmount;
            }
            else
            {
                this.playerActiveSkillProgressBar.gameObject.SetActive(false);
                this.playerBackendSkillProgressBar.gameObject.SetActive(true);
                this.playerActiveSkillIcon.gameObject.SetActive(false);
                this.playerBackendSkillIcon.gameObject.SetActive(true);

                this.playerBackendSkillProgressBar.fillAmount = fillAmount;
            }
        }
        else
        {
            if (isActiveSkill)
            {
                this.enemyActiveSkillProgressBar.gameObject.SetActive(true);
                this.enemyBackendSkillProgressBar.gameObject.SetActive(false);
                this.enemyActiveSkillIcon.gameObject.SetActive(true);
                this.enemyBackendSkillIcon.gameObject.SetActive(false);

                this.enemyActiveSkillProgressBar.fillAmount = fillAmount;
            }
            else
            {
                this.enemyActiveSkillProgressBar.gameObject.SetActive(false);
                this.enemyBackendSkillProgressBar.gameObject.SetActive(true);
                this.enemyActiveSkillIcon.gameObject.SetActive(false);
                this.enemyBackendSkillIcon.gameObject.SetActive(true);

                this.enemyBackendSkillProgressBar.fillAmount = fillAmount;
            }
        }
    }

    public void ShowCasterCurrentSkillInfo(GameCharacter caster)
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
        LeanTween.delayedCall(skillNameShowingDuration, Hide);

        if (caster.GetIsPlayer())
        {
            ShowSkillTag(_characterSkill, true);
        }
        else
        {
            ShowSkillTag(_characterSkill, false);
        }
    }

    public void ShowCommandPhase(string phaseName, bool isPlayer, float duration = 0)
    {
        if (isPlayer)
        {
            this.playerCommandPhaseGO.SetActive(true);
            this.playerCommandPhaseText.SetText(phaseName);

            LeanTween.cancel( this.playerCommandPhaseGO );
            LeanTween.moveLocalX(this.playerCommandPhaseGO, 0.0f, this.skillInfoPopSpeed);

            if (duration > 0)
            {
                LeanTween.moveLocalX(this.playerCommandPhaseGO, 600.0f, this.skillInfoPopSpeed).setDelay(duration).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.playerCommandPhaseGO);
            }
        }
        else
        {
            this.enemyCommandPhaseGO.SetActive(true);
            this.enemyCommandPhaseText.SetText(phaseName);

            LeanTween.cancel( this.enemyCommandPhaseGO );
            LeanTween.moveLocalX(this.enemyCommandPhaseGO, 0.0f, this.skillInfoPopSpeed);

            if (duration > 0)
            {
                LeanTween.moveLocalX(this.enemyCommandPhaseGO, -600.0f, this.skillInfoPopSpeed).setDelay(duration).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.enemyCommandPhaseGO);
            }
        }
    }

    public void HideCommandPhase(bool isPlayer)
    {
        if (isPlayer)
        {
            LeanTween.cancel(this.playerCommandPhaseGO);
            LeanTween.moveLocalX(this.playerCommandPhaseGO, 600.0f, this.skillInfoPopSpeed).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.playerCommandPhaseGO);

            this.playerActiveSkillProgressBar.fillAmount = 0;
            this.playerBackendSkillProgressBar.fillAmount = 0;
        }
        else
        {
            LeanTween.cancel(this.enemyCommandPhaseGO);
            LeanTween.moveLocalX(this.enemyCommandPhaseGO, -600.0f, this.skillInfoPopSpeed).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.enemyCommandPhaseGO);

            this.enemyActiveSkillProgressBar.fillAmount = 0;
            this.enemyBackendSkillProgressBar.fillAmount = 0;
        }
    }

    public void ShowSkillTag(CharacterSkill characterSkill, bool isPlayer)
    {
        if (isPlayer && characterSkill != null && !LeanTween.isTweening(this.playerSkillInfoGO))
        {
            CharacterSubskill _playerCharacterSubskill = characterSkill.GetCharacterSubskillData();

            this.playerSkillInfoGO.SetActive(true);

            if (_playerCharacterSubskill != null)
            {
                if (characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.derived)
                {
                    this.playerActiveSkillProgressBar.gameObject.SetActive(true);
                    this.playerBackendSkillProgressBar.gameObject.SetActive(false);
                    this.playerActiveSkillIcon.gameObject.SetActive(true);
                    this.playerBackendSkillIcon.gameObject.SetActive(false);

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
                    this.playerActiveSkillProgressBar.gameObject.SetActive(false);
                    this.playerBackendSkillProgressBar.gameObject.SetActive(true);
                    this.playerActiveSkillIcon.gameObject.SetActive(false);
                    this.playerBackendSkillIcon.gameObject.SetActive(true);

                    if (_playerCharacterSubskill.GetSubskillData().EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide)
                    {
                        this.playerSkillTagGO.SetActive(true);
                        this.playerSkillTagText.SetText("对廣角");
                    }
                    else if (_playerCharacterSubskill.GetSubskillData().IsObservingSkill)
                    {
                        this.playerSkillTagGO.SetActive( true );
                        this.playerSkillTagText.SetText( _playerCharacterSubskill.GetSubskillData().DisplayName );
                    }
                    else
                    {
                        this.playerSkillTagGO.SetActive(false);
                    }
                }
            }

            LeanTween.moveLocalX(this.playerSkillTagGO, 0.0f, this.skillInfoPopSpeed);
            LeanTween.moveLocalX(this.playerSkillTagGO, 600.0f, this.skillInfoPopSpeed).setDelay(this.skillNameShowingDuration).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.playerSkillTagGO);
        }
        else if (!isPlayer && characterSkill != null && !LeanTween.isTweening(this.enemySkillInfoGO))
        {
            CharacterSubskill _enemyCharacterSubskill = characterSkill.GetCharacterSubskillData();

            this.enemySkillInfoGO.SetActive(true);

            if (_enemyCharacterSubskill != null)
            {
                if (characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse
                    || characterSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.derived)
                {
                    this.enemyActiveSkillProgressBar.gameObject.SetActive(true);
                    this.enemyBackendSkillProgressBar.gameObject.SetActive(false);
                    this.enemyActiveSkillIcon.gameObject.SetActive(true);
                    this.enemyBackendSkillIcon.gameObject.SetActive(false);

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
                    this.enemyActiveSkillProgressBar.gameObject.SetActive(false);
                    this.enemyBackendSkillProgressBar.gameObject.SetActive(true);
                    this.enemyActiveSkillIcon.gameObject.SetActive(false);
                    this.enemyBackendSkillIcon.gameObject.SetActive(true);

                    if (_enemyCharacterSubskill.GetSubskillData().EffectType == DatabaseManager.Subskill.EffectTypeEnum.wide)
                    {
                        this.enemySkillTagGO.SetActive(true);
                        this.enemySkillTagText.SetText("对廣角");
                    }
                    else if (_enemyCharacterSubskill.GetSubskillData().IsObservingSkill)
                    {
                        this.playerSkillTagGO.SetActive( true );
                        this.playerSkillTagText.SetText( _enemyCharacterSubskill.GetSubskillData().DisplayName );
                    }
                    else
                    {
                        this.enemySkillTagGO.SetActive(false);
                    }
                }
            }

            LeanTween.moveLocalX(this.enemySkillTagGO, 0.0f, this.skillInfoPopSpeed);
            LeanTween.moveLocalX(this.enemySkillTagGO, -600.0f, this.skillInfoPopSpeed).setDelay(this.skillNameShowingDuration).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.enemySkillTagGO);
        }
    }

    private void OnCompleteTweenGameObject(object gameObject)
    {
        GameObject _gameObject = (GameObject)gameObject;
        _gameObject.SetActive(false);
    }

    public void ShowPassiveSkillEffectTag(string tagName, bool isPlayer)
    {
        HideNotTweeningPassiveSkillSlot();

        if (isPlayer)
        {
            if (!this.playerPassiveSkillSlot1.activeInHierarchy) // slot 1
            {
                PopPassiveSkillSlot(this.playerPassiveSkillSlot1, this.playerPassiveSkillBackgroundSlot1, this.playerPassiveSkillTextSlot1, tagName, isPlayer);

                if (this.playerPassiveSkillSlot3.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot3, this.playerPassiveSkillBackgroundSlot3, this.playerPassiveSkillTextSlot3, isPlayer);
                }
            }
            else if (this.playerPassiveSkillSlot1.activeInHierarchy && !this.playerPassiveSkillSlot2.activeInHierarchy) //slot 2
            {
                PopPassiveSkillSlot(this.playerPassiveSkillSlot2, this.playerPassiveSkillBackgroundSlot2, this.playerPassiveSkillTextSlot2, tagName, isPlayer);

                if (this.playerPassiveSkillSlot4.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot4, this.playerPassiveSkillBackgroundSlot4, this.playerPassiveSkillTextSlot4, isPlayer);
                }
            }
            else if (this.playerPassiveSkillSlot1.activeInHierarchy && this.playerPassiveSkillSlot2.activeInHierarchy && !this.playerPassiveSkillSlot3.activeInHierarchy) //slot 3
            {
                PopPassiveSkillSlot(this.playerPassiveSkillSlot3, this.playerPassiveSkillBackgroundSlot3, this.playerPassiveSkillTextSlot3, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot1, this.playerPassiveSkillBackgroundSlot1, this.playerPassiveSkillTextSlot1, isPlayer);
            }
            else if (this.playerPassiveSkillSlot1.activeInHierarchy && this.playerPassiveSkillSlot2.activeInHierarchy && this.playerPassiveSkillSlot3.activeInHierarchy && !this.playerPassiveSkillSlot4.activeInHierarchy) //slot 4
            {
                PopPassiveSkillSlot(this.playerPassiveSkillSlot4, this.playerPassiveSkillBackgroundSlot4, this.playerPassiveSkillTextSlot4, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot2, this.playerPassiveSkillBackgroundSlot2, this.playerPassiveSkillTextSlot2, isPlayer);
            }
        }
        else
        {
            if (!this.enemyPassiveSkillSlot1.activeInHierarchy) // slot 1
            {
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot1, this.enemyPassiveSkillBackgroundSlot1, this.enemyPassiveSkillTextSlot1, tagName, isPlayer);

                if (this.enemyPassiveSkillSlot3.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot3, this.enemyPassiveSkillBackgroundSlot3, this.enemyPassiveSkillTextSlot3, isPlayer);
                }
            }
            else if (this.enemyPassiveSkillSlot1.activeInHierarchy && !this.enemyPassiveSkillSlot2.activeInHierarchy) //slot 2
            {
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot2, this.enemyPassiveSkillBackgroundSlot2, this.enemyPassiveSkillTextSlot2, tagName, isPlayer);

                if (this.enemyPassiveSkillSlot4.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot4, this.enemyPassiveSkillBackgroundSlot4, this.enemyPassiveSkillTextSlot4, isPlayer);
                }
            }
            else if (this.enemyPassiveSkillSlot1.activeInHierarchy && this.enemyPassiveSkillSlot2.activeInHierarchy && !this.enemyPassiveSkillSlot3.activeInHierarchy) //slot 3
            {
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot3, this.enemyPassiveSkillBackgroundSlot3, this.enemyPassiveSkillTextSlot3, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot1, this.enemyPassiveSkillBackgroundSlot1, this.enemyPassiveSkillTextSlot1, isPlayer);
            }
            else if (this.enemyPassiveSkillSlot1.activeInHierarchy && this.enemyPassiveSkillSlot2.activeInHierarchy && this.enemyPassiveSkillSlot3.activeInHierarchy && !this.enemyPassiveSkillSlot4.activeInHierarchy) //slot 4
            {
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot4, this.enemyPassiveSkillBackgroundSlot4, this.enemyPassiveSkillTextSlot4, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot2, this.enemyPassiveSkillBackgroundSlot2, this.enemyPassiveSkillTextSlot2, isPlayer);
            }
        }
    }

    private void HideNotTweeningPassiveSkillSlot()
    {
        if (!LeanTween.isTweening(this.playerPassiveSkillSlot1))
        {
            this.playerPassiveSkillSlot1.SetActive(false);
        }

        if (!LeanTween.isTweening(this.playerPassiveSkillSlot2))
        {
            this.playerPassiveSkillSlot2.SetActive(false);
        }

        if (!LeanTween.isTweening(this.playerPassiveSkillSlot3))
        {
            this.playerPassiveSkillSlot3.SetActive(false);
        }

        if (!LeanTween.isTweening(this.playerPassiveSkillSlot4))
        {
            this.playerPassiveSkillSlot4.SetActive(false);
        }

        if (!LeanTween.isTweening(this.enemyPassiveSkillSlot1))
        {
            this.enemyPassiveSkillSlot1.SetActive(false);
        }

        if (!LeanTween.isTweening(this.enemyPassiveSkillSlot2))
        {
            this.enemyPassiveSkillSlot2.SetActive(false);
        }

        if (!LeanTween.isTweening(this.enemyPassiveSkillSlot3))
        {
            this.enemyPassiveSkillSlot3.SetActive(false);
        }

        if (!LeanTween.isTweening(this.enemyPassiveSkillSlot4))
        {
            this.enemyPassiveSkillSlot4.SetActive(false);
        }
    }

    private void SetAlphaToMax(Image backgroundImage, TextMeshProUGUI text)
    {
        text.alpha = 1.0f;

        Color _backgroundImageColor = backgroundImage.color;
        _backgroundImageColor.a = 1.0f;
        backgroundImage.color = _backgroundImageColor;
    }

    private void PopPassiveSkillSlot(GameObject slotToPop, Image slotBackground, TextMeshProUGUI slotText, string tagName, bool isPlayer)
    {
        slotToPop.SetActive(true);
        slotText.SetText(tagName);
        SetAlphaToMax(slotBackground, slotText);

        LeanTween.moveLocalX(slotToPop, 0.0f, this.skillInfoPopSpeed);
        LeanTween.moveLocalX(slotToPop, isPlayer? 600.0f : -600f, this.skillInfoPopSpeed).setDelay(this.passiveSkillSlotShowingDuration)
            .setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(slotToPop);
    }

    private void PushFurtherPassiveSkillSlot(GameObject slotToPush, Image slotBackground, TextMeshProUGUI slotText, bool isPlayer)
    {
        LeanTween.cancel(slotToPush);
        LeanTween.moveLocalX(slotToPush, isPlayer? -250.0f : 250.0f, this.skillInfoPopSpeed);

        LeanTween.value(slotText.gameObject, 1f, 0f, this.passiveSkillSlotFadeSpeed).setOnUpdate((float value) =>
        {
            slotText.alpha = value;
        });

        LeanTween.alpha(slotBackground.rectTransform, 0f, this.passiveSkillSlotFadeSpeed);
        LeanTween.moveLocalX(slotToPush, isPlayer ? 600.0f : -600f, this.skillInfoPopSpeed).setDelay(this.passiveSkillSlotFadeSpeed)
            .setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(slotToPush);
    }
}
