using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    [SerializeField] private BattleUiManager battleUiManager;
    [SerializeField] private SkillDatabase skillDatabase;

    private List<PlayerCharacter> playerCharacterList = null;
    private List<EnemyCharacter> enemyCharacterList = null;

    void Start()
    {
        battleUiManager.Initialize( this );
    }

    public SkillDatabase GetSkillDatabase()
    {
        return this.skillDatabase;
    }
}
