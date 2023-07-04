using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowRound : MonoBehaviour
{
    private int roundNumber = 0;
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowATL[] flowATLs = null;

    public enum PhaseType
    {
        None,
        Preparation,
        Execution
    }

    public BattleFlowRound( int roundNumber )
    {
        this.roundNumber = roundNumber;
    }
}
