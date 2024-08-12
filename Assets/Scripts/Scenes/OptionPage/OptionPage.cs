using UnityEngine;

public class OptionPage : MonoBehaviour
{
    [SerializeField] private string versionString = null;
    [SerializeField] private string configurationString = null;
    [SerializeField] private string characterString = null;
    [SerializeField] private string skillString = null;
    [SerializeField] private string subskillString = null;
    [SerializeField] private string animationString = null;
    [SerializeField] private string passiveSkillString = null;
    [SerializeField] private AudioClip buttonClickingAudioClip = null;

    public void ClickToUseLocalSettings()
    {
        AudioManager.Instance.PlaySoundEffect( this.buttonClickingAudioClip );

        string _versionsJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Versions" ).text;
        string _configurationsJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Configurations" ).text;
        string _charactersJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Characters" ).text;
        string _skillsJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Skills" ).text;
        string _subskillsJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Subskills" ).text;
        string _animationJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Animations" ).text;
        string _passiveSkillsJsonString = Resources.Load<TextAsset>( "Data/GoogleSpreadsheet/Passive_Skills" ).text;

        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.Version>( _versionsJsonString, this.versionString );
        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.Configuration>( _configurationsJsonString, this.configurationString );
        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.Character>( _charactersJsonString, this.characterString );
        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.Skill>( _skillsJsonString, this.skillString );
        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.Subskill>( _subskillsJsonString, this.subskillString );
        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.AnimationData>( _animationJsonString, this.animationString );
        DatabaseManager.Instance.ProcessJsonData<DatabaseManager.PassiveSkill>( _passiveSkillsJsonString, this.passiveSkillString );

        SceneControlManager.GoToBattleScene();
    }

    public void ClickToUseCloudSettings()
    {
        AudioManager.Instance.PlaySoundEffect( this.buttonClickingAudioClip );
        SceneControlManager.GoToAdminPage();
    }
}
