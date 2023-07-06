using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "CharacterDatabase", menuName = "ScriptableObjects/CharacterDatabase", order = 2 )]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private CharacterData[] playerCharacterDataArray = new CharacterData[ 0 ];
    [SerializeField] private CharacterData[] enemyCharacterDataArray = new CharacterData[ 0 ];

    public CharacterData GetPlayerCharacterDataById( int id )
    {
        return GetCharacterDataById( playerCharacterDataArray, id );
    }

    public CharacterData GetEnemyCharacterDataById( int id )
    {
        return GetCharacterDataById( enemyCharacterDataArray, id );
    }

    private CharacterData GetCharacterDataById( CharacterData[] characterDataArray, int id )
    {
        for (int i = 0; i < characterDataArray.Length; i++)
        {
            CharacterData characterData = characterDataArray[ i ];
            if (characterData.GetId() == id)
            {
                return characterData;
            }
        }

        return null;
    }

    [System.Serializable]
    public class CharacterData : ISerializationCallbackReceiver
    {
        [HideInInspector][SerializeField] private string elementName = "";
        [SerializeField] private int id = 0;
        [SerializeField] private string characterName = "";
        [SerializeField] private float maximumHealthPoint = 0.0f;
        [SerializeField] private float maximumActionPoint = 0.0f;
        [SerializeField] private int[] skillIdArray = new int[ 0 ];

        public void OnBeforeSerialize()
        {
            UpdateElementName();
        }

        public void OnAfterDeserialize()
        {
            UpdateElementName();
        }

        private void UpdateElementName()
        {
            this.elementName = "ID: " + this.id.ToString();
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetCharacterName()
        {
            return this.characterName;
        }

        public float GetMaximumHealthPoint()
        {
            return this.maximumHealthPoint;
        }

        public float GetMaximumActionPoint()
        {
            return this.maximumActionPoint;
        }

        public int[] GetSkillIdArray()
        {
            return this.skillIdArray;
        }
    }
}
