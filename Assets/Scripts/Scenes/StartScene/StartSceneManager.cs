using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    private void Start()
    {
        GoToAdminPage();
    }

    private void GoToAdminPage()
    {
        SceneManager.LoadScene("AdminPage");
    }
}
