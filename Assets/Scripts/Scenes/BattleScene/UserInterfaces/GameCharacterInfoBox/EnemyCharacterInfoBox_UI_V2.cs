using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCharacterInfoBox_UI_V2 : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private GameCharacter selectedCharacter = null;

    [Header("Enemy Info")]
    [SerializeField] private TextMeshProUGUI enemyNameText = null;

    [Header("Stress Value UI")]
    [SerializeField] private GameObject stressBreakUI = null;
    [SerializeField] private GameObject stressBreakBar = null;
    [SerializeField] private Image stressValueBarSlot = null;
    [SerializeField] private Image stressValueBar = null;
    [SerializeField] private Image stressValueBarAnimation = null;
    [SerializeField] private Image stressValueHittedSlot = null;
    [SerializeField] private Image stressValueGlowEffect = null;

    [Header("Effect Marker UI")]
    [SerializeField] private TextMeshProUGUI effectMarkerValue = null;
    [SerializeField] private GameObject effectMarker = null;

    [Header("State Point UI")]
    [SerializeField] private GameObject stateBreakUI = null;
    [SerializeField] private GameObject stateBreakBar = null;
    [SerializeField] private Image statePointBarSlot = null;
    [SerializeField] private Image statePointBar = null;
    [SerializeField] private Image statePointGlowEffect = null;

    [Header("Health Point UI")]
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image healthPointBarAnimation = null;

    //starting value
    private float startingStatePoint = 0.0f;
    private float startingHealthPoint = 0.0f;
    private float startingVirtualPoint = 0.0f;
    private float startingStressValue = -1;
    private float alphaValue = 255;

    //animation duration
    private readonly float barAnimationDuration = 1f;

    void Awake()
    {
        this.selectedCharacter.AddOnInitializedCallback( Initialize );
        this.selectedCharacter.AddOnCharacterInfoUpdatedCallback( UpdateDisplayInfo );
        UpdateDisplayInfo();
    }

    private void Initialize()
    {
        this.enemyNameText.text = this.selectedCharacter.GetCharacterName();
        this.startingStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        this.startingHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        this.startingVirtualPoint = this.selectedCharacter.GetMaximumHealthPoint();
        this.startingStressValue = this.selectedCharacter.GetMaximumStressValue();
    }

    public void UpdateDisplayInfo()
    {
        HealthPointInfo();
        StatePointInfo();
        StressValueInfo();
    }

    public void HealthPointInfo()
    {
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _currentHealthPoint = this.selectedCharacter.GetCurrentHealthPoint();
        float _virtualHealthPoint = this.selectedCharacter.GetVirtualHealthPoint();

        if (this.effectMarker != null)
        {
            if (this.selectedCharacter.HasEnergyMarker())
            {
                this.effectMarkerValue.SetText($"{ this.selectedCharacter.GetEnergyMarkerRemainingATLs() }");
                this.effectMarker.gameObject.SetActive( true );
            }
            else
            {
                this.effectMarker.gameObject.SetActive( false );
            }
        }

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }

        if(this.startingHealthPoint != _currentHealthPoint)
        {
            this.healthPointBar.fillAmount = _currentHealthPoint / _maximumHealthPoint;
            LeanTween.value(gameObject, this.startingHealthPoint, _currentHealthPoint, this.barAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    this.startingHealthPoint = Mathf.RoundToInt(val);
                    float _healthPointFillAmount = startingHealthPoint / _maximumHealthPoint;
                    this.healthPointBarAnimation.fillAmount = _healthPointFillAmount;
                });
        }

        if(this.startingVirtualPoint != _virtualHealthPoint)
        {
            LeanTween.value(gameObject, this.startingVirtualPoint, _virtualHealthPoint, 0.3f)
           .setOnUpdate((float val) =>
           {
               this.startingVirtualPoint = Mathf.RoundToInt(val);
               float _virtualPointFillAmount = this.startingVirtualPoint / _maximumHealthPoint;
               this.virtualHPBar.fillAmount = _virtualPointFillAmount;
           });
        }
    }

    public void StressValueInfo()
    {
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
        if (this.startingStressValue != _currentStressValue)
        {
            float _defaultAlphaValue = 255;
            Color _stressValueSlotColor = this.stressValueHittedSlot.color;
            Color32 redColor = new Color32(255, 0, 0, 191);
            Color32 blueColor = new Color32(0, 132, 255, 191);

            //red color
            float redR = redColor.r;
            float redG = redColor.g;
            float redB = redColor.b;

            //blue color
            float blueR = blueColor.r;
            float blueG = blueColor.g;
            float blueB = blueColor.b;

            ColorAlphaSetup(_stressValueSlotColor, _defaultAlphaValue, this.stressValueHittedSlot);
            this.stressValueGlowEffect.color = redColor;

            this.stressValueBar.fillAmount = _currentStressValue / 100;
            LeanTween.value(gameObject, this.startingStressValue, _currentStressValue, this.barAnimationDuration)
               .setOnUpdate((float val) =>
               {
                   this.startingStressValue = Mathf.RoundToInt(val);
                   float _stressValueFillAmount = val / 100;
                   this.stressValueBarAnimation.fillAmount = _stressValueFillAmount;
                   this.stressValueGlowEffect.fillAmount = _stressValueFillAmount;
               });

            LeanTween.value(gameObject, new Vector3(redR, redG, redB), new Vector3(blueR, blueG, blueB), this.barAnimationDuration)
                      .setOnUpdate((Vector3 color) =>
                      {
                          this.stressValueGlowEffect.color = new Color32((byte)color.x, (byte)color.y, (byte)color.z, 191);
                      });

            LeanTween.value(gameObject, _defaultAlphaValue, 0, this.barAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    ColorAlphaSetup(_stressValueSlotColor, val, this.stressValueHittedSlot);
                });
        }

        if (this.selectedCharacter.IsInStressBreakStatus())
        {
            this.stressBreakUI.gameObject.SetActive(true);
            this.stressBreakBar.SetActive(true);
            this.stressValueBarSlot.gameObject.SetActive(false);
        }
        else
        {
            this.stressBreakBar.SetActive(false);
            this.stressBreakUI.gameObject.SetActive(false);
            this.stressValueBarSlot.gameObject.SetActive(true);
        }
    }

    public void StatePointInfo()
    {
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        if (this.selectedCharacter.IsInStateBreakStatus())
        {
            this.statePointBarSlot.gameObject.SetActive(false);
            this.stateBreakBar.SetActive(true);
            this.stateBreakUI.SetActive(true);
        }
        else
        {
            LeanTween.value(gameObject, this.startingStatePoint, _currentStatePoint, this.barAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    this.startingStatePoint = Mathf.RoundToInt(val);
                    float _statePointFillAmount = this.startingStatePoint / _maximumStatePoint;
                    this.statePointBar.fillAmount = _statePointFillAmount;
                    this.statePointGlowEffect.fillAmount = _statePointFillAmount;
                });

            this.stateBreakUI.SetActive(false);
            this.stateBreakBar.SetActive(false);
            this.statePointBarSlot.gameObject.SetActive(true);
        }
    }

    private void ColorAlphaSetup(Color objectColor, float defaultAlphaValue, Image targetImage)
    {
        objectColor.a = defaultAlphaValue;
        targetImage.color = objectColor;
    }
}
