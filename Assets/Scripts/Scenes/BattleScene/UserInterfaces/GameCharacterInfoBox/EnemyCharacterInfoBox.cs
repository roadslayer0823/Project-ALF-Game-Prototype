using UnityEngine;
using TMPro;

public class EnemyCharacterInfoBox : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private GameCharacter selectedCharacter = null;

    [Header("Enemy Info")]
    [SerializeField] private TextMeshPro enemyNameText = null;

    [Header("Stress Value UI")]
    [SerializeField] private TextMeshPro stressPercentageText = null;
    [SerializeField] private GameObject stressBreakUI = null;
    [SerializeField] private Sprite stressPointBreak = null;
    [SerializeField] private Sprite stressPointNoBreak = null;
    [SerializeField] private SpriteRenderer stressPointStatus = null;

    [Header("Effect Marker UI")]
    [SerializeField] private TextMeshPro effectMarkerLabel = null;
    [SerializeField] private GameObject effectMarker = null;

    [Header("State Point UI")]
    [SerializeField] private Material statePointFillAmount = null;
    [SerializeField] private GameObject stateBreakUI = null;
    [SerializeField] private Sprite statePointBreak = null;
    [SerializeField] private Sprite statePointNoBreak = null;
    [SerializeField] private SpriteRenderer statePointStatus = null;

    [Header("Health Point UI")]
    [SerializeField] private Material healthPointFillAmount = null;
    [SerializeField] private Material virtualPointFillAmount = null;
    [SerializeField] private SpriteRenderer virtualHPBar = null;
    [SerializeField] private SpriteRenderer healthPointBar = null;
    [SerializeField] private SpriteRenderer statePointBar = null;

    //starting value
    private float startingStatePoint = 0.0f;
    private float startingHealthPoint = 0.0f;
    private float startingVirtualPoint = 0.0f;
    private float startingStressValue = -1;

    //animation duration
    private float barAnimationDuration = 1f;
    private float textAnimationDuration = 0.3f;

    private void Start()
    {
        float _startingStatePoint = this.selectedCharacter.GetMaximumStatePoint();
        float _startingHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();
        float _startingVirtualPoint = this.selectedCharacter.GetMaximumHealthPoint();

        startingStatePoint = _startingStatePoint;
        startingHealthPoint = _startingHealthPoint;
        startingVirtualPoint = _startingVirtualPoint;
    }

    void Awake()
    {
        this.selectedCharacter.SetOnCharacterInfoUpdated(UpdateDisplayInfo);
        this.enemyNameText.text = this.selectedCharacter.GetCharacterName();
        UpdateDisplayInfo();
    }

    void Update()
    {
        // TODO: Temporarily update this game object's position according to the selected character's pivot's position on every frame.
        this.transform.position = this.selectedCharacter.GetPivot().position + new Vector3( 0.0f, 2.0f, 0.0f );
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

        if (this.selectedCharacter.HasEnergyMarker())
        {
            this.effectMarkerLabel.gameObject.SetActive(true);
        }
        else
        {
            this.effectMarkerLabel.gameObject.SetActive(false);
        }

        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }

        LeanTween.value(gameObject, this.startingHealthPoint, _currentHealthPoint, barAnimationDuration)
            .setOnUpdate((float val) =>
            {
                this.startingHealthPoint = Mathf.RoundToInt(val);
                float _healthPointFillAmount = startingHealthPoint / _maximumHealthPoint;
                healthPointFillAmount.SetFloat("_FillAmount", _healthPointFillAmount);
            });
        
        LeanTween.value(gameObject, this.startingVirtualPoint, _virtualHealthPoint, 0.3f)
            .setOnUpdate((float val) =>
            {
                this.startingVirtualPoint = Mathf.RoundToInt(val);
                float _virtualPointFillAmount = startingVirtualPoint / _maximumHealthPoint;
                virtualPointFillAmount.SetFloat("_FillAmount", _virtualPointFillAmount);
            });
    }

    public void StressValueInfo()
    {
        float _currentStressValue = this.selectedCharacter.GetCurrentStressValue();
        if (this.selectedCharacter.GetIsBreakStatusCausedByStressValue())
        {
            this.stressPointStatus.sprite = this.stressPointBreak;
            this.stressPercentageText.gameObject.SetActive(false);
            this.stressBreakUI.gameObject.SetActive(true);
        }
        else if(startingStressValue != _currentStressValue)
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

            this.stressPointStatus.sprite = this.stressPointNoBreak;
            this.stressPercentageText.gameObject.SetActive(true);

            //number changing animation
            LeanTween.value(gameObject, startingStressValue, _currentStressValue, textAnimationDuration)
                    .setOnUpdate((float val) =>
                    {
                        //this.stressValueGradientStatus.SetFloat("_Slide", val / 100);
                        this.stressPercentageText.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, new Color32(255, 0, 0, 255));
                        this.startingStressValue = Mathf.RoundToInt(val);
                        this.stressPercentageText.SetText($"{startingStressValue}<size=2>%</size>");
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
            this.stressBreakUI.gameObject.SetActive(false);
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
            LeanTween.value(gameObject, this.startingStatePoint, _currentStatePoint, barAnimationDuration)
                .setOnUpdate((float val) =>
                {
                    this.startingStatePoint = Mathf.RoundToInt(val);
                    float _statePointFillAmount = startingStatePoint / _maximumStatePoint;
                    statePointFillAmount.SetFloat("_FillAmount", _statePointFillAmount);
                });

            this.stateBreakUI.SetActive(false);
            this.statePointStatus.sprite = this.statePointNoBreak;
        }
    }
}
