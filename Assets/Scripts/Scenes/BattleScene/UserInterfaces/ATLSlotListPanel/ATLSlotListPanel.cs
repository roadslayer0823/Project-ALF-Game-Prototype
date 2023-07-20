using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATLSlotListPanel : MonoBehaviour
{
    [SerializeField] private ATLSlot[] aTLSlots = new ATLSlot[ 0 ];

    public void Show()
    {
        this.gameObject.SetActive( true );
    }

    public void Hide()
    {
        this.gameObject.SetActive( false );
    }
}
