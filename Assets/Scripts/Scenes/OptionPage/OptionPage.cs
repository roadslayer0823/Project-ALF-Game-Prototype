using UnityEngine;
using UnityEngine.SceneManagement;
using Version = DatabaseManager.Version;
using Configuration = DatabaseManager.Configuration;
using Character = DatabaseManager.Character;
using Skill = DatabaseManager.Skill;
using SubSkill = DatabaseManager.Subskill;
using Animation = DatabaseManager.AnimationData;
using PassiveSkill = DatabaseManager.PassiveSkill;

public class OptionPage : MonoBehaviour
{
    [SerializeField] private string versionString = null;
    [SerializeField] private string configurationString = null;
    [SerializeField] private string characterString = null;
    [SerializeField] private string skillString = null;
    [SerializeField] private string subskillString = null;
    [SerializeField] private string animationString = null;
    [SerializeField] private string passiveSkillString = null;
    
    public void GoToBattleScene()
    {
        LoadDataFromResource();
    }

    public void GoToAdminPage()
    {
        SceneManager.LoadScene("AdminPage");
    }

    private void LoadDataFromResource()
    {
        TextAsset Version = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Versions");
        TextAsset Configuration = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Configurations");
        TextAsset Characters = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Characters");
        TextAsset Skills = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Skills");
        TextAsset Subskills = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Subskills");
        TextAsset Animations = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Animations");
        TextAsset Passive_Skills = (TextAsset)Resources.Load("Data/GoogleSpreadsheet/Passive_Skills");

        string VersionText = Version.text;
        string ConfigurationText = Configuration.text;
        string CharactersText = Characters.text;
        string SkillsText = Skills.text;
        string SubskillsText = Subskills.text;
        string AnimationsText = Animations.text;
        string Passive_Skills_Text = Passive_Skills.text;

        if (!string.IsNullOrEmpty(VersionText) &&
            !string.IsNullOrEmpty(ConfigurationText) &&
            !string.IsNullOrEmpty(CharactersText) &&
            !string.IsNullOrEmpty(SkillsText) &&
            !string.IsNullOrEmpty(SubskillsText) &&
            !string.IsNullOrEmpty(AnimationsText) &&
            !string.IsNullOrEmpty(Passive_Skills_Text))
        {
            DatabaseManager.Instance.ProcessJsonData<Version> (VersionText, this.versionString);
            DatabaseManager.Instance.ProcessJsonData<Configuration>(ConfigurationText, this.configurationString);
            DatabaseManager.Instance.ProcessJsonData<Character>(CharactersText, this.characterString);
            DatabaseManager.Instance.ProcessJsonData<Skill>(SkillsText, this.skillString);
            DatabaseManager.Instance.ProcessJsonData<SubSkill>(SubskillsText, this.subskillString);
            DatabaseManager.Instance.ProcessJsonData<Animation>(AnimationsText, this.animationString);
            DatabaseManager.Instance.ProcessJsonData<PassiveSkill>(Passive_Skills_Text, this.passiveSkillString);

            if (SceneUtility.GetBuildIndexByScenePath("BattleSceneV3") != -1)
            {
                SceneManager.LoadScene("BattleSceneV3");
            }
            else if (SceneUtility.GetBuildIndexByScenePath("BattleSceneV2") != -1)
            {
                SceneManager.LoadScene("BattleSceneV2");
            }
            else
            {
                SceneManager.LoadScene("BattleScene");
            }
        }
        else
        {
            return;
        }
    }
}
