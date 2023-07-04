using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    protected int id = 0;
    protected float healthPoint = 0.0f;
    protected float actionPoint = 0.0f;
    protected CharacterSkill[] skills = null;

    public GameCharacter()
    {
    }

    public void AddHealthPoint( float amount )
    {
        this.healthPoint += amount;
    }

    public void MinusHealthPoint( float amount )
    {
        this.healthPoint -= amount;
    }

    public void AddActionPoint( float amount )
    {
        this.actionPoint += amount;
    }

    public void MinusActionPoint( float amount )
    {
        this.actionPoint -= amount;
    }

    public int GetId()
    {
        return this.id;
    }

    public float GetHealthPoint()
    {
        return this.healthPoint;
    }

    public float GetActionPoint()
    {
        return this.actionPoint;
    }
}
