using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene( "BattleScene" );
    }
}
