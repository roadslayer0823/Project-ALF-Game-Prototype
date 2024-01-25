using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public delegate void SwipeAction(Vector2 swipeDirection);
    public static event SwipeAction OnSwipe;

    public RectTransform swipeableArea;

    private Vector2 mousePressPosition;
    private Vector2 mouseReleasePosition;
    private Vector2 currentSwipe;

    void Update()
    {
        Swipe();
    }

    void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Save starting touch 2D point 
            mousePressPosition = Input.mousePosition;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(swipeableArea, mousePressPosition))
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Save ended touch 2D point
            mouseReleasePosition = Input.mousePosition;

            if (!swipeableArea.gameObject.activeInHierarchy)
            {
                return;
            }

            // Create vector from the two points
            currentSwipe = new Vector2(mouseReleasePosition.x - mousePressPosition.x, mouseReleasePosition.y - mousePressPosition.y);

            currentSwipe.Normalize();
            OnSwipe?.Invoke(currentSwipe);
        }
    }
}
