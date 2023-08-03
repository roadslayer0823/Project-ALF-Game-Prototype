using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject container = null;
    [SerializeField] private TMP_Text titleLabel = null;

    public void ShowVictory()
    {
        this.titleLabel.text= "恭喜\n你贏了";
        this.container.SetActive( true );
    }

    public void ShowDefeat()
    {
        this.titleLabel.text = "闖關失敗";
        this.container.SetActive( true );
    }

    public void ClickToRestartBattle()
    {
        SceneManager.LoadScene( "BattleScene" );
    }
}
