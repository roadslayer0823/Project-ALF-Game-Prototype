using UnityEngine;

public class GameCharacterInfoBox : MonoBehaviour
{
    [SerializeField] private GameCharacter selectedCharacter = null;
    [SerializeField] private SpriteRenderer filler = null;

    void Awake()
    {
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
    }

    public void UpdateDisplayInfo()
    {
        float _percentage = this.selectedCharacter.GetRemainingHealthPoint() / this.selectedCharacter.GetMaximumHealthPoint();
        this.filler.size = new Vector2( _percentage * 10.0f, this.filler.size.y );
    }
}
