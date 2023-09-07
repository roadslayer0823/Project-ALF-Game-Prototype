using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Version = DatabaseManager.Version;

public class AdminPage : MonoBehaviour
{
    [SerializeField] private TableRow tableRowPrefab = null;
    [SerializeField] private RectTransform tableContentRect = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button reloadDatabaseButton = null;

    private List<TableRow> tableRowList = new List<TableRow>();

    private void Awake()
    {
        DatabaseManager.Instance.onAllDataLoadedCallback = GenerateTable;
        //DatabaseManager.Instance.onDataUpdatedCallback = UpdateStatus;
    }

    private void Start()
    {
        DisableStartButton();

        //GenerateTable();

        startGameButton.onClick.AddListener(OnStartGameButtonClick);
        reloadDatabaseButton.onClick.AddListener(DatabaseManager.Instance.LoadAllData);
    }

    private void GenerateTable()
    {
        DropTable();

        List<Version> versionList = DatabaseManager.Instance.GetVersionList();
        for (int i = 0; i < versionList.Count; i++)
        {
            Version _version = versionList[i];

            TableRow _tableRow = Instantiate(tableRowPrefab, tableContentRect, false);
            _tableRow.SetSheetName(_version.SheetName);
            _tableRow.SetVersionNumber(_version.VersionNumber);
            _tableRow.UpdateStatus("Processing...");

            this.tableRowList.Add(_tableRow);
        }

        UpdateStatus();

        EnableStartButton();
    }

    private void DropTable()
    {
        for (int i = 0; i < this.tableRowList.Count; i++)
        {
            TableRow _tableRow = this.tableRowList[i];

            Destroy(_tableRow.gameObject);
        }

        this.tableRowList.Clear();
    }

    /*private void UpdateStatus(string sheetName)
    {
        for (int i = 0; i < this.tableRowList.Count; i++)
        {
            TableRow _tableRow = this.tableRowList[i];
            DatabaseManager _databaseManager = DatabaseManager.Instance;

            if (sheetName == _databaseManager.GetVersionSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetVersionDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetConfigurationSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetConfigurationDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetCharacterSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetCharacterDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetSkillSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetSkillDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetSubskillSheetNamee())
            {
                _tableRow.UpdateStatus(_databaseManager.GetSubskillDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetSkillAnimationSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetSkillAnimationDatabaseStatus());
            }
        }
    }*/

    private void UpdateStatus()
    {
        for (int i = 0; i < this.tableRowList.Count; i++)
        {
            TableRow _tableRow = this.tableRowList[i];
            string sheetName = _tableRow.GetSheetName();
            DatabaseManager _databaseManager = DatabaseManager.Instance;

            if (sheetName == _databaseManager.GetVersionSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetVersionDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetConfigurationSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetConfigurationDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetCharacterSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetCharacterDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetSkillSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetSkillDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetSubskillSheetNamee())
            {
                _tableRow.UpdateStatus(_databaseManager.GetSubskillDatabaseStatus());
            }
            else if (sheetName == _databaseManager.GetSkillAnimationSheetName())
            {
                _tableRow.UpdateStatus(_databaseManager.GetSkillAnimationDatabaseStatus());
            }
            else
            {
                _tableRow.UpdateStatus("Error");
            }
        }
    }

    private void DisableStartButton()
    {
        startGameButton.interactable = false;
    }

    private void EnableStartButton()
    {
        startGameButton.interactable = true;
    }

    private void OnStartGameButtonClick()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
