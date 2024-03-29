using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoPanelV2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stressPercentageText = null;
    [SerializeField] private TextMeshProUGUI healthPointValueText = null;
    [SerializeField] private TextMeshProUGUI statePointValueText = null;
    [SerializeField] private TextMeshProUGUI maxStatePointValueText = null;
    [SerializeField] private GameObject statePointBreakText = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private Sprite stressPointBreak = null;
    [SerializeField] private Sprite stressPointNoBreak = null;
    [SerializeField] private Image stressPointStatus = null;
    [SerializeField] private Image statePointStatus = null;
    [SerializeField] private Image virtualHPBar = null;
    [SerializeField] private Image healthPointBar = null;
    [SerializeField] private Image statePointBar = null;

    private GameCharacter selectedCharacter = null;
    private bool isSetupComplete = false;

    private float startingStatePoint = 0.0f;
    private float startingHealthPoint = 0.0f;
    private float startingVirtualPoint = 0.0f;
    private float startingStressValue = -1;
    private float animationTime = 0.3f;

    public void Initialize()
    {
        float _startingStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _startingHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _startingVirtualPoint = this.selectedCharacter.GetMaximumHealthPoint();

        startingStatePoint = _startingStatePoint;
        startingHealthPoint = _startingHealthPoint;
        startingVirtualPoint = _startingVirtualPoint;
    }

    public void SetSelectedCharacter( GameCharacter selectedCharacter )
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
        if(isSetupComplete == false)
        {
            UpdateDisplayInfo();
            isSetupComplete = true;
        }
    }

    public void UpdateDisplayInfo()
    {
        HealthPointInfo();
        StatePointInfo();
        StressValueInfo();
    }

    private void StatePointInfo()
    {
        float _maximumStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _currentStatePoint = this.selectedCharacter.GetCurrentStatePoint();

        this.maxStatePointValueText.SetText(_maximumStatePoint.ToString());

        if (this.startingStatePoint != _currentStatePoint)
        {
            LeanTween.value(gameObject, this.startingStatePoint, _currentStatePoint, animationTime)
                .setOnUpdate((float val) =>
                {
                    this.startingStatePoint = Mathf.RoundToInt(val);
                    this.statePointValueText.SetText(startingStatePoint.ToString());
                    this.statePointBar.fillAmount = startingStatePoint / _maximumStatePoint;
                });
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
            LeanTween.value(gameObject, this.startingHealthPoint, _currentHealthPoint, animationTime)
                .setOnUpdate((float val) =>
                {
                    this.startingHealthPoint = Mathf.RoundToInt(val);
                    this.healthPointValueText.SetText(this.startingHealthPoint.ToString() + " / " + _maximumHealthPoint);
                    this.healthPointBar.fillAmount = this.startingHealthPoint / _maximumHealthPoint;
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

        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPercentageText.SetText("BREAK");
            this.stressPointStatus.sprite = this.stressPointBreak;
        }
        else if(startingStressValue != _currentStressValue)
        {
            LeanTween.value(gameObject, startingStressValue, _currentStressValue, animationTime)
                    .setOnUpdate((float val) =>
                    {
                        this.startingStressValue = Mathf.RoundToInt(val);
                        this.stressPercentageText.SetText(startingStressValue.ToString());
                    });
        }
        this.stressPointStatus.sprite = this.stressPointNoBreak;
    }
}
