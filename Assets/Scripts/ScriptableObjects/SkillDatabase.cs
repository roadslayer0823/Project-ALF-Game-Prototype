using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SkillDatabase", menuName = "ScriptableObjects/SkillDatabase", order = 1 )]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private SkillData[] skillDataArray = new SkillData[ 0 ];

    public SkillData GetSkillDataById( int id )
    {
        for (int i = 0; i < skillDataArray.Length; i++)
        {
            SkillData skillData = skillDataArray[ i ];
            if (skillData.GetId() == id)
            {
                return skillData;
            }
        }

        return null;
    }

    [System.Serializable]
    public class SkillData : ISerializationCallbackReceiver
    {
        [HideInInspector][SerializeField] private string elementName = "";
        [SerializeField] private int id = 0;
        [SerializeField] private string skillName = "";
        [SerializeField] private SkillType skillType = SkillType.None;
        [SerializeField] private int actionPointCost = 0;

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
            this.elementName = "ID: " + this.id.ToString();
        }

        public int GetId()
        {
            return this.id;
        }

        public string GetSkillName()
        {
            return this.skillName;
        }

        public SkillType GetSkillType()
        {
            return this.skillType;
        }

        public int GetActionPointCost()
        {
            return this.actionPointCost;
        }
    }
}
