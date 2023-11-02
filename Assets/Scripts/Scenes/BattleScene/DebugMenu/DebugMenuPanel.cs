using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DebugMenuPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private EnemyCharacter enemyCharacter = null;

    [Header("References")]
    [SerializeField] private GameObject container = null;
    [SerializeField] private TMP_Dropdown playerStatList = null;
    [SerializeField] private TMP_Dropdown enemyStatList = null;
    [SerializeField] private TMP_InputField playerStatValue = null;
    [SerializeField] private TMP_InputField enemyStatValue = null;

    //variable name
    private string selectedPlayerStat;
    private string selectedEnemyStat;
    private float newPlayerStatValue;
    private float newEnemyStatValue;

    private const string AUDIO_ID_CLICK = "click";

    //display function
    public void Start()
    {
        InitializeDropDowns(playerStatList);
        InitializeDropDowns(enemyStatList);
    }

    private void InitializeDropDowns(TMP_Dropdown characterList)
    {
        characterList.ClearOptions();

        var stateNames = new List<string>
        {
            "參數",
            "虛傷",
            "當前以太值",
            "當前負荷值",
            "當前生命值"
        };

        characterList.AddOptions(stateNames);
    }


    public void Show()
    {
        this.container.SetActive(true);
    }

    public void Hide()
    {
        this.container.SetActive(false);
    }

    public void ClickToShow()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        Show();
    }

    public void ClickToHide()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        Hide();
    }

    //state value calculation
    public void OnPlayerStateListChange()
    {
        this.selectedPlayerStat = playerStatList.options[playerStatList.value].text;
    }

    public void OnEnemyStateListChange()
    {
        this.selectedEnemyStat = enemyStatList.options[enemyStatList.value].text;
    }

    public void OnPlayerStateValueChange()
    {
        if (float.TryParse(playerStatValue.text, out newPlayerStatValue))
        {
            Debug.Log("press confirm button");
        }
        else
        {
            Debug.Log("plese insert the state value");
        }
    }

    public void OnEnemyStateValueChange()
    {
        if (float.TryParse(enemyStatValue.text, out newEnemyStatValue))
        {
            Debug.Log("press confirm button");
        }
        else
        {
            Debug.Log("plese insert the state value");
        }
    }

    public bool IsPlayerDropdownActive()
    {
        return playerStatList.gameObject.activeSelf;
    }

    public bool IsEnemyDropdownActive()
    {
        return enemyStatList.gameObject.activeSelf;
    }

    public void ChangeStateValue(string stateNames, float newStateValue, GameCharacter characterType)
    {
        if (stateNames == "當前以太值")
        {
            float _difference = newStateValue - characterType.GetCurrentStatePoint();
            if (_difference > 0)
            {
                characterType.AddCurrentStatePoint(_difference);
            }
            else if (_difference < 0)
            {
                characterType.MinusCurrentStatePoint(Mathf.Abs(_difference), false);
            }
        }
        else if (stateNames == "當前負荷值")
        {
            if (newStateValue > 0)
            {
                characterType.AddCurrentStressValue(newStateValue);
            }
        }
        else if (stateNames == "虛傷")
        {
            if (characterType.GetCurrentHealthPoint() >= characterType.GetVirtualHealthPoint())
            {
                characterType.MinusCurrentHealthPoint(newStateValue);
            }
            else if (characterType.GetCurrentHealthPoint() <= characterType.GetVirtualHealthPoint())
            {
                characterType.AddCurrentHealthPoint(newStateValue);

                if (characterType.GetVirtualHealthPoint() == 0)
                {
                    characterType.AddVirtualHealthPoint(newStateValue);
                }
            }
        }
        else if (stateNames == "當前生命值")
        {
            float _difference = newStateValue - characterType.GetCurrentHealthPoint();
            if (_difference > 0)
            {
                characterType.AddCurrentHealthPoint(_difference);
                characterType.AddVirtualHealthPoint(newStateValue - characterType.GetVirtualHealthPoint());
            }
            else if (_difference < 0)
            {
                characterType.MinusCurrentHealthPoint(Mathf.Abs(_difference));
                characterType.ClearVirtualHealthPoint();
            }
        }
    }

    public void OnPlayerButtonClick()
    {
        ChangeStateValue(selectedPlayerStat, newPlayerStatValue, playerCharacter);
    }

    public void OnEnemyButtonClick()
    {
        ChangeStateValue(selectedEnemyStat, newEnemyStatValue, enemyCharacter);
    }
}
