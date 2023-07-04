using UnityEngine;

public class PlayerCharacter : GameCharacter
{
    public PlayerCharacter( int id, float healthPoint, float actionPoint )
    {
        base.id = id;
        base.healthPoint = healthPoint;
        base.actionPoint = actionPoint;
    }
}
