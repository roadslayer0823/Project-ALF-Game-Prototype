using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DebugMenuPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private EnemyCharacter enemyCharacter = null;
    [SerializeField] private BattleGameManager battleGameManager = null;
    [SerializeField] private GameObject debugModeObjectLabel = null;

    [Header("References")]
    [SerializeField] private GameObject container = null;
    [SerializeField] private TMP_Dropdown playerStatList = null;
    [SerializeField] private TMP_Dropdown enemyStatList = null;
    [SerializeField] private TMP_InputField playerStatValue = null;
    [SerializeField] private TMP_InputField enemyStatValue = null;
    [SerializeField] private TextMeshProUGUI enemyInputPlaceHolder = null;
    [SerializeField] private TextMeshProUGUI playerDisplayInfo = null;
    [SerializeField] private TextMeshProUGUI enemyDisplayInfo = null;
    [SerializeField] private TextMeshProUGUI playerInputPlaceHolder = null;
    [SerializeField] private TextMeshProUGUI debugModeButtonLabel = null;
    [SerializeField] private List<char> validateInputWord = null;

    //variable name
    private string selectedPlayerStat;
    private string selectedEnemyStat;
    private string newPlayerStatValue;
    private string newEnemyStatValue;
    private bool isFirstDigit = true;

    private const string AUDIO_ID_CLICK = "click";

    //display function
    public void Start()
    {
        InitializeDropDowns(this.playerStatList);
        InitializeDropDowns(this.enemyStatList);
        this.playerStatValue.interactable = false;
        this.enemyStatValue.interactable = false;

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

        var enemyOnlyState = new List<string>
        {
            "使用技能ID"
        };

        characterList.AddOptions(stateNames);
        this.enemyStatList.AddOptions(enemyOnlyState);
    }

    public void Show()
    {
        Time.timeScale = 0.0f;
        this.container.SetActive(true);
    }

    public void Hide()
    {
        Time.timeScale = 1.0f;
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

    //state value declaration
    public void OnPlayerStateListChange()
    {
        this.selectedPlayerStat = this.playerStatList.options[playerStatList.value].text;
        this.playerInputPlaceHolder.text = "輸入數值";
        CheckValueInput(this.selectedPlayerStat, this.playerStatValue, true);
    }

    public void OnEnemyStateListChange()
    {
        this.selectedEnemyStat = this.enemyStatList.options[enemyStatList.value].text;
        this.enemyInputPlaceHolder.text = "輸入數值";
        CheckValueInput(this.selectedEnemyStat, this.enemyStatValue, false);
    }

    public void OnPlayerStateValueChange()
    {
        ResetInfoText();
        this.newPlayerStatValue = this.playerStatValue.text;
        if(this.playerStatValue.text == "")
        {
            isFirstDigit = true;
        }
    }

    public void OnEnemyStateValueChange()
    {
        ResetInfoText();
        this.newEnemyStatValue = this.enemyStatValue.text;
        if(this.enemyStatValue.text == "")
        {
            isFirstDigit = true;
        }
    }

    public bool IsPlayerDropdownActive()
    {
        return this.playerStatList.gameObject.activeSelf;
    }

    public bool IsEnemyDropdownActive()
    {
        return this.enemyStatList.gameObject.activeSelf;
    }

    //state option
    public void ChangeStateValue(string statNames, string newStatValue, GameCharacter characterObject)
    {
        // negative
        if (statNames == "當前以太值") //current state point
        {
            if (float.TryParse(newStatValue, out float value))
            {
                float _difference = value - characterObject.GetCurrentStatePoint();
                if (_difference > 0)
                {
                    characterObject.AddCurrentStatePoint(_difference);
                }
                else if (_difference < 0)
                {
                    characterObject.MinusCurrentStatePoint( Mathf.Abs( _difference ), false, true );
                }
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "當前負荷值")//current stress point
        {
            if (float.TryParse(newStatValue, out float value))
            {
                float _difference = value - characterObject.GetCurrentStressValue();
                if (_difference > 0)
                {
                    //characterObject.AddCurrentStressValue( _difference, true );

                    new BattleResultData().AddGameCharacterResultData_StressValueDamage( characterObject, Mathf.Abs( _difference ), true, out BattleResultData.BattleResultData_GameCharacter result );
                    characterObject.ApplyBattleResultData( result );
                }
                else if (_difference < 0)
                {
                    //characterObject.MinusCurrentStressValue(Mathf.Abs(_difference));

                    new BattleResultData().AddGameCharacterResultData_StressValueDamageRecovered( characterObject, Mathf.Abs( _difference ), out BattleResultData.BattleResultData_GameCharacter result );
                    characterObject.ApplyBattleResultData( result );
                }
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "虛傷")//current virtual value
        {
            if (float.TryParse(newStatValue, out float value))
            {
                if (characterObject.GetCurrentHealthPoint() >= characterObject.GetVirtualHealthPoint())
                {
                    characterObject.MinusCurrentHealthPoint(value);
                }
                else if (characterObject.GetCurrentHealthPoint() <= characterObject.GetVirtualHealthPoint())
                {
                    characterObject.AddCurrentHealthPoint(value);

                    if (characterObject.GetVirtualHealthPoint() == 0)
                    {
                        characterObject.AddVirtualHealthPoint(value);
                    }
                }
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "當前生命值")//current health
        {
            if (float.TryParse(newStatValue, out float value))
            {
                float _difference = value - characterObject.GetCurrentHealthPoint();
                if (_difference > 0)
                {
                    characterObject.AddCurrentHealthPoint(_difference);
                    characterObject.AddVirtualHealthPoint(value - characterObject.GetVirtualHealthPoint());
                }
                else if (_difference < 0)
                {
                    characterObject.MinusCurrentHealthPoint(Mathf.Abs(_difference));
                    characterObject.ClearVirtualHealthPoint();
                }
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "使用技能ID")//current skill id
        {
            this.enemyCharacter.SetSkillForNextATL(newStatValue, out string errorMessage);
            if(errorMessage == "")
            {
                this.enemyDisplayInfo.text = "輸入技能：" + this.enemyCharacter.GetSkillForNextATL().GetCharacterSubskillData().GetSubskillData().DisplayName;
            }
            else
            {
                this.enemyDisplayInfo.text = errorMessage;
            }
        }

        this.battleGameManager.GetBattleAnimationManager().CheckHasBattleEnded();
    }

    //change state
    public void OnPlayerButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        ChangeStateValue(this.selectedPlayerStat, this.newPlayerStatValue, this.playerCharacter);
    }

    public void OnEnemyButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        ChangeStateValue(this.selectedEnemyStat, this.newEnemyStatValue, this.enemyCharacter);
    }

    public void ClickToToggleDebugMode()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        this.debugModeObjectLabel.SetActive( !this.debugModeObjectLabel.activeSelf );

        bool _isDebugMode = this.debugModeObjectLabel.activeSelf;
        this.battleGameManager.GetBattleAnimationManager().SetIsDebugMode( _isDebugMode );
        this.debugModeButtonLabel.SetText( ( _isDebugMode ) ? "Debug Mode ON" : "Debug Mode OFF" );
    }

    public void ResetInfoText()
    {
        this.playerDisplayInfo.text = null;
        this.enemyDisplayInfo.text = null;
    }

    public void DisplaySuccessText(bool isPlayer)
    {
        if(isPlayer)
        {
            this.playerDisplayInfo.text = "設定成功";
        }
        else
        {
            this.enemyDisplayInfo.text = "設定成功";
        }
    }
    public void CheckValueInput(string selectedCharacterStat, TMP_InputField characterStatValue, bool isPlayer)
    {
        characterStatValue.text = null;
        if (selectedCharacterStat == "參數")
        {
            characterStatValue.interactable = false;
        }
        else
        {
            characterStatValue.interactable = true;

            if (selectedCharacterStat == "使用技能ID")
            {
                characterStatValue.onValidateInput = (string input, int charIndex, char addedChar) => { return ValidateSkillId(addedChar); };
            }
            else
            {
                characterStatValue.onValidateInput = (string input, int charIndex, char addedChar) => { return ValidateNumber(addedChar, isPlayer); };
            }
        }
    }

    public char ValidateSkillId(char addedChar)
    {
        if (this.validateInputWord.Contains(addedChar) || char.IsNumber(addedChar))
        {
            if(char.IsLetter(addedChar))
            {
                addedChar = char.ToUpper(addedChar);
            }
            return addedChar;
        }
        else
        {
            this.enemyDisplayInfo.text = "技能ID_格式: S1_1";
            return '\0';
        }
    }

    public char ValidateNumber(char addedChar, bool isPlayer)
    {
        if (this.selectedEnemyStat == "當前以太值" || this.selectedPlayerStat == "當前以太值")
        {
            if (addedChar == '-' && isFirstDigit)
            {
                isFirstDigit = false;
                return addedChar;
            }
        }
        if (char.IsNumber(addedChar))
        {
            isFirstDigit = false;
            return addedChar;
        }
        else
        {
            if(isPlayer)
            {
                this.playerDisplayInfo.text = "請輸入數字";
            }
            else
            {
                this.enemyDisplayInfo.text = "請輸入數字";
            }
            return '\0';
        }
    }
}
