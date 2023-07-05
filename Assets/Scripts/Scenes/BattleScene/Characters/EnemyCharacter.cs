using UnityEngine;

public class EnemyCharacter : GameCharacter
{
    public EnemyCharacter( int id, float maximumHealthPoint, float maximumActionPoint )
    {
        base.Initialize( id, maximumHealthPoint, maximumActionPoint );
    }
}
