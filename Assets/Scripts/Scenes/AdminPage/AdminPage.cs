using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version = DatabaseManager.Version;

public class AdminPage : MonoBehaviour
{
    [SerializeField] private TableRow tableRowPrefab = null;
    [SerializeField] private RectTransform tableRect = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button reloadDatabaseButton = null;

    private void Start()
    {
        GenerateTable();

        startGameButton.onClick.AddListener(StartSceneManager.Instance.GoToBattleScene);
        reloadDatabaseButton.onClick.AddListener(DatabaseManager.Instance.LoadAllData);
    }

    private void GenerateTable()
    {
        for (int i = 0; i < DatabaseManager.Instance.GetVersionList().Count; i++)
        {
            Version _version = DatabaseManager.Instance.GetVersionList()[i];

            TableRow tableRow = Instantiate(tableRowPrefab, tableRect, false);
            tableRow.SetSheetName(_version.SheetName);
            tableRow.SetVersionNumber(_version.VersionNumber);
            tableRow.SetStatus("Up to date.");
        }
    }
}
