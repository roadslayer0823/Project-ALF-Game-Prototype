using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    protected int id = 0;
    protected float maximumHealthPoint = 0.0f;
    protected float remainingHealthPoint = 0.0f;
    protected float maximumActionPoint = 0.0f;
    protected float remainingActionPoint = 0.0f;
    protected CharacterSkill[] skills = null;

    public void Initialize( int id, float maximumHealthPoint, float maximumActionPoint )
    {
        this.id = id;
        this.maximumHealthPoint = maximumHealthPoint;
        this.remainingHealthPoint = maximumHealthPoint;
        this.maximumActionPoint = maximumActionPoint;
        this.remainingActionPoint = maximumActionPoint;
    }

    public void AddRemainingHealthPoint( float amount )
    {
        this.remainingHealthPoint += amount;
    }

    public void MinusRemainingHealthPoint( float amount )
    {
        this.remainingHealthPoint -= amount;
    }

    public void AddRemainingActionPoint( float amount )
    {
        this.remainingActionPoint += amount;
    }

    public void MinusRemainingActionPoint( float amount )
    {
        this.remainingActionPoint -= amount;
    }

    public int GetId()
    {
        return this.id;
    }

    public float GetMaximumHealthPoint()
    {
        return this.maximumHealthPoint;
    }

    public float GetMaximumActionPoint()
    {
        return this.maximumActionPoint;
    }

    public CharacterSkill[] GetSkills()
    {
        return skills;
    }
}
