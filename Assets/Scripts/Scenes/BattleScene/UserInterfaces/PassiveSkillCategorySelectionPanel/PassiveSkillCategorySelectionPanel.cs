using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PassiveSkillCategorySelectionPanel : MonoBehaviour
{ 
    [Header("Reference")]
    [SerializeField] private GameObject passiveSkillHolder;
    [SerializeField] private GameObject passiveSkillInfoBox;
    [SerializeField] private TextMeshProUGUI passiveSkillName;
    [SerializeField] private Image passiveSkillButton;
    [SerializeField] private Image statePointSkillColor;
    [SerializeField] private Image stressPointSkillColor;
    [SerializeField] private Image healthPointSkillColor;

    private TextMeshProUGUI selectedOption = null;
    private bool isSelecting = false;
    private bool isHolding = false;
    private bool isClicked = false;

    public void HoldButton()
    {
        isHolding = true;
        passiveSkillHolder.SetActive(true);
    }

    public void ReleaseHoldButton()
    {
        isHolding = false;
        isSelecting = false;
        selectedOption = null;
        passiveSkillHolder.SetActive(false);
    }

    public void ClickButton()
    {
        if(isClicked == false)
        {
            isClicked = true;
            passiveSkillHolder.SetActive(true);
        }
        else if(isClicked == true)
        {
            isClicked = false;
            isSelecting = false;
            passiveSkillHolder.SetActive(false);
        }
    }

    public void ClickOption(TextMeshProUGUI option)
    {
        if(isClicked == true)
        {
            isSelecting = true;
            selectedOption = option;
            passiveSkillName.text = selectedOption.text;
            changeButtonColor();
            Debug.Log(selectedOption.name);
        }
    }

    public void OptionEnter(TextMeshProUGUI option)
    {
        if (isHolding == true && isClicked == false)
        {
            isSelecting = true;
            selectedOption = option;
            passiveSkillName.text = selectedOption.text;
            changeButtonColor();
        }
    }

    public void onOptionExit()
    {
        if(isHolding == true)
        {
            passiveSkillName.text = "流向系統";
            passiveSkillButton.color = Color.white;
        }
    }

    public void changeButtonColor()
    {
        if(passiveSkillName.text == "以太流")
        {
            passiveSkillButton.color = statePointSkillColor.color;
        }
        else if(passiveSkillName.text == "生命流")
        {
            passiveSkillButton.color = healthPointSkillColor.color;
        }
        else if(passiveSkillName.text == "负荷流")
        {
            passiveSkillButton.color = stressPointSkillColor.color;
        }
    }
}
