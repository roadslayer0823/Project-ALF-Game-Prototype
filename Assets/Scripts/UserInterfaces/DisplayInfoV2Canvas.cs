using UnityEngine;
using TMPro;

public class DisplayInfoV2Canvas: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayNumber = null;
    [SerializeField] private Transform leftSpawnPosition = null;
    [SerializeField] private Animator animator = null;

    public void DisplayPopUpNumber(float numberToDisplay, bool isLeft)
    {
        if (CheckIfCanDisplayNumber(numberToDisplay))
        {
            if (isLeft)
            {
                this.gameObject.transform.position = leftSpawnPosition.position;
            }
            this.displayNumber.SetText(numberToDisplay.ToString());
            this.animator.SetTrigger("start");
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public bool CheckIfCanDisplayNumber(float numberToDisplay)
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
