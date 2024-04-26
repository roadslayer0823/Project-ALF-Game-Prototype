using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class SkillSlotV2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header( "Settings" )]
    [SerializeField] private SkillType skillType = SkillType.None;
    [SerializeField] public BackendSkillType backendSkillType = BackendSkillType.None;
    [SerializeField] private float skillIconScale = 1.0f;
    [SerializeField] private float alphaThreshold = 0.1f;

    [Header("Skill UI")]
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
    [SerializeField] private Sprite activeteRepulseSkillFrame;
    [SerializeField] private Sprite activeteDerivedSkillFrame;
    [SerializeField] private Sprite activeteBackendSkillFrame;
    [SerializeField] private Sprite activeteCounterSkillFrame;
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
    private Vector2 mousePressPosition = new Vector2();
    private Vector2 mouseReleasePosition = new Vector2();
    private Vector2 currentSwipe = new Vector2();

    public int skillLevel = 1;
    private bool isSkillLevelChanged = false;
    private char[] splitSymbols = { '[', ']', '‧' };
    private StateType currentStateType = StateType.None;
    private bool isSwipeable = false;

    //audio and animation clip id
    private const string AUDIO_ID_BOOST_LEVEL_UP = "boost_level_up";
    private const string AUDIO_ID_BOOST_LEVEL_DOWN = "boost_level_down";
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
        Selected,
        Activated,
    }

    public enum PhaseType
    {
        None,
        PartA,
        PartB,
        AfterPartB
    }

    private void Start()
    {
        this.skillFrame.alphaHitTestMinimumThreshold = this.alphaThreshold;
    }

    public void Initialize(ActiveSkillSlotListPanelV2 activeSkillSlotListPanelV2)
    {
        this.activeSkillSlotListPanelV2 = activeSkillSlotListPanelV2;
        this.SetBlankFrame( this.skillType );
        this.skillIcon.transform.localScale = new Vector3( this.skillIconScale, this.skillIconScale, 1.0f );
    }

    public void InitializeBackendSkillSlot(BackendSkillSlotListPanel backendSkillSlotListPanel)
    {
        this.backendSkillSlotListPanel = backendSkillSlotListPanel;
        this.SetBlankFrame(this.skillType);
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
        if (this.currentStateType == StateType.Enabled)
        {
            GameCharacter _selectedGameCharacter = null;
            bool _isObservingSkill = this.selectedSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill;

            if (this.activeSkillSlotListPanelV2 != null)
            {
                _selectedGameCharacter = this.activeSkillSlotListPanelV2.GetSelectedGameCharacter();
                this.activeSkillSlotListPanelV2.OnSkillSlotSelected( this );
            }
            else if (this.backendSkillSlotListPanel != null)
            {
                _selectedGameCharacter = this.backendSkillSlotListPanel.GetSelectedGameCharacter();

                if (!_isObservingSkill)
                {
                    this.backendSkillSlotListPanel.OnSkillSlotSelected( this );
                }
            }

            if (_isObservingSkill)
            {
                _selectedGameCharacter.SetCurrentObservingSkill( this.selectedSkill );
                SetCurrentStateType( StateType.Activated );
            }
            else
            {
                _selectedGameCharacter.SetAssignedSkill( this.selectedSkill );
                SetCurrentStateType( StateType.Selected );
            }

            PlaySkillOutlineAnimation();
        }
    }

    public void EnableButton()
    {
        this.skillSelectionButton.interactable = true;
    }

    public void DisableButton()
    {
        this.skillSelectionButton.interactable = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.mousePressPosition = Input.mousePosition;
    }

    //changing skill level mechanics
    public void OnPointerUp(PointerEventData eventData)
    {
        //save ended touch 2D point
        this.mouseReleasePosition = Input.mousePosition;

        //create vector from the two point
        this.currentSwipe = this.mouseReleasePosition - this.mousePressPosition;

        //swipe up
        if (this.currentSwipe.y > 30 && this.currentSwipe.x > -30 && this.currentSwipe.x < 30)
        {
            isSwipeable = true;
            IncreaseSkillLevel();
        }

        //swipe down
        if (this.currentSwipe.y < -30 && this.currentSwipe.x > -30 && this.currentSwipe.x < 30)
        {
            isSwipeable = true;
            DecreaseSkillLevel();
        }

        if (!isSwipeable)
        {
            SelectSkill();
        }
        else
        {
            isSwipeable = false;
        }
    }

    public void IncreaseSkillLevel()
    {
        int _minimumSkillLevel = this.selectedSkill.GetMinumumSkillLevel();
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
    }

    public void DecreaseSkillLevel()
    {
        int _minimumSkillLevel = this.selectedSkill.GetMinumumSkillLevel();
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

        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();
        this.skillLevel = _subskillData.Level;

        if (_subskillData.NamePartB == "-")
        {
            this.bottomRowText.SetText(_subskillData.NamePartA);
        }

        else
        {
            this.topRowText.SetText(_subskillData.NamePartA);
            this.bottomRowText.SetText(_subskillData.NamePartB);
        }

        if (this.skillLevel > 1)
        {
            this.skillLevelGameObject.SetActive(true);
        }
        else
        {
            this.skillLevelGameObject.SetActive(false);
        }

        UpdateCharacterSkillLevel(this.skillLevel);
        this.skillIcon.gameObject.SetActive(true);
        UpdateDisplay();
    }

    public void ShowSkillFrame(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        switch (this.selectedSkill.GetSkillData().skillType)
        {
            case Skill.SkillType.active:
                this.skillFrame.sprite = this.activeSkillFrame;
                break;

            case Skill.SkillType.backend:
                if (selectedSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                {
                    this.skillFrame.sprite = this.observedSkillFrame;
                    this.currentObsevedPercentage.gameObject.SetActive(true);
                }
                else
                {
                    this.skillFrame.sprite = this.backendSkillFrame;
                    this.currentObsevedPercentage.gameObject.SetActive(false);
                }
                break;

            case Skill.SkillType.repulse:
                this.skillFrame.sprite = this.repulseSkillFrame;
                break;

            case Skill.SkillType.derived:
                this.skillFrame.sprite = this.derivedSkillFrame;
                break;

            case Skill.SkillType.counter:
                this.skillFrame.sprite = this.counterSkillFrame;
                break;
        }
        this.skillFrame.SetNativeSize();
        ShowSkillDisplayTextBackground(this.selectedSkill);
    }

    public void ShowActivateSkillFrame(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        switch (this.selectedSkill.GetSkillData().skillType)
        {
            case Skill.SkillType.active:
                this.skillFrame.sprite = this.activateActiveSkillFrame;
                break;

            case Skill.SkillType.backend:
                if (selectedSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                {
                    this.skillFrame.sprite = this.activateObservedSkillFrame;
                    this.currentObsevedPercentage.gameObject.SetActive(true);
                }
                else
                {
                    this.skillFrame.sprite = this.activeteBackendSkillFrame;
                    this.currentObsevedPercentage.gameObject.SetActive(false);
                }
                break;

            case Skill.SkillType.repulse:
                this.skillFrame.sprite = this.activeteRepulseSkillFrame;
                break;

            case Skill.SkillType.derived:
                this.skillFrame.sprite = this.activeteDerivedSkillFrame;
                break;

            case Skill.SkillType.counter:
                this.skillFrame.sprite = this.activeteCounterSkillFrame;
                break;
        }
        this.skillFrame.SetNativeSize();
    }

    public void ShowSkillDisplayTextBackground(CharacterSkill selectedSkill)
    {
        this.selectedSkill = selectedSkill;
        Skill.SkillType _skillType = this.selectedSkill.GetSkillData().skillType;

        if (_skillType == Skill.SkillType.active || _skillType == Skill.SkillType.counter)
        {
            this.skillNameBackground.sprite = yellowSkillNameBackground;
            this.skillPrefixBackground.sprite = yellowSkillPrefixBackground;
        }
        else if(_skillType == Skill.SkillType.backend || _skillType == Skill.SkillType.repulse)
        {
            this.skillNameBackground.sprite = blueSkillNameBackground;
            this.skillPrefixBackground.sprite = blueSkillPrefixBackground;
        }
        else if(_skillType == Skill.SkillType.derived)
        {
            this.skillNameBackground.sprite = derivedSkillNameBackground;
            this.skillPrefixBackground.sprite = derivedSkillPrefixBackground;
        }
    }

    public void SetBlankFrame(SkillType frameType)
    {
        if (frameType == SkillType.ActiveSkill)
        {
            this.skillFrame.sprite = this.blankActiveSkillFrame;
            this.currentObsevedPercentage.gameObject.SetActive(false);
            this.skillFrame.SetNativeSize();
        }
        else if (frameType == SkillType.BackendSkill)
        {
            this.skillFrame.sprite = this.blankBackendSkillFrame;
            this.currentObsevedPercentage.gameObject.SetActive(false);
            this.skillFrame.SetNativeSize();
        }
        else if(frameType == SkillType.ObservedSkill)
        {
            this.skillFrame.sprite = this.blankObservedSkillFrame;
            this.currentObsevedPercentage.gameObject.SetActive(true);
            this.skillFrame.SetNativeSize();
        }
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

    public StateType GetCurrentStateType()
    {
        return this.currentStateType;
    }

    private void UpdateDisplay()
    {
        switch ( this.currentStateType )
        {
            case StateType.Enabled:

                EnableButton();
                this.selectedSkillEffect.SetActive( false );
                ShowSkillFrame( this.selectedSkill );
                UpdateSkillIcon( false );
                break;

            case StateType.Disabled:

                this.selectedSkillEffect.SetActive( false );
                SetBlankFrame( this.skillType );
                UpdateSkillIcon( false );
                break;

            case StateType.Selected:

                DisableButton();
                this.selectedSkillEffect.SetActive( true );
                ShowActivateSkillFrame(this.selectedSkill);
                UpdateSkillIcon( true );
                break;

            case StateType.Activated:

                this.selectedSkillEffect.SetActive( false );
                SetBlankFrame(this.skillType);
                UpdateSkillIcon( false );
                PlaySkillOutlineAnimation();

                break;
        }
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

    public CharacterSkill GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public bool CheckIsSkillLevelChanged()
    {
        return this.isSkillLevelChanged;
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

    public void UpdateCurrentObservedStatus(PhaseType currentPhase)
    {
        if(this.observedSkillData.GetCurrentObservedRate() > 0)
        {
            this.currentObsevedPercentage.gameObject.SetActive(true);
            if (currentPhase == PhaseType.PartB)
            {
                this.currentObsevedPercentage.text = "---";
            }
            else if (currentPhase == PhaseType.AfterPartB)
            {
                float _observationRate = this.observedSkillData.GetCurrentObservedRate();
                this.currentObsevedPercentage.SetText(_observationRate * 100 + "%");
            }
        }
        else
        {
            this.currentObsevedPercentage.text = "---";
        }
    }

    private void UpdateSkillIcon( bool isOn)
    {
        Subskill _subskillData = this.selectedSkill.GetCharacterSubskillData().GetSubskillData();
        this.skillIcon.sprite = Resources.Load<Sprite>( ( isOn ) ? _subskillData.IconFilePathOn : _subskillData.IconFilePathOff );
        this.skillIcon.SetNativeSize();
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
}
