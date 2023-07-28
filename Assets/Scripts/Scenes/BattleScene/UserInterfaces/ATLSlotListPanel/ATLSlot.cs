using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ATLSlot : MonoBehaviour
{
    [SerializeField] private Image selectedHighlight = null;
    [SerializeField] private TextMeshProUGUI ownerName = null;
    [SerializeField] private TextMeshProUGUI skillName = null;
    [SerializeField] private Image slotBackground = null;

    [Header("ATL slot color")]
    [SerializeField] private Color activeATLSlotColor = new Color();
    [SerializeField] private Color inactiveATLSlotColor = new Color();

    private BattleFlowATL battleFlowATL = null;

    public void Initialize()
    {
        
    }

    private void OnEnable()
    {
        slotBackground.color = activeATLSlotColor;
    }

    public void Show( BattleFlowATL flowATL )
    {
        this.battleFlowATL = flowATL;
        base.gameObject.SetActive( true );
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }

    public void ShowSelectionHighlight()
    {
        this.selectedHighlight.gameObject.SetActive(true);
    }

    public void HideSelectionHighlight()
    {
        this.selectedHighlight.gameObject.SetActive(false);
    }

    public void MarkATLSlotColorInactive()
    {
        slotBackground.color = inactiveATLSlotColor;
    }

    public void SetOwnerName(string ownerName)
    {
        this.ownerName.SetText(ownerName);
    }

    public void SetSkillName(string skillName)
    {
        this.skillName.SetText(skillName);
    }
}