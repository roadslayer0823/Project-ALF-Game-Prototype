using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Version = DatabaseManager.Version;
using TableStatus = DatabaseManager.TableStatus;

public class AdminPage : MonoBehaviour
{
    [SerializeField] private TMP_Text titleLabel = null;
    [SerializeField] private TableRow tableRowPrefab = null;
    [SerializeField] private RectTransform tableContentRect = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button updateButton = null;
    [SerializeField] private Button resetButton = null;
    [SerializeField] private GameObject loadingObject = null;
    [SerializeField] private AudioClip buttonClickingAudioClip = null;

    private List<TableRow> tableRowList = new List<TableRow>();

    void Awake()
    {
        this.titleLabel.SetText( $"Project ALF Game Prototype (version { Application.version })" );

        DatabaseManager.Instance.onAllVersionsLoadedCallback = GenerateTable;

        DatabaseManager.Instance.onDataCheckingCallback = () =>
        {
            UpdateAllTableRowStatuses( TableStatus.CheckingForUpdates );
        };

        DatabaseManager.Instance.onDataUpdatedCallback = ( sheetName, tableStatus ) =>
        {
            TableRow _tableRow = GetTableRowBySheetName( sheetName );
            if (_tableRow != null)
            {
                UpdateTableRowStatus( _tableRow, tableStatus );
            }
        };

        DatabaseManager.Instance.onVersionUpdatedCallback = ( sheetName, versionNumber ) =>
        {
            if (versionNumber > 0)
            {
                TableRow _tableRow = GetTableRowBySheetName( sheetName );
                if (_tableRow != null)
                {
                    _tableRow.SetVersionNumber( versionNumber );
                }
            }
        };

        DatabaseManager.Instance.onAllDataLoadedCallback = EnableStartButton;

        DatabaseManager.Instance.Initialize();
    }

    void Start()
    {
        this.startGameButton.onClick.AddListener( OnStartGameButtonClicked );
        this.updateButton.onClick.AddListener( OnUpdateButtonClicked );
        this.resetButton.onClick.AddListener( OnResetButtonClicked );
    }

    private void OnStartGameButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffect( this.buttonClickingAudioClip );

        if (SceneUtility.GetBuildIndexByScenePath( "BattleSceneV3" ) != -1)
        {
            SceneManager.LoadScene( "BattleSceneV3" );
        }
        else if (SceneUtility.GetBuildIndexByScenePath( "BattleSceneV2" ) != -1)
        {
            SceneManager.LoadScene( "BattleSceneV2" );
        }
        else
        {
            SceneManager.LoadScene( "BattleScene" );
        }
    }

    private void OnUpdateButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffect( this.buttonClickingAudioClip );
        DisableStartButton();
        DatabaseManager.Instance.LoadAllData();
    }

    private void OnResetButtonClicked()
    {
        AudioManager.Instance.PlaySoundEffect( this.buttonClickingAudioClip );
        DisableStartButton();
        PlayerPrefsManager.DeleteAll();
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        DatabaseManager.Instance.Initialize();
    }

    private void GenerateTable()
    {
        this.loadingObject.SetActive( false );

        DropTable();

        List<Version> _versionList = DatabaseManager.Instance.GetVersionList();
        for (int i = 0; i < _versionList.Count; i++)
        {
            Version _version = _versionList[ i ];

            TableRow _tableRow = Instantiate(tableRowPrefab, tableContentRect, false);
            _tableRow.SetSheetName(_version.SheetName);
            _tableRow.SetVersionNumber(_version.VersionNumber);
            _tableRow.UpdateStatus("Processing...");

            this.tableRowList.Add(_tableRow);
        }

        UpdateStatus();
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

    private void UpdateStatus()
    {
        for (int i = 0; i < this.tableRowList.Count; i++)
        {
            TableRow _tableRow = this.tableRowList[ i ];
            string _sheetName = _tableRow.GetSheetName();
            DatabaseManager _databaseManager = DatabaseManager.Instance;
            TableStatus _tableStatus = TableStatus.None;

            if (_sheetName == _databaseManager.GetVersionSheetName())
            {
                _tableStatus = _databaseManager.GetVersionTableStatus();
            }
            else if (_sheetName == _databaseManager.GetConfigurationSheetName())
            {
                _tableStatus = _databaseManager.GetConfigurationTableStatus();
            }
            else if (_sheetName == _databaseManager.GetCharacterSheetName())
            {
                _tableStatus = _databaseManager.GetCharacterTableStatus();
            }
            else if (_sheetName == _databaseManager.GetSkillSheetName())
            {
                _tableStatus = _databaseManager.GetSkillTableStatus();
            }
            else if (_sheetName == _databaseManager.GetSubskillSheetNamee())
            {
                _tableStatus = _databaseManager.GetSubskillTableStatus();
            }
            else if (_sheetName == _databaseManager.GetSkillAnimationSheetName())
            {
                _tableStatus = _databaseManager.GetSkillAnimationTableStatus();
            }
            else if (_sheetName == _databaseManager.GetPassiveSkillSheetName())
            {
                _tableStatus = _databaseManager.GetPassiveSkillTableStatus();
            }

            UpdateTableRowStatus( _tableRow, _tableStatus );
        }
    }

    private void UpdateTableRowStatus( TableRow tableRow, TableStatus tableStatus )
    {
        switch ( tableStatus )
        {
            case TableStatus.CheckingForUpdates:

                tableRow.UpdateStatus( "Checking for updates..." );

                break;

            case TableStatus.Updating:

                tableRow.UpdateStatus( "Updating..." );

                break;

            case TableStatus.UpdateFailed:

                tableRow.UpdateStatus( "Update failed." );

                break;

            case TableStatus.UpToDate:

                tableRow.UpdateStatus( "Up-to-date." );

                break;

            default:

                tableRow.UpdateStatus( "Error." );

                break;
        }
    }

    private void UpdateAllTableRowStatuses( TableStatus tableStatus )
    {
        for (int i = 0; i < this.tableRowList.Count; i++)
        {
            UpdateTableRowStatus( this.tableRowList[ i ], tableStatus );
        }
    }

    private TableRow GetTableRowBySheetName( string sheetName )
    {
        for (int i = 0; i < this.tableRowList.Count; i++)
        {
            TableRow _tableRow = this.tableRowList[ i ];
            if (_tableRow.GetSheetName() == sheetName)
            {
                return _tableRow;
            }
        }

        return null;
    }

    private void DisableStartButton()
    {
        this.startGameButton.interactable = false;
    }

    private void EnableStartButton()
    {
        this.startGameButton.interactable = true;
    }
}
