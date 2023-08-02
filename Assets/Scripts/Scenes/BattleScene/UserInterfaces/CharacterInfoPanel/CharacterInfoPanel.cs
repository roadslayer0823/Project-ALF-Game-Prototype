using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoPanel : MonoBehaviour
{
    private GameCharacter selectedCharacter = null;

    public void SetSelectedCharacter( GameCharacter selectedCharacter )
    {
        this.selectedCharacter = selectedCharacter;
        this.selectedCharacter.SetOnCharacterInfoUpdated( UpdateDisplayInfo );
    }

    public void UpdateDisplayInfo()
    {
    }
}
