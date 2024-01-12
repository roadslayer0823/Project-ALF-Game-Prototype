using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ATLSlotV2 : MonoBehaviour
{
    [SerializeField] private Image atlSlot;

    [Header("ATL Status")]
    [SerializeField] private Sprite playerUnuseAtlSlot;
    [SerializeField] private Sprite playerUsingAtlSlot;
    [SerializeField] private Sprite playerUsedAtlSlot;

    [SerializeField] private Sprite enemyUnuseAtlSlot;
    [SerializeField] private Sprite enemyUsingAtlSlot;
    [SerializeField] private Sprite enemyUsedAtlSlot;

    [Header("ATL Point")]
    [SerializeField] private RectTransform PlayerStartPoint;
    [SerializeField] private RectTransform EnemyStartPoint;
    [SerializeField] private RectTransform MiddlePoint;

    [Header("")]
    private BattleFlowATL battleFlowATL = null;
    private BattleFlowATL_V2 battleFlowATL_V2 = null;

    public Action onSkillSlotSwipedCallback = null;
    public Action onATLSlotExecutedCallback = null;

    private float startingPointX = 0.0f;

    public enum ATLCurrentStatus
    {
        None,
        Unused,
        Using,
        Used
    }

    public void Initialize(Action onSkillSlotSwipedCallback, Action onATLSlotExecutedCallback)
    {
        this.onSkillSlotSwipedCallback = onSkillSlotSwipedCallback;
        this.onATLSlotExecutedCallback = onATLSlotExecutedCallback;
    }

    public void DefaultATLSetup(BattleFlowATL flowATL)
    {
        this.battleFlowATL = flowATL;
        Show(ATLCurrentStatus.Unused);
        this.atlSlot.SetNativeSize();
    }

    public void DefaultATLSetup( BattleFlowATL_V2 flowATL )
    {
        this.battleFlowATL_V2 = flowATL;
        Show( ATLCurrentStatus.Unused );
        this.atlSlot.SetNativeSize();
    }

    public void Show(ATLCurrentStatus aTLCurrentStatus)
    {
        if (this.battleFlowATL != null)
        {
            if (this.battleFlowATL.CheckIsPlayer())
            {
                if (aTLCurrentStatus == ATLCurrentStatus.Unused)
                {
                    this.atlSlot.sprite = this.playerUnuseAtlSlot;
                }
                else if (aTLCurrentStatus == ATLCurrentStatus.Using)
                {
                    this.atlSlot.sprite = this.playerUsingAtlSlot;
                }
                else if (aTLCurrentStatus == ATLCurrentStatus.Used)
                {
                    this.atlSlot.sprite = this.playerUsedAtlSlot;
                }

                this.startingPointX = this.PlayerStartPoint.position.x;
            }
            else
            {
                if (aTLCurrentStatus == ATLCurrentStatus.Unused)
                {
                    this.atlSlot.sprite = this.enemyUnuseAtlSlot;
                }
                else if (aTLCurrentStatus == ATLCurrentStatus.Using)
                {
                    this.atlSlot.sprite = this.enemyUsingAtlSlot;
                }
                else if (aTLCurrentStatus == ATLCurrentStatus.Used)
                {
                    this.atlSlot.sprite = this.enemyUsedAtlSlot;
                }

                this.startingPointX = this.EnemyStartPoint.position.x;
            }
        }

        if (this.battleFlowATL_V2 != null)
        {
            int _atlNumber = this.battleFlowATL_V2.GetATLNumber();
            bool _isPlayer = ( _atlNumber % 2 == 1 );

            if (aTLCurrentStatus == ATLCurrentStatus.Unused)
            {
                this.atlSlot.sprite = ( _isPlayer ) ? this.playerUnuseAtlSlot : this.enemyUnuseAtlSlot;
            }
            else if (aTLCurrentStatus == ATLCurrentStatus.Using)
            {
                this.atlSlot.sprite = ( _isPlayer ) ? this.playerUsingAtlSlot : this.enemyUsingAtlSlot;
            }
            else if (aTLCurrentStatus == ATLCurrentStatus.Used)
            {
                this.atlSlot.sprite = ( _isPlayer ) ? this.playerUsedAtlSlot : this.enemyUsedAtlSlot;
            }

            this.startingPointX = this.PlayerStartPoint.position.x;
        }

        base.gameObject.SetActive(true);
    }

    public void Hide()
    {
        base.gameObject.SetActive(false);
    }

    public float GetStartingPointX()
    {
        return this.startingPointX;
    }

    public float GetMiddlePointX()
    {
        return this.MiddlePoint.position.x;
    }
}
