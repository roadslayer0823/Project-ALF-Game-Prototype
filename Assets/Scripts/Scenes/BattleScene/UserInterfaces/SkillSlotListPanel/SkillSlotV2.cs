using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;

public class SkillSlotV2 : MonoBehaviour
{
    [Header("Skill UI")]
    [SerializeField] private Image skillFrame;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Sprite DefaultSkillFrame;
    [SerializeField] private Sprite ActiveSkillFrame;
    [SerializeField] private Sprite BackendSkillFrame;
    [SerializeField] private Sprite RepulseSkillFrame;
    [SerializeField] private Sprite DerivedSkillFrame;
    [SerializeField] private Sprite CounterSkillFrame;
    [SerializeField] private Sprite SampleSkillIcon;

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
    private const string ANIMATION_ID_OUTLINE_RESIZE = "OutlineResize";
    private const string ANIMATION_ID_OUTLINE_EXPAND = "OutlineExpand";

    private void Update()
    {
        Swipe();
    }

    public void Initialize(SkillSlotListPanelV2 skillSlotListPanelV2)
    {
        this.skillSlotListPanelV2 = skillSlotListPanelV2;
        this.skillFrame.sprite = this.DefaultSkillFrame;
        this.skillIcon.gameObject.SetActive(false);
    }

    public void Clear()
    {
        this.selectedSkill = null;
        this.skillSlotText.SetText("");
    }

    public void ClickToSelectSkill()
    {
        this.skillSlotListPanelV2.GetSelectedGameCharacter().SetCurrentSkill(this.selectedSkill);
        SkillBoxAnimation();
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

    public bool CheckIsSkillLevelChanged()
    {
        return this.isSkillLevelChanged;
    }

    public void SetSkillSlotText(string slotText)
    {
        this.skillSlotText.SetText(slotText);
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
        if (this.selectedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
        {
            this.skillFrame.sprite = this.ActiveSkillFrame;
        }
        else if (this.selectedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.backend)
        {
            this.skillFrame.sprite = this.BackendSkillFrame;
        }
        else if (this.selectedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse)
        {
            this.skillFrame.sprite = this.RepulseSkillFrame;
        }
        else if (this.selectedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.derived)
        {
            this.skillFrame.sprite = this.DerivedSkillFrame;
        }
        else if (this.selectedSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.counter)
        {
            this.skillFrame.sprite = this.CounterSkillFrame;
        }
    }

    public void SkillBoxAnimation()
    {
        this.skillBoxAnimation.Play(ANIMATION_ID_OUTLINE_RESIZE, 1, 0f);
        this.skillBoxAnimation.Play(ANIMATION_ID_OUTLINE_EXPAND, 0, 0f);
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }
}
