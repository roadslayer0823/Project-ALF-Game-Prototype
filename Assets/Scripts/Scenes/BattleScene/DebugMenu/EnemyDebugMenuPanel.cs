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
        this.container.SetActive( true );
    }

    public void ClickToHide()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        this.container.SetActive( false );
    }

    public void TickAllPasiveSkill()
    {
        if (this.allPasiveSkillToggle.isOn)
        {
            this.healthPassiveSkillToggle.isOn = true;
            this.stressPassiveSkillToggle.isOn = true;
            this.statePassiveSkillToggle.isOn = true;
        }
        else
        {
            this.healthPassiveSkillToggle.isOn = false;
            this.stressPassiveSkillToggle.isOn = false;
            this.statePassiveSkillToggle.isOn = false;
        }
    }

    public void TickAllActiveSkill()
    {
        if (this.allActiveSkillToggle.isOn)
        {
            this.rangedSkillToggle.isOn = true;
            this.meleeSkillToggle.isOn = true;
            this.rangedMeleeSkillToggle.isOn = true;
        }
        else
        {
            this.rangedSkillToggle.isOn = false;
            this.meleeSkillToggle.isOn = false;
            this.rangedMeleeSkillToggle.isOn = false;
        }
    }

    public void TickAllBackendSkill()
    {
        if (this.allBackendSkillToggle.isOn)
        {
            this.defendingToggle.isOn = true;
            this.evadingToggle.isOn = true;
            this.observingToggle.isOn = true;
        }
        else
        {
            this.defendingToggle.isOn = false;
            this.evadingToggle.isOn = false;
            this.observingToggle.isOn = false;
        }
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
