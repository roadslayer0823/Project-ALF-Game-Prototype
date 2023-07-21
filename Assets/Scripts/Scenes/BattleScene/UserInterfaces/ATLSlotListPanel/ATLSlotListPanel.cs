using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATLSlotListPanel : MonoBehaviour
{
    [SerializeField] private ATLSlot[] theATLSlots = new ATLSlot[ 0 ];

    public void Show( BattleFlowATL[] flowATLs )
    {
        for (int i = 0; i < theATLSlots.Length; i++)
        {
            ATLSlot _altSlot = theATLSlots[ i ];

            if (i < flowATLs.Length)
            {
                _altSlot.Show( flowATLs[ i ] );
            }
            else
            {
                _altSlot.Hide();
            }
        }

        base.gameObject.SetActive( true );
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }
}
