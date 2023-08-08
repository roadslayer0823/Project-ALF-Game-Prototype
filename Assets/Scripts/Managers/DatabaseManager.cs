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
    [SerializeField] private string characterSheetName = "";
    [SerializeField] private string skillSheetName = "";
    [SerializeField] private string subskillSheetName = "";
    [SerializeField] private string skillAnimationSheetName = "";

    private List<Version> versionList = new List<Version>();
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

            string _characterJsonData = PlayerPrefsManager.LoadCharacterDatabase();
            string _skillJsonData = PlayerPrefsManager.LoadSkillDatabase();
            string _subskillJsonData = PlayerPrefsManager.LoadSubskillDatabase();
            string _skillAnimationJsonData = PlayerPrefsManager.LoadSkillAnimationDatabase();

            if (!string.IsNullOrEmpty(_versionJsonData)
                && !string.IsNullOrEmpty( _characterJsonData )
                && !string.IsNullOrEmpty( _skillJsonData )
                && !string.IsNullOrEmpty( _subskillJsonData )
                && !string.IsNullOrEmpty(_skillAnimationJsonData))
            {
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
                        if (latestDatabaseVersion.SheetName == this.characterSheetName)
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

    private void LoadAllData()
    {
        StartCoroutine(GetJsonData<Version>(this.versionSheetName));
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
                subskill.effectArea = (Subskill.EffectArea)Enum.Parse(typeof(Subskill.EffectArea), subskill.EffectAreaString);
                subskill.effectType = (Subskill.EffectType)Enum.Parse(typeof(Subskill.EffectType), subskill.EffectTypeString);
                subskill.IsAttackingSkill = bool.Parse(subskill.IsAttackingSkillString);
                subskill.IsInterceptable = bool.Parse(subskill.IsInterceptableString);
                subskill.Effects = ConvertStringToIntArray(subskill.EffectsString);
            }

            PlayerPrefsManager.SaveSubskillDatabase( jsonData );
        }
        else if (sheetName == this.skillAnimationSheetName)
        {
            this.skillAnimationList = dataList as List<SkillAnimation>;

            foreach (SkillAnimation skillAnimation in this.skillAnimationList)
            {
                skillAnimation.animationType = (SkillAnimation.AnimationType)Enum.Parse(typeof(SkillAnimation.AnimationType), skillAnimation.AnimationTypeString);
            }

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

    // Return correspond skill data based on skill id
    public Skill GetSkillDataById(string id)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            Skill skillData = skillList[i];
            if (skillData.GetId() == id)
            {
                return skillData;
            }
        }

        return null;
    }

    // Return correspond character data based on character id
    public Character GetCharacterDataById(string id)
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            Character characterData = characterList[i];
            if (characterData.GetId() == id)
            {
                return characterData;
            }
        }

        return null;
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
        public string SheetName;

        [JsonProperty("version")]
        public int VersionNumber;
    }

    [Serializable]
    public class Character
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("display_name")]
        public string DisplayName;

        [JsonProperty("maximum_health_point")]
        public int MaximumHealthPoint;

        [JsonProperty("maximum_state_point")]
        public int MaximumStatePoint;

        [JsonProperty("skill_id_array")]
        [HideInInspector] public string SkillIdArrayString;
        public string[] SkillIdArray;

        #region Getter
        public string GetId()
        {
            return Id;
        }

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public int GetMaximumHealthPoint()
        {
            return MaximumHealthPoint;
        }

        public int GetMaximumStatePoint()
        {
            return MaximumStatePoint;
        }

        public string[] GetSkillIdArray()
        {
            return SkillIdArray;
        }
        #endregion
    }

    [Serializable]
    public class Skill
    {
        [JsonProperty("id")]
        public string Id;

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

        #region Getter
        public string GetId()
        {
            return Id;
        }

        public string GetDisplayName()
        {
            return DisplayName;
        }

        public SkillType GetSkillType()
        {
            return skillType;
        }
        #endregion
    }

    [Serializable]
    public class Subskill
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("skill_id")]
        public string SkillId;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("repulse_skill_id")]
        public string RepulseSkillId;

        [JsonProperty("derived_skill_id")]
        public string DerivedSkillId;

        [JsonProperty("state_point_cost")]
        public int StatePointCost;

        [JsonProperty("attack_damage")]
        public int AttackDamage;

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

        #region Getter
        public string GetId()
        {
            return Id;
        }

        public string GetSkillId()
        {
            return SkillId;
        }

        public int GetLevel()
        {
            return Level;
        }

        public string GetRepulseSkillId()
        {
            return RepulseSkillId;
        }

        public string GetDerivedSkillId()
        {
            return DerivedSkillId;
        }

        public int GetStatePointCost()
        {
            return StatePointCost;
        }

        public int GetAttackDamage()
        {
            return AttackDamage;
        }

        public int GetDefense()
        {
            return Defense;
        }

        public int GetStrength()
        {
            return Strength;
        }

        public EffectArea GetEffectArea()
        {
            return effectArea;
        }

        public EffectType GetEffectType()
        {
            return effectType;
        }

        public bool GetIsAttackingSkill()
        {
            return IsAttackingSkill;
        }

        public bool GetIsInterceptable()
        {
            return IsInterceptable;
        }

        public int GetStressResistance()
        {
            return StressResistance;
        }

        public int[] GetEffects()
        {
            return Effects;
        }

        #endregion
    }

    [Serializable]
    public class SkillAnimation
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("subskill_id")]
        public string SubskillId;

        [JsonProperty("animation_type")]
        [HideInInspector] public string AnimationTypeString;
        public enum AnimationType
        {
            none,
            melee,
            ranged
        }
        public AnimationType animationType;

        [JsonProperty("character_part_a")]
        public string CharacterPartA;

        [JsonProperty("character_part_b")]
        public string CharacterPartB;

        [JsonProperty("skill_effect_part_a")]
        public string SkillEffectPartA;

        [JsonProperty("skill_effect_part_b")]
        public string SkillEffectPartB;

        #region Getter
        public string GetId()
        {
            return Id;
        }

        public string GetSubskillId()
        {
            return SubskillId;
        }

        public AnimationType GetAnimationType()
        {
            return animationType;
        }

        public string GetCharacterPartA()
        {
            return CharacterPartA;
        }

        public string GetCharacterPartB()
        {
            return CharacterPartB;
        }

        public string GetSkillEffectPartA()
        {
            return SkillEffectPartA;
        }

        public string GetSkillEffectPartB()
        {
            return SkillEffectPartB;
        }

        #endregion
    }

    #endregion
}