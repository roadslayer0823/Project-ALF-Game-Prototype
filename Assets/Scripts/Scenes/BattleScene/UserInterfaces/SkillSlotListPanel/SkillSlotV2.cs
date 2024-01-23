using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Subskill = DatabaseManager.Subskill;

public class SkillSlotV2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillSlotText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
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

    private CharacterSkill selectedSkill = null;
    private SkillSlotListPanelV2 skillSlotListPanelV2 = null;
    private Vector2 mousePressPosition = new Vector2();
    private Vector2 mouseReleasePosition = new Vector2();
    private Vector2 currentSwipe = new Vector2();

    private bool isSkillLevelChanged = false;
    private int skillLevel = 1;

    private const string AUDIO_ID_BOOST_LEVEL_UP = "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";

    public void Initialize(SkillSlotListPanelV2 skillSlotListPanelV2)
    {
        this.skillSlotListPanelV2 = skillSlotListPanelV2;
    }
    public void Update()
    {
         Swipe();
    }

    public void Clear()
    {
        this.selectedSkill = null;
        this.skillSlotText.SetText("NODATA");
    }

    public void ClickToSelectSkill()
    {
        this.skillSlotListPanelV2.GetSelectedGameCharacter().SetCurrentSkill(this.selectedSkill);
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
                plusSkillLevel();
            }

            //swipe down
            if (this.currentSwipe.y < 0)
            {
                minusSkillLevel();
            }
        }
    }

    public void plusSkillLevel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_UP);

        int _minimumSkillLevel = this.selectedSkill.GetMinumumSkillLevel();
        int _maximumSkillLevel = this.selectedSkill.GetMaximumSkillLevel();

        this.skillLevel = Math.Clamp(this.skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.selectedSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(this.skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);
        }
        UpdateCharacterSkillLevel(this.skillLevel);
        ModifySkillLevelAnimation(plusLevelImage, plusLevelBackground, plusLevelOriginalPosition, plusLevelTargetPosition);
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_UP);

    }

    public void minusSkillLevel()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_DOWN);

        int _minimumSkillLevel = this.selectedSkill.GetMinumumSkillLevel();
        int _maximumSkillLevel = this.selectedSkill.GetMaximumSkillLevel();

        this.skillLevel = Math.Clamp(this.skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.selectedSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(this.skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);
        }
        UpdateCharacterSkillLevel(this.skillLevel);
        ModifySkillLevelAnimation(minusLevelImage, minusLevelBackground, minusLevelOriginalPosition, minusLevelTargetPosition);
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_DOWN);
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
        float duration = 0.1f;
        float targetScale = 3f;
        Vector3 skillTextScale = skillLevelGameObject.transform.localScale;
        levelModifierImage.color = new Color(levelModifierImage.color.r, levelModifierImage.color.g, levelModifierImage.color.b, 0f);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);

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

        SetSkillSlotText(selectedSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);
    }

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }
}
