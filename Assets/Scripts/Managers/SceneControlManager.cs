using UnityEngine.SceneManagement;

public class SceneControlManager
{
    public static void GoToOptionPage()
    {
        SceneManager.LoadScene( "OptionPage" );
    }

    public static void GoToAdminPage()
    {
        SceneManager.LoadScene( "AdminPage" );
    }

    public static void GoToBattleScene()
    {
        if (SceneUtility.GetBuildIndexByScenePath( "BattleSceneV3" ) != -1)
        {
            SceneManager.LoadScene( "BattleSceneV3" );
        }
        else if (SceneUtility.GetBuildIndexByScenePath( "BattleSceneV2" ) != -1)
        {
            SceneManager.LoadScene( "BattleSceneV2" );
        }
        else
        {
            SceneManager.LoadScene( "BattleScene" );
        }
    }

    public static void RestartCurrentScene()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}
