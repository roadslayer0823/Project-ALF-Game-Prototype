using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DatabaseManager : Singleton<DatabaseManager>
{
    [Header("Google Spreadsheet")]
    [SerializeField] private string databaseSpreadsheetId = "";
    [SerializeField] private string characterSheetName = "";
    [SerializeField] private string skillSheetName = "";
    [SerializeField] private string subskillSheetName = "";

    private List<Character> characterList = new List<Character>();
    private List<Skill> skillList = new List<Skill>();
    private List<Subskill> subskillList = new List<Subskill>();

    public Action onDataUpdatedCallback = null;

    void Start()
    {
        LoadAllData();
    }

    private void LoadAllData()
    {
        StartCoroutine(GetJsonData<Character>(this.characterSheetName));
        StartCoroutine(GetJsonData<Skill>(this.skillSheetName));
        StartCoroutine(GetJsonData<Subskill>(this.subskillSheetName));
    }

    IEnumerator GetJsonData<T>(string sheetName) where T : class
    {
        string jsonURL = "https://opensheet.elk.sh/" + databaseSpreadsheetId + "/" + sheetName;

        while ( true )
        {
            Debug.Log("Processing Data, Please Wait");

            //Download the data from json
            UnityWebRequest webRequest = UnityWebRequest.Get(jsonURL);

            //wait for it to download finish
            yield return webRequest.SendWebRequest();

            //Check to make sure no error, then pass the loaded data to other function for using
            if (string.IsNullOrEmpty(webRequest.error))
            {
                ProcessJsonData<T>(webRequest.downloadHandler.text, sheetName);

                if (this.onDataUpdatedCallback != null)
                {
                    this.onDataUpdatedCallback();
                }

                Debug.Log("Done.");
            }
            else
            {
                Debug.Log("Oops something went wrong");
            }

            yield return null;
        }
    }

    private void ProcessJsonData<T>(string dataBytes, string sheetName) where T : class
    {
        List<T> dataList = JsonConvert.DeserializeObject<List<T>>(dataBytes);

        //Check and assign the value into the respective list
        if (sheetName == this.characterSheetName)
        {
            this.characterList = dataList as List<Character>;

            foreach (Character character in this.characterList)
            {
                character.SkillIdArray = ConvertStringToIntArray(character.SkillIdArrayString);
            }
        }
        else if (sheetName == this.skillSheetName)
        {
            this.skillList = dataList as List<Skill>;

            foreach (Skill skill in this.skillList)
            {
                skill.skillType = (Skill.SkillType)Enum.Parse(typeof(Skill.SkillType), skill.SkillTypeString);
            }
        }
        else if (sheetName == this.subskillSheetName)
        {
            this.subskillList = dataList as List<Subskill>;

            foreach (Subskill subskill in this.subskillList)
            {
                subskill.effectArea = (Subskill.EffectArea)Enum.Parse(typeof(Subskill.EffectArea), subskill.EffectAreaString);
                subskill.effectType = (Subskill.EffectType)Enum.Parse(typeof(Subskill.EffectType), subskill.EffectTypeString);
                subskill.IsAttackingSkill = bool.Parse(subskill.IsAttackingSkillString);
                subskill.IsInterceptable = bool.Parse(subskill.IsInterceptableString);
                subskill.Effects = ConvertStringToIntArray(subskill.EffectsString);
            }
        }
    }

    private int[] ConvertStringToIntArray(string arrayString)
    {
        string removeLeftSquareBracket = arrayString.Replace("[", "");
        arrayString = removeLeftSquareBracket;

        string removeRightSquareBracket = arrayString.Replace("]", "");
        arrayString = removeRightSquareBracket;

        if (arrayString != "")
        {
            int[] nums = Array.ConvertAll(arrayString.Split(','), int.Parse);
            return nums;
        }
        else
        {
            return null;
        }
    }

    public List<Character> GetCharacterList()
    {
        return this.characterList;
    }

    public List<Skill> GetSkillList()
    {
        return this.skillList;
    }

    public List<Subskill> GetSubskillList()
    {
        return this.subskillList;
    }


#region Inner Classes
    [System.Serializable]
    public class Character
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("maximum_health_point")]
        public int MaximumHealthPoint;

        [JsonProperty("maximum_action_point")]
        public int MaximumActionPoint;

        [JsonProperty("skill_id_array")]
        [HideInInspector] public string SkillIdArrayString;
        public int[] SkillIdArray;
    }

    [System.Serializable]
    public class Skill
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("skill_type")]
        [HideInInspector] public string SkillTypeString;
        public enum SkillType
        {
            active,
            backend,
            derived
        }
        public SkillType skillType;
    }

    [System.Serializable]
    public class Subskill
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("skill_id")]
        public int SkillId;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("derived_skill_id")]
        public int DerivedSkillId;

        [JsonProperty("action_point_cost")]
        public int ActionPointCost;

        [JsonProperty("attack_damage")]
        public int ActionDamage;

        [JsonProperty("defense")]
        public int Defense;

        [JsonProperty("strength")]
        public int Strength;

        [JsonProperty("effect_area")]
        [HideInInspector] public string EffectAreaString;
        public enum EffectArea
        {
            none,
            target_one,
            target_all
        }
        public EffectArea effectArea;

        [JsonProperty("effect_type")]
        [HideInInspector] public string EffectTypeString;
        public enum EffectType
        {
            none,
            basic,
            wide
        }
        public EffectType effectType;

        [JsonProperty("is_attacking_skill")]
        [HideInInspector] public string IsAttackingSkillString;
        public bool IsAttackingSkill;

        [JsonProperty("is_interceptable")]
        [HideInInspector] public string IsInterceptableString;
        public bool IsInterceptable;

        [JsonProperty("stress_resistance")]
        public int StressResistance;

        [JsonProperty("effects")]
        [HideInInspector] public string EffectsString;
        public int[] Effects;
    }

#endregion
}