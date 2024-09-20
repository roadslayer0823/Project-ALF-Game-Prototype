using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillPromptPanelV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float skillNameShowingDuration = 1.0f;
    [SerializeField] private float skillInfoPopSpeed = 0.1f;
    [SerializeField] private float passiveSkillSlotFadeSpeed = 0.5f;
    [SerializeField] private float passiveSkillSlotShowingDuration = 1.5f;
    [SerializeField] private RectTransform speedEffectImage = null;
    [SerializeField] private RectTransform strengthEffectImage = null;

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
    [SerializeField] private Image playerMarkIcon = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot1 = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot2 = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot3 = null;
    [SerializeField] private Image playerPassiveSkillBackgroundSlot4 = null;
    [SerializeField] private TextMeshProUGUI playerCommandPhaseText = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot1 = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot2 = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot3 = null;
    [SerializeField] private TextMeshProUGUI playerPassiveSkillTextSlot4 = null;

    private Vector3 enemyPassiveSkillSlot1_Position;
    private Vector3 enemyPassiveSkillSlot2_Position;
    private Vector3 enemyPassiveSkillSlot3_Position;
    private Vector3 enemyPassiveSkillSlot4_Position;

    private Vector3 playerPassiveSkillSlot1_Position;
    private Vector3 playerPassiveSkillSlot2_Position;
    private Vector3 playerPassiveSkillSlot3_Position;
    private Vector3 playerPassiveSkillSlot4_Position;

    public void Initialize()
    {
        this.enemyPassiveSkillSlot1_Position = this.enemyPassiveSkillSlot1.transform.position;
        this.enemyPassiveSkillSlot2_Position = this.enemyPassiveSkillSlot2.transform.position;
        this.enemyPassiveSkillSlot3_Position = this.enemyPassiveSkillSlot3.transform.position;
        this.enemyPassiveSkillSlot4_Position = this.enemyPassiveSkillSlot4.transform.position;

        this.playerPassiveSkillSlot1_Position = this.playerPassiveSkillSlot1.transform.position;
        this.playerPassiveSkillSlot2_Position = this.playerPassiveSkillSlot2.transform.position;
        this.playerPassiveSkillSlot3_Position = this.playerPassiveSkillSlot3.transform.position;
        this.playerPassiveSkillSlot4_Position = this.playerPassiveSkillSlot4.transform.position;
    }

    // hide the skill prompt panel
    private void Hide()
    {
        this.skillNameGO.SetActive(false);
        this.enemyPassiveSkillName.SetActive(false);
        this.playerPassiveSkillName.SetActive(false);
    }

    // set the command phase progress bar's percentage
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
                this.playerActiveSkillIcon.gameObject.SetActive(false);
                this.playerBackendSkillProgressBar.gameObject.SetActive(true);

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

    // setup and show the command phase progress bar
    public void ShowCasterCurrentSkillInfo(GameCharacter caster)
    {
        CharacterSkill _characterSkill = caster.GetCurrentSkill();

        if (_characterSkill == null || this.skillNameGO.activeInHierarchy)
        {
            return;
        }

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

        this.skillNameBackgroundImage.SetNativeSize();
        this.skillNameBackgroundImage.rectTransform.anchoredPosition = new Vector3(0f, 0f, 0f);
        this.skillNameText.SetText(_characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);


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

    public void PlaySpeedStrengthAnimation(GameCharacter gameCharacter)
    {
        CharacterSkill _characterSkill = gameCharacter.GetCurrentSkill();
        int _skillStatIncrement = gameCharacter.GetCurrentSkillStatIncrement();

        this.speedEffectGO.gameObject.SetActive(true);
        this.strengthEffectGO.gameObject.SetActive(true);

        int _speed = _characterSkill.GetCharacterSubskillData().GetSubskillData().Speed;
        int _strength = _characterSkill.GetCharacterSubskillData().GetSubskillData().Strength;

        if (_speed + _skillStatIncrement == 3)
        {
            //this.speedEffectAnimator.Play("SpeedV2");
            this.speedEffectAnimator.Play("SpeedV3");
        }
        else if (_speed + _skillStatIncrement >= 4)
        {
            this.speedEffectAnimator.Play("GodSpeedV2");
        }

        else if (_strength + _skillStatIncrement == 2)
        {
            //this.strengthEffectAnimator.Play("Strength_1_V2");
            this.strengthEffectAnimator.Play("Strength_1_V3");
        }
        else if (_strength + _skillStatIncrement >= 3)
        {
            this.strengthEffectAnimator.Play("Strength_2_V2");
        }

        if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.PlayerOne))
        {
            ChangeSpeedAndStrengthEffectScaleX(2);
        }
        else
        {
            ChangeSpeedAndStrengthEffectScaleX(-2);
        }
    }

    // show the command phase
    public void ShowCommandPhase(string phaseName, bool isPlayer, float duration = 0)
    {
        if (isPlayer)
        {
            this.playerCommandPhaseGO.SetActive(true);
            this.playerCommandPhaseText.SetText(phaseName);

            LeanTween.cancel( this.playerCommandPhaseGO );
            LeanTween.moveLocalX(this.playerCommandPhaseGO, -20.0f, this.skillInfoPopSpeed);
            if(phaseName == TerminologyManager.REPULSE_COMMAND_TIME)
            {
                StartCoroutine(WaitBeforeRepulse());
            }

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
            LeanTween.moveLocalX(this.enemyCommandPhaseGO, 20.0f, this.skillInfoPopSpeed);

            if (duration > 0)
            {
                LeanTween.moveLocalX(this.enemyCommandPhaseGO, -600.0f, this.skillInfoPopSpeed).setDelay(duration).setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(this.enemyCommandPhaseGO);
            }
        }
    }


    public IEnumerator WaitBeforeRepulse()
    {
        yield return new WaitForSeconds(0.2f);
        this.playerBackendSkillIcon.gameObject.SetActive(false);
        this.playerMarkIcon.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.playerBackendSkillIcon.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.playerMarkIcon.gameObject.SetActive(false);
    }

    // hide the command phase
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

    // show the skill tag
    private void ShowSkillTag(CharacterSkill characterSkill, bool isPlayer)
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
                        this.playerSkillTagText.SetText("對廣角");
                    }
                    else
                    {
                        this.playerSkillTagGO.SetActive(false);
                    }
                }
            }

            LeanTween.moveLocalX(this.playerSkillTagGO, 150.0f, this.skillInfoPopSpeed);
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
                        this.enemySkillTagText.SetText("對廣角");
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

    // hide when the specific game object finish tweened
    private void OnCompleteTweenGameObject(object gameObject)
    {
        GameObject _gameObject = (GameObject)gameObject;
        _gameObject.SetActive(false);
    }

    // show passive skill effect tag
    public void ShowPassiveSkillEffectTag(string tagName, bool isPlayer)
    {
        HideNotTweeningPassiveSkillSlot();

        if (isPlayer)
        {
            if (!this.playerPassiveSkillSlot1.activeInHierarchy) // slot 1
            {
                this.playerPassiveSkillSlot1.transform.position = this.playerPassiveSkillSlot1_Position;
                PopPassiveSkillSlot(this.playerPassiveSkillSlot1, this.playerPassiveSkillBackgroundSlot1, this.playerPassiveSkillTextSlot1, tagName, isPlayer);

                if (this.playerPassiveSkillSlot3.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot3, this.playerPassiveSkillBackgroundSlot3, this.playerPassiveSkillTextSlot3, isPlayer);
                }
            }
            else if (this.playerPassiveSkillSlot1.activeInHierarchy && !this.playerPassiveSkillSlot2.activeInHierarchy) //slot 2
            {
                this.playerPassiveSkillSlot2.transform.position = this.playerPassiveSkillSlot2_Position;
                PopPassiveSkillSlot(this.playerPassiveSkillSlot2, this.playerPassiveSkillBackgroundSlot2, this.playerPassiveSkillTextSlot2, tagName, isPlayer);

                if (this.playerPassiveSkillSlot4.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot4, this.playerPassiveSkillBackgroundSlot4, this.playerPassiveSkillTextSlot4, isPlayer);
                }
            }
            else if (this.playerPassiveSkillSlot1.activeInHierarchy && this.playerPassiveSkillSlot2.activeInHierarchy && !this.playerPassiveSkillSlot3.activeInHierarchy) //slot 3
            {
                this.playerPassiveSkillSlot3.transform.position = this.playerPassiveSkillSlot3_Position;
                PopPassiveSkillSlot(this.playerPassiveSkillSlot3, this.playerPassiveSkillBackgroundSlot3, this.playerPassiveSkillTextSlot3, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot1, this.playerPassiveSkillBackgroundSlot1, this.playerPassiveSkillTextSlot1, isPlayer);
            }
            else if (this.playerPassiveSkillSlot1.activeInHierarchy && this.playerPassiveSkillSlot2.activeInHierarchy && this.playerPassiveSkillSlot3.activeInHierarchy && !this.playerPassiveSkillSlot4.activeInHierarchy) //slot 4
            {
                this.playerPassiveSkillSlot4.transform.position = this.playerPassiveSkillSlot4_Position;
                PopPassiveSkillSlot(this.playerPassiveSkillSlot4, this.playerPassiveSkillBackgroundSlot4, this.playerPassiveSkillTextSlot4, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.playerPassiveSkillSlot2, this.playerPassiveSkillBackgroundSlot2, this.playerPassiveSkillTextSlot2, isPlayer);
            }
        }
        else
        {
            if (!this.enemyPassiveSkillSlot1.activeInHierarchy) // slot 1
            {
                this.enemyPassiveSkillSlot1.transform.position = this.enemyPassiveSkillSlot1_Position;
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot1, this.enemyPassiveSkillBackgroundSlot1, this.enemyPassiveSkillTextSlot1, tagName, isPlayer);

                if (this.enemyPassiveSkillSlot3.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot3, this.enemyPassiveSkillBackgroundSlot3, this.enemyPassiveSkillTextSlot3, isPlayer);
                }
            }
            else if (this.enemyPassiveSkillSlot1.activeInHierarchy && !this.enemyPassiveSkillSlot2.activeInHierarchy) //slot 2
            {
                this.enemyPassiveSkillSlot2.transform.position = this.enemyPassiveSkillSlot2_Position;
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot2, this.enemyPassiveSkillBackgroundSlot2, this.enemyPassiveSkillTextSlot2, tagName, isPlayer);

                if (this.enemyPassiveSkillSlot4.activeInHierarchy)
                {
                    PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot4, this.enemyPassiveSkillBackgroundSlot4, this.enemyPassiveSkillTextSlot4, isPlayer);
                }
            }
            else if (this.enemyPassiveSkillSlot1.activeInHierarchy && this.enemyPassiveSkillSlot2.activeInHierarchy && !this.enemyPassiveSkillSlot3.activeInHierarchy) //slot 3
            {
                this.enemyPassiveSkillSlot3.transform.position = this.enemyPassiveSkillSlot3_Position;
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot3, this.enemyPassiveSkillBackgroundSlot3, this.enemyPassiveSkillTextSlot3, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot1, this.enemyPassiveSkillBackgroundSlot1, this.enemyPassiveSkillTextSlot1, isPlayer);
            }
            else if (this.enemyPassiveSkillSlot1.activeInHierarchy && this.enemyPassiveSkillSlot2.activeInHierarchy && this.enemyPassiveSkillSlot3.activeInHierarchy && !this.enemyPassiveSkillSlot4.activeInHierarchy) //slot 4
            {
                this.enemyPassiveSkillSlot4.transform.position = this.enemyPassiveSkillSlot4_Position;
                PopPassiveSkillSlot(this.enemyPassiveSkillSlot4, this.enemyPassiveSkillBackgroundSlot4, this.enemyPassiveSkillTextSlot4, tagName, isPlayer);

                PushFurtherPassiveSkillSlot(this.enemyPassiveSkillSlot2, this.enemyPassiveSkillBackgroundSlot2, this.enemyPassiveSkillTextSlot2, isPlayer);
            }
        }
    }

    // hide the passive Skill Slot that not tweening
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

    // set the alpha back to max after after being pushed away
    private void SetAlphaToMax(Image backgroundImage, TextMeshProUGUI text)
    {
        text.alpha = 1.0f;

        Color _backgroundImageColor = backgroundImage.color;
        _backgroundImageColor.a = 1.0f;
        backgroundImage.color = _backgroundImageColor;
    }

    // show the passive skill slot
    private void PopPassiveSkillSlot(GameObject slotToPop, Image slotBackground, TextMeshProUGUI slotText, string tagName, bool isPlayer)
    {
        LeanTween.cancel( slotToPop );

        slotToPop.SetActive(true);
        slotText.SetText(tagName);
        SetAlphaToMax(slotBackground, slotText);

        LeanTween.moveLocalX(slotToPop, 0.0f, this.skillInfoPopSpeed);
        LeanTween.moveLocalX(slotToPop, isPlayer? 600.0f : -600f, this.skillInfoPopSpeed).setDelay(this.passiveSkillSlotShowingDuration)
            .setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(slotToPop);
    }

    // push further the passive skill slot
    private void PushFurtherPassiveSkillSlot(GameObject slotToPush, Image slotBackground, TextMeshProUGUI slotText, bool isPlayer)
    {
        LeanTween.cancel(slotToPush);
        LeanTween.moveLocalX(slotToPush, isPlayer? -250.0f : 250.0f, this.skillInfoPopSpeed);

        // gradually decrease the alpha to zero
        LeanTween.value(slotText.gameObject, 1f, 0f, this.passiveSkillSlotFadeSpeed).setOnUpdate((float value) =>
        {
            slotText.alpha = value;
        });

        LeanTween.alpha(slotBackground.rectTransform, 0f, this.passiveSkillSlotFadeSpeed);
        LeanTween.moveLocalX(slotToPush, isPlayer ? 600.0f : -600f, this.skillInfoPopSpeed).setDelay(this.passiveSkillSlotFadeSpeed)
            .setOnComplete(OnCompleteTweenGameObject).setOnCompleteParam(slotToPush);
    }

    private void ChangeSpeedAndStrengthEffectScaleX(float currentScale)
    {
        /*Vector3 playerSpeedEffectScaleX = this.speedEffectImage.localScale;
        Vector3 playerStrengthEffectScaleX = this.strengthEffectImage.localScale;
        playerSpeedEffectScaleX.x = currentScale;
        playerStrengthEffectScaleX.x = currentScale;
        this.speedEffectImage.localScale = playerSpeedEffectScaleX;
        this.strengthEffectImage.localScale = playerStrengthEffectScaleX;*/

        this.speedEffectImage.localScale = this.strengthEffectImage.localScale =
            new Vector3(currentScale, this.speedEffectImage.localScale.y, this.speedEffectImage.localScale.z);
    }
}
