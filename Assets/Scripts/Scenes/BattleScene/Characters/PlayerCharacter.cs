using UnityEngine;

public class PlayerCharacter : GameCharacter
{
    public PlayerCharacter( int id, float maximumHealthPoint, float maximumActionPoint )
    {
        base.Initialize( id, maximumHealthPoint, maximumActionPoint );
    }
}
