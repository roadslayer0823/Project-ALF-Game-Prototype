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
        this.gameObject.SetActive( true );
    }

    public void Hide()
    {
        this.gameObject.SetActive( false );
    }
}
