using UnityEngine;

public class EnemyCharacter : GameCharacter
{
    public EnemyCharacter( int id, float healthPoint, float actionPoint )
    {
        base.id = id;
        base.healthPoint = healthPoint;
        base.actionPoint = actionPoint;
    }
}
