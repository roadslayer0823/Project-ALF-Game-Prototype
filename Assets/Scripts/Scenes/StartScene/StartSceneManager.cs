using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    void Start()
    {
        GoToAdminPage();
    }

    private void GoToAdminPage()
    {
        SceneManager.LoadScene("OptionPage");
    }
}
