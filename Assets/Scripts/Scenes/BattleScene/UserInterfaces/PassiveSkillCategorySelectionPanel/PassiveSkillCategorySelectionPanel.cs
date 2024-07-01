using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PassiveSkillCategorySelectionPanel : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PassiveSkillSlot[] passiveSkillSlotsList = new PassiveSkillSlot[0];
    [SerializeField] private GameObject passiveSkillHolder;
    [SerializeField] private GameObject passiveSkillInfoBox;
    [SerializeField] private Image passiveSkillButton;

    private bool isClicked = false;
    private bool isHolding = false;
    private bool isPointerDown;
    private float lastHoldingTime = 0f;
    private float holdingDelay = 0.2f;

    public void Start()
    {
        for(int i=0; i < passiveSkillSlotsList.Length; i++)
        {
            passiveSkillSlotsList[i].UpdateDefaultPassiveSkillUI();
        }
    }

    public void ClickButton()
    {
        if (isClicked == false)
        {
            isClicked = true;
            passiveSkillHolder.SetActive(true);
        }
        else
        {
            isClicked = false;
            passiveSkillHolder.SetActive(false);
        }
    }

    public void StartHolding()
    {
        lastHoldingTime = Time.time;
        isPointerDown = true;
        this.passiveSkillHolder.SetActive(true);
    }

    public void ReleaseButton()
    {
        if (isPointerDown)
        {
            isPointerDown = false;
            float holdingDuration = Time.time - lastHoldingTime;
            if(holdingDuration >= holdingDelay)
            {
                passiveSkillHolder.SetActive(false);
            }
            else
            {
                ClickButton();
            }
        }
    }
}
