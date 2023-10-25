using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DebugMenuPanel : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private EnemyCharacter enemyCharacter = null;

    [Header( "References" )]
    [SerializeField] private GameObject container = null;
    [SerializeField] private TMP_Dropdown playerStateList = null;
    [SerializeField] private TMP_Dropdown enemyStateList = null;
    [SerializeField] private TMP_InputField playerStateValue = null;
    [SerializeField] private TMP_InputField enemyStateValue = null;

    //variable name
    private string selectedPlayerState;
    private string selectedEnemyState;
    private float newPlayerStateValue;
    private float newEnemyStateValue;
    private bool isPlayerState;
    private bool isEnemyState;

    private const string AUDIO_ID_CLICK = "click";

    //display function
    public void Start()
    {
        playerStateList.ClearOptions();
        enemyStateList.ClearOptions();

        var stateNames = new List<string>
        {
            "參數",
            "當前以太值",
            "當前負荷值",
            "虛傷"
        };

        playerStateList.AddOptions(stateNames);
        enemyStateList.AddOptions(stateNames);

        selectedPlayerState = "參數";
        selectedEnemyState = "參數";
    }

    public void Show()
    {
        this.container.SetActive( true );
    }

    public void Hide()
    {
        this.container.SetActive( false );
    }

    public void ClickToShow()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        Show();
    }

    public void ClickToHide()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        Hide();
    }

    //state value calculation
    public void OnPlayerStateListChange()
    {
        selectedPlayerState = playerStateList.options[playerStateList.value].text;
        isPlayerState = true;
        isEnemyState = false;
    }

    public void OnEnemyStateListChange()
    {
        selectedEnemyState = enemyStateList.options[enemyStateList.value].text;
        isPlayerState = false;
        isEnemyState = true;
    }

    public void OnPlayerStateValueChange()
    {
        if(float.TryParse(playerStateValue.text, out newPlayerStateValue))
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
        if (float.TryParse(enemyStateValue.text, out newEnemyStateValue))
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
        return playerStateList.gameObject.activeSelf;
    }

    public bool IsEnemyDropdownActive()
    {
        return enemyStateList.gameObject.activeSelf;
    }

    public void ChangeStateValue(string stateNames, float newStateValue)
    {
        if (isPlayerState == true)
        {
            if (stateNames == "當前以太值")
            {
                float _difference = newStateValue - playerCharacter.GetCurrentStatePoint();
                if (_difference > 0)
                {
                    playerCharacter.AddCurrentStatePoint(_difference);
                }
                else if (_difference < 0)
                {
                    playerCharacter.MinusCurrentStatePoint(Mathf.Abs(_difference), false);
                }
            }
            else if (stateNames == "當前負荷值")
            {
                if (newStateValue > 0)
                {
                    playerCharacter.AddCurrentStressValue(newStateValue);
                }
            }
            else if (stateNames == "虛傷")
            {
                playerCharacter.MinusCurrentHealthPoint(newStateValue);
            }
        }

        if (isEnemyState == true)
        {
            if (stateNames == "當前以太值")
            {
                float _difference = newStateValue - enemyCharacter.GetCurrentStatePoint();
                if (_difference > 0)
                {
                    enemyCharacter.AddCurrentStatePoint(_difference);
                }
                else if (_difference < 0)
                {
                    enemyCharacter.MinusCurrentStatePoint(Mathf.Abs(_difference), false);
                }
            }
            else if (stateNames == "當前負荷值")
            {
                if (newStateValue > 0)
                {
                    enemyCharacter.AddCurrentStressValue(newStateValue);
                }
            }

            else if (stateNames == "虛傷")
            {
                enemyCharacter.MinusCurrentHealthPoint(newStateValue);
            }
        }
    }

    public void OnConfirmButtonClick()
    {
        if (IsPlayerDropdownActive() && isPlayerState == true)
        {
            ChangeStateValue(selectedPlayerState, newPlayerStateValue);
            Debug.Log("我方"+selectedPlayerState);
        }
        else if (IsEnemyDropdownActive() && isEnemyState == true)
        {
            ChangeStateValue(selectedEnemyState, newEnemyStateValue);
            Debug.Log("對方"+selectedEnemyState);
        }
    }
}
