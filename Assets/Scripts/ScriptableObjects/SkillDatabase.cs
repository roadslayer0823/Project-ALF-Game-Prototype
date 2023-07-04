using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SkillDatabase", menuName = "ScriptableObjects/SkillDatabase", order = 1 )]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private SkillData[] skillDataList = new SkillData[ 0 ];

    [System.Serializable]
    public class SkillData : ISerializationCallbackReceiver
    {
        [HideInInspector][SerializeField] private string elementName = "";
        [SerializeField] private int id = 0;
        [SerializeField] private string skillName = "";
        [SerializeField] private SkillType skillType = SkillType.None;

        public enum SkillType
        {
            None,
            Active,
            Backend
        }

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
            elementName = "ID: " + id.ToString();
        }
    }
}
