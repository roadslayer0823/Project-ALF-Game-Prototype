using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private string passiveSkillSheetName = "";
    [SerializeField] private string animationSheetName = "";

    private List<Version> versionList = new();
    private List<Configuration> configurationList = new();
    private List<Character> characterList = new();
    private List<Skill> skillList = new();
    private List<Subskill> subskillList = new();
    private List<SkillAnimation> skillAnimationList = new();
    private List<PassiveSkill> passiveSkillList = new();
    private List<AnimationData> animationList = new();

    private TableStatus versionTableStatus = TableStatus.None;
    private TableStatus configurationTableStatus = TableStatus.None;
    private TableStatus characterTableStatus = TableStatus.None;
    private TableStatus skillTableStatus = TableStatus.None;
    private TableStatus subskillTableStatus = TableStatus.None;
    private TableStatus skillAnimationTableStatus = TableStatus.None;
    private TableStatus passiveSkillTableStatus = TableStatus.None;
    private TableStatus animationTableStatus = TableStatus.None;

    public Action onAllVersionsLoadedCallback = null;
    public Action onDataCheckingCallback = null;
    public Action<string,TableStatus> onDataUpdatedCallback = null;
    public Action<string,int> onVersionUpdatedCallback = null;
    public Action onAllDataLoadedCallback = null;

    public enum TableStatus
    {
        None,
        CheckingForUpdates,
        Updating,
        UpdateFailed,
        UpToDate
    }

    public void Initialize()
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
                ProcessJsonData<Version>( _versionJsonData, this.versionSheetName );
                this.onAllVersionsLoadedCallback?.Invoke();

                StartCoroutine(CheckDatabaseVersion(_versionJsonData));
            }

            string _configurationJsonData = PlayerPrefsManager.LoadConfigurationDatabase();
            string _characterJsonData = PlayerPrefsManager.LoadCharacterDatabase();
            string _skillJsonData = PlayerPrefsManager.LoadSkillDatabase();
            string _subskillJsonData = PlayerPrefsManager.LoadSubskillDatabase();
            string _skillAnimationJsonData = PlayerPrefsManager.LoadSkillAnimationDatabase();
            string _passiveSkillJsonData = PlayerPrefsManager.LoadPassiveSkillDatabase();
            string _animationJsonData = PlayerPrefsManager.LoadAnimationDatabase();

            if (!string.IsNullOrEmpty( _versionJsonData )
                && !string.IsNullOrEmpty( _configurationJsonData )
                && !string.IsNullOrEmpty( _characterJsonData )
                && !string.IsNullOrEmpty( _skillJsonData )
                && !string.IsNullOrEmpty( _subskillJsonData )
                && !string.IsNullOrEmpty( _skillAnimationJsonData )
                && !string.IsNullOrEmpty( _passiveSkillJsonData )
                && !string.IsNullOrEmpty( _animationJsonData )
                )
            {
                ProcessJsonData<Configuration>(_configurationJsonData, this.configurationSheetName);
                ProcessJsonData<Character>( _characterJsonData, this.characterSheetName );
                ProcessJsonData<Skill>( _skillJsonData, this.skillSheetName );
                ProcessJsonData<Subskill>( _subskillJsonData, this.subskillSheetName );
                ProcessJsonData<SkillAnimation>(_skillAnimationJsonData, this.skillAnimationSheetName);
                ProcessJsonData<PassiveSkill>( _passiveSkillJsonData, this.passiveSkillSheetName );
                ProcessJsonData<AnimationData>(_animationJsonData, this.animationSheetName);
            }
            else
            {
                LoadAllData();
            }
        }
    }

    private IEnumerator CheckDatabaseVersion(string versionJsonData)
    {
        this.onDataCheckingCallback?.Invoke();

        // Get latest database version list
        yield return StartCoroutine(GetJsonData<Version>(this.versionSheetName));

        Debug.Log("Previous version json data: " + versionJsonData);

        if (this.versionList.Count != 0)
        {
            Debug.Log("Check Database Version.");

            List<Version> _latestDatabaseVersionList = this.versionList;
            List<Version> _previousDatabaseVersionList = JsonConvert.DeserializeObject<List<Version>>(versionJsonData);

            foreach (Version _latestDatabaseVersion in _latestDatabaseVersionList)
            {
                foreach (Version _previousDatabaseVersion in _previousDatabaseVersionList)
                {
                    // Compare the loaded previous database version number with latest database version number
                    if (_previousDatabaseVersion.SheetName == _latestDatabaseVersion.SheetName)
                    {
                        int _latestVersionNumber = _latestDatabaseVersion.VersionNumber;
                        if (_previousDatabaseVersion.VersionNumber < _latestVersionNumber)
                        {
                            if (_latestDatabaseVersion.SheetName == this.versionSheetName)
                            {
                                StartCoroutine( GetJsonData<Version>( this.versionSheetName ) );
                                Debug.Log( "Update: " + this.versionSheetName );
                            }
                            else if (_latestDatabaseVersion.SheetName == this.configurationSheetName)
                            {
                                StartCoroutine( GetJsonData<Configuration>( this.configurationSheetName, _latestVersionNumber ) );
                                Debug.Log( "Update: " + this.configurationSheetName );
                            }
                            else if (_latestDatabaseVersion.SheetName == this.characterSheetName)
                            {
                                StartCoroutine( GetJsonData<Character>( this.characterSheetName, _latestVersionNumber ) );
                                Debug.Log( "Update: " + this.characterSheetName );
                            }
                            else if (_latestDatabaseVersion.SheetName == this.skillSheetName)
                            {
                                StartCoroutine( GetJsonData<Skill>( this.skillSheetName, _latestVersionNumber ) );
                                Debug.Log( "Update: " + this.skillSheetName );
                            }
                            else if (_latestDatabaseVersion.SheetName == this.subskillSheetName)
                            {
                                StartCoroutine( GetJsonData<Subskill>( this.subskillSheetName, _latestVersionNumber ) );
                                Debug.Log( "Update: " + this.subskillSheetName );
                            }
                            else if (_latestDatabaseVersion.SheetName == this.skillAnimationSheetName)
                            {
                                StartCoroutine( GetJsonData<SkillAnimation>( this.skillAnimationSheetName, _latestVersionNumber ) );
                                Debug.Log( "Update: " + this.skillAnimationSheetName );
                            }
                            else if (_latestDatabaseVersion.SheetName == this.passiveSkillSheetName)
                            {
                                StartCoroutine( GetJsonData<PassiveSkill>( this.passiveSkillSheetName, _latestVersionNumber ) );
                                Debug.Log( "Update: " + this.passiveSkillSheetName );
                            }
                            else if(_latestDatabaseVersion.SheetName == this.animationSheetName)
                            {
                                StartCoroutine(GetJsonData<AnimationData>(this.animationSheetName, _latestVersionNumber));
                                Debug.Log("Update: " + this.animationSheetName);
                            }
                        }
                        else
                        {
                            if (this.onDataUpdatedCallback != null)
                            {
                                this.onDataUpdatedCallback.Invoke( _latestDatabaseVersion.SheetName, TableStatus.UpToDate );
                                this.onVersionUpdatedCallback.Invoke( _latestDatabaseVersion.SheetName, _latestVersionNumber );
                            }
                        }
                    }
                }
            }
        }
    }

    public void LoadAllData()
    {
        this.onDataCheckingCallback?.Invoke();
        StartCoroutine( RunLoadingAllData() );
    }

    private IEnumerator RunLoadingAllData()
    {
        yield return StartCoroutine( GetJsonData<Version>( this.versionSheetName ) );

        this.onAllVersionsLoadedCallback?.Invoke();
        this.onDataCheckingCallback?.Invoke();

        StartCoroutine( GetJsonData<Configuration>( this.configurationSheetName ) );
        StartCoroutine( GetJsonData<Character>( this.characterSheetName ) );
        StartCoroutine( GetJsonData<Skill>( this.skillSheetName ) );
        StartCoroutine( GetJsonData<Subskill>( this.subskillSheetName ) );
        StartCoroutine( GetJsonData<SkillAnimation>( this.skillAnimationSheetName ) );
        StartCoroutine( GetJsonData<PassiveSkill>( this.passiveSkillSheetName ) );
        StartCoroutine( GetJsonData<AnimationData>(this.animationSheetName));
    }

    private IEnumerator GetJsonData<T>( string sheetName, int versionNumber = 0 ) where T : class
    {
        UpdateTableStatus( sheetName, TableStatus.Updating );

        string _jsonURL = "https://opensheet.elk.sh/" + this.databaseSpreadsheetId + "/" + sheetName;

        Debug.Log( "Processing Data, Please Wait" );

        //Download the data from json
        UnityWebRequest _webRequest = UnityWebRequest.Get( _jsonURL );

        //wait for it to download finish
        yield return _webRequest.SendWebRequest();

        //Check to make sure no error, then pass the loaded data to other function for using
        if (string.IsNullOrEmpty( _webRequest.error ))
        {
            ProcessJsonData<T>( _webRequest.downloadHandler.text, sheetName );

            UpdateTableStatus( sheetName, TableStatus.UpToDate );
            this.onVersionUpdatedCallback.Invoke( sheetName, versionNumber );

            Debug.Log( "Done." );
        }
        else
        {
            UpdateTableStatus( sheetName, TableStatus.UpdateFailed );

            Debug.Log( "Oops something went wrong");
        }

        if (this.versionTableStatus == TableStatus.UpToDate
            && this.configurationTableStatus == TableStatus.UpToDate
            && this.characterTableStatus == TableStatus.UpToDate
            && this.skillTableStatus == TableStatus.UpToDate
            && this.subskillTableStatus == TableStatus.UpToDate
            && this.skillTableStatus == TableStatus.UpToDate
            && this.passiveSkillTableStatus == TableStatus.UpToDate
            && this.animationTableStatus == TableStatus.UpToDate)
        {
            onAllDataLoadedCallback?.Invoke();
        }
    }

    public void ProcessJsonData<T>(string jsonData, string sheetName) where T : class
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
                subskill.IsAvailable = bool.Parse(subskill.IsAvailableString);
                subskill.IsDefault = bool.Parse( subskill.IsDefaultString );
                subskill.Range = (Subskill.RangeType)Enum.Parse(typeof(Subskill.RangeType), subskill.RangeString);
                subskill.EffectArea = (Subskill.EffectAreaEnum)Enum.Parse(typeof(Subskill.EffectAreaEnum), subskill.EffectAreaString);
                subskill.EffectType = (Subskill.EffectTypeEnum)Enum.Parse(typeof(Subskill.EffectTypeEnum), subskill.EffectTypeString);
                subskill.IsAttackingSkill = bool.Parse(subskill.IsAttackingSkillString);
                subskill.IsDefendingSkill = bool.Parse(subskill.IsDefendingSkillString);
                subskill.IsEvadingSkill = bool.Parse(subskill.IsEvadingSkillString);
                subskill.IsObservingSkill = bool.Parse(subskill.IsObservingSkillString);
                subskill.IsInterceptable = bool.Parse(subskill.IsInterceptableString);
                subskill.WillRemoveEnergyMarker = bool.Parse( subskill.WillRemoveEnergyMarkerString );
                subskill.RepulseSubskillIds = ConvertStringToStringArray(subskill.RepulseSubskillIdsString);
                subskill.DerivedSubskillIds = ConvertStringToStringArray(subskill.DerivedSubskillIdsString);
                subskill.CounterSubskillIds = ConvertStringToStringArray(subskill.CounterSubskillIdsString);
                //subskill.Effects = ConvertStringToIntArray(subskill.EffectsString);
            }

            PlayerPrefsManager.SaveSubskillDatabase( jsonData );
        }
        else if (sheetName == this.skillAnimationSheetName)
        {
            this.skillAnimationList = dataList as List<SkillAnimation>;

            PlayerPrefsManager.SaveSkillAnimationDatabase(jsonData);
        }
        else if (sheetName == this.passiveSkillSheetName)
        {
            this.passiveSkillList = dataList as List<PassiveSkill>;

            foreach (PassiveSkill passiveSkill in this.passiveSkillList)
            {
                passiveSkill.Category = ( PassiveSkill.CategoryType )Enum.Parse( typeof( PassiveSkill.CategoryType ), passiveSkill.CategoryString );
            }

            PlayerPrefsManager.SavePassiveSkillDatabase( jsonData );
        }
        else if (sheetName == this.animationSheetName)
        {
            this.animationList = dataList as List<AnimationData>;

            foreach(AnimationData animation in this.animationList)
            {
                animation.Code = (AnimationData.CodeType)Enum.Parse(typeof(AnimationData.CodeType), animation.CodeString);
                animation.SubskillIdsArray = ConvertStringToStringArray(animation.SubskillIdsArrayString);
                animation.ActionsArray = ConvertStringToStringArray(animation.ActionsArrayString);
                animation.EffectsArray = ConvertStringToStringArray(animation.EffectsArrayString);
                animation.AudiosArray = ConvertStringToStringArray(animation.AudiosArrayString);
            }
            PlayerPrefsManager.SaveAnimationDatabase(jsonData);
        }

        UpdateTableStatus( sheetName, TableStatus.UpToDate );
    }

    private void UpdateTableStatus( string sheetName, TableStatus tableStatus )
    {
        if (sheetName == this.versionSheetName)
        {
            this.versionTableStatus = tableStatus;
        }
        else if (sheetName == this.configurationSheetName)
        {
            this.configurationTableStatus = tableStatus;
        }
        else if (sheetName == this.characterSheetName)
        {
            this.characterTableStatus = tableStatus;
        }
        else if (sheetName == this.skillSheetName)
        {
            this.skillTableStatus = tableStatus;
        }
        else if (sheetName == this.subskillSheetName)
        {
            this.subskillTableStatus = tableStatus;
        }
        else if (sheetName == this.skillAnimationSheetName)
        {
            this.skillAnimationTableStatus = tableStatus;
        }
        else if (sheetName == this.passiveSkillSheetName)
        {
            this.passiveSkillTableStatus = tableStatus;
        }
        else if(sheetName == this.animationSheetName)
        {
            this.animationTableStatus = tableStatus;
        }

        if (this.onDataUpdatedCallback != null)
        {
            this.onDataUpdatedCallback.Invoke( sheetName, tableStatus );
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

    public List<PassiveSkill> GetPassiveSkillList()
    {
        return this.passiveSkillList;
    }

    public AnimationData GetAnimationData(AnimationData.CodeType codeType, string subskillId = "", int type = 0)
    {
        for(int i = 0; i < animationList.Count; i++)
        {
            AnimationData animationData = animationList[i];

            if (codeType == animationData.Code)
            {
                if (String.IsNullOrEmpty(subskillId))
                {
                   if(type == 0)
                   {
                        return animationData;
                   }
                   else if (type == animationData.Type)
                   {
                        return animationData;
                   }
                }
                else if(type == animationData.Type)
                {
                    for(int j = 0; j < animationData.SubskillIdsArray.Length;j++)
                    {
                        if (animationData.SubskillIdsArray[j] == subskillId)
                        {
                            return animationData;
                        }
                    }
                }
            }
        }
        return null;
    }

    public string GetVersionSheetName()
    {
        return this.versionSheetName;
    }

    public string GetConfigurationSheetName()
    {
        return this.configurationSheetName;
    }

    public string GetCharacterSheetName()
    {
        return this.characterSheetName;
    }

    public string GetSkillSheetName()
    {
        return this.skillSheetName;
    }

    public string GetSubskillSheetNamee()
    {
        return this.subskillSheetName;
    }

    public string GetSkillAnimationSheetName()
    {
        return this.skillAnimationSheetName;
    }

    public string GetPassiveSkillSheetName()
    {
        return this.passiveSkillSheetName;
    }

    public string GetAnimationSheetName()
    {
        return this.animationSheetName;
    }

    public TableStatus GetVersionTableStatus()
    {
        return this.versionTableStatus;
    }

    public TableStatus GetConfigurationTableStatus()
    {
        return this.configurationTableStatus;
    }

    public TableStatus GetCharacterTableStatus()
    {
        return this.characterTableStatus;
    }

    public TableStatus GetSkillTableStatus()
    {
        return this.skillTableStatus;
    }

    public TableStatus GetSubskillTableStatus()
    {
        return this.subskillTableStatus;
    }

    public TableStatus GetSkillAnimationTableStatus()
    {
        return this.skillAnimationTableStatus;
    }

    public TableStatus GetPassiveSkillTableStatus()
    {
        return this.passiveSkillTableStatus;
    }

    public TableStatus GetAnimationTableStatus()
    {
        return this.animationTableStatus;
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
            none,
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

        [JsonProperty("is_available")]
        [HideInInspector] public string IsAvailableString { get; private set; }
        public bool IsAvailable;

        [JsonProperty( "is_default" )]
        [HideInInspector] public string IsDefaultString { get; private set; }
        public bool IsDefault;

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

        [JsonProperty("name_part_a")]
        public string NamePartA { get; private set; }

        [JsonProperty("name_part_b")]
        public string NamePartB { get; private set; }

        [JsonProperty( "icon_file_path_on" )]
        public string IconFilePathOn { get; private set; }

        [JsonProperty( "icon_file_path_off" )]
        public string IconFilePathOff { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("repulse_subskill_ids")]
        [HideInInspector] public string RepulseSubskillIdsString { get; private set; }
        public string[] RepulseSubskillIds;

        [JsonProperty("derived_subskill_ids")]
        [HideInInspector] public string DerivedSubskillIdsString { get; private set; }
        public string[] DerivedSubskillIds;

        [JsonProperty("counter_subskill_ids")]
        [HideInInspector] public string CounterSubskillIdsString { get; private set; }
        public string[] CounterSubskillIds;

        [JsonProperty("range")]
        [HideInInspector] public string RangeString { get; private set; }
        public enum RangeType
        {
            none,
            melee,
            ranged,
            melee_or_ranged
        }
        public RangeType Range;

        [JsonProperty("attack_damage")]
        public float AttackDamage { get; private set; }

        [JsonProperty("max_state_point_up")]
        public float MaxStatePointUp { get; private set; }

        [JsonProperty("state_point_cost")]
        public float StatePointCost { get; private set; }

        [JsonProperty("state_point_damage")]
        public float StatePointDamage { get; private set; }

        [JsonProperty("stress_value_damage")]
        public float StressValueDamage { get; private set; }

        [JsonProperty("evasion_stress")]
        public float EvasionStress { get; private set; }

        [JsonProperty( "damage_reduction" )]
        public float DamageReduction { get; private set; }

        [JsonProperty( "damage_reduction_rate" )]
        public float DamageReductionRate { get; private set; }

        [JsonProperty("speed")]
        public int Speed { get; private set; }

        [JsonProperty("strength")]
        public int Strength { get; private set; }

        [JsonProperty("accuracy")]
        [Obsolete] public int Accuracy { get; private set; }

        [JsonProperty("evasion")]
        [Obsolete] public int Evasion { get; private set; }

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
        [Obsolete] public float FailedRepulseDamageRate { get; private set; }

        [JsonProperty("is_defending_skill")]
        [HideInInspector] public string IsDefendingSkillString { get; private set; }
        public bool IsDefendingSkill;

        [JsonProperty("failed_defense_damage_rate")]
        [Obsolete] public float FailedDefenseDamageRate { get; private set; }

        [JsonProperty("is_evading_skill")]
        [HideInInspector] public string IsEvadingSkillString { get; private set; }
        public bool IsEvadingSkill;

        [JsonProperty("is_observing_skill")]
        [HideInInspector] public string IsObservingSkillString { get; private set; }
        public bool IsObservingSkill;

        [JsonProperty("observation_rate")]
        public float ObservationRate { get; private set; }

        [JsonProperty("is_interceptable")]
        [HideInInspector] public string IsInterceptableString { get; private set; }
        public bool IsInterceptable;

        [JsonProperty( "use_active_skill_to_counter" )]
        [HideInInspector] public string UseActiveSkillToCounterString { get; private set; }
        public bool UseActiveSkillToCounter;

        [JsonProperty( "energy_marker_atl" )]
        public int EnergyMarkerATL { get; private set; }

        [JsonProperty( "will_remove_energy_marker" )]
        [HideInInspector] public string WillRemoveEnergyMarkerString { get; private set; }
        public bool WillRemoveEnergyMarker;

        [JsonProperty( "energy_marker_health_damage_rate" )]
        public float EnergyMarkerHealthDamageRate { get; private set; }

        [JsonProperty( "energy_marker_state_damage_rate" )]
        public float EnergyMarkerStateDamageRate { get; private set; }

        [JsonProperty( "energy_marker_stress_damage_rate" )]
        public float EnergyMarkerStressDamageRate { get; private set; }

        [JsonProperty( "energy_marker_evasion_stress_rate" )]
        public float EnergyMarkerEvasionStressRate { get; private set; }

        [JsonProperty("stress_resistance")]
        public float StressResistance { get; private set; }

        [JsonProperty("effects")]
        [Obsolete] [HideInInspector] public string EffectsString { get; private set; }
        [Obsolete] public int[] Effects;
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

    [Serializable]
    public class PassiveSkill
    {
        [JsonProperty( "id" )]
        public string Id { get; private set; }

        [JsonProperty( "category" )]
        [HideInInspector] public string CategoryString { get; private set; }
        public enum CategoryType
        {
            life,
            state,
            stress
        }
        public CategoryType Category;

        [JsonProperty( "display_name" )]
        public string DisplayName { get; private set; }

        [JsonProperty( "description" )]
        public string Description { get; private set; }
    }

    [Serializable]
    public class AnimationData
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("code")]
        [HideInInspector] public string CodeString { get; private set; }
        public enum CodeType
        {
            camB_partA_V1,
            camB_partA_VF,
            camA_type_BDVC,
            camA_type_BDV1,
            camA_type_BDV2,
            camB_type_CDV1,
            camB_type_CDV2,
            camB_type_CDVF,
            camA_type_AVC,
            camA_type_AV1,
            camBA_type_AV1,
            camA_type_AV2,
            camB_type_CFW,
            camB_type_CFWVF,
            camA_type_BFL,
            camA_type_D_L,
            camB_type_D_H,
            camA_type_BLPVC_L,
            camA_type_BLPVC_H,
            camA_type_BLPV1_L,
            camA_type_BLPV1_H,
            camA_type_BLPV2_L,
            camA_type_BLPV2_H,
            camB_type_CDV3,
            camB_type_CLSV1_L,
            camB_type_CLSV1_H,
            camB_type_CLSV2_L,
            camB_type_CLSV2_H,
            camB_type_CLSVF_L,
            camB_type_CLSVF_H,
            camB_type_CLPV1_L,
            camB_type_CLPV1_H,
            camB_type_CLPV2_L,
            camB_type_CLPV2_H,
            camB_type_CLPVF_L,
            camB_type_CLPVF_H,
        }
        public CodeType Code;

        [JsonProperty("subskill_ids")]
        [HideInInspector] public string SubskillIdsArrayString { get; private set; }
        public string[] SubskillIdsArray;

        [JsonProperty("type")]
        public int Type { get; private set; }

        [JsonProperty("actions")]
        [HideInInspector] public string ActionsArrayString { get; private set; }
        public string[] ActionsArray;

        [JsonProperty("effects")]
        [HideInInspector] public string EffectsArrayString { get; private set; }
        public string[] EffectsArray;

        [JsonProperty("audios")]
        [HideInInspector] public string AudiosArrayString { get; private set; }
        public string[] AudiosArray;
    }

    #endregion
}
