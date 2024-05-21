using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static T GetRandomElement<T>( this T[] array )
    {
        return array[ Random.Range( 0, array.Length ) ];
    }

    public static T GetRandomElement<T>( this List<T> list )
    {
        return list[ Random.Range( 0, list.Count ) ];
    }
}
