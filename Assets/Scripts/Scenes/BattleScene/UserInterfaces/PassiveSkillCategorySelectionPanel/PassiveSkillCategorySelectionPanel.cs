using System;
using UnityEngine;
using UnityEngine.UI;

public class PassiveSkillCategorySelectionPanel : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PassiveSkillSlot[] passiveSkillSlotsList = new PassiveSkillSlot[0];
    [SerializeField] private Animator passiveSkillSelectionPanelAnimation;
    [SerializeField] private GameObject passiveSkillHolder;
    [SerializeField] private GameObject passiveSkillInfoBox;
    [SerializeField] private Transform passiveSkillButtonPosition;
    [SerializeField] private Image passiveSkillButton;

    private bool isPointerDown;
    private Vector3 initialPosition;

    private Action<PassiveSkillType> onPassiveSkillTypeUpdated = null;
    private PassiveSkillType passiveSkillType = PassiveSkillType.None;

    private const string ANIMATION_ID_EXPAND = "Expand";
    private const string ANIMATION_ID_HIDE = "Hide";

    public enum PassiveSkillType
    {
        None,
        HealthPoint,
        StatePoint,
        StressValue
    }

    public void Initialize( Action<PassiveSkillType> onPassiveSkillTypeUpdated )
    {
        this.onPassiveSkillTypeUpdated = onPassiveSkillTypeUpdated;

        for (int i = 0; i < passiveSkillSlotsList.Length; i++)
        {
            PassiveSkillSlot _passiveSkillSlot = passiveSkillSlotsList[ i ];
            _passiveSkillSlot.Initialize( this );
        }
    }

    public void StartHolding()
    {
        isPointerDown = true;
        this.passiveSkillHolder.SetActive(true);
        initialPosition = passiveSkillButtonPosition.transform.position;
        this.passiveSkillSelectionPanelAnimation.Play(ANIMATION_ID_EXPAND);
    }

    public void ReleaseButton()
    {
        if (isPointerDown)
        {
            isPointerDown = false;
            this.passiveSkillSelectionPanelAnimation.Play(ANIMATION_ID_HIDE);
            for(int i=0; i < passiveSkillSlotsList.Length; i++)
            {
                passiveSkillSlotsList[i].UpdateDefaultPassiveSkillUI();
            }
            OnPassiveSkillSlotSelected();
            Debug.Log("current passive skill:" + GetPassiveSkillType());
        }
    }

    public void DraggingDirection()
    {
        Debug.Log("is working");
        if (!isPointerDown) return;

        Vector3 currentPosition = Input.mousePosition;
        Vector3 direction = currentPosition - initialPosition;

        //<60 angle == life, >120 == stress, >60 angle and < 120 angle == state
        if(direction != Vector3.zero)
        {
            float angle = Vector3.Angle(Vector3.up, direction);

            if(Vector3.Cross(Vector3.up, direction).z > 0)
            {
                angle = 360 - angle;
            }

            Debug.Log("Angle:" + angle);

            if (angle < 60)
            {
                passiveSkillType = PassiveSkillType.HealthPoint;
            }
            else if (angle > 60 && angle < 120)
            {
                passiveSkillType = PassiveSkillType.StatePoint;
            }
            else if (angle > 120)
            {
                passiveSkillType = PassiveSkillType.StressValue;
            }
            else
            {
                Debug.Log("none skill");
            }
            UpdateCurrentPassiveSkillSlot(passiveSkillType);
        }
    }

    public void UpdateCurrentPassiveSkillSlot(PassiveSkillType currentPassiveSkill)
    {
        switch (currentPassiveSkill)
        {
            case PassiveSkillType.HealthPoint:
                passiveSkillSlotsList[0].UpdateHighlightPassiveSkillUI();
                passiveSkillSlotsList[1].UpdateDefaultPassiveSkillUI();
                passiveSkillSlotsList[2].UpdateDefaultPassiveSkillUI();
                break;

            case PassiveSkillType.StatePoint:
                passiveSkillSlotsList[2].UpdateDefaultPassiveSkillUI();
                passiveSkillSlotsList[0].UpdateDefaultPassiveSkillUI();
                passiveSkillSlotsList[1].UpdateHighlightPassiveSkillUI();
                break;

            case PassiveSkillType.StressValue:
                passiveSkillSlotsList[2].UpdateHighlightPassiveSkillUI();
                passiveSkillSlotsList[1].UpdateDefaultPassiveSkillUI();
                passiveSkillSlotsList[0].UpdateDefaultPassiveSkillUI();
                break;
        }
    }

    public void HidePassiveSkillHolder()
    {
        passiveSkillHolder.SetActive(false);
    }

    public void OnPassiveSkillSlotSelected()
    {
        PassiveSkillType _passiveSkillType = PassiveSkillType.None;

        _passiveSkillType = GetPassiveSkillType();
        this.onPassiveSkillTypeUpdated?.Invoke( _passiveSkillType );
    }

    public PassiveSkillType GetPassiveSkillType()
    {
        return this.passiveSkillType;
    }
}
