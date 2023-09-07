using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ATLSlot : MonoBehaviour
{
    [SerializeField] private Image selectedHighlight = null;
    [SerializeField] private TextMeshProUGUI ownerName = null;
    [SerializeField] private TextMeshProUGUI skillName = null;
    [SerializeField] private GameObject deactivationIndicator = null;
    [SerializeField] private GameObject passedIndicator = null;
    [SerializeField] private Image slotBackground = null;

    [Header("ATL slot color")]
    [SerializeField] private Color playerActiveATLSlotColor = new Color();
    [SerializeField] private Color enemyActiveATLSlotColor = new Color();

    [Header("")]
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

        this.passedIndicator.gameObject.SetActive(false);

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

    public void MarkATLSlotInactive()
    {
        this.passedIndicator.gameObject.SetActive(true);
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
