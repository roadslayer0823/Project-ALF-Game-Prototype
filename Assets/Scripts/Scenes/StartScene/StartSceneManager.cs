using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    void Start()
    {
        SceneControlManager.GoToOptionPage();
    }
}
