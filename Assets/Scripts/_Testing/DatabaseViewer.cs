using UnityEngine;
using System.Collections.Generic;

public class DatabaseViewer : MonoBehaviour
{
    private List<DatabaseManager.Character> characterList = new List<DatabaseManager.Character>();
    private List<DatabaseManager.Skill> skillList = new List<DatabaseManager.Skill>();
    private List<DatabaseManager.Subskill> subskillList = new List<DatabaseManager.Subskill>();

    void Start()
    {
        DatabaseManager.Instance.onDataUpdatedCallback = DisplayAllData;
    }

    private void DisplayAllData()
    {
        this.characterList = DatabaseManager.Instance.GetCharacterList();
        this.skillList = DatabaseManager.Instance.GetSkillList();
        this.subskillList = DatabaseManager.Instance.GetSubskillList();
    }
}
