using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelV2 : MonoBehaviour
{
    [Header("Stress Value UI")]
    [SerializeField] private Image stressValueStatus = null;
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;

    [Header("Health Point UI")]
    [SerializeField] private TextMeshProUGUI healthPointValueText = null;
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image healthPointBarAnimation = null;

    [Header("State Point UI")]
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private RectTransform maxStatePointAnimStaringPoint = null;
    [SerializeField] private RectTransform maxStatePointAnimEndingPoint = null;
    [SerializeField] private TextMeshProUGUI statePointValueText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueFirstText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueSecondText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueThirdText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueFourthText = null;
    [SerializeField] private GameObject statePointBreakText = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private Image maxStatePointIncreaseIcon = null;
    [SerializeField] private Image statePointStatus = null;
    [SerializeField] private Image statePointBar = null;

    [Header("Character Info Panel Animation")]
    [SerializeField] private Animator characterInfoPanelAnimation = null;

    //audio and animation clip id
    private const string ANIMATION_ID_STRESS_POINT_INCREASE = "StressPointIncrease";
    private const string ANIMATION_ID_MAX_STATE_POINT_INCREASE = "MaxStatePointIncrease";

    private GameCharacter selectedCharacter = null;
    public bool isSetupComplete = false;

    //character info panel attributes
    private float startingStatePoint = 0.0f;
    private float startingHealthPoint = 0.0f;
    private float startingVirtualPoint = 0.0f;
    private float startingStressValue;

    //animation duration
    private float textAnimationDuration = 0.3f;
    private float maxStatePointScaleDuration = 0.2f;
    private float maxStatePointGlowDuration = 0.1f;
    private float stressValueStatusDuration = 0.2f;
    private float scaleMultiplier = 0.7f;

    //spacing
    private float twoCharacterSpacing = -174;
    private float threeCharacterSpacing = -148;
    private float fourCharacterSpacing = -130;

    //glow effect
    private float defaultTextOffset = -1f;
    private float defaultTextOuter = 0.0f;
    private float targetTextOffset = -0.3f;
    private float targetTextOuter = 0.75f;

    //gradient color percentage
    private float orangeColor = 0.5f;
    private float redColor = 0.2f;
    private float breakStatusColor = 0.0f;
    private float defaultColor = 1f;

    public void Initialize()
    {
        float _startingStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _startingHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _startingVirtualPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _startingStressValue = -1f;

        startingStatePoint = _startingStatePoint;
        startingHealthPoint = _startingHealthPoint;
        startingVirtualPoint = _startingVirtualPoint;
        startingStressValue = _startingStressValue;
        
    }

    public void SetSelectedCharacter(GameCharacter selectedCharacter)
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated(UpdateDisplayInfo);
        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        HealthPointInfo();
        StatePointInfo();
        StressValueInfo();
        isSetupComplete = true;
    }

    private void StatePointInfo()
    {
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        string maximumStatePointText = _maximumStatePoint.ToString();

        if (maximumStatePointText.Length == 2)
        {
            this.maxStatePointValueThirdText.gameObject.SetActive(false);
            this.maxStatePointValueFourthText.gameObject.SetActive(false);
            this.maxStatePointValueFirstText.text = maximumStatePointText[0].ToString();
            this.maxStatePointValueSecondText.text = maximumStatePointText[1].ToString();
            this.horizontalLayoutGroup.spacing = twoCharacterSpacing;
        }

        else if (maximumStatePointText.Length == 3)
        {
            this.maxStatePointValueThirdText.gameObject.SetActive(true);
            this.maxStatePointValueFourthText.gameObject.SetActive(false);
            this.maxStatePointValueFirstText.text = maximumStatePointText[0].ToString();
            this.maxStatePointValueSecondText.text = maximumStatePointText[1].ToString();
            this.maxStatePointValueThirdText.text = maximumStatePointText[2].ToString();
            this.horizontalLayoutGroup.spacing = threeCharacterSpacing;
        }

        else if (maximumStatePointText.Length == 4)
        {
            this.maxStatePointValueThirdText.gameObject.SetActive(true);
            this.maxStatePointValueFourthText.gameObject.SetActive(true);
            this.maxStatePointValueFirstText.text = maximumStatePointText[0].ToString();
            this.maxStatePointValueSecondText.text = maximumStatePointText[1].ToString();
            this.maxStatePointValueThirdText.text = maximumStatePointText[2].ToString();
            this.horizontalLayoutGroup.spacing = fourCharacterSpacing;
        }

        if (this.startingStatePoint != _currentStatePoint)
        {
            //curret state point animation
            LeanTween.value(gameObject, this.startingStatePoint, _currentStatePoint, textAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    this.startingStatePoint = Mathf.RoundToInt(val);
                    this.statePointValueText.SetText(startingStatePoint.ToString());
                    this.statePointBar.fillAmount = startingStatePoint / _maximumStatePoint;
                });

            //max state point increase animation
            if (this.startingStatePoint != _maximumStatePoint)
            {
                if (isSetupComplete == true)
                {
                    Vector2 currentScale = maxStatePointValueFirstText.rectTransform.localScale;
                    Vector2 newScale = new Vector3(currentScale.x / scaleMultiplier, currentScale.y / scaleMultiplier);
                    Vector2 maxStatePointIconOriginalPosition = maxStatePointIncreaseIcon.gameObject.transform.position;
                    float maxStatePointIncreaseOpacity = maxStatePointIncreaseIcon.color.a;

                    //max state point increase animation
                    maxStatePointIncreaseIcon.gameObject.SetActive(true);
                    this.characterInfoPanelAnimation.Play(ANIMATION_ID_MAX_STATE_POINT_INCREASE, 1, 0f);
                    LeanTween.move(maxStatePointIncreaseIcon.gameObject, maxStatePointAnimEndingPoint, 0.5f)
                       .setOnComplete(() =>
                        {
                            LeanTween.alpha(maxStatePointIncreaseIcon.gameObject, 0, 0.3f);
                            maxStatePointIncreaseIcon.gameObject.SetActive(false);
                            maxStatePointIncreaseOpacity = 255;
                            maxStatePointIncreaseIcon.gameObject.transform.position = maxStatePointIconOriginalPosition;
                        });

                    //first text
                    maxStatePointScale(maxStatePointValueFirstText, newScale, maxStatePointScaleDuration);
                    maxStatePointOffset(maxStatePointValueFirstText, defaultTextOffset, targetTextOffset, maxStatePointGlowDuration);
                    maxStatePointOuter(maxStatePointValueFirstText, defaultTextOuter, targetTextOuter, maxStatePointGlowDuration)
                        .setOnComplete(() =>
                        {
                            maxStatePointOffset(maxStatePointValueFirstText, targetTextOffset, defaultTextOffset, maxStatePointGlowDuration);
                            maxStatePointOuter(maxStatePointValueFirstText, targetTextOuter, defaultTextOuter, maxStatePointGlowDuration);
                            maxStatePointScale(maxStatePointValueFirstText, currentScale, maxStatePointScaleDuration);

                            //second text
                            maxStatePointScale(maxStatePointValueSecondText, newScale, maxStatePointScaleDuration);
                            maxStatePointOffset(maxStatePointValueSecondText, defaultTextOffset, targetTextOffset, maxStatePointGlowDuration);
                            maxStatePointOuter(maxStatePointValueSecondText, defaultTextOuter, targetTextOuter, maxStatePointGlowDuration)
                                .setOnComplete(() =>
                                {
                                    maxStatePointOffset(maxStatePointValueSecondText, targetTextOffset, defaultTextOffset, maxStatePointGlowDuration);
                                    maxStatePointOuter(maxStatePointValueSecondText, targetTextOuter, defaultTextOuter, maxStatePointGlowDuration);
                                    maxStatePointScale(maxStatePointValueSecondText, currentScale, maxStatePointScaleDuration);

                                    //third text
                                    maxStatePointScale(maxStatePointValueThirdText, newScale, maxStatePointScaleDuration);
                                    maxStatePointOffset(maxStatePointValueThirdText, defaultTextOffset, targetTextOffset, maxStatePointGlowDuration);
                                    maxStatePointOuter(maxStatePointValueThirdText, defaultTextOuter, targetTextOuter, maxStatePointGlowDuration)
                                        .setOnComplete(() =>
                                        {
                                            maxStatePointOffset(maxStatePointValueThirdText, targetTextOffset, defaultTextOffset, maxStatePointGlowDuration);
                                            maxStatePointOuter(maxStatePointValueThirdText, targetTextOuter, defaultTextOuter, maxStatePointGlowDuration);
                                            maxStatePointScale(maxStatePointValueThirdText, currentScale, maxStatePointScaleDuration);

                                            //fourth text
                                            maxStatePointScale(maxStatePointValueThirdText, newScale, maxStatePointScaleDuration);
                                            maxStatePointOffset(maxStatePointValueThirdText, defaultTextOffset, targetTextOffset, maxStatePointGlowDuration);
                                            maxStatePointOuter(maxStatePointValueThirdText, defaultTextOuter, targetTextOuter, maxStatePointGlowDuration)
                                                .setOnComplete(() =>
                                                {
                                                    maxStatePointOffset(maxStatePointValueThirdText, targetTextOffset, defaultTextOffset, maxStatePointGlowDuration);
                                                    maxStatePointOuter(maxStatePointValueThirdText, targetTextOuter, defaultTextOuter, maxStatePointGlowDuration);
                                                    maxStatePointScale(maxStatePointValueThirdText, currentScale, maxStatePointScaleDuration);
                                                });
                                        });
                                });
                        });
                 }
            }
        }

        if (this.selectedCharacter.GetIsBreakStatusCausedByStatePoint())
        {
            this.statePointBreakText.SetActive(true);
            this.statePointValueText.gameObject.SetActive(false);
            this.statePointStatus.sprite = this.statePointBreak;
        }
        else
        {
            this.statePointBreakText.SetActive(false);
            this.statePointValueText.gameObject.SetActive(true);
            this.statePointStatus.sprite = this.statePointNoBreak;
        }
    }


    private void HealthPointInfo()
    {
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _currentHealthPoint = this.selectedCharacter.GetCurrentHealthPoint();
        float _virtualHealthPoint = this.selectedCharacter.GetVirtualHealthPoint();

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }
        if (this.startingHealthPoint != _currentHealthPoint)
        {
            this.healthPointBar.fillAmount = _currentHealthPoint / _maximumHealthPoint;
            LeanTween.value(gameObject, this.startingHealthPoint, _currentHealthPoint, textAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    this.startingHealthPoint = Mathf.RoundToInt(val);
                    this.healthPointValueText.SetText(this.startingHealthPoint.ToString() + " / " + _maximumHealthPoint);
                    this.healthPointBarAnimation.fillAmount = this.startingHealthPoint / _maximumHealthPoint;
                });
        }

        if (this.startingVirtualPoint != _virtualHealthPoint)
        {
            LeanTween.value(gameObject, this.startingVirtualPoint, _virtualHealthPoint, 0.3f)
                .setOnUpdate((float val) =>
                {
                    this.startingVirtualPoint = Mathf.RoundToInt(val);
                    this.virtualHPBar.fillAmount = this.startingVirtualPoint / _maximumHealthPoint;
                });
        }
    }

    private void StressValueInfo()
    {
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
        if (isSetupComplete == false)
        {
            this.stressValueStatus.material.SetFloat("_Slide", 0);
        }

        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPercentageText.SetText("BREAK");
            stressValueStatusAnimation(breakStatusColor, stressValueStatusDuration, "_Color1_G_Percentage");
            stressValueStatusAnimation(breakStatusColor, stressValueStatusDuration, "_Color2_G_Percentage");
            stressValueStatusAnimation(breakStatusColor, stressValueStatusDuration, "_Color1_B_Percentage");
        }

        if (startingStressValue != _currentStressValue)
        {
            this.stressPercentageText.SetText(_currentStressValue.ToString());
            if (isSetupComplete == true)
            {
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

                
                this.characterInfoPanelAnimation.Play(ANIMATION_ID_STRESS_POINT_INCREASE, 0, 0f);

                //stress value status animation
                if (_currentStressValue == 0 || _currentStressValue >= 50)
                {
                    stressValueStatusAnimation(this.defaultColor, stressValueStatusDuration, "_Color1_G_Percentage");
                }
                else if(_currentStressValue >= 70)
                {
                    stressValueStatusAnimation(this.orangeColor, stressValueStatusDuration, "_Color1_G_Percentage");
                }
                else if (_currentStressValue >= 80)
                {
                    stressValueStatusAnimation(this.redColor, stressValueStatusDuration, "_Color1_G_Percentage");
                }

                //number changing animation
                LeanTween.value(gameObject, startingStressValue, _currentStressValue, textAnimationDuration)
                        .setOnUpdate((float val) =>
                        {
                            this.stressValueStatus.material.SetFloat("_Slide", val / 100);
                            this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, new Color32(255, 0, 0, 255));
                            this.startingStressValue = Mathf.RoundToInt(val);
                            this.stressPercentageText.SetText($"{startingStressValue}<size=40>%</size>");
                        });

                //number outline animation
                this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, redColor);
                LeanTween.value(gameObject, new Vector3(blueR, blueG, blueB), new Vector3(redR, redG, redB), textAnimationDuration)
                    .setOnUpdate((Vector3 color) =>
                    {
                        Color32 currentColor = new Color32((byte)(color.x + 255), (byte)(color.y - 100), (byte)(color.z - 255), 255);
                        this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentColor);
                    }).setOnComplete(() =>
                    {
                        LeanTween.value(gameObject, new Vector3(redR, redG, redB), new Vector3(blueR, blueG, blueB), textAnimationDuration)
                            .setOnUpdate((Vector3 color) =>
                            {
                                Color32 currentColor = new Color32((byte)(color.x - 255), (byte)(color.y + 100), (byte)(color.z + 255), 255);
                                this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentColor);
                            });
                    });
            }
        }
    }

    public LTDescr maxStatePointScale(TextMeshProUGUI textObject, Vector3 targetScale, float scaleDuration)
    {
        return LeanTween.scale(textObject.gameObject, targetScale, scaleDuration);
    }

    public LTDescr maxStatePointOffset(TextMeshProUGUI textObject, float defaultOffset, float targetOffset, float glowDuration)
    {
        return LeanTween.value(textObject.gameObject, defaultOffset, targetOffset, glowDuration)
           .setOnUpdate((float val) =>
           {
               textObject.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, val);
           });
    }

    public LTDescr maxStatePointOuter(TextMeshProUGUI textObject, float defaultOuter, float targetOuter, float glowDuration)
    {
        return LeanTween.value(textObject.gameObject, defaultOuter, targetOuter, glowDuration)
           .setOnUpdate((float val) =>
           {
               textObject.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOuter, val);
           });
    }

    public LTDescr stressValueStatusAnimation(float colorPercentage, float animationDuration, string targetColor)
    {
        return LeanTween.value(gameObject, this.stressValueStatus.material.GetFloat(targetColor), colorPercentage, animationDuration)
                        .setOnUpdate((float val) =>
                        {
                            this.stressValueStatus.material.SetFloat(targetColor, val);
                        });
    }
}
