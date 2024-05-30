using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static DatabaseManager;

public class SkillSelectionBoxV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float clickDelay = 0.2f;
    [SerializeField] private float lastClickTime = 0f;
    [SerializeField] private float alphaThreshold = 0.1f;
    [SerializeField] private float flashDelay = 1.0f;
    [SerializeField] private float flashIntervalTime = 1.5f;
    [SerializeField][Range(0, 1)] private float flashFadeOutAlpha = 0.5f;

    [Header("ActiveSkillSelectionBox")]
    [SerializeField] private Button skillBoxButton = null;
    [SerializeField] private Image currentSkillSelectionSequence = null;
    [SerializeField] private TextMeshProUGUI skillTypeText = null;
    [SerializeField] private TextMeshProUGUI skillLevelAnimationText = null;
    [SerializeField] private TextMeshProUGUI skillLevelText = null;
    [SerializeField] private Image plusLevelImage = null;
    [SerializeField] private Image plusLevelBackground = null;
    [SerializeField] private Image minusLevelImage = null;
    [SerializeField] private Image minusLevelBackground = null;
    [SerializeField] private Transform plusLevelTargetPosition = null;
    [SerializeField] private Transform plusLevelOriginalPosition = null;
    [SerializeField] private Transform minusLevelTargetPosition = null;
    [SerializeField] private Transform minusLevelOriginalPosition = null;

    [Header("BackendSkillSelectionBox")]
    [SerializeField] private Image skillSelectedImage = null;

    [Header("")]
    [SerializeField] private Image skillSlot = null;
    [SerializeField] private Image selectionHighlight = null;
    [SerializeField] private Image skillBoxFrame = null;
    [SerializeField] private Image skillIcon = null;
    [SerializeField] private GameObject skillNameGO = null;
    [SerializeField] private TextMeshProUGUI skillNameText = null;
    [SerializeField] private SwipeDetector swipeDetector = null;
    [SerializeField] private LongPressDetector longPressDetector = null;
    [SerializeField] private DoubleTapDetector doubleTapDetector = null;

    private SkillSelectionPanelV2 skillSelectionPanel = null;
    private CharacterSkill characterSkill = null;
    private bool isSelected = false;
    private int skillLevel = 0;
    private Skill.SkillType skillType = Skill.SkillType.none;

    private const string AUDIO_ID_BOOST_LEVEL_UP = "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";
    private const string AUDIO_ID_HIGHLIGHT = "highlight";
    private const string AUDIO_ID_CLICK = "click";

    public void Initialize(SkillSelectionPanelV2 skillSelectionPanel, CharacterSkill characterSkill)
    {
        this.skillSelectionPanel = skillSelectionPanel;
        this.characterSkill = characterSkill;
        this.skillType = characterSkill.GetSkillData().skillType;

        this.skillSlot.alphaHitTestMinimumThreshold = this.alphaThreshold;
        this.skillBoxFrame.alphaHitTestMinimumThreshold = this.alphaThreshold;
        this.skillBoxFrame.gameObject.SetActive(true);
        this.skillNameGO.SetActive(true);

        UpdateSkillIcon(false);

        if (characterSkill != null)
        {
            if (characterSkill.GetCharacterSubskillData().GetSubskillData().Prefix != "-" && this.skillTypeText != null)
            {
                this.skillTypeText.SetText("[" + characterSkill.GetCharacterSubskillData().GetSubskillData().Prefix + "]");
            }
            
            this.skillNameText.SetText(characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);

            this.skillLevel = characterSkill.GetMinimumSkillLevel();
        }
    }

    public void ClickToSelectSkill()
    {
        if (!this.longPressDetector.GetIsPointerDown() && !this.longPressDetector.GetIsLongPress())
        {
            this.doubleTapDetector.StartTap();
        }
    }

    // display the highlight image
    public void ShowSelectionHighlight()
    {
        this.selectionHighlight.gameObject.SetActive(true);
        this.skillNameGO.SetActive(false);

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_HIGHLIGHT);
    }

    // hide the highlight image
    private void HideSelectionHighlight()
    {
        if (this.skillType == Skill.SkillType.active)
        {
            if (!this.isSelected)
            {
                SetSkillBoxFrame(this.skillSelectionPanel.GetActiveSkillBoxUnselectBackgroundImage());
            }
            else
            {
                SetSkillBoxFrame(this.skillSelectionPanel.GetActiveSkillBoxSelectedBackgroundImage());
            }
        }
        else
        {
            if (!this.isSelected)
            {
                SetSkillBoxFrame(this.skillSelectionPanel.GetBackendSkillBoxUnselectBackgroundImage());
            }
            else
            {
                SetSkillBoxFrame(this.skillSelectionPanel.GetBackendSkillBoxSelectedBackgroundImage());
            }
        }

        this.selectionHighlight.gameObject.SetActive(false);
        this.skillNameGO.SetActive(true);
    }

    // select the skill box
    public void SelectSkillBox()
    {
        if (this.skillType == Skill.SkillType.active)
        {
            SelectActiveSkill();
        }
        else if (this.skillType == Skill.SkillType.backend)
        {
            SelectBackendSkill();
        }
    }

    // the selected skill box is active skill
    private void SelectActiveSkill()
    {
        if (!this.isSelected)
        {
            this.doubleTapDetector.ResetDoubleTapValue();
        }

        SkillSelectionBoxV2 _lastSelectedSkillSelectionBox = this.skillSelectionPanel.GetLastSelectedActiveSkillSelectionBox();

        if (!this.doubleTapDetector.IsDoubleTap())
        {
            if (_lastSelectedSkillSelectionBox != null && _lastSelectedSkillSelectionBox != this)
            {
                _lastSelectedSkillSelectionBox.HideSelectionHighlight();
            }

            if (_lastSelectedSkillSelectionBox == this && this.selectionHighlight.gameObject.activeSelf && !this.isSelected)
            {
                if (this.skillSelectionPanel.IsCharacterAllowedToAddOrRemoveSkill(this.characterSkill, true))
                {
                    this.skillSelectionPanel.AddSelectedSkillBoxV2(this);
                    this.isSelected = true;
                    UpdateSkillIcon(true);
                }
                else
                {
                    Debug.Log("Unable to add skill");
                }
            }

            ShowSelectionHighlight();
            this.skillSelectionPanel.ShowSkillInfoPanel(this);
            this.skillSelectionPanel.SetLastSelectedActiveSkillSelectionBox(this);
        }
        else if(this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(this) && this.isSelected)
        {
            this.skillSelectionPanel.MoveSelectedSkillToFirst(this, this.skillSelectionPanel.GetSelectedActiveSkillList());
        }
    }

    // the selected skill box is backend skill
    private void SelectBackendSkill()
    {
        SkillSelectionBoxV2 _lastSelectedSkillSelectionBox = this.skillSelectionPanel.GetLastSelectedBackendSkillSelectionBox();

        if (_lastSelectedSkillSelectionBox != null && _lastSelectedSkillSelectionBox != this)
        {
            _lastSelectedSkillSelectionBox.HideSelectionHighlight();
        }

        if (_lastSelectedSkillSelectionBox == this && this.selectionHighlight.gameObject.activeSelf && !this.isSelected)
        {
            if(this.skillSelectionPanel.IsCharacterAllowedToAddOrRemoveSkill(this.characterSkill,true))
            {
                this.skillSelectionPanel.AddSelectedSkillBoxV2(this);
            }
            else
            {
                Debug.Log("Unable to add skill");
            }
        }

        ShowSelectionHighlight();
        this.skillSelectionPanel.ShowSkillInfoPanel(this);
        this.skillSelectionPanel.SetLastSelectedBackendSkillSelectionBox(this);
    }

    // deselect the selected skill box
    public void DeselectSkill()
    {
        if (!this.isSelected)
        {
            return;
        }

        if (this.skillType == Skill.SkillType.active)
        {
            this.currentSkillSelectionSequence.gameObject.SetActive(false);
        }

        if (this.skillSelectionPanel.IsCharacterAllowedToAddOrRemoveSkill(this.characterSkill, false))
        {
            if(this.skillType == Skill.SkillType.active)
            {
                this.skillSelectionPanel.RemoveSelectedSkillBox(this);
                this.doubleTapDetector.ResetDoubleTapValue();
            }
            else if(this.skillType == Skill.SkillType.backend)
            {
                this.skillSelectionPanel.RemoveSelectedSkillBox(this,true);
            }
        }
        else
        {
            Debug.Log("Cannot remove skill");
        }
    }

    public void testingDeselectSkill()
    {
        Debug.Log("deselect skill");
    }

    // Level down
    public void DecreaseSkillLevel()
    {
        int _minimumSkillLevel = this.characterSkill.GetMinimumSkillLevel();
        int _maximumSkillLevel = this.characterSkill.GetMaximumSkillLevel();

        if (this.skillLevel == _minimumSkillLevel)
        {
            return;
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_DOWN);

        this.skillLevel = Math.Clamp(this.skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.characterSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(this.skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);
        }

        UpdateCharacterSkillLevel();
        ModifySkillLevelAnimation(this.minusLevelImage, this.minusLevelBackground, this.minusLevelOriginalPosition, this.minusLevelTargetPosition);
    }

    // Level up
    public void IncreaseSkillLevel()
    {
        int _minimumSkillLevel = this.characterSkill.GetMinimumSkillLevel();
        int _maximumSkillLevel = this.characterSkill.GetMaximumSkillLevel();

        if (this.skillLevel == _maximumSkillLevel)
        {
            return;
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_UP);

        this.skillLevel = Math.Clamp(this.skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.characterSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(this.skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);
        }

        UpdateCharacterSkillLevel();
        ModifySkillLevelAnimation(this.plusLevelImage, this.plusLevelBackground, this.plusLevelOriginalPosition, this.plusLevelTargetPosition);
    }

    // update the character skill level
    private void UpdateCharacterSkillLevel()
    {
        this.characterSkill.SetSelectedSkillLevel(this.skillLevel);

        // If the skill is already selected
        if (this.isSelected && this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(this))
        {
            for (int i = 0; i < this.skillSelectionPanel.GetSelectedActiveSkillList().Count; i++)
            {
                SkillSelectionBoxV2 _skillSelectionBox = this.skillSelectionPanel.GetSelectedActiveSkillList()[i];

                if (_skillSelectionBox == this)
                {
                    this.skillSelectionPanel.GetSelectedActiveSkillList()[i].characterSkill = this.characterSkill;
                    this.skillSelectionPanel.GetGameCharacter().GetSelectedActiveSkillList()[i] = this.characterSkill;
                }
            }
        }

        UpdateSkillSelectionBoxData();
    }

    // Set the skill data that needed to display into SkillSelectionBox.
    public void UpdateSkillSelectionBoxData()
    {
        if (this.characterSkill == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        if (this.skillLevel > 1)
        {
            this.skillLevelText.gameObject.SetActive(true);
        }
        else
        {
            this.skillLevelText.gameObject.SetActive(false);
        }

        Subskill _subskillData = this.characterSkill.GetCharacterSubskillData().GetSubskillData();

        this.skillLevelText.SetText($"<size=30>LV.</size> {this.skillLevel}");
        this.skillLevelAnimationText.SetText($"<size=30>LV.</size> {this.skillLevel}");

        this.skillSelectionPanel.ShowSkillInfoPanel(this);

        this.skillNameText.SetText(_subskillData.DisplayName);

        if (_subskillData.Prefix.ToString() == "-")
        {
            this.skillTypeText.SetText("");
        }
        else
        {
            this.skillTypeText.SetText("[" + _subskillData.Prefix.ToString() + "]");
        }

        UpdateSkillIcon(this.isSelected);
    }

    //Modify current skill level animation
    private void ModifySkillLevelAnimation(Image levelModifierImage, Image background, Transform originalPosition, Transform targetPosition)
    {
        if (this.skillLevel > 1)
        {
            this.skillLevelText.gameObject.SetActive(true);
        }
        else
        {
            this.skillLevelText.gameObject.SetActive(false);
        }

        this.skillLevelAnimationText.gameObject.SetActive(true);
        levelModifierImage.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        float duration = 0.1f;
        float targetScale = 3f;
        Vector3 skillTextScale = this.skillLevelText.transform.localScale;
        levelModifierImage.color = new Color(levelModifierImage.color.r, levelModifierImage.color.g, levelModifierImage.color.b, 0f);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);

        //animation
        LeanTween.move(levelModifierImage.gameObject, targetPosition, duration);
        LeanTween.alpha(levelModifierImage.rectTransform, 1f, duration);
        LeanTween.alpha(background.rectTransform, 1f, duration);
        LeanTween.scale(skillLevelAnimationText.gameObject, skillTextScale * targetScale, duration);
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
                this.skillLevelAnimationText.gameObject.SetActive(false);
                levelModifierImage.transform.position = originalPosition.transform.position;
                this.skillLevelAnimationText.transform.localScale = skillTextScale;
            });
            });
    }

    // get the character skill
    public CharacterSkill GetCharacterSkill()
    {
        return this.characterSkill;
    }

    // set the character skill
    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public bool GetIsSelected()
    {
        return this.isSelected;
    }

    public void SetCurrentSkillLevel(int skillLevel)
    {
        this.skillLevel = skillLevel;
    }

    public int GetCurrentSkillLevel()
    {
        return this.skillLevel;
    }

    public void SetSkillBoxFrame(Sprite boxFrame)
    {
        this.skillBoxFrame.sprite = boxFrame;
    }

    public bool IsHighlighted()
    {
        if (this.selectionHighlight.gameObject.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Skill.SkillType GetCharacterSkillType()
    {
        return this.skillType;
    }

    // Set sequence image for selected active skill box
    public void SetCurrentSkillSelectionSequence(Sprite numberImage)
    {
        if (numberImage == null)
        {
            this.currentSkillSelectionSequence.gameObject.SetActive(false);

            Color _initialSkillSelectionSequenceColor = this.currentSkillSelectionSequence.color;
            _initialSkillSelectionSequenceColor.a = 1.0f;

            this.currentSkillSelectionSequence.color = _initialSkillSelectionSequenceColor;

            LeanTween.cancel(this.currentSkillSelectionSequence.gameObject);
        }
        else
        {
            this.currentSkillSelectionSequence.gameObject.SetActive(true);
            this.currentSkillSelectionSequence.sprite = numberImage;

            Color _newSkillSelectionSequenceColor = this.currentSkillSelectionSequence.color;
            _newSkillSelectionSequenceColor.a = this.currentSkillSelectionSequence.color.a * this.flashFadeOutAlpha;

            LeanTween.color(currentSkillSelectionSequence.rectTransform, _newSkillSelectionSequenceColor, this.flashIntervalTime).setLoopPingPong().setDelay(this.flashDelay);
        }
    }

    // Set selected image for selected backend skill box based on skill type
    public void SetSkillSelectedImage(Sprite imageToDisplay)
    {
        if (imageToDisplay == null)
        {
            this.skillSelectedImage.gameObject.SetActive(false);

            Color _initialSkillSelectionSequenceColor = this.skillSelectedImage.color;
            _initialSkillSelectionSequenceColor.a = 1.0f;

            this.skillSelectedImage.color = _initialSkillSelectionSequenceColor;

            LeanTween.cancel(this.skillSelectedImage.gameObject);
        }
        else
        {
            this.skillSelectedImage.gameObject.SetActive(true);
            this.skillSelectedImage.sprite = imageToDisplay;

            Color _newSkillSelectionSequenceColor = this.skillSelectedImage.color;
            _newSkillSelectionSequenceColor.a = this.skillSelectedImage.color.a * this.flashFadeOutAlpha;

            LeanTween.color(skillSelectedImage.rectTransform, _newSkillSelectionSequenceColor, this.flashIntervalTime).setLoopPingPong().setDelay(this.flashDelay);
        }
    }

    // update the skill icon
    public void UpdateSkillIcon(bool isOn)
    {
        this.skillIcon.gameObject.SetActive(true);

        Subskill _subskillData = this.characterSkill.GetCharacterSubskillData().GetSubskillData();
        this.skillIcon.sprite = Resources.Load<Sprite>((isOn) ? _subskillData.IconFilePathOn : _subskillData.IconFilePathOff);
        this.skillIcon.SetNativeSize();
    }

    // get the skill selection panel
    public SkillSelectionPanelV2 GetSkillSelectionPanel()
    {
        return this.skillSelectionPanel;
    }
}
