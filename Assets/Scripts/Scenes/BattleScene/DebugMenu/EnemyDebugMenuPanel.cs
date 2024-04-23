using UnityEngine;
using UnityEngine.UI;

public class EnemyDebugMenuPanel : MonoBehaviour
{
    [Header("PassiveSkill")]
    [SerializeField] private Toggle allPasiveSkillToggle;
    [SerializeField] private Toggle healthPointToggle;
    [SerializeField] private Toggle stressPointToggle;
    [SerializeField] private Toggle statePointToggle;

    [Header("ActiveSkill")]
    [SerializeField] private Toggle allActiveSkillToggle;
    [SerializeField] private Toggle rangeSkillToggle;
    [SerializeField] private Toggle meleeSkillToggle;
    [SerializeField] private Toggle rangeMeleeSkillToggle;

    [Header("BackendSkill")]
    [SerializeField] private Toggle allBackendSkillToggle;
    [SerializeField] private Toggle defendingToggle;
    [SerializeField] private Toggle evadingToggle;
    [SerializeField] private Toggle observingToggle;

    [Header("SkillLevel")]
    [SerializeField] private Toggle skillLevel1;
    [SerializeField] private Toggle skillLevel2;
    [SerializeField] private Toggle skillLevel3;

    public void TickAllPasiveSkill()
    {
        if (this.allPasiveSkillToggle.isOn)
        {
            this.healthPointToggle.isOn = true;
            this.stressPointToggle.isOn = true;
            this.statePointToggle.isOn = true;
        }
        else
        {
            this.healthPointToggle.isOn = false;
            this.stressPointToggle.isOn = false;
            this.statePointToggle.isOn = false;
        }
    }

    public void TickAllActiveSkill()
    {
        if (this.allActiveSkillToggle.isOn)
        {
            this.rangeSkillToggle.isOn = true;
            this.meleeSkillToggle.isOn = true;
            this.rangeMeleeSkillToggle.isOn = true;
        }
        else
        {
            this.rangeSkillToggle.isOn = false;
            this.meleeSkillToggle.isOn = false;
            this.rangeMeleeSkillToggle.isOn = false;
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
    public bool IsHealthPassiveSkillEnabled()
    {
        return this.healthPointToggle.isOn;
    }

    public bool IsStressPassiveSkillEnabled()
    {
        return this.stressPointToggle.isOn;
    }

    public bool IsStatePassiveSkillEnabled()
    {
        return this.statePointToggle.isOn;
    }

    // Active Skills
    public bool IsRangeSkillEnabled()
    {
        return this.rangeSkillToggle.isOn;
    }

    public bool IsMeleeSkillEnabled()
    {
        return this.meleeSkillToggle.isOn;
    }

    public bool IsRangeMeleeSkillEnabled()
    {
        return this.rangeMeleeSkillToggle.isOn;
    }

    // Backend Skills
    public bool IsDefendingSkillEnabled()
    {
        return this.defendingToggle.isOn;
    }

    public bool IsEvadingSkillEnabled()
    {
        return this.evadingToggle.isOn;
    }

    public bool IsObservingSkillEnabled()
    {
        return this.observingToggle.isOn;
    }

    // Skills Level
    public bool IsSkillLevel1Enabled()
    {
        return this.skillLevel1.isOn;
    }

    public bool IsSkillLevel2Enabled()
    {
        return this.skillLevel2.isOn;
    }

    public bool IsSkillLevel3Enabled()
    {
        return this.skillLevel3.isOn;
    }
}
