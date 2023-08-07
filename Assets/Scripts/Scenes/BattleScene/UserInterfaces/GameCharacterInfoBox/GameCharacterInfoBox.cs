using UnityEngine;
using TMPro;

public class GameCharacterInfoBox : MonoBehaviour
{
    [SerializeField] private GameCharacter selectedCharacter = null;
    [SerializeField] private SpriteRenderer healthPointFiller = null;
    [SerializeField] private TextMeshPro healthPointLabel = null;

    void Awake()
    {
        this.selectedCharacter.SetGameCharacterInfoBox( this );
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
    }

    public void UpdateDisplayInfo()
    {
        float _remainingHealthPoint = this.selectedCharacter.GetRemainingHealthPoint();
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();

        if (_remainingHealthPoint < 0)
        {
            _remainingHealthPoint = 0;
        }

        this.healthPointLabel.text = Mathf.CeilToInt( _remainingHealthPoint ) + " / " + Mathf.CeilToInt( _maximumHealthPoint );

        float _percentage = _remainingHealthPoint / _maximumHealthPoint;
        if (_percentage > 0)
        {
            this.healthPointFiller.size = new Vector2( _percentage * 10.0f, this.healthPointFiller.size.y );
        }
        else
        {
            this.gameObject.SetActive( false );
        }
    }
}
