using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
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
    [SerializeField] private Image currentBackground = null;
    [SerializeField] private Sprite backgroundOne = null;
    [SerializeField] private Sprite backgroundTwo = null;
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

    [Header( "Backgrounds" )]
    [SerializeField] private Sprite backgroundOnePartA = null;
    [SerializeField] private Sprite backgroundOnePartB = null;
    [SerializeField] private Sprite backgroundTwoPartA = null;
    [SerializeField] private Sprite backgroundTwoPartB = null;

    //variable name
    private string selectedPlayerStat;
    private string selectedEnemyStat;
    private string newPlayerStatValue;
    private string newEnemyStatValue;
    private char tempChar = '\0';
    private int backgroundId = 1;

    private const string AUDIO_ID_CLICK = "click";

    //display function
    public void Start()
    {
        this.playerStatValue.interactable = false;
        this.enemyStatValue.interactable = false;
    }

    public void Show()
    {
        Time.timeScale = 0.0f;
        this.container.SetActive(true);
    }

    public void Hide()
    {
        Time.timeScale = PauseButton.currentTimeScale;
        this.container.SetActive(false);
        ResetInfoText();
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

    public void ClickToSwitchBackground()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        BattleAnimationManager _battleAnimationManager = battleGameManager.GetBattleAnimationManager();

        if (this.backgroundId == 1)
        {
            this.backgroundId = 2;
            this.currentBackground.sprite = this.backgroundTwoPartA;
            _battleAnimationManager.SetBackgroundSprites( this.backgroundTwoPartA, this.backgroundTwoPartB );
        }
        else if (this.backgroundId == 2)
        {
            this.backgroundId = 1;
            this.currentBackground.sprite = this.backgroundOnePartA;
            _battleAnimationManager.SetBackgroundSprites( this.backgroundOnePartA, this.backgroundOnePartB );
        }

        battleGameManager.GetBattleVisualEffectManager().SwitchCombatCommandBackground(this.backgroundId);
    }

    //state value declaration
    public void OnPlayerStateListChange()
    {
        this.selectedPlayerStat = this.playerStatList.options[playerStatList.value].text;
        this.playerInputPlaceHolder.text = "輸入數值";
        CheckValueInput(this.selectedPlayerStat, this.playerStatValue);
    }

    public void OnEnemyStateListChange()
    {
        this.selectedEnemyStat = this.enemyStatList.options[enemyStatList.value].text;
        this.enemyInputPlaceHolder.text = "輸入數值";
        CheckValueInput(this.selectedEnemyStat, this.enemyStatValue);
    }

    public void OnPlayerStateValueChange()
    {
        ResetInfoText();
        this.newPlayerStatValue = this.playerStatValue.text;
        if(tempChar == ' ')
        {
            this.playerDisplayInfo.text = "請輸入數字";
            tempChar = '\0';
        }
        if (this.selectedPlayerStat == "當前以太值")
        {
            playerStatValue.text = playerStatValue.text.Replace(" ", "").Replace(".", "");
        }
    }

    public void OnEnemyStateValueChange()
    {
        ResetInfoText();
        this.newEnemyStatValue = this.enemyStatValue.text;
        if (tempChar == ' ')
        {
            if (this.selectedEnemyStat == "使用技能ID")
            {
                this.enemyDisplayInfo.text = "技能ID的格式: S1_1";
            }
            else
            {
                this.enemyDisplayInfo.text = "請輸入數字";
            }
            tempChar = '\0';
        }
        if (this.selectedEnemyStat == "當前以太值")
        {
            enemyStatValue.text = enemyStatValue.text.Replace(" ", "").Replace(".", "");
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
                    characterObject.MinusCurrentStatePoint(Mathf.Abs(_difference), false, true);
                }
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "最大以太值") //maximum state point
        {
            if (float.TryParse(newStatValue, out float value))
            {
                float _difference = value - characterObject.GetCurrentStatePoint();
                if (_difference > 0)
                {
                    characterObject.AddMaximumStatePoint(_difference);
                }
                else if (_difference < 0)
                {
                    characterObject.MinusMaximumStatePoint(Mathf.Abs(_difference));
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

                    new BattleResultData().AddGameCharacterResultData_StressValueDamage(characterObject, Mathf.Abs(_difference), out BattleResultData.BattleResultData_GameCharacter result);
                    characterObject.ApplyBattleResultData(result);
                }
                else if (_difference < 0)
                {
                    //characterObject.MinusCurrentStressValue(Mathf.Abs(_difference));

                    new BattleResultData().AddGameCharacterResultData_StressValueDamageRecovered(characterObject, Mathf.Abs(_difference), out BattleResultData.BattleResultData_GameCharacter result);
                    characterObject.ApplyBattleResultData(result);
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
            if (errorMessage == "")
            {
                this.enemyDisplayInfo.text = "輸入技能：" + this.enemyCharacter.GetSkillForNextATL().GetCharacterSubskillData().GetSubskillData().DisplayName;
            }
            else
            {
                this.enemyDisplayInfo.text = errorMessage;
            }
        }
        else if (statNames == "生命積分") //life score
        {
            if (int.TryParse(newStatValue, out int value))
            {
                characterObject.SetDebugLifeScore(value);
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if(statNames == "循環點") //life cycle point
        {
            if (int.TryParse(newStatValue, out int value))
            {
                characterObject.SetDebugLifeCyclePoint(value);
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "負荷積分") //stress score
        {
            if (int.TryParse(newStatValue, out int value))
            {
                characterObject.SetDebugStressScore(value);
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }
        else if (statNames == "負荷等級") //stress level
        {
            if (int.TryParse(newStatValue, out int value))
            {
                characterObject.SetDebugStressLevel(value);
                DisplaySuccessText(characterObject == this.playerCharacter);
            }
        }

        this.battleGameManager.GetBattleAnimationManager().CheckHasBattleEnded();
    }

    //change state
    public void OnPlayerButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        ChangeStateValue(this.selectedPlayerStat, this.newPlayerStatValue, this.playerCharacter);
    }

    public void OnEnemyButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        if (this.selectedEnemyStat == "當前為無流向") //none passive
        {
            this.enemyCharacter.SetSelectedPassiveSkillCategoryType(CategorizedPassiveSkillManager.CategoryType.None);
            DisplaySuccessText(this.enemyCharacter == this.playerCharacter);
        }
        else if (this.selectedEnemyStat == "當前為生命流") //life passive
        {
            this.enemyCharacter.SetSelectedPassiveSkillCategoryType(CategorizedPassiveSkillManager.CategoryType.Life);
            DisplaySuccessText(this.enemyCharacter == this.playerCharacter);
        }
        else if (this.selectedEnemyStat == "當前為以太流") //state passive
        {
            this.enemyCharacter.SetSelectedPassiveSkillCategoryType(CategorizedPassiveSkillManager.CategoryType.State);
            DisplaySuccessText(this.enemyCharacter == this.playerCharacter);
        }
        else if (this.selectedEnemyStat == "當前為負荷流") //stress passive
        {
            this.enemyCharacter.SetSelectedPassiveSkillCategoryType(CategorizedPassiveSkillManager.CategoryType.Stress);
            DisplaySuccessText(this.enemyCharacter == this.playerCharacter);
        }
        else
        {
            ChangeStateValue(this.selectedEnemyStat, this.newEnemyStatValue, this.enemyCharacter);
        }
    }

    public void ClickToToggleDebugMode()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        this.debugModeObjectLabel.SetActive(!this.debugModeObjectLabel.activeSelf);

        bool _isDebugMode = this.debugModeObjectLabel.activeSelf;
        this.battleGameManager.GetBattleAnimationManager().SetIsDebugMode(_isDebugMode);
        this.debugModeButtonLabel.SetText((_isDebugMode) ? "Debug Mode ON" : "Debug Mode OFF");
    }

    public void ResetInfoText()
    {
        this.playerDisplayInfo.text = null;
        this.enemyDisplayInfo.text = null;
    }

    public void DisplaySuccessText(bool isPlayer)
    {
        if (isPlayer)
        {
            this.playerDisplayInfo.text = "設定成功";
        }
        else
        {
            this.enemyDisplayInfo.text = "設定成功";
        }
    }

    public void CheckValueInput(string selectedCharacterStat, TMP_InputField characterStatValue)
    {
        characterStatValue.text = null;
        if (selectedCharacterStat == "參數" || selectedCharacterStat == "當前為無流向" || selectedCharacterStat == "當前為生命流"
            || selectedCharacterStat == "當前為以太流" || selectedCharacterStat == "當前為負荷流")
        {
            characterStatValue.interactable = false;
        }
        else
        {
            characterStatValue.interactable = true;
            characterStatValue.ActivateInputField();

            if (selectedCharacterStat == "使用技能ID")
            {
                characterStatValue.contentType = TMP_InputField.ContentType.Standard;
                characterStatValue.onValidateInput = (string input, int charIndex, char addedChar) =>
                {
                    tempChar = ValidateSkillId(addedChar);
                    characterStatValue.text = characterStatValue.text.Replace(" ", "");
                    return tempChar;
                };
            }
            else
            {
                characterStatValue.contentType = TMP_InputField.ContentType.DecimalNumber;
                if (selectedCharacterStat == "當前以太值" || selectedCharacterStat == "生命積分" || selectedCharacterStat == "負荷積分")
                {
                    characterStatValue.onValidateInput = null;
                }
                else
                {
                    characterStatValue.onValidateInput = (string input, int charIndex, char addedChar) =>
                    {
                        tempChar = ValidateNumber(addedChar);
                        characterStatValue.text = characterStatValue.text.Replace(" ", "").Replace(".", "");
                        return tempChar;
                    };
                }
            }
        }
    }

    public char ValidateSkillId(char addedChar)
    {
        if (this.validateInputWord.Contains(addedChar) || char.IsNumber(addedChar))
        {
            if (char.IsLetter(addedChar))
            {
                addedChar = char.ToUpper(addedChar);
            }
            return addedChar;
        }
        else
        {
            return ' ';
        }
    }

    public char ValidateNumber(char addedChar)
    {
        if (char.IsNumber(addedChar))
        {
            return addedChar;
        }
        else
        {
            return ' ';
        }
    }
}
