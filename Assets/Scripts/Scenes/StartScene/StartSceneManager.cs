using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : Singleton<StartSceneManager>
{
    private void Start()
    {
        DatabaseManager.Instance.onAllDataLoadedCallback = GoToAdminPage;
    }

    public void GoToBattleScene()
    {
        SceneManager.LoadScene( "BattleScene" );
    }

    private void GoToAdminPage()
    {
        SceneManager.LoadScene("AdminPage");
    }
}
