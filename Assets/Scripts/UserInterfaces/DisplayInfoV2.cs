using TMPro;
using UnityEngine;

public class DisplayInfoV2 : MonoBehaviour
{
    [SerializeField] private TMP_Text displayNumber = null;
    [SerializeField] private Transform leftSpawnPosition = null;

    public void DisplayPopUpNumber(float damageNumber, bool isLeft)
    {
        if (CheckIfCanDisplayNumber(damageNumber))
        {
            if (isLeft)
            {
                this.gameObject.transform.position = leftSpawnPosition.position;
            }
            this.displayNumber.SetText(damageNumber.ToString());
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public bool CheckIfCanDisplayNumber(float damageNumber)
    {
        if (damageNumber > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
