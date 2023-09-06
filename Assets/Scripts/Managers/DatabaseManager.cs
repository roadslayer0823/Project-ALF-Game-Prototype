using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DatabaseManager : Singleton<DatabaseManager>
{
    [Header( "Settings" )]
    [SerializeField] private bool isCheckingForUpdatesEnabled = false;

    [Header("Google Spreadsheet")]
    [SerializeField] private string databaseSpreadsheetId = "";
    [SerializeField] private string versionSheetName = "";
    [SerializeField] private string configurationSheetName = "";
    [SerializeField] private string characterSheetName = "";
    [SerializeField] private string skillSheetName = "";
    [SerializeField] private string subskillSheetName = "";
    [SerializeField] private string skillAnimationSheetName = "";

    private List<Version> versionList = new List<Version>();
    private List<Configuration> configurationList = new List<Configuration>();
    private List<Character> characterList = new List<Character>();
    private List<Skill> skillList = new List<Skill>();
    private List<Subskill> subskillList = new List<Subskill>();
    private List<SkillAnimation> skillAnimationList = new List<SkillAnimation>();

    public Action onDataUpdatedCallback = null;
    public Action onAllDataLoadedCallback = null;

    void Start()
    {
        if (this.isCheckingForUpdatesEnabled)
        {
            LoadAllData();
        }
        else
        {
            string _versionJsonData = PlayerPrefsManager.LoadVersionDatabase();
            if (!string.IsNullOrEmpty(_versionJsonData))
            {
                StartCoroutine(CheckDatabaseVersion(_versionJsonData));
            }

            string _configurationJsonData = PlayerPrefsManager.LoadConfigurationDatabase();
            string _characterJsonData = PlayerPrefsManager.LoadCharacterDatabase();
            string _skillJsonData = PlayerPrefsManager.LoadSkillDatabase();
            string _subskillJsonData = PlayerPrefsManager.LoadSubskillDatabase();
            string _skillAnimationJsonData = PlayerPrefsManager.LoadSkillAnimationDatabase();

            if (!string.IsNullOrEmpty( _versionJsonData )
                && !string.IsNullOrEmpty( _configurationJsonData )
                && !string.IsNullOrEmpty( _characterJsonData )
                && !string.IsNullOrEmpty( _skillJsonData )
                && !string.IsNullOrEmpty( _subskillJsonData )
                && !string.IsNullOrEmpty( _skillAnimationJsonData ) )
            {
                ProcessJsonData<Configuration>(_configurationJsonData, this.configurationSheetName);
                ProcessJsonData<Character>( _characterJsonData, this.characterSheetName );
                ProcessJsonData<Skill>( _skillJsonData, this.skillSheetName );
                ProcessJsonData<Subskill>( _subskillJsonData, this.subskillSheetName );
                ProcessJsonData<SkillAnimation>(_skillAnimationJsonData, this.skillAnimationSheetName);

                this.onAllDataLoadedCallback?.Invoke();
            }
            else
            {
                LoadAllData();
            }
        }
    }

    private IEnumerator CheckDatabaseVersion(string versionJsonData)
    {
        // Get latest database version list
        yield return StartCoroutine(GetJsonData<Version>(this.versionSheetName));

        Debug.Log("Previous version json data: " + versionJsonData);

        if (this.versionList.Count != 0)
        {
            Debug.Log("Check Database Version.");

            List<Version> latestDatabaseVersionList = this.versionList;
            List<Version> previousDatabaseVersionList = JsonConvert.DeserializeObject<List<Version>>(versionJsonData);

            foreach (Version latestDatabaseVersion in latestDatabaseVersionList)
            {
                foreach (Version previousDatabaseVersion in previousDatabaseVersionList)
                {
                    // Compare the loaded previous database version number with latest database version number
                    if (previousDatabaseVersion.SheetName == latestDatabaseVersion.SheetName
                        && previousDatabaseVersion.VersionNumber < latestDatabaseVersion.VersionNumber)
                    {
                        if (latestDatabaseVersion.SheetName == this.configurationSheetName)
                        {
                            StartCoroutine(GetJsonData<Configuration>(this.configurationSheetName));
                            Debug.Log("Update: " + this.configurationSheetName);
                        }
                        else if (latestDatabaseVersion.SheetName == this.characterSheetName)
                        {
                            StartCoroutine(GetJsonData<Character>(this.characterSheetName));
                            Debug.Log("Update: " + this.characterSheetName);
                        }
                        else if (latestDatabaseVersion.SheetName == this.skillSheetName)
                        {
                            StartCoroutine(GetJsonData<Skill>(this.skillSheetName));
                            Debug.Log("Update: " + this.skillSheetName);
                        }
                        else if (latestDatabaseVersion.SheetName == this.subskillSheetName)
                        {
                            StartCoroutine(GetJsonData<Subskill>(this.subskillSheetName));
                            Debug.Log("Update: " + this.subskillSheetName);
                        }
                        else if (latestDatabaseVersion.SheetName == this.skillAnimationSheetName)
                        {
                            StartCoroutine(GetJsonData<SkillAnimation>(this.skillAnimationSheetName));
                            Debug.Log("Update: " + this.skillAnimationSheetName);
                        }
                    }
                }
            }
        }
    }

    public void LoadAllData()
    {
        StartCoroutine(GetJsonData<Version>(this.versionSheetName));
        StartCoroutine(GetJsonData<Configuration>(this.configurationSheetName));
        StartCoroutine(GetJsonData<Character>(this.characterSheetName));
        StartCoroutine(GetJsonData<Skill>(this.skillSheetName));
        StartCoroutine(GetJsonData<Subskill>(this.subskillSheetName));
        StartCoroutine(GetJsonData<SkillAnimation>(this.skillAnimationSheetName));
    }

    private IEnumerator GetJsonData<T>( string sheetName ) where T : class
    {
        string jsonURL = "https://opensheet.elk.sh/" + this.databaseSpreadsheetId + "/" + sheetName;

        Debug.Log( "Processing Data, Please Wait" );

        //Download the data from json
        UnityWebRequest webRequest = UnityWebRequest.Get( jsonURL );

        //wait for it to download finish
        yield return webRequest.SendWebRequest();

        //Check to make sure no error, then pass the loaded data to other function for using
        if (string.IsNullOrEmpty( webRequest.error ))
        {
            ProcessJsonData<T>( webRequest.downloadHandler.text, sheetName );

            if (this.onDataUpdatedCallback != null)
            {
                this.onDataUpdatedCallback();
            }

            Debug.Log( "Done." );
        }
        else
        {
            Debug.Log( "Oops something went wrong" );
        }

        if (this.versionList != null
            && this.configurationList != null
            && this.characterList != null
            && this.skillList != null
            && this.subskillList != null
            && this.skillAnimationList != null)
        {
            onAllDataLoadedCallback?.Invoke();
        }
    }

    private void ProcessJsonData<T>(string jsonData, string sheetName) where T : class
    {
        List<T> dataList = JsonConvert.DeserializeObject<List<T>>( jsonData );

        //Check and assign the value into the respective list
        if (sheetName == this.versionSheetName)
        {
            Debug.Log("Latest json version data: " + jsonData);

            this.versionList = dataList as List<Version>;

            PlayerPrefsManager.SaveVersionDatabase(jsonData);
        }
        else if (sheetName == this.configurationSheetName)
        {
            this.configurationList = dataList as List<Configuration>;

            for (int i = 0; i < this.configurationList.Count; i++)
            {
                Configuration configuration = this.configurationList[i];

                configuration.categoryType = (Configuration.Category)Enum.Parse(typeof(Configuration.Category), configuration.CategoryString);
            }

            GameConfiguration.Instance.SetBattleConfiguration( new GameConfiguration.Battle( this.configurationList ) );

            PlayerPrefsManager.SaveConfigurationDatabase(jsonData);
        }
        else if (sheetName == this.characterSheetName)
        {
            this.characterList = dataList as List<Character>;

            foreach (Character character in this.characterList)
            {
                character.SkillIdArray = ConvertStringToStringArray(character.SkillIdArrayString);
            }

            PlayerPrefsManager.SaveCharacterDatabase( jsonData );
        }
        else if (sheetName == this.skillSheetName)
        {
            this.skillList = dataList as List<Skill>;

            foreach (Skill skill in this.skillList)
            {
                skill.skillType = (Skill.SkillType)Enum.Parse(typeof(Skill.SkillType), skill.SkillTypeString);
            }

            PlayerPrefsManager.SaveSkillDatabase( jsonData );
        }
        else if (sheetName == this.subskillSheetName)
        {
            this.subskillList = dataList as List<Subskill>;

            foreach (Subskill subskill in this.subskillList)
            {
                subskill.Range = (Subskill.RangeType)Enum.Parse(typeof(Subskill.RangeType), subskill.RangeString);
                subskill.EffectArea = (Subskill.EffectAreaEnum)Enum.Parse(typeof(Subskill.EffectAreaEnum), subskill.EffectAreaString);
                subskill.EffectType = (Subskill.EffectTypeEnum)Enum.Parse(typeof(Subskill.EffectTypeEnum), subskill.EffectTypeString);
                subskill.IsAttackingSkill = bool.Parse(subskill.IsAttackingSkillString);
                subskill.IsDefendingSkill = bool.Parse(subskill.IsDefendingSkillString);
                subskill.IsEvadingSkill = bool.Parse(subskill.IsEvadingSkillString);
                subskill.IsInterceptable = bool.Parse(subskill.IsInterceptableString);
                subskill.Effects = ConvertStringToIntArray(subskill.EffectsString);
            }

            PlayerPrefsManager.SaveSubskillDatabase( jsonData );
        }
        else if (sheetName == this.skillAnimationSheetName)
        {
            this.skillAnimationList = dataList as List<SkillAnimation>;

            PlayerPrefsManager.SaveSkillAnimationDatabase(jsonData);
        }
    }

    private int[] ConvertStringToIntArray(string arrayString)
    {
        arrayString = RemoveSquareBracket(arrayString);

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

    private string[] ConvertStringToStringArray(string arrayString)
    {
        arrayString = RemoveSquareBracket(arrayString);

        if (arrayString != "")
        {
            string[] word = arrayString.Split(",");
            return word;
        }
        else
        {
            return null;
        }
    }

    // Remove [] from string
    private string RemoveSquareBracket(string arrayString)
    {
        string removeLeftSquareBracket = arrayString.Replace("[", "");
        arrayString = removeLeftSquareBracket;

        string removeRightSquareBracket = arrayString.Replace("]", "");
        arrayString = removeRightSquareBracket;

        return arrayString;
    }

    // Return correspond character data based on character id
    public Character GetCharacterDataById(string id)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            Character _characterData = characterList[i];
            if (_characterData.Id == id)
            {
                return _characterData;
            }
        }

        return null;
    }

    // Return correspond skill data based on skill id
    public Skill GetSkillDataById(string id)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            Skill _skillData = skillList[i];
            if (_skillData.Id == id)
            {
                return _skillData;
            }
        }

        return null;
    }

    // Return correspond skill animation data based on subskill id
    public SkillAnimation GetSkillAnimation(string subskillId)
    {
        for (int i = 0; i < this.skillAnimationList.Count; i++)
        {
            SkillAnimation _skillAnimation = this.skillAnimationList[i];
            if (_skillAnimation.SubskillId == subskillId)
            {
                return _skillAnimation;
            }
        }

        return null;
    }

    public List<Version> GetVersionList()
    {
        return this.versionList;
    }

    public List<Configuration> GetConfigurationList()
    {
        return this.configurationList;
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

    public List<SkillAnimation> GetSkillAnimationList()
    {
        return this.skillAnimationList;
    }

    // Inner classes declaration
#region Inner Classes
    [Serializable]
    public class Version
    {
        [JsonProperty("sheet_name")]
        public string SheetName { get; private set; }

        [JsonProperty("version")]
        public int VersionNumber { get; private set; }
    }

    [Serializable]
    public class Configuration
    {
        [JsonProperty("category")]
        public string CategoryString { get; private set; }
        public enum Category
        {
            battle
        }
        public Category categoryType;

        [JsonProperty("key")]
        public string Key { get; private set; }

        [JsonProperty("value")]
        public float Value { get; private set; }
    }

    [Serializable]
    public class Character
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        [JsonProperty("maximum_health_point")]
        public int MaximumHealthPoint { get; private set; }

        [JsonProperty("maximum_state_point")]
        public int MaximumStatePoint { get; private set; }

        [JsonProperty("maximum_stress_value")]
        public int MaximumStressValue { get; private set; }

        [JsonProperty("skill_id_array")]
        [HideInInspector] public string SkillIdArrayString { get; private set; }
        public string[] SkillIdArray;
    }

    [Serializable]
    public class Skill
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("group_name")]
        public string GroupName { get; private set; }

        [JsonProperty("skill_type")]
        [HideInInspector] public string SkillTypeString { get; private set; }
        public enum SkillType
        {
            active,
            backend,
            repulse,
            derived,
            counter
        }
        public SkillType skillType;
    }

    [Serializable]
    public class Subskill
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("reference")]
        public string Reference { get; private set; }

        [JsonProperty("skill_id")]
        public string SkillId { get; private set; }

        [JsonProperty("level")]
        public int Level { get; private set; }

        [JsonProperty("feature_id")]
        public int FeatureId { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        [JsonProperty("repulse_skill_id")]
        public string RepulseSkillId { get; private set; }

        [JsonProperty("derived_skill_id")]
        public string DerivedSkillId { get; private set; }

        [JsonProperty("counter_skill_id")]
        public string CounterSkillId { get; private set; }

        [JsonProperty("range")]
        [HideInInspector] public string RangeString { get; private set; }
        public enum RangeType
        {
            ranged,
            melee
        }
        public RangeType Range;

        [JsonProperty("attack_damage")]
        public int AttackDamage { get; private set; }

        [JsonProperty("max_state_point_up")]
        public int MaxStatePointUp { get; private set; }

        [JsonProperty("state_point_cost")]
        public int StatePointCost { get; private set; }

        [JsonProperty("stress_damage")]
        public int StressDamage { get; private set; }

        [JsonProperty("state_point_damage")]
        public int StatePointDamage { get; private set; }

        [JsonProperty("strength")]
        public int Strength { get; private set; }

        [JsonProperty("accuracy")]
        public int Accuracy { get; private set; }

        [JsonProperty("evasion")]
        public int Evasion { get; private set; }

        [JsonProperty("effect_area")]
        [HideInInspector] public string EffectAreaString { get; private set; }
        public enum EffectAreaEnum
        {
            none,
            target_one,
            target_all
        }
        public EffectAreaEnum EffectArea;

        [JsonProperty("effect_type")]
        [HideInInspector] public string EffectTypeString { get; private set; }
        public enum EffectTypeEnum
        {
            none = 0,
            basic = 1,
            wide = 2
        }
        public EffectTypeEnum EffectType;

        [JsonProperty("is_attacking_skill")]
        [HideInInspector] public string IsAttackingSkillString { get; private set; }
        public bool IsAttackingSkill;

        [JsonProperty("failed_repulse_damage_rate")]
        public float FailedRepulseDamageRate { get; private set; }

        [JsonProperty("is_defending_skill")]
        [HideInInspector] public string IsDefendingSkillString { get; private set; }
        public bool IsDefendingSkill;

        [JsonProperty("failed_defense_damage_rate")]
        public float FailedDefenseDamageRate { get; private set; }

        [JsonProperty("is_evading_skill")]
        [HideInInspector] public string IsEvadingSkillString { get; private set; }
        public bool IsEvadingSkill;

        [JsonProperty("is_interceptable")]
        [HideInInspector] public string IsInterceptableString { get; private set; }
        public bool IsInterceptable;

        [JsonProperty("stress_resistance")]
        public int StressResistance { get; private set; }

        [JsonProperty("effects")]
        [HideInInspector] public string EffectsString { get; private set; }
        public int[] Effects;
    }

    [Serializable]
    public class SkillAnimation
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("subskill_id")]
        public string SubskillId { get; private set; }

        [JsonProperty("character_part_a")]
        public string CharacterPartA { get; private set; }

        [JsonProperty("character_part_b")]
        public string CharacterPartB { get; private set; }

        [JsonProperty("skill_effect_part_a")]
        public string SkillEffectPartA { get; private set; }

        [JsonProperty("skill_effect_part_b")]
        public string SkillEffectPartB { get; private set; }
    }

    #endregion
}