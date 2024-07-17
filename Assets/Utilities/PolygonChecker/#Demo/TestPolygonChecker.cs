using UnityEngine;
using UnityEngine.UI;

public class TestPolygonChecker : MonoBehaviour
{
    [SerializeField] private PolygonChecker polygonChecker = null;
    [SerializeField] private Image[] images = new Image[ 0 ];

    void Update()
    {
        bool _isInside = this.polygonChecker.IsMousePositionInsidePolygonArea();

        for (int i = 0; i < this.images.Length; i++)
        {
            this.images[ i ].color = ( _isInside ) ? Color.yellow : Color.white;
        }
    }
}
