using UnityEngine;

public class ObservedSkillData
{
    private int featureId = 0;
    private float maximumObservedRate = 0.0f;
    private float currentObservedRate = 0.0f;

    public ObservedSkillData( int featureId, float maximumObservedRate )
    {
        this.featureId = featureId;
        this.maximumObservedRate = maximumObservedRate;
        this.currentObservedRate = 0.0f;
    }

    public void IncreaseObservedRate( float amount )
    {
        this.currentObservedRate = Mathf.Clamp( this.currentObservedRate + amount, 0.0f, this.maximumObservedRate );
    }

    public int GetFeatureId()
    {
        return this.featureId;
    }

    public float GetCurrentObservedRate()
    {
        return this.currentObservedRate;
    }
}
