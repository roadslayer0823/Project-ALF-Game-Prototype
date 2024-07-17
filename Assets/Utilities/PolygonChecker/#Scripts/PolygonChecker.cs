using System.Collections.Generic;
using UnityEngine;

public class PolygonChecker : MonoBehaviour
{
    [SerializeField] private Transform[] polygonPoints = new Transform[ 0 ];
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private Transform cursor = null;

    public bool IsMousePositionInsidePolygonArea()
    {
        this.cursor.position = Input.mousePosition;

        List<Vector2> _vectors = new();

        for (int i = 0; i < this.polygonPoints.Length; i++)
        {
            Vector3 _point = this.polygonPoints[ i ].position;
            _vectors.Add( new Vector2( _point.x, _point.y ) );
        }

        return PolygonChecker.IsPointInsidePolygon( this.cursor.position, _vectors );
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        if (this.polygonPoints.Length > 0)
        {
            for (int i = 0; i < this.polygonPoints.Length - 1; i++)
            {
                Gizmos.DrawLine( this.polygonPoints[ i ].position, this.polygonPoints[ i + 1 ].position );
            }

            Gizmos.DrawLine( this.polygonPoints[ this.polygonPoints.Length - 1 ].position, this.polygonPoints[ 0 ].position );
        }
    }

#endif

    public static bool IsPointInsidePolygon( Vector2 point, List<Vector2> polygonPoints )
    {
        int num_vertices = polygonPoints.Count;
        float x = point.x, y = point.y;
        bool inside = false;

        // Store the first point in the polygon and initialize the second point
        Vector2 p1 = polygonPoints[ 0 ], p2;

        // Loop through each edge in the polygon
        for (int i = 1; i <= num_vertices; i++)
        {
            // Get the next point in the polygon
            p2 = polygonPoints[ i % num_vertices ];

            // Check if the point is above the minimum y coordinate of the edge
            if (y > Mathf.Min( p1.y, p2.y ))
            {
                // Check if the point is below the maximum y coordinate of the edge
                if (y <= Mathf.Max( p1.y, p2.y ))
                {
                    // Check if the point is to the left of the maximum x coordinate of the edge
                    if (x <= Mathf.Max( p1.x, p2.x ))
                    {
                        // Calculate the x-intersection of the line connecting the point to the edge
                        float x_intersection = ( y - p1.y ) * ( p2.x - p1.x ) / ( p2.y - p1.y ) + p1.x;

                        // Check if the point is on the same line as the edge or to the left of the x-intersection
                        if (p1.x == p2.x || x <= x_intersection)
                        {
                            // Flip the inside flag
                            inside = !inside;
                        }
                    }
                }
            }

            // Store the current point as the first point for the next iteration
            p1 = p2;
        }

        // Return the value of the inside flag
        return inside;
    }
}
