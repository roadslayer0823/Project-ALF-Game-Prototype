using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static DatabaseManager;

public class SkillSelectionBoxV2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private float clickDelay = 0.2f;

    [Header("")]
    [SerializeField] private Image selectionHighlight = null;
    [SerializeField] private Image skillSlotFrame = null;
    [SerializeField] private Image currentSkillSelectionSequence = null;
    [SerializeField] private TextMeshProUGUI skillTypeText = null;
    [SerializeField] private TextMeshProUGUI skillNameText = null;
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
    private SkillSelectionPanelV2 skillSelectionPanel = null;
    private CharacterSkill characterSkill = null;
    private bool isSelected = false;
    private bool isLongPress = false;
    private bool isDoubleTap = false;
    private bool isWaitingSecondClick = false;
    private bool isSkillLevelChanged = false;
    private IEnumerator deselectSkillTimer = null;
    private Vector2 mousePressPosition = new Vector2();
    private Vector2 mouseReleasePosition = new Vector2();
    private Vector2 currentSwipe = new Vector2();
    private int skillLevel = 0;
    private int clickCount = 0;

    private const string AUDIO_ID_BOOST_LEVEL_UP = "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";

    public void Initialize(SkillSelectionPanelV2 skillSelectionPanel, CharacterSkill characterSkill)
    {
        this.skillSelectionPanel = skillSelectionPanel;
        this.characterSkill = characterSkill;

        this.skillSlotFrame.gameObject.SetActive(true);
        this.skillNameText.gameObject.SetActive(true);

        if (characterSkill != null)
        {
            this.skillTypeText.SetText("[" + characterSkill.GetCharacterSubskillData().GetSubskillData().Prefix + "]");
            this.skillNameText.SetText(characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);

            this.skillLevel = characterSkill.GetMinumumSkillLevel();
        }
    }

    public void ShowSelectionHighlight()
    {
        if (!this.isSelected)
        {
            SetSkillSlotFrame(this.skillSelectionPanel.GetSkillSlotSelectBackgroundImage());
        }
        
        this.selectionHighlight.gameObject.SetActive(true);
    }

    private void HideSelectionHighlight()
    {
        if (!this.isSelected)
        {
            SetSkillSlotFrame(this.skillSelectionPanel.GetSkillSlotUnselectBackgroundImage());
        }
        
        this.selectionHighlight.gameObject.SetActive(false);
    }

    IEnumerator SelectSkillSlot()
    {
        this.isWaitingSecondClick = true;

        yield return new WaitForSeconds(this.clickDelay);

        this.isWaitingSecondClick = false;
        this.clickCount = 0;

        SkillSelectionBoxV2 _lastSelectedSkillSelectionBox = this.skillSelectionPanel.GetLastSelectedSkillSelectionBox();

        if (!this.isDoubleTap)
        {
            if (_lastSelectedSkillSelectionBox != null && _lastSelectedSkillSelectionBox != this)
            {
                _lastSelectedSkillSelectionBox.HideSelectionHighlight();
            }

            if (_lastSelectedSkillSelectionBox == this && this.selectionHighlight.gameObject.activeSelf
                && this.skillSelectionPanel.GetSelectedActiveSkillList().Count < GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills()
                && !this.isSelected)
            {
                this.skillSelectionPanel.AddSelectedActiveSkilSlot(this);

                this.skillSelectionPanel.OnSkillSelected(this);

                this.isSelected = true;
            }

            ShowSelectionHighlight();
            this.skillSelectionPanel.ShowSkillInfoPanel(this);
            this.skillSelectionPanel.SetLastSelectedSkillSelectionBox(this);
        }
        else
        {
            /*if (_lastSelectedSkillSelectionBox != null && _lastSelectedSkillSelectionBox != this && !this.selectionHighlight.gameObject.activeSelf)
            {
                if (this.isSelected && this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(_lastSelectedSkillSelectionBox) && this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(this))
                {
                    //Swap these two selected skill
                    this.skillSelectionPanel.SwapSelectedActiveSkill(this);
                }
                else if (!this.isSelected && !this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(this) && this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(_lastSelectedSkillSelectionBox))
                {
                    //Replace selected skill with this one
                    this.skillSelectionPanel.ReplaceSelectedActiveSkill(this);
                }
            }*/

            if (this.skillSelectionPanel.GetSelectedActiveSkillList().Contains(this) && this.isSelected)
            {
                this.skillSelectionPanel.MoveSelectedSkillToFirst(this, this.skillSelectionPanel.GetSelectedActiveSkillList());
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.skillSlotFrame.gameObject.activeSelf)
        {
            this.deselectSkillTimer = DeselectSkillCountDownTimer(2);
            StartCoroutine(this.deselectSkillTimer);

            //save began touch 2d point
            this.mousePressPosition = Input.mousePosition;
        }
        else
        {
            Debug.Log("Empty slot");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.skillSlotFrame.gameObject.activeSelf)
        {
            if (this.selectionHighlight.gameObject.activeSelf)
            {
                //save ended touch 2d point
                this.mouseReleasePosition = Input.mousePosition;

                //create vector from the two points
                this.currentSwipe = new Vector2(this.mouseReleasePosition.x - this.mousePressPosition.x, this.mouseReleasePosition.y - this.mousePressPosition.y);

                //normalize the 2d vector
                this.currentSwipe.Normalize();

                if (this.currentSwipe.x < 0 && this.currentSwipe.y > -0.5f && this.currentSwipe.y < 0.5f) // swipe left
                {
                    DeselectSkill();

                    this.isLongPress = true;
                }
                else if (this.currentSwipe.y > 0 && this.currentSwipe.x > -0.5f && this.currentSwipe.x < 0.5f) // swipe up
                {
                    IncreaseSkillLevel();

                    this.isLongPress = true;
                }
                else if (this.currentSwipe.y < 0 && this.currentSwipe.x > -0.5f && this.currentSwipe.x < 0.5f) // swipe down
                {
                    DecreaseSkillLevel();

                    this.isLongPress = true;
                }
            }

            if (!this.isLongPress && !this.isWaitingSecondClick)
            {

                StartCoroutine(SelectSkillSlot());
            }
            else
            {
                StopCoroutine(SelectSkillSlot());
            }

            this.clickCount += 1;
            if (this.clickCount == 2)
            {
                this.isDoubleTap = true;
            }
            else
            {
                this.isDoubleTap = false;
            }

            if (this.isLongPress)
            {
                this.clickCount = 0;
            }

            if (this.deselectSkillTimer != null)
            {
                StopCoroutine(this.deselectSkillTimer);

                this.isLongPress = false;
            }
        }
    }

    private void DeselectSkill()
    {
        if (!this.isSelected)
        {
            return;
        }

        this.skillSelectionPanel.RemoveSelectedActiveSkilSlot(this);
        this.currentSkillSelectionSequence.gameObject.SetActive(false);

        this.isSelected = false;
    }

    IEnumerator DeselectSkillCountDownTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (this.isSelected && this.selectionHighlight.gameObject.activeSelf)
        {
            DeselectSkill();

            this.isLongPress = true;
        }
    }

    public void SetCurrentSkillSelectionSequence(Sprite numberImage)
    {
        if (numberImage == null)
        {
            this.currentSkillSelectionSequence.gameObject.SetActive(false);
        }
        else
        {
            this.currentSkillSelectionSequence.gameObject.SetActive(true);
            this.currentSkillSelectionSequence.sprite = numberImage;
        }
    }

    // Level down
    public void DecreaseSkillLevel()
    {
        int _minimumSkillLevel = this.characterSkill.GetMinumumSkillLevel();
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

        UpdateCharacterSkillLevel(this.skillLevel);
        ModifySkillLevelAnimation(this.minusLevelImage, this.minusLevelBackground, this.minusLevelOriginalPosition, this.minusLevelTargetPosition);
    }

    //Level up
    public void IncreaseSkillLevel()
    {
        int _minimumSkillLevel = this.characterSkill.GetMinumumSkillLevel();
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

        UpdateCharacterSkillLevel(this.skillLevel);
        ModifySkillLevelAnimation(this.plusLevelImage, this.plusLevelBackground, this.plusLevelOriginalPosition, this.plusLevelTargetPosition);
    }

    private void UpdateCharacterSkillLevel(int skillLevel)
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
                }
            }
        }

        this.skillLevelText.SetText($"<size=30>LV.</size> {skillLevel}");
        this.skillLevelAnimationText.SetText($"<size=30>LV.</size> {skillLevel}");

        //TODO: Show skill info
        //this.skillSelectionListBox.ShowSelectedSkillInfo(this);
        this.skillSelectionPanel.ShowSkillInfoPanel(this);

        UpdateSkillSelectionBoxData();
    }

    // Set the skill data that needed to display into SkillSelectionBox.
    private void UpdateSkillSelectionBoxData()
    {
        if (this.characterSkill == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        Subskill _subskillData = this.characterSkill.GetCharacterSubskillData().GetSubskillData();

        this.skillNameText.SetText(_subskillData.DisplayName);

        if (_subskillData.Prefix.ToString() == "-")
        {
            this.skillTypeText.SetText("");
        }
        else
        {
            this.skillTypeText.SetText("[" + _subskillData.Prefix.ToString() + "]");
        }
    }

    //modify current skill level animation
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

    public CharacterSkill GetCharacterSkill()
    {
        return this.characterSkill;
    }

    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public int GetCurrentSkillLevel()
    {
        return this.skillLevel;
    }

    public void SetSkillSlotFrame(Sprite slotFrame)
    {
        this.skillSlotFrame.sprite = slotFrame;
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
}
