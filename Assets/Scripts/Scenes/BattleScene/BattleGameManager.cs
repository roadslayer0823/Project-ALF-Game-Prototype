using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    [SerializeField] private BattleUiManager battleUiManager;

    private List<PlayerCharacter> playerCharacterList = null;
    private List<EnemyCharacter> enemyCharacterList = null;
}
