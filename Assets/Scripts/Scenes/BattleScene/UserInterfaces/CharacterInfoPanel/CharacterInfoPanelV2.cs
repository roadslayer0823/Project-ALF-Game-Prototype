using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelV2 : MonoBehaviour
{
    [Header("Stress Value UI")]
    [SerializeField] private Image stressValueStatus = null;
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;
    [SerializeField] private TextMeshProUGUI stressValueBreak = null;

    [Header("Effect Marker UI")]
    [SerializeField] private TextMeshProUGUI effectMarkerValue = null;
    [SerializeField] private GameObject effectMarker = null;

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
    [SerializeField] private TextMeshProUGUI[] maxStatePointText = new TextMeshProUGUI[0];
    [SerializeField] private TextMeshProUGUI maxStatePointValueFirstText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueSecondText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueThirdText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueFourthText = null;
    [SerializeField] private TextMeshProUGUI statePointBreakText = null;
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
    private float maxStatePointDuration = 0.1f;
    private float stressValueStatusDuration = 0.2f;
    private float scaleMultiplier = 0.7f;

    //glow effect
    private float defaultTextOffset = -1f;
    private float defaultTextOuter = 0.0f;
    private float targetTextOffset = -0.3f;
    private float targetTextOuter = 0.75f;

    //gradient color percentage
    private float orangeColor = 0.5f;
    private float redColor = 0.0f;
    private float breakStatusColor = 0.0f;
    private float defaultColor = 1f;

    public void Initialize()
    {
        float _startingStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _startingHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _startingVirtualPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _startingStressValue = -1f;

        this.startingStatePoint = _startingStatePoint;
        this.startingHealthPoint = _startingHealthPoint;
        this.startingVirtualPoint = _startingVirtualPoint;
        this.startingStressValue = _startingStressValue;
    }

    public void SetSelectedCharacter(GameCharacter selectedCharacter)
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.AddOnCharacterInfoUpdatedCallback( UpdateDisplayInfo );
        UpdateDisplayInfo();
    }

    public void UpdateDisplayInfo()
    {
        if (this.effectMarker != null)
        {
            if (this.selectedCharacter.HasEnergyMarker())
            {
                this.effectMarkerValue.SetText($"{ this.selectedCharacter.GetEnergyMarkerRemainingATLs() }");
                this.effectMarker.SetActive(true);
            }
            else
            {
                this.effectMarker.SetActive(false);
            }
        }

        HealthPointInfo();
        StatePointInfo();
        StressValueInfo();
        this.isSetupComplete = true;
    }

    private void StatePointInfo()
    {
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        string maximumStatePointText = _maximumStatePoint.ToString();

        (bool isThirdTextActive, bool isFourthTextActive) = maximumStatePointText.Length switch
        {
            2 => (false, false),
            3 => (true, false),
            4 => (true, true),
            _ => throw new NotImplementedException()
        };
        MaxStatePointTextSpacing(isThirdTextActive, isFourthTextActive, maximumStatePointText);

        if (this.startingStatePoint != _currentStatePoint)
        {
            //curret state point animation
            LeanTween.value(gameObject, this.startingStatePoint, _currentStatePoint, this.textAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    this.startingStatePoint = Mathf.RoundToInt(val);
                    this.statePointValueText.SetText(this.startingStatePoint.ToString());
                    this.statePointBar.fillAmount = this.startingStatePoint / _maximumStatePoint;
                });

            //max state point increase animation
            if (this.startingStatePoint != _maximumStatePoint)
            {
                if (this.isSetupComplete == true)
                {
                    Vector2 maxStatePointIconOriginalPosition = this.maxStatePointIncreaseIcon.gameObject.transform.position;
                    float maxStatePointIncreaseOpacity = this.maxStatePointIncreaseIcon.color.a;

                    //max state point increase animation
                    this.maxStatePointIncreaseIcon.gameObject.SetActive(true);
                    this.characterInfoPanelAnimation.Play(ANIMATION_ID_MAX_STATE_POINT_INCREASE, 1, 0f);
                    LeanTween.move(this.maxStatePointIncreaseIcon.gameObject, this.maxStatePointAnimEndingPoint, 0.5f)
                       .setOnComplete(() =>
                        {
                            LeanTween.alpha(this.maxStatePointIncreaseIcon.gameObject, 0, 0.3f);
                            this.maxStatePointIncreaseIcon.gameObject.SetActive(false);
                            maxStatePointIncreaseOpacity = 255;
                            maxStatePointIncreaseIcon.gameObject.transform.position = maxStatePointIconOriginalPosition;
                        });
                    StartCoroutine(PlayMaxStatePointAnimation());
                }
            }
        }

        (bool isBreakTextActive, bool isValueTextActive, Sprite statePointStatus) = this.selectedCharacter.IsInStateBreakStatus() switch
        {
            true => (true, false, this.statePointBreak),
            false => (false, true, this.statePointNoBreak)
        };
        this.statePointBreakText.gameObject.SetActive(isBreakTextActive);
        this.statePointValueText.gameObject.SetActive(isValueTextActive);
        this.statePointStatus.sprite = statePointStatus;
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
        if (this.isSetupComplete == false)
        {
            this.stressValueStatus.material.SetFloat("_Slide", 0);
        }

        if (this.selectedCharacter.IsInStressBreakStatus())
        {
            this.stressValueBreak.gameObject.SetActive(true);
            this.stressPercentageText.gameObject.SetActive(false);
            stressValueStatusAnimation(breakStatusColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
            stressValueStatusAnimation(breakStatusColor, this.stressValueStatusDuration, "_Color2_G_Percentage");
            stressValueStatusAnimation(breakStatusColor, this.stressValueStatusDuration, "_Color1_B_Percentage");
        }
        else if (this.startingStressValue != _currentStressValue)
        {
            this.stressPercentageText.SetText(_currentStressValue.ToString());
            this.stressPercentageText.gameObject.SetActive(true);
            this.stressValueBreak.gameObject.SetActive(false);
            if (this.isSetupComplete == true)
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

                var _sequence = LeanTween.sequence();

                this.characterInfoPanelAnimation.Play(ANIMATION_ID_STRESS_POINT_INCREASE, 0, 0f);

                //stress value status animation
                if (_currentStressValue >= 0)
                {
                    stressValueStatusAnimation(defaultColor, this.stressValueStatusDuration, "_Color1_G_Percentage");
                    stressValueStatusAnimation(defaultColor, this.stressValueStatusDuration, "_Color2_G_Percentage");
                    stressValueStatusAnimation(defaultColor, this.stressValueStatusDuration, "_Color1_B_Percentage");
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
                LeanTween.value(gameObject, this.startingStressValue, _currentStressValue, textAnimationDuration)
                        .setOnUpdate((float val) =>
                        {
                            this.stressValueStatus.material.SetFloat("_Slide", val / 100);
                            this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, new Color32(255, 0, 0, 255));
                            this.startingStressValue = Mathf.RoundToInt(val);
                            this.stressPercentageText.SetText($"{this.startingStressValue}<size=40>%</size>");
                        });

                //number outline animation
                this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, redColor);

                _sequence.append(LeanTween.value(gameObject, new Vector3(blueR, blueG, blueB), new Vector3(redR, redG, redB), this.textAnimationDuration)
                    .setOnUpdate((Vector3 color) =>
                    {
                        Color32 currentColor = new Color32((byte)(color.x + 255), (byte)(color.y - 100), (byte)(color.z - 255), 255);
                        this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentColor);
                    }));

                _sequence.append(LeanTween.value(gameObject, new Vector3(redR, redG, redB), new Vector3(blueR, blueG, blueB), this.textAnimationDuration)
                    .setOnUpdate((Vector3 color) =>
                    {
                        Color32 currentColor = new Color32((byte)(color.x - 255), (byte)(color.y + 100), (byte)(color.z + 255), 255);
                        this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentColor);
                    }));
            }
        }
    }


    public LTDescr stressValueStatusAnimation(float colorPercentage, float animationDuration, string targetColor)
    {
        return LeanTween.value(gameObject, this.stressValueStatus.material.GetFloat(targetColor), colorPercentage, animationDuration)
                        .setOnUpdate((float val) =>
                        {
                            this.stressValueStatus.material.SetFloat(targetColor, val);
                        });
    }

    public void MaxStatePointTextList(string targetText)
    {
        for (int i = 0; i < targetText.Length; i++)
        {
            this.maxStatePointText[i].text = targetText[i].ToString();
        }
    }

    public void MaxStatePointTextSpacing(bool isThirdText, bool isFourthText, string targetString)
    {
        this.maxStatePointValueThirdText.gameObject.SetActive(isThirdText);
        this.maxStatePointValueFourthText.gameObject.SetActive(isFourthText);
        MaxStatePointTextList(targetString);
    }

    public IEnumerator PlayMaxStatePointAnimation()
    {
        Vector2 currentScale = this.maxStatePointValueFirstText.rectTransform.localScale;
        Vector2 newScale = new Vector3(currentScale.x / this.scaleMultiplier, currentScale.y / this.scaleMultiplier);
        for (int i = 0; i < this.maxStatePointText.Length; i++)
        {
            var targetText = this.maxStatePointText[i];
            targetText.rectTransform.localScale = currentScale;
            targetText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, this.defaultTextOffset);
            targetText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOuter, this.defaultTextOuter);
            yield return StartCoroutine(MaxStatePointAnimation(targetText, newScale, currentScale, this.defaultTextOffset, this.targetTextOffset, this.defaultTextOuter, this.targetTextOuter, this.maxStatePointDuration));
            yield return new WaitForEndOfFrame();
        }
    }


    public IEnumerator MaxStatePointAnimation(TextMeshProUGUI targetText, Vector3 targetScale, Vector3 currentScale, float defaultOffset, float targetOffset, float defaultOuter, float targetOuter, float duration)
    {
        var _sequence = LeanTween.sequence();

        LeanTween.cancel(targetText.gameObject);

        _sequence.append(LeanTween.scale(targetText.gameObject, targetScale, duration));

        _sequence.insert(LeanTween.value(targetText.gameObject, defaultOffset, targetOffset, duration)
           .setOnUpdate((float val) => { targetText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, val); }));

        _sequence.insert(LeanTween.value(targetText.gameObject, defaultOuter, targetOuter, duration)
           .setOnUpdate((float val) => { targetText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOuter, val); }));

        yield return new WaitForSeconds(0.1f);

        _sequence.insert(LeanTween.value(targetText.gameObject, targetOffset, defaultOffset, duration)
           .setOnUpdate((float val) => { targetText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOffset, val); }));

        _sequence.insert(LeanTween.value(targetText.gameObject, targetOuter, defaultOuter, duration)
           .setOnUpdate((float val) => { targetText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowOuter, val); }));

        _sequence.insert(LeanTween.scale(targetText.gameObject, currentScale, duration));

        yield return new WaitForSeconds(0.1f);
    }
}
