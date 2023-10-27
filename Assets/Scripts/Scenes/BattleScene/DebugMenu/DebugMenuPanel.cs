using UnityEngine;
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
        this.playerStateList.ClearOptions();
        this.enemyStateList.ClearOptions();

        var stateNames = new List<string>
        {
            "參數",
            "虛傷",
            "當前以太值",
            "當前負荷值",
            "當前生命值"
        };

        this.playerStateList.AddOptions(stateNames);
        this.enemyStateList.AddOptions(stateNames);

        this.selectedPlayerState = "參數";
        this.selectedEnemyState = "參數";
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
        this.selectedPlayerState = playerStateList.options[playerStateList.value].text;
        this.isPlayerState = true;
        this.isEnemyState = false;
    }

    public void OnEnemyStateListChange()
    {
        this.selectedEnemyState = enemyStateList.options[enemyStateList.value].text;
        this.isPlayerState = false;
        this.isEnemyState = true;
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
                if (playerCharacter.GetCurrentHealthPoint() >= playerCharacter.GetVirtualHealthPoint())
                {
                    playerCharacter.MinusCurrentHealthPoint(newStateValue);
                }
                else if (playerCharacter.GetCurrentHealthPoint() <= playerCharacter.GetVirtualHealthPoint())
                {
                    playerCharacter.AddCurrentHealthPoint(newStateValue);

                    if(playerCharacter.GetVirtualHealthPoint() == 0)
                    {
                        playerCharacter.AddVirtualHealthPoint(newStateValue);
                    }
                }
            }
            else if (stateNames == "當前生命值")
            {
                float _difference = newStateValue - playerCharacter.GetCurrentHealthPoint();
                if (_difference > 0)
                {
                    playerCharacter.AddCurrentHealthPoint(_difference);
                }
                else if (_difference < 0)
                {
                    playerCharacter.MinusCurrentHealthPoint(Mathf.Abs(_difference));
                }
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
                if (enemyCharacter.GetCurrentHealthPoint() >= enemyCharacter.GetVirtualHealthPoint())
                {
                    enemyCharacter.MinusCurrentHealthPoint(newStateValue);
                }
                else if (enemyCharacter.GetCurrentHealthPoint() <= enemyCharacter.GetVirtualHealthPoint())
                {
                    enemyCharacter.AddCurrentHealthPoint(newStateValue);

                    if (enemyCharacter.GetVirtualHealthPoint() == 0)
                    {
                        enemyCharacter.AddVirtualHealthPoint(newStateValue);
                    }
                }
            }

            else if (stateNames == "當前生命值")
            {
                float _difference = newStateValue - playerCharacter.GetCurrentHealthPoint();
                if (_difference > 0)
                {
                    playerCharacter.AddCurrentHealthPoint(_difference);
                    playerCharacter.AddVirtualHealthPoint(newStateValue - playerCharacter.GetVirtualHealthPoint());
                }
                else if (_difference < 0)
                {
                    playerCharacter.MinusCurrentHealthPoint(Mathf.Abs(_difference));
                    playerCharacter.ClearVirtualHealthPoint();
                }
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
