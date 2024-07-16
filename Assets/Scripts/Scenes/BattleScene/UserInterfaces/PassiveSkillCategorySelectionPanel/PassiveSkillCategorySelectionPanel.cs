using UnityEngine;
using UnityEngine.UI;

public class PassiveSkillCategorySelectionPanel : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PassiveSkillSlot[] passiveSkillSlotsList = new PassiveSkillSlot[0];
    [SerializeField] private Animator passiveSkillSelectionPanelAnimation;
    [SerializeField] private GameObject passiveSkillHolder;
    [SerializeField] private GameObject passiveSkillInfoBox;
    [SerializeField] private Transform passiveSkillButtonPosition;
    [SerializeField] private Button passiveSkillButtonInteratable;
    [SerializeField] private Image passiveSkillButton;

    [SerializeField] private Sprite lifeTypeButton;
    [SerializeField] private Sprite stateTypeButton;
    [SerializeField] private Sprite stressTypeButton;
    [SerializeField] private Sprite noneTypeButton;

    [SerializeField] private Sprite lifeTypeIcon;
    [SerializeField] private Sprite stateTypeIcon;
    [SerializeField] private Sprite stressTypeIcon;

    [Header("Selected Passive Skill Effect")]
    [SerializeField] private Image selectedPassiveSkillEffect;
    [SerializeField] private Image LifeIconGameObject;
    [SerializeField] private Image StateIconGameObject;
    [SerializeField] private Image StressIconGameObject;

    private PassiveSkillType currentPassiveSkillType = PassiveSkillType.None;
    private PassiveSkillType highlightedPassiveSkillType = PassiveSkillType.None;

    private Vector3 initialPosition;
    private Vector2 touchEndPos;
    private bool isPointerDown;

    private const string ANIMATION_ID_EXPAND = "Expand";
    private const string ANIMATION_ID_HIDE = "Hide";
    private const string ANIMATION_ID_STARTUP = "Startup";
    private const string COLOR_ID_LIFE = "#CEFFB5";
    private const string COLOR_ID_STRESS = "#37D2FF";
    private const string COLOR_ID_STATE = "#FDEEA5";

    public enum PassiveSkillType
    {
        None,
        HealthPoint,
        StatePoint,
        StressValue
    }

    public void Initialize()
    {
        for (int i = 0; i < passiveSkillSlotsList.Length; i++)
        {
            PassiveSkillSlot _passiveSkillSlot = passiveSkillSlotsList[ i ];
            _passiveSkillSlot.Initialize( this );
        }
    }

    public void StartHolding()
    {
        isPointerDown = true;
        initialPosition = passiveSkillButtonPosition.transform.position;

        for (int i = 0; i < passiveSkillSlotsList.Length; i++)
        {
            passiveSkillSlotsList[i].ActivePassiveSkillIcon();
        }
        this.passiveSkillHolder.SetActive(true);
        this.passiveSkillSelectionPanelAnimation.Play(ANIMATION_ID_EXPAND);
    }

    public void ReleaseButton()
    {
        if (isPointerDown)
        {
            isPointerDown = false;
            this.touchEndPos = Input.mousePosition;
            DistanceDetector();
            this.passiveSkillSelectionPanelAnimation.Play(ANIMATION_ID_HIDE);
        }
    }

    public void DistanceDetector()
    {
        float swipeDistance = Vector2.Distance(this.touchEndPos, initialPosition);
        if (swipeDistance <= 35)
        {
            if (this.currentPassiveSkillType != PassiveSkillType.None)
            {
                SetCurrentPassiveSkillType( PassiveSkillType.None );
                for(int i = 0; i < passiveSkillSlotsList.Length; i++)
                {
                    passiveSkillSlotsList[i].UpdateDefaultPassiveSkillUI();
                }
            }
        }
        else
        {
            SetCurrentPassiveSkillType( this.highlightedPassiveSkillType );
            if (this.highlightedPassiveSkillType == PassiveSkillType.HealthPoint)
            {
                SelectingPassiveSkillAnimation(LifeIconGameObject, selectedPassiveSkillEffect, COLOR_ID_LIFE);
            }
            else if (this.highlightedPassiveSkillType == PassiveSkillType.StatePoint)
            {
                SelectingPassiveSkillAnimation(StateIconGameObject, selectedPassiveSkillEffect, COLOR_ID_STATE);
            }
            else if (this.highlightedPassiveSkillType == PassiveSkillType.StressValue)
            {
                SelectingPassiveSkillAnimation(StressIconGameObject, selectedPassiveSkillEffect, COLOR_ID_STRESS);
            }
        }

        this.highlightedPassiveSkillType = PassiveSkillType.None;
    }

    public void DraggingDirection()
    {
        if (!isPointerDown) return;

        Vector3 currentPosition = Input.mousePosition;
        Vector3 direction = currentPosition - initialPosition;

        if(direction != Vector3.zero)
        {
            float angle = Vector3.Angle(Vector3.up, direction);

            if(Vector3.Cross(Vector3.up, direction).z > 0)
            {
                angle = 360 - angle;
            }
            if (angle < 60)
            {
                this.highlightedPassiveSkillType = PassiveSkillType.HealthPoint;
            }
            else if (angle > 60 && angle < 120)
            {
                this.highlightedPassiveSkillType = PassiveSkillType.StatePoint;
            }
            else if (angle > 120)
            {
                this.highlightedPassiveSkillType = PassiveSkillType.StressValue;
            }
            else
            {
                this.highlightedPassiveSkillType = PassiveSkillType.None;
            }

            UpdateCurrentPassiveSkillSlot();
        }
    }

    public void UpdateCurrentPassiveSkillSlot()
    {
        for(int i=0; i<passiveSkillSlotsList.Length; i++)
        {
            if(this.highlightedPassiveSkillType == passiveSkillSlotsList[i].passiveSkillTypeSlot)
            {
                passiveSkillSlotsList[i].UpdateHighlightPassiveSkillUI();
            }
            else
            {
                passiveSkillSlotsList[i].UpdateDefaultPassiveSkillUI();
            }
        }
    }

    public void UpdateSelectedButtonUI()
    {
        this.passiveSkillButton.sprite = this.currentPassiveSkillType switch
        {
            PassiveSkillType.HealthPoint => this.lifeTypeButton,
            PassiveSkillType.StatePoint => this.stateTypeButton,
            PassiveSkillType.StressValue => this.stressTypeButton,
            _ => this.noneTypeButton
        };
        this.passiveSkillButton.SetNativeSize();
    }

    public void ShowCurrentButtonUI()
    {
        this.passiveSkillButton.sprite = this.currentPassiveSkillType switch
        {
            PassiveSkillType.HealthPoint => this.lifeTypeIcon,
            PassiveSkillType.StatePoint => this.stateTypeIcon,
            PassiveSkillType.StressValue => this.stressTypeIcon,
            _ => this.noneTypeButton
        };
        this.passiveSkillButton.SetNativeSize();
    }

    public void SelectingPassiveSkillAnimation(Image currentIcon, Image background, string color)
    {
        Color newColor;
        Vector2 _currentIconScale = currentIcon.rectTransform.localScale;
        Vector2 _targetIconScale = currentIcon.rectTransform.localScale * 3;

        if(ColorUtility.TryParseHtmlString(color, out newColor))
        {
            Debug.Log("changed color");
            background.color = newColor;
        }
        background.gameObject.SetActive(true);
        LeanTween.alpha(background.gameObject, 1, 0.1f).setOnComplete(() =>
        {
            LeanTween.scale(currentIcon.gameObject, _targetIconScale, 0.1f);
            LeanTween.alpha(currentIcon.gameObject, 0, 0.1f);
            LeanTween.alpha(background.gameObject, 0, 0.1f).setOnComplete(() =>
            {
                background.gameObject.SetActive(false);
                currentIcon.gameObject.SetActive(false);
                currentIcon.rectTransform.localScale = _currentIconScale;
            });
        });
    }

    public void SetCurrentPassiveSkillType( PassiveSkillType currentPassiveSkillType )
    {
        this.currentPassiveSkillType = currentPassiveSkillType;
        UpdateSelectedButtonUI();
    }

    public PassiveSkillType GetCurrentPassiveSkillType()
    {
        return this.currentPassiveSkillType;
    }

    public void HidePassiveSkillHolder()
    {
        passiveSkillHolder.SetActive(false);
    }

    public void DisableChangingPassiveSkillType()
    {
        this.passiveSkillButtonInteratable.interactable = false;
    }

    public void EnableChangingPassiveSkillType()
    {
        this.passiveSkillButtonInteratable.interactable = true;
    }

    public void PlayPassiveSkillSelectionStartupAnimation()
    {
        this.passiveSkillSelectionPanelAnimation.Play(ANIMATION_ID_STARTUP);
    }
}
