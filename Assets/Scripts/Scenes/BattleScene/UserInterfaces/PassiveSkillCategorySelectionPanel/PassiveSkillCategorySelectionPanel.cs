using System;
using UnityEngine;
using UnityEngine.UI;
using PassiveSkillType = PassiveSkillSlot.PassiveSkillType;

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

    private PassiveSkillSlot selectedPassiveSkillSlot = null;
    private Action<PassiveSkillType> onPassiveSkillTypeUpdated = null;

    public void Initialize( Action<PassiveSkillType> onPassiveSkillTypeUpdated )
    {
        this.onPassiveSkillTypeUpdated = onPassiveSkillTypeUpdated;

        for (int i = 0; i < passiveSkillSlotsList.Length; i++)
        {
            PassiveSkillSlot _passiveSkillSlot = passiveSkillSlotsList[ i ];
            _passiveSkillSlot.Initialize( this );
            _passiveSkillSlot.SetIsSelected( false );
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

    public void OnPassiveSkillSlotSelected( PassiveSkillSlot passiveSkillSlot )
    {
        PassiveSkillType _passiveSkillType = PassiveSkillType.None;

        if (this.selectedPassiveSkillSlot != null)
        {
            this.selectedPassiveSkillSlot.SetIsSelected( false );
        }

        if (this.selectedPassiveSkillSlot != passiveSkillSlot)
        {
            this.selectedPassiveSkillSlot = passiveSkillSlot;
            this.selectedPassiveSkillSlot.SetIsSelected( true );
            _passiveSkillType = this.selectedPassiveSkillSlot.GetPassiveSkillType();
        }
        else
        {
            this.selectedPassiveSkillSlot = null;
        }

        this.onPassiveSkillTypeUpdated?.Invoke( _passiveSkillType );
    }
}
