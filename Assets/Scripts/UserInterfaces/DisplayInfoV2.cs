using TMPro;
using UnityEngine;

public class DisplayInfoV2 : MonoBehaviour
{
    [SerializeField] private TMP_Text displayNumber = null;
    [SerializeField] private Transform leftSpawnPosition = null;
    [SerializeField] private Animator animator = null;

    public void DisplayPopUpNumber( float numberToDisplay, bool isLeft )
    {
        if (CheckIfCanDisplayNumber( numberToDisplay ))
        {
            if (isLeft)
            {
                this.gameObject.transform.position = leftSpawnPosition.position;
            }
            this.displayNumber.SetText( numberToDisplay.ToString() );
            this.animator.SetTrigger("start");
        }
        else
        {
            this.gameObject.SetActive( false );
        }
    }

    public bool CheckIfCanDisplayNumber( float numberToDisplay )
    {
        if (numberToDisplay > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
