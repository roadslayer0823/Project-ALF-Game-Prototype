using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;
using Skill = DatabaseManager.Skill;

public class SkillSlotV2 : MonoBehaviour
{
    [Header("Skill UI")]
    [SerializeField] private Image skillFrame;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Sprite SampleSkillIcon;
    [SerializeField] private Sprite BlankActiveSkillFrame;
    [SerializeField] private Sprite BlankBackendSkillFrame;
    [SerializeField] private Sprite ActiveSkillFrame;
    [SerializeField] private Sprite BackendSkillFrame;
    [SerializeField] private Sprite RepulseSkillFrame;
    [SerializeField] private Sprite DerivedSkillFrame;
    [SerializeField] private Sprite CounterSkillFrame;
    [SerializeField] private GameObject SelectedSkillEffect;

    [Header("Modify Skill Level UI")]
    [SerializeField] private TextMeshProUGUI skillSlotText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private TextMeshProUGUI skillPrefixText;
    [SerializeField] private TextMeshProUGUI skillLevelAnimationText;
    [SerializeField] private RectTransform swipeableArea = null;
    [SerializeField] private GameObject skillLevelGameObject;
    [SerializeField] private GameObject skillTextAnimation;
    [SerializeField] private Button skillSelectionButton;
    [SerializeField] private Image plusLevelImage;
    [SerializeField] private Image plusLevelBackground;
    [SerializeField] private Image minusLevelImage;
    [SerializeField] private Image minusLevelBackground;
    [SerializeField] private Transform plusLevelTargetPosition;
    [SerializeField] private Transform plusLevelOriginalPosition;
    [SerializeField] private Transform minusLevelTargetPosition;
    [SerializeField] private Transform minusLevelOriginalPosition;

    //animation reference
    [SerializeField] private Animator skillBoxAnimation = null;

    private CharacterSkill selectedSkill = null;
    private SkillSlotListPanelV2 skillSlotListPanelV2 = null;
    private Vector2 mousePressPosition = new Vector2();
    private Vector2 mouseReleasePosition = new Vector2();
    private Vector2 currentSwipe = new Vector2();

    private bool isSkillLevelChanged = false;
    private int skillLevel = 1;

    //audio and animation clip id
    private const string AUDIO_ID_BOOST_LEVEL_UP = "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";
    private const string ANIMATION_ID_ACTIVE_SKILL_OUTLINE_RESIZE = "ActiveSkillOutlineResize";
    private const string ANIMATION_ID_ACTIVE_SKILL_OUTLINE_EXPAND = "ActiveSkillOutlineExpand";
    private const string ANIMATION_ID_BACKEND_SKILL_OUTLINE_RESIZE = "BackendSkillOutlineResize";
    private const string ANIMATION_ID_BACKEND_SKILL_OUTLINE_EXPAND = "BackendSkillOutlineExpand";

    public enum StateType
    {
        None,
        Selected,
        Activate,
        Disable,
        Enable
    }

    public enum SkillType
    {
        None,
        ActiveSkill,
        BackendSkill
    }

    private void Update()
    {
        Swipe();
    }

    public void Initialize(SkillSlotListPanelV2 skillSlotListPanelV2)
    {
        this.skillSlotListPanelV2 = skillSlotListPanelV2;
        this.SetBlankFrame(SkillType.ActiveSkill);
    }

    public void Clear()
    {
        this.selectedSkill = null;
        this.SetBlankFrame(SkillType.ActiveSkill);
        this.skillSlotText.SetText("");
        this.skillPrefixText.SetText("");
    }

    public void ClickToSelectActiveSkill()
    {
        this.skillSlotListPanelV2.GetSelectedGameCharacter().SetCurrentSkill(this.selectedSkill);
        SetButtonStateType(StateType.Selected, SkillType.ActiveSkill);
       
    }

    public void ClickToSelectBackendSkill()
    {
        this.skillSlotListPanelV2.GetSelectedGameCharacter().SetCurrentSkill(this.selectedSkill);
        SetButtonStateType(StateType.Selected, SkillType.BackendSkill);
    }

    public void ButtonOnDisable()
    {
        this.skillSelectionButton.interactable = false;
    }

    public void ButtonOnEnable()
    {
        this.skillSelectionButton.interactable = true;
    }

    //changing skill level mechanics
    public void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save starting touch 2D point 
            this.mousePressPosition = Input.mousePosition;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(this.swipeableArea, this.mousePressPosition))
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2D point
            this.mouseReleasePosition = Input.mousePosition;

            if (!this.swipeableArea.gameObject.activeInHierarchy)
            {
                return;
            }

            //create vector from the two point
            this.currentSwipe = new Vector2(this.mouseReleasePosition.x - this.mousePressPosition.x, this.mouseReleasePosition.y - this.mousePressPosition.y);

            //normalize the 2D vector
            this.currentSwipe.Normalize();

            //swipe up
            if (this.currentSwipe.y > 0)
            {
                IncreaseSkillLevel();
            }

            //swipe down
            if (this.currentSwipe.y < 0)
            {
                DecreaseSkillLevel();
            }
        }
    }

    public void IncreaseSkillLevel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_UP);

        int _minimumSkillLevel = this.selectedSkill.GetMinumumSkillLevel();
        int _maximumSkillLevel = this.selectedSkill.GetMaximumSkillLevel();

        this.skillLevel = Math.Clamp( skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.selectedSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);
        }
        UpdateCharacterSkillLevel(this.skillLevel);
        ModifySkillLevelAnimation(plusLevelImage, plusLevelBackground, plusLevelOriginalPosition, plusLevelTargetPosition);
    }

    public void DecreaseSkillLevel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_DOWN);

        int _minimumSkillLevel = this.selectedSkill.GetMinumumSkillLevel();
        int _maximumSkillLevel = this.selectedSkill.GetMaximumSkillLevel();

        this.skillLevel = Math.Clamp(skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.selectedSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);
        }
        UpdateCharacterSkillLevel(this.skillLevel);
        ModifySkillLevelAnimation(minusLevelImage, minusLevelBackground, minusLevelOriginalPosition, minusLevelTargetPosition);
    }

    private void UpdateCharacterSkillLevel(int skillLevel)
    {
        this.selectedSkill.SetSelectedSkillLevel(this.skillLevel);
        this.skillLevelText.SetText($"<size=30>LV.</size> {skillLevel}");
        //this.skillPanelUI.currentPanelLevelText.SetText(skillLevel.ToString());
        this.skillLevelAnimationText.SetText($"<size=30>LV.</size> {skillLevel}");

        UpdateSkillSelectionBoxData();
    }

    private void UpdateSkillSelectionBoxData()
    {
        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();
        this.skillSlotText.SetText(_subskillData.DisplayName);
        this.skillSlotText.SetText(_subskillData.DisplayName.ToString());
    }

    //modify current skill level animation
    private void ModifySkillLevelAnimation(Image levelModifierImage, Image background, Transform originalPosition, Transform targetPosition)
    {
        if (this.skillLevel > 1)
        {
            skillLevelGameObject.SetActive(true);
            skillTextAnimation.gameObject.SetActive(true);
            levelModifierImage.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
        }
        else
        {
            skillLevelGameObject.SetActive(false);
        }

        float duration = 0.1f;
        float targetScale = 3f;
        Vector3 skillTextScale = skillLevelGameObject.transform.localScale;
        levelModifierImage.color = new Color(levelModifierImage.color.r, levelModifierImage.color.g, levelModifierImage.color.b, 0f);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);

        //animation
        LeanTween.move(levelModifierImage.gameObject, targetPosition, duration);
        LeanTween.alpha(levelModifierImage.rectTransform, 1f, duration);
        LeanTween.alpha(background.rectTransform, 1f, duration);
        LeanTween.scale(skillTextAnimation, skillTextScale * targetScale, duration);
        LeanTween.value(skillLevelAnimationText.gameObject, 1f, 0f, duration).setOnUpdate((float value) =>
        {
            skillLevelAnimationText.alpha = value;
        })
            .setOnComplete(() => {
                LeanTween.alpha(background.rectTransform, 0f, 0.3f)
            .setOnComplete(() =>
            {
                //reset all related game object to regular position/opacity
                LeanTween.alpha(levelModifierImage.rectTransform, 0f, duration);
                levelModifierImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
                skillTextAnimation.gameObject.SetActive(false);
                levelModifierImage.transform.position = originalPosition.transform.position;
                skillTextAnimation.transform.localScale = skillTextScale;
            });
       });
    }

    public void SetSelectedSkill(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        this.skillLevel = this.selectedSkill.GetCharacterSubskillData().GetSubskillData().Level;
        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();

        if (_subskillData.Prefix.ToString() == "-")
        {
            this.skillPrefixText.SetText("");
        }
        else
        {
            this.skillPrefixText.SetText("[" + _subskillData.Prefix.ToString() + "]");
        }

        ShowSkillFrame(this.selectedSkill);
        UpdateCharacterSkillLevel(this.skillLevel);
        SetSkillSlotText(_subskillData.DisplayName);
        this.skillIcon.sprite = this.SampleSkillIcon;
        this.skillIcon.gameObject.SetActive(true);
    }

    public void ShowSkillFrame(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        switch (this.selectedSkill.GetSkillData().skillType)
        {
            case Skill.SkillType.active:
                this.skillFrame.sprite = this.ActiveSkillFrame;
                break;

            case Skill.SkillType.backend:
                this.skillFrame.sprite = this.BackendSkillFrame;
                break;

            case Skill.SkillType.repulse:
                this.skillFrame.sprite = this.RepulseSkillFrame;
                break;

            case Skill.SkillType.derived:
                this.skillFrame.sprite = this.DerivedSkillFrame;
                break;

            case Skill.SkillType.counter:
                this.skillFrame.sprite = this.CounterSkillFrame;
                break;
        }
    }

    public void SetBlankFrame(SkillType frameType)
    {
        if (frameType == SkillType.ActiveSkill)
        {
            this.skillFrame.sprite = this.BlankActiveSkillFrame;
            this.skillFrame.SetNativeSize();
        }
        else if (frameType == SkillType.BackendSkill)
        {
            this.skillFrame.sprite = this.BlankBackendSkillFrame;
            this.skillFrame.SetNativeSize();
        }
    }

    public void SetButtonStateType(StateType buttonStateType, SkillType currentSkillType)
    {
        if (buttonStateType == StateType.Selected)
        {
            this.SelectedSkillEffect.SetActive(true);
            ButtonOnDisable();
        }

        else if (buttonStateType == StateType.Enable)
        {
            ButtonOnEnable();
            ShowSkillFrame(this.selectedSkill);
        }
        else
        {
            if (currentSkillType == SkillType.ActiveSkill)
            {
                if (buttonStateType == StateType.Activate)
                {
                    this.SelectedSkillEffect.SetActive(false);
                    SetBlankFrame(SkillType.ActiveSkill);
                }
                else if (buttonStateType == StateType.Disable)
                {
                    SetBlankFrame(SkillType.ActiveSkill);
                }
            }

            if (currentSkillType == SkillType.BackendSkill)
            {
                if (buttonStateType == StateType.Activate)
                {
                    this.SelectedSkillEffect.SetActive(false);
                    SetBlankFrame(SkillType.BackendSkill);
                }
                else if (buttonStateType == StateType.Disable)
                {
                    SetBlankFrame(SkillType.BackendSkill);
                }
            }
        }
    }

    public void SetSelectSkillAnimation(SkillType currentSkillType)
    {
        if(currentSkillType == SkillType.ActiveSkill)
        {
            this.skillBoxAnimation.Play(ANIMATION_ID_ACTIVE_SKILL_OUTLINE_RESIZE, 1, 0f);
            this.skillBoxAnimation.Play(ANIMATION_ID_ACTIVE_SKILL_OUTLINE_EXPAND, 0, 0f);
        }
        else if(currentSkillType == SkillType.BackendSkill)
        {
            this.skillBoxAnimation.Play(ANIMATION_ID_BACKEND_SKILL_OUTLINE_RESIZE, 1, 0f);
            this.skillBoxAnimation.Play(ANIMATION_ID_BACKEND_SKILL_OUTLINE_EXPAND, 0, 0f);
        }
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public bool CheckIsSkillLevelChanged()
    {
        return this.isSkillLevelChanged;
    }

    public void SetSkillSlotText(string slotText)
    {
        this.skillSlotText.SetText(slotText);
    }

}
