using UnityEngine;

public static class MathExtensions
{
    public static int ConvertToIntegerInPercentage( this float f )
    {
        return Mathf.RoundToInt( f * 100 );
    }
}
