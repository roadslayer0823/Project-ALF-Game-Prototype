using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    void Awake()
    {
        DatabaseManager.Instance.onAllDataLoadedCallback = GoToBattleScene;
    }

    private void GoToBattleScene()
    {
        SceneManager.LoadScene( "BattleScene" );
    }
}
