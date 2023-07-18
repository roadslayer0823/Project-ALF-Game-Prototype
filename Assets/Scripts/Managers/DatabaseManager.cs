using UnityEngine;
using Newtonsoft.Json;

public class DatabaseManager : Singleton<DatabaseManager>
{
    [Header( "Google Spreadsheet" )]
    [SerializeField] private string databaseSpreadsheetId = "";
    [SerializeField] private string characterSheetName = "";
    [SerializeField] private string skillSheetName = "";
    [SerializeField] private string subskillSheetName = "";

    void Start()
    {
    }

#region Inner Classes

    [System.Serializable]
    public class Character
    {
        [JsonProperty( "id" )]
        public int ID { get; set; }

        [JsonProperty( "display_name" )]
        public string DisplayName { get; set; }

        [JsonProperty( "maximum_health_point" )]
        public int MaximumHealthPoint { get; set; }

        [JsonProperty( "maximum_action_point" )]
        public int MaximumActionPoint { get; set; }

        [JsonProperty( "skill_id_array" )]
        public int[] SkillIdArray { get; set; }
    }

    [System.Serializable]
    public class Skill
    {
    }

    [System.Serializable]
    public class Subskill
    {
    }

#endregion
}
