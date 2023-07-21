using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATLSlot : MonoBehaviour
{
    public void Initialize()
    {
    }

    public void Show( BattleFlowATL flowATL )
    {
        base.gameObject.SetActive( true );
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }
}
