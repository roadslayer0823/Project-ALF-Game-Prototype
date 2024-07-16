using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class SkillSlotV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SkillType skillType = SkillType.None;
    [SerializeField] public BackendSkillType backendSkillType = BackendSkillType.None;
    [SerializeField] private float skillIconScale = 1.0f;
    [SerializeField] private float alphaThreshold = 0.1f;
    [SerializeField] private SwipeDetector swipeDetector;

    [Header("Skill UI")]
    [SerializeField] private Image unSelectSkillFrame;
    [SerializeField] private Image skillFrame;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Sprite blankActiveSkillFrame;
    [SerializeField] private Sprite blankBackendSkillFrame;
    [SerializeField] private Sprite blankObservedSkillFrame;
    [SerializeField] private Sprite observedSkillFrame;
    [SerializeField] private Sprite activeSkillFrame;
    [SerializeField] private Sprite backendSkillFrame;
    [SerializeField] private Sprite repulseSkillFrame;
    [SerializeField] private Sprite derivedSkillFrame;
    [SerializeField] private Sprite counterSkillFrame;
    [SerializeField] private GameObject selectedSkillEffect;
    [SerializeField] private TextMeshProUGUI currentObsevedPercentage;

    [Header("Activate Skill UI Frame")]
    [SerializeField] private Sprite activateActiveSkillFrame;
    [SerializeField] private Sprite activateRepulseSkillFrame;
    [SerializeField] private Sprite activateDerivedSkillFrame;
    [SerializeField] private Sprite activateBackendSkillFrame;
    [SerializeField] private Sprite activateCounterSkillFrame;
    [SerializeField] private Sprite activateObservedSkillFrame;

    [Header("Skill Display Text Background")]
    [SerializeField] private Image skillNameBackground;
    [SerializeField] private Image skillPrefixBackground;
    [SerializeField] private Sprite blankSkillNameBackground;
    [SerializeField] private Sprite blankSkillPrefixBackground;
    [SerializeField] private Sprite yellowSkillNameBackground;
    [SerializeField] private Sprite yellowSkillPrefixBackground;
    [SerializeField] private Sprite blueSkillNameBackground;
    [SerializeField] private Sprite blueSkillPrefixBackground;
    [SerializeField] private Sprite derivedSkillNameBackground;
    [SerializeField] private Sprite derivedSkillPrefixBackground;
    [SerializeField] private GameObject skillDisplayTextUI;

    [Header("Modify Skill Level UI")]
    [SerializeField] private GameObject topRowTextObject;
    [SerializeField] private TextMeshProUGUI topRowText;
    [SerializeField] private TextMeshProUGUI bottomRowText;
    [SerializeField] private TextMeshProUGUI skillLevelText;
    [SerializeField] private TextMeshProUGUI skillLevelAnimationText;
    [SerializeField] private GameObject skillLevelGameObject;
    [SerializeField] private GameObject skillTextAnimation;
    [SerializeField] private Button skillSelectionButton;
    [SerializeField] private Transform plusLevelTargetPosition;
    [SerializeField] private Transform plusLevelOriginalPosition;
    [SerializeField] private Transform minusLevelTargetPosition;
    [SerializeField] private Transform minusLevelOriginalPosition;
    [SerializeField] private Image plusLevelImage;
    [SerializeField] private Image plusLevelBackground;
    [SerializeField] private Image minusLevelImage;
    [SerializeField] private Image minusLevelBackground;

    //animation reference
    [SerializeField] private Animator skillBoxAnimation = null;

    private CharacterSkill selectedSkill = null;
    private ActiveSkillSlotListPanelV2 activeSkillSlotListPanelV2 = null;
    private BackendSkillSlotListPanel backendSkillSlotListPanel = null;
    private ObservedSkillData observedSkillData;

    public int skillLevel = 1;
    private bool isSwipeable = false;
    private bool isSkillLevelChanged = false;
    private char[] splitSymbols = { '[', ']', '‧' };
    private StateType currentStateType = StateType.None;
    private bool isObserving = false;

    //public variable
    private bool isMiddleSlot = false;
    private bool isActivated = false;
    private bool isSelected = false;

    //audio and animation clip id
    private const string AUDIO_ID_BOOST_LEVEL_UP = "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";
    private const string AUDIO_ID_BUTTON_ENABLE = "skill_enabled";
    private const string AUDIO_ID_HIGHLIGHT = "highlight";
    private const string AUDIO_ID_ACTIVE_SKILL_SELECTED = "active_skill_selected";
    private const string AUDIO_ID_BACKEND_SKILL_SELECTED = "backend_skill_selected";
    private const string ANIMATION_ID_ACTIVE_SKILL_OUTLINE_RESIZE = "ActiveSkillOutlineResize";
    private const string ANIMATION_ID_ACTIVE_SKILL_OUTLINE_EXPAND = "ActiveSkillOutlineExpand";
    private const string ANIMATION_ID_BACKEND_SKILL_OUTLINE_RESIZE = "BackendSkillOutlineResize";
    private const string ANIMATION_ID_BACKEND_SKILL_OUTLINE_EXPAND = "BackendSkillOutlineExpand";

    public enum SkillType
    {
        None,
        ActiveSkill,
        BackendSkill,
        ObservedSkill
    }

    public enum BackendSkillType
    {
        None,
        Defense,
        Evasion,
        Generic
    }

    public enum StateType
    {
        None,
        Enabled,
        Disabled,
        Activated
    }

    private void Start()
    {
        this.skillFrame.alphaHitTestMinimumThreshold = this.alphaThreshold;
    }

    public void Initialize(ActiveSkillSlotListPanelV2 activeSkillSlotListPanelV2)
    {
        SetSelectedSkill(this.selectedSkill);
        this.activeSkillSlotListPanelV2 = activeSkillSlotListPanelV2;
        this.SetBlankFrame( this.skillType );
        this.skillFrame.SetNativeSize();
        this.unSelectSkillFrame.SetNativeSize();
        this.skillFrame.material.SetFloat("_Brightness", 1);
        this.skillIcon.transform.localScale = new Vector3( this.skillIconScale, this.skillIconScale, 1.0f );
    }

    public void InitializeBackendSkillSlot(BackendSkillSlotListPanel backendSkillSlotListPanel)
    {
        this.backendSkillSlotListPanel = backendSkillSlotListPanel;
        this.SetBlankFrame(this.skillType);
        this.skillFrame.SetNativeSize();
        this.unSelectSkillFrame.SetNativeSize();
        this.skillIcon.transform.localScale = new Vector3(this.skillIconScale, this.skillIconScale, 1.0f);
    }

    public void InitializeObserveSkillSlot(ObservedSkillData observedSkillData)
    {
        this.observedSkillData = observedSkillData;
    }

    public void Clear()
    {
        this.selectedSkill = null;
        this.SetBlankFrame(this.skillType);
        this.skillIcon.gameObject.SetActive(false);
        this.currentStateType = StateType.Disabled;
        this.topRowText.SetText("");
        this.bottomRowText.SetText("");
    }

    public void SelectSkill()
    {
        if (this.currentStateType == StateType.Enabled
            && !this.isSelected)
        {
            GameCharacter _selectedGameCharacter = null;
            bool _isObservingSkill = this.selectedSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill;

            if (this.activeSkillSlotListPanelV2 != null)
            {
                _selectedGameCharacter = this.activeSkillSlotListPanelV2.GetSelectedGameCharacter();
                this.activeSkillSlotListPanelV2.OnSkillSlotSelected(this, true);
                AudioManager.Instance.PlaySoundEffect(AUDIO_ID_ACTIVE_SKILL_SELECTED);
            }
            else if (this.backendSkillSlotListPanel != null)
            {
                _selectedGameCharacter = this.backendSkillSlotListPanel.GetSelectedGameCharacter();
                AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BACKEND_SKILL_SELECTED);
                if (!_isObservingSkill)
                {
                    this.backendSkillSlotListPanel.OnSkillSlotSelected(this, true);
                }
            }

            if (_isObservingSkill)
            {
                _selectedGameCharacter.SetCurrentObservingSkill( this.selectedSkill, true );
                SetIsSelected( true );
            }
            else
            {
                _selectedGameCharacter.SetAssignedSkill( this.selectedSkill );
            }
        }
    }

    public void EnableButton()
    {
        this.skillSelectionButton.interactable = true;
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BUTTON_ENABLE);
    }

    public void IncreaseSkillLevel()
    {
        int _minimumSkillLevel = this.selectedSkill.GetMinimumSkillLevel();
        int _maximumSkillLevel = this.selectedSkill.GetMaximumSkillLevel();

        if (this.skillLevel == _maximumSkillLevel)
        {
            return;
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_UP);

        this.skillLevel = Math.Clamp(skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.selectedSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(skillLevel + 1, _minimumSkillLevel, _maximumSkillLevel);
        }
        UpdateCharacterSkillLevel(this.skillLevel);
        SetSelectedSkill(this.selectedSkill);
        ModifySkillLevelAnimation(this.plusLevelImage, this.plusLevelBackground, this.plusLevelOriginalPosition, this.plusLevelTargetPosition);
        SelectSkill();
    }

    public void DecreaseSkillLevel()
    {
        int _minimumSkillLevel = this.selectedSkill.GetMinimumSkillLevel();
        int _maximumSkillLevel = this.selectedSkill.GetMaximumSkillLevel();

        if (this.skillLevel == _minimumSkillLevel)
        {
            return;
        }

        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BOOST_LEVEL_DOWN);

        this.skillLevel = Math.Clamp(skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);

        while (!this.selectedSkill.IsSkillLevelAvailable(this.skillLevel))
        {
            this.skillLevel = Math.Clamp(skillLevel - 1, _minimumSkillLevel, _maximumSkillLevel);
        }
        UpdateCharacterSkillLevel(this.skillLevel);
        SetSelectedSkill(this.selectedSkill);
        ModifySkillLevelAnimation(this.minusLevelImage, this.minusLevelBackground, this.minusLevelOriginalPosition, this.minusLevelTargetPosition);
        SelectSkill();
    }

    public void UpdateCharacterSkillLevel(int skillLevel)
    {
        this.selectedSkill.SetSelectedSkillLevel(this.skillLevel);
        if(skillLevel == 1)
        {
            this.skillLevelText.SetText("");
        }
        else
        {
            this.skillLevelText.SetText($"<size=30>LV.</size> {skillLevel}");
            this.skillLevelAnimationText.SetText($"<size=30>LV.</size> {skillLevel}");
        }
    }

    //modify current skill level animation
    private void ModifySkillLevelAnimation(Image levelModifierImage, Image background, Transform originalPosition, Transform targetPosition)
    {
        if (this.skillLevel > 1)
        {
            this.skillLevelGameObject.SetActive(true);
        }
        else
        {
            this.skillLevelGameObject.SetActive(false);
        }

        this.skillTextAnimation.gameObject.SetActive(true);
        levelModifierImage.gameObject.SetActive(true);
        background.gameObject.SetActive(true);

        float duration = 0.1f;
        float targetScale = 3f;
        Vector3 skillTextScale = this.skillLevelGameObject.transform.localScale;
        levelModifierImage.color = new Color(levelModifierImage.color.r, levelModifierImage.color.g, levelModifierImage.color.b, 0f);
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);

        //animation
        LeanTween.move(levelModifierImage.gameObject, targetPosition, duration);
        LeanTween.alpha(levelModifierImage.rectTransform, 1f, duration);
        LeanTween.alpha(background.rectTransform, 1f, duration);
        LeanTween.scale(this.skillTextAnimation, skillTextScale * targetScale, duration);
        LeanTween.value(this.skillLevelAnimationText.gameObject, 1f, 0f, duration).setOnUpdate((float value) =>
        {
            this.skillLevelAnimationText.alpha = value;
        })
            .setOnComplete(() => {
                LeanTween.alpha(background.rectTransform, 0f, 0.3f)
            .setOnComplete(() =>
            {
                //reset all related game object to regular position/opacity
                LeanTween.alpha(levelModifierImage.rectTransform, 0f, duration);
                levelModifierImage.gameObject.SetActive(false);
                background.gameObject.SetActive(false);
                this.skillTextAnimation.gameObject.SetActive(false);
                levelModifierImage.transform.position = originalPosition.transform.position;
                this.skillTextAnimation.transform.localScale = skillTextScale;
            });
       });
    }

    public void SetSelectedSkill(CharacterSkill selectedSkill)
    {
        if (selectedSkill == null)
        {
            Clear();

            return;
        }

        this.selectedSkill = selectedSkill;
        this.skillLevel = this.selectedSkill.GetSelectedSkillLevel();

        // Check the current skill level to determine whether it needs to show the modify-skill-level animation.
        this.skillLevelGameObject.SetActive( this.skillLevel > 1 );

        UpdateCharacterSkillLevel(this.skillLevel);
        this.skillIcon.gameObject.SetActive(true);
        UpdateDisplay();

        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();

        if (_subskillData.IsObservingSkill && !isObserving)
        {
            UpdateCurrentObservedStatus( false );
        }

        //setup skill name
        if (_subskillData.NamePartB == "-")
        {
            this.topRowText.SetText("");
            this.bottomRowText.SetText(_subskillData.NamePartA);
            this.skillPrefixBackground.rectTransform.sizeDelta = new Vector2(0, 0);
        }
        else
        {
            this.topRowText.SetText(_subskillData.NamePartA);
            this.bottomRowText.SetText(_subskillData.NamePartB);
            if(isActivated == true)
            {
                StartCoroutine(DelayedSizeDeltaUpdate());
            }
        }
    }

    public void ShowSkillFrame(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        this.skillFrame.sprite = this.selectedSkill.GetSkillData().skillType switch
        {
            Skill.SkillType.active => this.activeSkillFrame,
            Skill.SkillType.backend when selectedSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill => this.observedSkillFrame,
            Skill.SkillType.backend => this.backendSkillFrame,
            Skill.SkillType.repulse => this.repulseSkillFrame,
            Skill.SkillType.derived => this.derivedSkillFrame,
            Skill.SkillType.counter => this.counterSkillFrame,
            _ => throw new NotImplementedException("Unhandled skill type")
        };

        this.skillFrame.SetNativeSize();
        ShowSkillDisplayTextBackground(this.selectedSkill);
    }

    public void ShowActivateSkillFrame(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        this.skillFrame.sprite = this.selectedSkill.GetSkillData().skillType switch
        {
            Skill.SkillType.active => this.activateActiveSkillFrame,
            Skill.SkillType.backend when selectedSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill => this.activateObservedSkillFrame,
            Skill.SkillType.backend => this.activateBackendSkillFrame,
            Skill.SkillType.repulse => this.activateRepulseSkillFrame,
            Skill.SkillType.derived => this.activateDerivedSkillFrame,
            Skill.SkillType.counter => this.activateCounterSkillFrame,
            _ => throw new NotImplementedException("Unhandled skill type")
        };
        this.skillFrame.SetNativeSize();
    }

    public void ShowSkillDisplayTextBackground(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        Skill.SkillType _skillType = this.selectedSkill.GetSkillData().skillType;

        var DisplayTextBackground = _skillType switch
        {
            Skill.SkillType.active or Skill.SkillType.counter => (yellowSkillNameBackground, yellowSkillPrefixBackground),
            Skill.SkillType.backend or Skill.SkillType.repulse => (blueSkillNameBackground, blueSkillPrefixBackground),
            Skill.SkillType.derived => (derivedSkillNameBackground, derivedSkillPrefixBackground),
            _ => (null,null)
        };
        this.skillNameBackground.sprite = DisplayTextBackground.Item1;
        this.skillPrefixBackground.sprite = DisplayTextBackground.Item2;
    }

    public void SetBlankFrame(SkillType frameType)
    {
        (Sprite frameSprite, bool isShowing) = frameType switch
        {
            SkillType.ActiveSkill => (this.blankActiveSkillFrame, false),
            SkillType.BackendSkill => (this.blankBackendSkillFrame, false),
            SkillType.ObservedSkill => (this.blankObservedSkillFrame, true),
            SkillType.None => (null, false),
            _ => throw new NotImplementedException()
        };
        this.unSelectSkillFrame.SetNativeSize();
        this.skillFrame.sprite = frameSprite;
        this.unSelectSkillFrame.sprite = frameSprite;
        this.currentObsevedPercentage.gameObject.SetActive(isShowing);

        DeselectSkillFrame();
        this.skillNameBackground.sprite = blankSkillNameBackground;
        this.skillPrefixBackground.sprite = blankSkillPrefixBackground;
    }

    public void SetCurrentStateType( StateType currentStateType )
    {
        if (this.selectedSkill == null)
        {
            return;
        }

        this.currentStateType = currentStateType;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        switch ( this.currentStateType )
        {
            case StateType.Enabled:

                if (this.skillType == SkillType.ActiveSkill
                    && !this.isMiddleSlot)
                {
                    goto case StateType.Disabled;
                }

                ActivateSkillFrame();

                if (this.isMiddleSlot)
                {
                    EnableButton();
                    BrightnessLoopAnimation();
                    AudioManager.Instance.PlaySoundEffect(AUDIO_ID_HIGHLIGHT);
                }

                if (this.isSelected == true)
                {
                    this.selectedSkillEffect.SetActive(true);
                    ShowActivateSkillFrame(this.selectedSkill);
                }
                else
                {
                    this.selectedSkillEffect.SetActive(false);
                    ShowSkillFrame(this.selectedSkill);
                }

                UpdateSkillIcon( true );
                this.swipeDetector.enabled = true;
                PlayActivateOutlineAnimation();

                break;

            case StateType.Disabled:

                if (this.isSelected == true)
                {
                    this.selectedSkillEffect.SetActive(true);
                    ShowActivateSkillFrame(this.selectedSkill);
                    UpdateSkillIcon(true);
                    this.swipeDetector.enabled = true;
                }
                else
                {
                    this.selectedSkillEffect.SetActive(false);
                    SetBlankFrame(this.skillType);
                    UpdateSkillIcon(false);
                    this.swipeDetector.enabled = false;
                }

                break;

            case StateType.Activated:

                this.isSelected = false;
                this.selectedSkillEffect.SetActive( false );
                SetBlankFrame(this.skillType);
                UpdateSkillIcon( false );
                PlaySkillOutlineAnimation();
                this.swipeDetector.enabled = false;

                break;
        }
    }

    public void BrightnessLoopAnimation()
    {
        LeanTween.value(this.skillFrame.gameObject, 1, 1.5f, 0.5f).setLoopPingPong().setDelay(0.1f).
        setOnUpdate((float var) =>
        {
            this.skillFrame.material.SetFloat("_Brightness", var);
        });
    }

    public void ActivateSkillFrame()
    {
        this.skillFrame.gameObject.SetActive(true);
        this.unSelectSkillFrame.gameObject.SetActive(false);
    }

    public void DeselectSkillFrame()
    {
        this.skillFrame.gameObject.SetActive(false);
        this.unSelectSkillFrame.gameObject.SetActive(true);
    }

    public void PlaySkillOutlineAnimation()
    {
        if (this.skillType == SkillType.ActiveSkill)
        {
            this.skillBoxAnimation.Play( ANIMATION_ID_ACTIVE_SKILL_OUTLINE_RESIZE, 1, 0f );
            this.skillBoxAnimation.Play( ANIMATION_ID_ACTIVE_SKILL_OUTLINE_EXPAND, 0, 0f );
        }
        else if (this.skillType == SkillType.BackendSkill)
        {
            this.skillBoxAnimation.Play( ANIMATION_ID_BACKEND_SKILL_OUTLINE_RESIZE, 1, 0f );
            this.skillBoxAnimation.Play( ANIMATION_ID_BACKEND_SKILL_OUTLINE_EXPAND, 0, 0f );
        }
    }

    public void PlayActivateOutlineAnimation()
    {
        if (this.skillType == SkillType.ActiveSkill)
        {
            this.skillBoxAnimation.Play(ANIMATION_ID_ACTIVE_SKILL_OUTLINE_EXPAND, 0, 0f);
        }
        else if (this.skillType == SkillType.BackendSkill)
        {
            this.skillBoxAnimation.Play(ANIMATION_ID_BACKEND_SKILL_OUTLINE_EXPAND, 0, 0f);
        }
    }

    public void UpdateCurrentSkillDisplayTextUI(bool isEnable)
    {
        if(isEnable == true)
        {
            this.skillDisplayTextUI.SetActive(true);
        }
        else if(isEnable == false)
        {
            this.skillDisplayTextUI.SetActive(false);
        }
    }

    public void UpdateCurrentObservedStatus( bool isObserving )
    {
        this.isObserving = isObserving;

        if (this.isObserving == true)
        {
            this.currentObsevedPercentage.text = "...";
        }
        else
        {
            bool _hasObservationRate = false;

            ObservedSkillRecord _observedSkillRecord = this.selectedSkill.GetObservedSkillRecord();
            if (_observedSkillRecord != null)
            {
                float _observationRate = _observedSkillRecord.GetCurrentObservedRate();
                if (_observationRate > 0)
                {
                    this.currentObsevedPercentage.SetText( $"{ _observationRate.ConvertToIntegerInPercentage() }%" );
                    _hasObservationRate = true;
                }
            }

            if (!_hasObservationRate)
            {
                this.currentObsevedPercentage.text = "---";
            }
        }
    }

    private void UpdateSkillIcon( bool isOn )
    {
        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();
        this.skillIcon.sprite = Resources.Load<Sprite>( ( isOn ) ? _subskillData.IconFilePathOn : _subskillData.IconFilePathOff );
        this.skillIcon.SetNativeSize();
    }

    private IEnumerator DelayedSizeDeltaUpdate()
    {
        yield return new WaitForEndOfFrame();
        this.skillPrefixBackground.rectTransform.sizeDelta = new Vector2(topRowText.rectTransform.sizeDelta.x, topRowText.rectTransform.sizeDelta.y);
    }

    private void SpiltTextFunction(string text)
    {
       if(text.Length > 6)
       {
            int splitIndex = -1;
            foreach(char symbol in splitSymbols)
            {
                splitIndex = text.IndexOf(symbol, 0, 6);
                if(splitIndex != -1)
                {
                    break;
                }
            }

            if(splitIndex != -1)
            {
                string firstPart = text.Substring(0, splitIndex + 1);
                string secondPart = text.Substring(splitIndex + 1).TrimStart();

                if(this.topRowText != null)
                {
                    this.topRowText.text = firstPart;
                }

                if(this.bottomRowText != null)
                {
                    this.bottomRowText.text = secondPart;
                }
            }
       }
       else
       {
           if(this.bottomRowText != null)
           {
               this.bottomRowText.text = text;
           }
       }
    }

    public void ClickToSelectSkill()
    {
        if (!isSwipeable)
        {
            SelectSkill();
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

    public StateType GetCurrentStateType()
    {
        return this.currentStateType;
    }

    public bool GetIsObserving()
    {
        return this.isObserving;
    }

    public void SetIsMiddleSlot( bool isMiddleSlot, bool needToUpdateDisplay = true )
    {
        this.isMiddleSlot = isMiddleSlot;

        if (needToUpdateDisplay)
        {
            UpdateDisplay();
        }
    }

    public void SetIsActivated( bool isActivated )
    {
        this.isActivated = isActivated;
    }

    public void SetIsSelected( bool isSelected )
    {
        this.isSelected = isSelected;
        UpdateDisplay();
    }

    public bool GetIsSelected()
    {
        return this.isSelected;
    }
}
