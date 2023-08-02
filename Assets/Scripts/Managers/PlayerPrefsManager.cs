using UnityEngine;

public class PlayerPrefsManager
{
    private const string VERSION_DATABASE = "version_database";
    private const string CHARACTER_DATABASE = "character_database";
    private const string SKILL_DATABASE = "skill_database";
    private const string SUBSKILL_DATABASE = "subskill_database";
    private const string SKILL_ANIMATION_DATABASE = "skill_animation_database";

    public static void SaveVersionDatabase(string jsonData)
    {
        PlayerPrefs.SetString(VERSION_DATABASE, jsonData);
    }

    public static string LoadVersionDatabase()
    {
        return PlayerPrefs.GetString(VERSION_DATABASE, "");
    }

    public static void SaveCharacterDatabase( string jsonData )
    {
        PlayerPrefs.SetString( CHARACTER_DATABASE, jsonData );
    }

    public static string LoadCharacterDatabase()
    {
        return PlayerPrefs.GetString( CHARACTER_DATABASE, "" );
    }

    public static void SaveSkillDatabase( string jsonData )
    {
        PlayerPrefs.SetString( SKILL_DATABASE, jsonData );
    }

    public static string LoadSkillDatabase()
    {
        return PlayerPrefs.GetString( SKILL_DATABASE, "" );
    }

    public static void SaveSubskillDatabase( string jsonData )
    {
        PlayerPrefs.SetString( SUBSKILL_DATABASE, jsonData );
    }

    public static string LoadSubskillDatabase()
    {
        return PlayerPrefs.GetString( SUBSKILL_DATABASE, "" );
    }

    public static void SaveSkillAnimationDatabase(string jsonData)
    {
        PlayerPrefs.SetString(SKILL_ANIMATION_DATABASE, jsonData);
    }

    public static string LoadSkillAnimationDatabase()
    {
        return PlayerPrefs.GetString(SKILL_ANIMATION_DATABASE, "");
    }
}
