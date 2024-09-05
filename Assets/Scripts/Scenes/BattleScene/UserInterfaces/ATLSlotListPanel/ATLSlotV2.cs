using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ATLSlotV2 : MonoBehaviour
{
    [SerializeField] private Image atlSlot;
    [SerializeField] private Image atlSlotAnimation;

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
    [SerializeField] private RectTransform EndingPoint;

    [Header("")]
    private BattleFlowATL battleFlowATL = null;
    private BattleFlowATL_V2 battleFlowATL_V2 = null;

    public Action onSkillSlotSwipedCallback = null;
    public Action onATLSlotExecutedCallback = null;

    private float startingPointX = 0.0f;
    private int atlNumberForPlayer = 0;

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

    public void DefaultATLSetup( BattleFlowATL_V2 flowATL, int atlNumberForPlayer )
    {
        this.battleFlowATL_V2 = flowATL;
        this.atlNumberForPlayer = atlNumberForPlayer;

        Show( ATLCurrentStatus.Unused );
        this.atlSlot.SetNativeSize();
    }

    public void Show(ATLCurrentStatus aTLCurrentStatus)
    {
        if (this.battleFlowATL != null)
        {
            if (this.battleFlowATL.CheckIsPlayer())
            {
                currentATlSlotStatus(aTLCurrentStatus, this.playerUnuseAtlSlot, this.playerUsingAtlSlot, this.playerUsedAtlSlot, this.PlayerStartPoint.position.x);
            }
            else
            {
                currentATlSlotStatus(aTLCurrentStatus, this.enemyUnuseAtlSlot, this.enemyUsingAtlSlot, this.enemyUsedAtlSlot, this.PlayerStartPoint.position.x);
            }
        }
        if (this.battleFlowATL_V2 != null)
        {
            int _atlNumber = this.battleFlowATL_V2.GetATLNumber();
            bool _isPlayer = ( _atlNumber % 2 == this.atlNumberForPlayer );
            float atlSlotAlphaValue = this.atlSlotAnimation.color.a;

            switch ( aTLCurrentStatus )
            {
                case ATLCurrentStatus.Unused:

                    this.atlSlot.sprite = ( _isPlayer ) ? this.playerUnuseAtlSlot : this.enemyUnuseAtlSlot;

                    break;

                case ATLCurrentStatus.Using:

                    this.atlSlotAnimation.sprite = ( _isPlayer ) ? this.playerUsingAtlSlot : this.enemyUsingAtlSlot;

                    LeanTween.alpha( this.atlSlotAnimation.rectTransform, 1f, 0.5f )
                        .setOnComplete( () =>
                        {
                            LeanTween.alpha( this.atlSlotAnimation.rectTransform, 0, 0.5f );
                            this.atlSlot.sprite = ( _isPlayer ) ? this.playerUsedAtlSlot : this.enemyUsedAtlSlot;
                        } );

                    break;

                case ATLCurrentStatus.Used:

                    this.atlSlot.sprite = ( _isPlayer ) ? this.playerUsedAtlSlot : this.enemyUsedAtlSlot;

                    break;
            }

            this.startingPointX = this.PlayerStartPoint.position.x;
        }

        base.gameObject.SetActive(true);
    }

    public void currentATlSlotStatus(ATLCurrentStatus aTLCurrentStatus, Sprite unuseSlot, Sprite usingSlot, Sprite usedSlot, float startingPointX)
    {
        this.atlSlot.sprite = aTLCurrentStatus switch
        {
            ATLCurrentStatus.Unused => unuseSlot,
            ATLCurrentStatus.Using => usingSlot,
            ATLCurrentStatus.Used => usedSlot,
            _ => throw new NotImplementedException()
        };
        this.startingPointX = startingPointX;
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

    public float GetEndingPointX()
    {
        return this.EndingPoint.position.x;
    }
}
