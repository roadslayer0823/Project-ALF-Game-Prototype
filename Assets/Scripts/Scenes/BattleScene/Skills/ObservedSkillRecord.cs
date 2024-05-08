using Subskill = DatabaseManager.Subskill;

public class ObservedSkillRecord
{
    private Subskill subskillData = null;
    private float currentObservedRate = 0.0f;
    private bool isRecording = false;

    public ObservedSkillRecord( Subskill subskillData )
    {
        this.subskillData = subskillData;
        this.currentObservedRate = 0.0f;
        this.isRecording = false;
    }

    public Subskill GetSubskillData()
    {
        return this.subskillData;
    }

    public void SetCurrentObservedRate( float currentObservedRate )
    {
        this.currentObservedRate = currentObservedRate;

        if (this.currentObservedRate <= 0.0f)
        {
            this.subskillData = null;
            this.currentObservedRate = 0.0f;
        }
    }

    public float GetCurrentObservedRate()
    {
        return this.currentObservedRate;
    }

    public void SetIsRecording( bool isRecording )
    {
        this.isRecording = isRecording;
    }

    public bool GetIsRecording()
    {
        return this.isRecording;
    }
}
