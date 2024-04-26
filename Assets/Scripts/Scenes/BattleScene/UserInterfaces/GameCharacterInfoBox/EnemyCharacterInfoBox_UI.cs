using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCharacterInfoBox_UI : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private GameCharacter selectedCharacter = null;

    [Header( "Materials" )]
    [SerializeField] private Material gradientShaderMaterial = null;

    [Header("Enemy Info")]
    [SerializeField] private TextMeshProUGUI enemyNameText = null;

    [Header("Stress Value UI")]
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;
    [SerializeField] private GameObject stressBreakUI = null;
    [SerializeField] private Image stressPointStatus = null;

    [Header("Effect Marker UI")]
    [SerializeField] private TextMeshProUGUI effectMarkerValue = null;
    [SerializeField] private GameObject effectMarker = null;

    [Header("State Point UI")]
    [SerializeField] private GameObject stateBreakUI = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private Image statePointStatus = null;
    [SerializeField] private Image statePointBar = null;

    [Header("Health Point UI")]
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image healthPointBar = null;

    private Material stressValueGradientStatus = null;

    //starting value
    private float startingStatePoint = 0.0f;
    private float startingHealthPoint = 0.0f;
    private float startingVirtualPoint = 0.0f;
    private float startingStressValue = -1;

    //animation duration
    private readonly float barAnimationDuration = 1f;
    private readonly float textAnimationDuration = 0.3f;
    private readonly float stressValueStatusDuration = 0.2f;

    //gradient color percentage
    private readonly float orangeColor = 0.5f;
    private readonly float redColor = 0.2f;
    private readonly float breakStatusColor = 0.0f;
    private readonly float defaultColor = 1f;

    //glow percentage
    private readonly float defaultGlowPercentage = 1f;
    private readonly float targetGlowPercentage = 2f;

    void Awake()
    {
        this.stressValueGradientStatus = new Material( this.gradientShaderMaterial );

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

        LeanTween.value(gameObject, this.startingHealthPoint, _currentHealthPoint, this.barAnimationDuration)
            .setOnUpdate((float val) =>
            {
                this.startingHealthPoint = Mathf.RoundToInt(val);
                float _healthPointFillAmount = startingHealthPoint / _maximumHealthPoint;
                this.healthPointBar.fillAmount = _healthPointFillAmount;
            });
        
        LeanTween.value(gameObject, this.startingVirtualPoint, _virtualHealthPoint, 0.3f)
            .setOnUpdate((float val) =>
            {
                this.startingVirtualPoint = Mathf.RoundToInt(val);
                float _virtualPointFillAmount = this.startingVirtualPoint / _maximumHealthPoint;
                this.virtualHPBar.fillAmount = _virtualPointFillAmount;
            });
    }

    public void StressValueInfo()
    {
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
       
        if(this.startingStressValue != _currentStressValue)
        {
            //glow color effect
            Color32 redColor = new Color32(255, 0, 0, 255);
            Color32 blueColor = new Color32(0, 100, 255, 255);

            //blue color
            float redR = redColor.r;
            float redG = redColor.g;
            float redB = redColor.b;

            //red color
            float blueR = blueColor.r;
            float blueG = blueColor.g;
            float blueB = blueColor.b;

            this.stressPercentageText.gameObject.SetActive(true);

            if (_currentStressValue >= 0)
            {
                stressValueStatusAnimation(this.defaultColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
            }
            else if (_currentStressValue >= 70)
            {
                stressValueStatusAnimation(this.orangeColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
            }
            else if (_currentStressValue >= 80)
            {
                stressValueStatusAnimation(this.redColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
            }

            //number changing animation
            LeanTween.value(gameObject, this.startingStressValue, _currentStressValue, this.textAnimationDuration)
                    .setOnUpdate((float val) =>
                    {
                        this.stressValueGradientStatus.SetFloat("_Slide", val / 100);
                        this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, new Color32(255, 0, 0, 255));
                        this.startingStressValue = Mathf.RoundToInt(val);
                        this.stressPercentageText.SetText($"{this.startingStressValue}<size=20>%</size>");
                    });

            //number outline animation
            this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, redColor);
            LeanTween.value(gameObject, new Vector3(blueR, blueG, blueB), new Vector3(redR, redG, redB), 0.7f)
                .setOnUpdate((Vector3 color) =>
                {
                    Color32 currentColor = new Color32((byte)(color.x + 255), (byte)(color.y - 100), (byte)(color.z - 255), 255);
                    this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentColor);
                }).setOnComplete(() =>
                {
                    LeanTween.value(gameObject, new Vector3(redR, redG, redB), new Vector3(blueR, blueG, blueB), 0.7f)
                        .setOnUpdate((Vector3 color) =>
                        {
                            Color32 currentColor = new Color32((byte)(color.x - 255), (byte)(color.y + 100), (byte)(color.z + 255), 255);
                            this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentColor);
                        });
                });

            //glowing effect
            stressValueStatusAnimation(this.targetGlowPercentage, this.stressValueStatusDuration, "_Color_A_Percentage")
                .setOnComplete(() =>
                {
                    stressValueStatusAnimation(this.defaultGlowPercentage, this.stressValueStatusDuration, "_Color_A_Percentage");
                });

            this.stressBreakUI.gameObject.SetActive(false);
        }

        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPercentageText.gameObject.SetActive(false);
            this.stressBreakUI.gameObject.SetActive(true);
            stressValueStatusAnimation(this.breakStatusColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
            stressValueStatusAnimation(this.breakStatusColor, this.stressValueStatusDuration, "_Color2_G_Percentage");
            stressValueStatusAnimation(this.breakStatusColor, this.stressValueStatusDuration, "_Color2_B_Percentage");
        }
        else
        {
            this.stressPercentageText.gameObject.SetActive(true);
            this.stressBreakUI.gameObject.SetActive(false);
            stressValueStatusAnimation(this.defaultColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
            stressValueStatusAnimation(this.defaultColor, this.stressValueStatusDuration, "_Color2_G_Percentage");
            stressValueStatusAnimation(this.defaultColor, this.stressValueStatusDuration, "_Color2_B_Percentage");
        }
    }

    public void StatePointInfo()
    {
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        if (this.selectedCharacter.GetIsBreakStatusCausedByStatePoint())
        {
            this.statePointStatus.sprite = this.statePointBreak;
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
                });

            this.stateBreakUI.SetActive(false);
            this.statePointStatus.sprite = this.statePointNoBreak;
        }
    }

    public LTDescr stressValueStatusAnimation(float colorPercentage, float animationDuration, string targetColor)
    {
        return LeanTween.value(gameObject, this.stressValueGradientStatus.GetFloat(targetColor), colorPercentage, animationDuration)
                        .setOnUpdate((float val) =>
                        {
                            this.stressValueGradientStatus.SetFloat(targetColor, val);
                        });
    }
}
