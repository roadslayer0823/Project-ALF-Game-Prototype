using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    [Header( "Managers" )]
    [SerializeField] private BattleUiManager battleUiManager = null;
    [SerializeField] private BattleFlowManager battleFlowManager = null;

    [Header( "Data" )]
    [SerializeField] private SkillDatabase skillDatabase = null;
    [SerializeField] private CharacterDatabase characterDatabase = null;

    private List<PlayerCharacter> playerCharacterList = null;
    private List<EnemyCharacter> enemyCharacterList = null;

    void Start()
    {
        this.battleUiManager.Initialize( this );
        this.battleFlowManager.Initialize( this, OnPreparationPhaseStarted );

        // -------------------- Set up the player's characters --------------------

        this.playerCharacterList = new List<PlayerCharacter>();

        GameObject _playerCharacterObj = new GameObject();
        PlayerCharacter _playerCharacter = _playerCharacterObj.AddComponent<PlayerCharacter>();
        _playerCharacter.Initialize( this.characterDatabase.GetPlayerCharacterDataById( 1 ), this.skillDatabase );

        this.playerCharacterList.Add( _playerCharacter );

        // ------------------------------------------------------------------------

        // -------------------- Set up the enemy's characters --------------------

        this.enemyCharacterList = new List<EnemyCharacter>();

        GameObject _enemyCharacterObj = new GameObject();
        EnemyCharacter _enemyCharacter = _enemyCharacterObj.AddComponent<EnemyCharacter>();
        _enemyCharacter.Initialize( this.characterDatabase.GetPlayerCharacterDataById( 1 ), this.skillDatabase );

        this.enemyCharacterList.Add( _enemyCharacter );

        // -----------------------------------------------------------------------

        this.battleFlowManager.StartGame();
    }

    private void OnPreparationPhaseStarted()
    {
        this.battleUiManager.SetSelectedGameCharacter( this.playerCharacterList[ 0 ] );
        this.battleUiManager.ShowSkillSelectionPanel();
    }

    public List<PlayerCharacter> GetPlayerCharacterList()
    {
        return this.playerCharacterList;
    }

    public List<EnemyCharacter> GetEnemyCharacterList()
    {
        return this.enemyCharacterList;
    }

    public SkillDatabase GetSkillDatabase()
    {
        return this.skillDatabase;
    }

    public CharacterDatabase GetCharacterDatabase()
    {
        return this.characterDatabase;
    }
}
