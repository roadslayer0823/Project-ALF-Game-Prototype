using UnityEngine;

public class DebugMenuPanel : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private EnemyCharacter enemyCharacter = null;

    [Header( "References" )]
    [SerializeField] private GameObject container = null;

    private const string AUDIO_ID_CLICK = "click";

    public void Show()
    {
        this.container.SetActive( true );
    }

    public void Hide()
    {
        this.container.SetActive( false );
    }

    public void ClickToShow()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        Show();
    }

    public void ClickToHide()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        Hide();
    }
}
