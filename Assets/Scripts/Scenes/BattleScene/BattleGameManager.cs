using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    [SerializeField] private BattleUiManager battleUiManager = null;
    [SerializeField] private SkillDatabase skillDatabase = null;
    [SerializeField] private CharacterDatabase characterDatabase = null;

    private List<PlayerCharacter> playerCharacterList = null;
    private List<EnemyCharacter> enemyCharacterList = null;

    void Start()
    {
        battleUiManager.Initialize( this );

        GameObject playerCharacterObj = new GameObject();
        PlayerCharacter playerCharacter = playerCharacterObj.AddComponent<PlayerCharacter>();
        playerCharacter.Initialize( characterDatabase.GetPlayerCharacterDataById( 1 ), this.skillDatabase );

        battleUiManager.ShowSkillSelectionPanel( playerCharacter );
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
