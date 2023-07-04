using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowManager : MonoBehaviour
{
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowRound currentRound = null;

    public enum PhaseType
    {
        None,
        GameStarted,
        GameEnded
    }
}
