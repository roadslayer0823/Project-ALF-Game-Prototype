using UnityEngine;
using TMPro;

public class GameCharacterInfoBox : MonoBehaviour
{
    [SerializeField] private GameCharacter selectedCharacter = null;
    [SerializeField] private SpriteRenderer healthPointFiller = null;
    [SerializeField] private TextMeshPro healthPointLabel = null;

    void Awake()
    {
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
    }

    public void UpdateDisplayInfo()
    {
        float _remainingHealthPoint = this.selectedCharacter.GetRemainingHealthPoint();
        float _maximumHealthPoint = this.selectedCharacter.GetMaximumHealthPoint();

        this.healthPointLabel.text = _remainingHealthPoint + " / " + _maximumHealthPoint;

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
