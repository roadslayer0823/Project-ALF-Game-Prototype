using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ATLSlot : MonoBehaviour
{
    [SerializeField] private Image selectedHighlight = null;
    [SerializeField] private TextMeshProUGUI ownerName = null;
    [SerializeField] private TextMeshProUGUI skillName = null;
    [SerializeField] private Image slotBackground = null;
    [SerializeField] private GameObject deactivationIndicator = null;

    [Header("ATL slot color")]
    [SerializeField] private Color playerActiveATLSlotColor = new Color();
    [SerializeField] private Color enemyActiveATLSlotColor = new Color();
    [SerializeField] private Color playerInactiveATLSlotColor = new Color();
    [SerializeField] private Color enemyInactiveATLSlotColor = new Color();

    private BattleFlowATL battleFlowATL = null;

    public Action onSkillSlotSwipedCallback = null;
    public Action onATLSlotExecutedCallback = null;

    public void Initialize(Action onSkillSlotSwipedCallback, Action onATLSlotExecutedCallback)
    {
        this.onSkillSlotSwipedCallback = onSkillSlotSwipedCallback;
        this.onATLSlotExecutedCallback = onATLSlotExecutedCallback;
    }

    public void Show( BattleFlowATL flowATL )
    {
        this.battleFlowATL = flowATL;

        Activate();

        if (battleFlowATL.CheckIsPlayer())
        {
            this.slotBackground.color = this.playerActiveATLSlotColor;
        }
        else
        {
            this.slotBackground.color = this.enemyActiveATLSlotColor;
        }

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
        if (this.battleFlowATL.CheckIsPlayer())
        {
            this.slotBackground.color = this.playerInactiveATLSlotColor;
        }
        else
        {
            this.slotBackground.color = this.enemyInactiveATLSlotColor;
        }
    }

    public void SetOwnerName(string ownerName)
    {
        this.ownerName.SetText(ownerName);
    }

    public void SetSkillName(string skillName)
    {
        this.skillName.SetText(skillName);
    }

    public void Activate()
    {
        this.deactivationIndicator.SetActive( false );
    }

    public void Deactivate()
    {
        this.deactivationIndicator.SetActive( true );
    }
}
