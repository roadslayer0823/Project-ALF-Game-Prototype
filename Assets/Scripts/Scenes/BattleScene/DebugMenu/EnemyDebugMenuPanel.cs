using UnityEngine;
using UnityEngine.UI;

public class EnemyDebugMenuPanel : MonoBehaviour
{
    [SerializeField] private GameObject container = null;

    [Header("PassiveSkill")]
    [SerializeField] private Toggle allPasiveSkillToggle;
    [SerializeField] private Toggle healthPassiveSkillToggle;
    [SerializeField] private Toggle stressPassiveSkillToggle;
    [SerializeField] private Toggle statePassiveSkillToggle;

    [Header("ActiveSkill")]
    [SerializeField] private Toggle allActiveSkillToggle;
    [SerializeField] private Toggle rangedSkillToggle;
    [SerializeField] private Toggle meleeSkillToggle;
    [SerializeField] private Toggle rangedMeleeSkillToggle;

    [Header("BackendSkill")]
    [SerializeField] private Toggle allBackendSkillToggle;
    [SerializeField] private Toggle defendingToggle;
    [SerializeField] private Toggle evadingToggle;
    [SerializeField] private Toggle observingToggle;

    [Header("SkillLevel")]
    [SerializeField] private Toggle skillLevel1;
    [SerializeField] private Toggle skillLevel2;
    [SerializeField] private Toggle skillLevel3;

    private const string AUDIO_ID_CLICK = "click";

    public void ClickToShow()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        Time.timeScale = 0.0f;
        this.container.SetActive( true );
    }

    public void ClickToHide()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        Time.timeScale = PauseButton.currentTimeScale;
        this.container.SetActive( false );
    }

    public void TickAllPasiveSkill()
    {
        bool isToggleOn = this.allPasiveSkillToggle.isOn;

        this.healthPassiveSkillToggle.SetIsOnWithoutNotify(isToggleOn);
        this.stressPassiveSkillToggle.SetIsOnWithoutNotify(isToggleOn);
        this.statePassiveSkillToggle.SetIsOnWithoutNotify(isToggleOn);
    }

    public void TickPassiveSkillParent()
    {
        this.allPasiveSkillToggle.SetIsOnWithoutNotify(this.healthPassiveSkillToggle.isOn && this.stressPassiveSkillToggle.isOn && this.statePassiveSkillToggle.isOn);
    }

    public void TickAllActiveSkill()
    {
        bool isToggleOn = this.allActiveSkillToggle.isOn;

        this.rangedSkillToggle.SetIsOnWithoutNotify(isToggleOn);
        this.meleeSkillToggle.SetIsOnWithoutNotify(isToggleOn);
        this.rangedMeleeSkillToggle.SetIsOnWithoutNotify(isToggleOn);
    }

    public void TickActiveSkillParent()
    {
        this.allActiveSkillToggle.SetIsOnWithoutNotify(this.rangedSkillToggle.isOn && this.meleeSkillToggle.isOn && this.rangedMeleeSkillToggle.isOn);
    }

    public void TickAllBackendSkill()
    {
        bool isToggleOn = this.allBackendSkillToggle.isOn;

        this.defendingToggle.SetIsOnWithoutNotify(isToggleOn);
        this.evadingToggle.SetIsOnWithoutNotify(isToggleOn);
        this.observingToggle.SetIsOnWithoutNotify(isToggleOn);
    }

    public void TickBackendSkillParent()
    {
        this.allBackendSkillToggle.SetIsOnWithoutNotify(this.defendingToggle.isOn && this.evadingToggle.isOn && this.observingToggle.isOn);
    }

    // Passive Skills
    public bool IsHealthPassiveSkillToggleOn()
    {
        return this.healthPassiveSkillToggle.isOn;
    }

    public bool IsStressPassiveSkillToggleOn()
    {
        return this.stressPassiveSkillToggle.isOn;
    }

    public bool IsStatePassiveSkillToggleOn()
    {
        return this.statePassiveSkillToggle.isOn;
    }

    // Active Skills
    public bool IsRangedSkillToggleOn()
    {
        return this.rangedSkillToggle.isOn;
    }

    public bool IsMeleeSkillToggleOn()
    {
        return this.meleeSkillToggle.isOn;
    }

    public bool IsRangedMeleeSkillToggleOn()
    {
        return this.rangedMeleeSkillToggle.isOn;
    }

    // Backend Skills
    public bool IsDefendingSkillToggleOn()
    {
        return this.defendingToggle.isOn;
    }

    public bool IsEvadingSkillToggleOn()
    {
        return this.evadingToggle.isOn;
    }

    public bool IsObservingSkillToggleOn()
    {
        return this.observingToggle.isOn;
    }

    // Skills Level
    public bool IsSkillLevel1ToggleOn()
    {
        return this.skillLevel1.isOn;
    }

    public bool IsSkillLevel2ToggleOn()
    {
        return this.skillLevel2.isOn;
    }

    public bool IsSkillLevel3ToggleOn()
    {
        return this.skillLevel3.isOn;
    }
}
