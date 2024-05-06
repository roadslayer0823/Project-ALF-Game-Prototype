using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PassiveSkillCategorySelectionPanel : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PassiveSkillSlot[] passiveSkillSlots = new PassiveSkillSlot[0];
    [SerializeField] private GameObject passiveSkillHolder;
    [SerializeField] private GameObject passiveSkillInfoBox;
    [SerializeField] private Image passiveSkillButton;

    private bool isHolding = false;
    private bool isClicked = false;

    public void Start()
    {
        for(int i = 0; i<passiveSkillSlots.Length; i++)
        {
            passiveSkillSlots[i].UpdateDefaultPassiveSkillUI();
        }
    }

    public void HoldButton()
    {
        isHolding = true;
        passiveSkillButton.gameObject.SetActive(false);
        passiveSkillHolder.SetActive(true);
    }

    public void ReleaseHoldButton()
    {
        isHolding = false;
        passiveSkillButton.gameObject.SetActive(true);
        passiveSkillHolder.SetActive(false);
    }

    public void ClickButton()
    {
        if(isClicked == false)
        {
            isClicked = true;
            passiveSkillButton.gameObject.SetActive(false);
            passiveSkillHolder.SetActive(true);
        }
        else if(isClicked == true)
        {
            isClicked = false;
            passiveSkillButton.gameObject.SetActive(true);
            passiveSkillHolder.SetActive(false);
        }
    }

    public void ClickOption()
    {
        if(isClicked == true)
        {
            for (int i = 0; i < passiveSkillSlots.Length; i++)
            {
                passiveSkillSlots[i].UpdateSelectedPassiveSkillUI();
            }
        }
    }

    public void OptionEnter()
    {
        if (isHolding == true && isClicked == false)
        {
            for (int i = 0; i < passiveSkillSlots.Length; i++)
            {
                passiveSkillSlots[i].UpdateSelectedPassiveSkillUI();
            }
        }
    }

    public void OptionExit()
    {
        if(isHolding == true)
        {
            for (int i = 0; i < passiveSkillSlots.Length; i++)
            {
                passiveSkillSlots[i].UpdateDefaultPassiveSkillUI();
            }
        }
    }
}
