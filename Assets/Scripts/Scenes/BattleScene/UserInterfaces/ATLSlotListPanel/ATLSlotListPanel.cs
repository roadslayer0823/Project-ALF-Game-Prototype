using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATLSlotListPanel : MonoBehaviour
{
    [SerializeField] private ATLSlot[] theATLSlots = new ATLSlot[ 0 ];

    private BattleFlowATL[] battleFlowATLs;

    public void Show( BattleFlowATL[] flowATLs, Action onSkillSlotSwipedCallback, Action onATLSlotExecutedCallback)
    {
        for (int i = 0; i < theATLSlots.Length; i++)
        {
            ATLSlot _altSlot = theATLSlots[ i ];

            if (i < flowATLs.Length)
            {
                flowATLs[i].SetATLSlot(_altSlot);
                flowATLs[i].SetIsATLSlotExecuted(false);
                _altSlot.Initialize(onSkillSlotSwipedCallback, onATLSlotExecutedCallback);
                _altSlot.Show(flowATLs[i]);
            }
            else
            {
                _altSlot.Hide();
            }
        }

        this.battleFlowATLs = flowATLs;
        OnSkillSlotUpdated();

        base.gameObject.SetActive( true );
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }

    public void OnSkillSlotUpdated()
    {
        int playerSkillCounter = 0;

        foreach (BattleFlowATL flowATL in this.battleFlowATLs)
        {
            if (flowATL.GetIsATLSlotExecuted())
            {
                continue;
            }

            //Check and assign the character selected skill into correct atl slot
            if (flowATL.CheckIsPlayer() && playerSkillCounter < flowATL.GetSelectedCharacter().GetSelectedActiveSkillList().Count)
            {
                flowATL.SetSelectedSkill(flowATL.GetSelectedCharacter().GetSelectedActiveSkillList()[playerSkillCounter]);
                playerSkillCounter++;
            }

            flowATL.UpdateATLSlotInfo();
        }
    }
}
